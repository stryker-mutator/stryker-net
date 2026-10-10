using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Shouldly;

namespace Validation;

/// <summary>
/// Mutant counts parsed from a Stryker mutation-report.json file.
/// </summary>
internal readonly record struct MutationStatusCounts(int Killed, int Survived, int NoCoverage, int Timeout)
{
    public int CoveredVerdicts => Killed + Survived;
}

/// <summary>
/// Shared harness for MTP concurrency kill-count parity checks (stryker-net#3832).
/// </summary>
internal static class StrykerMtpConcurrencyKillCountTestSupport
{
    public static string FindDirectoryUnderRepository(params string[] relativeSegments)
    {
        var dir = new DirectoryInfo(AppContext.BaseDirectory);
        while (dir is not null)
        {
            var candidate = Path.Combine(dir.FullName, Path.Combine(relativeSegments));
            if (Directory.Exists(candidate))
            {
                return candidate;
            }

            dir = dir.Parent;
        }

        return null;
    }

    public static async Task EnsureProjectBuiltAsync(
        string projectRoot,
        string csprojRelativePath,
        string releaseAssemblyFileName)
    {
        var csproj = Path.Combine(projectRoot, csprojRelativePath);
        if (!File.Exists(csproj))
        {
            return;
        }

        var built = Directory
            .EnumerateFiles(Path.GetDirectoryName(csproj)!, releaseAssemblyFileName, SearchOption.AllDirectories)
            .FirstOrDefault(path => path.Contains($"{Path.DirectorySeparatorChar}Release{Path.DirectorySeparatorChar}", StringComparison.Ordinal));
        if (built is not null)
        {
            return;
        }

        var build = await RunProcessAsync(
            "dotnet",
            $"build \"{csproj}\" -c Release",
            projectRoot,
            TimeSpan.FromMinutes(10));
        build.ExitCode.ShouldBe(0, $"build failed:\n{build.StdOut}\n{build.StdErr}");
    }

    public static async Task<MutationStatusCounts> RunStrykerAndGetCountsAsync(
        string cliDll,
        string testProjectDirectory,
        string configPath,
        int concurrency,
        TimeSpan timeout,
        string solutionPath = null)
    {
        var outputDir = Path.Combine(testProjectDirectory, "StrykerOutput");
        if (Directory.Exists(outputDir))
        {
            Directory.Delete(outputDir, recursive: true);
        }

        var solutionArg = string.IsNullOrWhiteSpace(solutionPath)
            ? string.Empty
            : $" -s \"{solutionPath}\"";

        (int ExitCode, string StdOut, string StdErr) run;
        try
        {
            run = await RunProcessAsync(
                "dotnet",
                $"exec \"{cliDll}\" -f \"{configPath}\" -c {concurrency} -V warning -L --skip-version-check{solutionArg}",
                testProjectDirectory,
                timeout);
        }
        catch (TimeoutException exception)
        {
            throw new TimeoutException($"{exception.Message}{Environment.NewLine}{TailOfNewestStrykerLog(outputDir)}", exception);
        }

        run.ExitCode.ShouldBe(0, $"stryker exited {run.ExitCode} at concurrency {concurrency}:\n{run.StdOut}\n{run.StdErr}");

        var report = Directory
            .EnumerateFiles(outputDir, "mutation-report.json", SearchOption.AllDirectories)
            .OrderByDescending(File.GetLastWriteTimeUtc)
            .FirstOrDefault();
        report.ShouldNotBeNull($"no mutation-report.json under {outputDir} (concurrency {concurrency})");

        return ParseMutationReport(report);
    }

    public static async Task<int> RunStrykerAndCountKilledAsync(
        string cliDll,
        string testProjectDirectory,
        string configPath,
        int concurrency,
        TimeSpan timeout,
        string solutionPath = null)
    {
        var counts = await RunStrykerAndGetCountsAsync(
            cliDll,
            testProjectDirectory,
            configPath,
            concurrency,
            timeout,
            solutionPath);
        return counts.Killed;
    }

    public static MutationStatusCounts ParseMutationReport(string reportPath)
    {
        using var stream = File.OpenRead(reportPath);
        using var document = JsonDocument.Parse(stream);
        var killed = 0;
        var survived = 0;
        var noCoverage = 0;
        var timeout = 0;
        if (!document.RootElement.TryGetProperty("files", out var files))
        {
            return new MutationStatusCounts(killed, survived, noCoverage, timeout);
        }

        foreach (var file in files.EnumerateObject())
        {
            if (!file.Value.TryGetProperty("mutants", out var mutants))
            {
                continue;
            }

            foreach (var mutant in mutants.EnumerateArray())
            {
                if (!mutant.TryGetProperty("status", out var statusElement))
                {
                    continue;
                }

                switch (statusElement.GetString())
                {
                    case "Killed":
                        killed++;
                        break;
                    case "Survived":
                        survived++;
                        break;
                    case "NoCoverage":
                        noCoverage++;
                        break;
                    case "Timeout":
                        timeout++;
                        break;
                }
            }
        }

        return new MutationStatusCounts(killed, survived, noCoverage, timeout);
    }

    public static int CountKilled(string reportPath) => ParseMutationReport(reportPath).Killed;

    public static async Task<(int ExitCode, string StdOut, string StdErr)> RunProcessAsync(
        string fileName,
        string arguments,
        string workingDirectory,
        TimeSpan timeout)
    {
        var psi = new ProcessStartInfo
        {
            FileName = fileName,
            Arguments = arguments,
            WorkingDirectory = workingDirectory,
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            UseShellExecute = false
        };

        using var process = new Process { StartInfo = psi };
        process.Start();
        var stdoutTask = process.StandardOutput.ReadToEndAsync();
        var stderrTask = process.StandardError.ReadToEndAsync();
        using var timeoutSource = new CancellationTokenSource(timeout);
        try
        {
            await process.WaitForExitAsync(timeoutSource.Token);
        }
        catch (OperationCanceledException)
        {
            try
            {
                process.Kill(entireProcessTree: true);
            }
            catch
            {
                // Best effort if the process already exited.
            }

            // The killed process closes its pipes, so the reads complete; bound them anyway.
            var stdoutTail = await TailAsync(stdoutTask);
            var stderrTail = await TailAsync(stderrTask);
            throw new TimeoutException($"Process timed out after {timeout}: {fileName} {arguments}{Environment.NewLine}stdout (tail):{Environment.NewLine}{stdoutTail}{Environment.NewLine}stderr (tail):{Environment.NewLine}{stderrTail}");
        }

        return (process.ExitCode, await stdoutTask, await stderrTask);
    }

    private static async Task<string> TailAsync(Task<string> readTask)
    {
        try
        {
            return Tail(await readTask.WaitAsync(TimeSpan.FromSeconds(5)));
        }
        catch (Exception exception) when (exception is TimeoutException or OperationCanceledException)
        {
            return "<output not available>";
        }
    }

    private static string Tail(string text, int maxLines = 60) =>
        string.Join(Environment.NewLine, text.Split('\n').TakeLast(maxLines));

    /// <summary>
    /// The Stryker file log (written with <c>-L</c>, always at trace level) says which mutant run was in flight when a
    /// run timed out, which the redirected console output at warning level cannot.
    /// </summary>
    private static string TailOfNewestStrykerLog(string outputDir)
    {
        var log = Directory.Exists(outputDir)
            ? Directory.EnumerateFiles(outputDir, "*.log", SearchOption.AllDirectories)
                .Concat(Directory.EnumerateFiles(outputDir, "*.txt", SearchOption.AllDirectories).Where(path => path.Contains("logs", StringComparison.OrdinalIgnoreCase)))
                .OrderByDescending(File.GetLastWriteTimeUtc)
                .FirstOrDefault()
            : null;
        if (log is null)
        {
            return $"No Stryker log file found under {outputDir}.";
        }

        using var stream = new FileStream(log, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
        using var reader = new StreamReader(stream);
        return $"Stryker log tail ({log}):{Environment.NewLine}{Tail(reader.ReadToEnd(), maxLines: 200)}";
    }
}
