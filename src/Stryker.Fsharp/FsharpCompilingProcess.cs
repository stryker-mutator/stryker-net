using System.IO.Abstractions;
using Buildalyzer;
using FSharp.Compiler.CodeAnalysis;
using FSharp.Compiler.Diagnostics;
using Microsoft.Extensions.Logging;
using Microsoft.FSharp.Control;
using Stryker.Abstractions.ProjectComponents;
using Stryker.Utilities.Buildalyzer;

namespace Stryker.Fsharp;

/// <summary>Outcome of an F# compilation: assembly bytes on success, diagnostics always.</summary>
internal sealed record FsharpCompileResult(bool Success, byte[] Assembly, IReadOnlyList<FSharpDiagnostic> Diagnostics)
{
    /// <summary>Temp copy path (as named in diagnostics) to the original source path, for overridden files.</summary>
    public IReadOnlyDictionary<string, string> SourceMap { get; init; } = new Dictionary<string, string>();
}

/// <summary>
/// Compiles an F# project in-process with FCS (ADR compile-strategy, option A), from the exact fsc
/// arguments Buildalyzer captured. File order always comes from those arguments, never from the component tree.
/// </summary>
internal sealed class FsharpCompilingProcess(IAnalyzerResult sourceProject, IFileSystem fileSystem, ILogger logger)
{
    // Options whose value is a path that Buildalyzer reports relative to the project directory.
    private static readonly string[] PathOptions = ["-o:", "--out:", "--embed:", "--resource:", "--keyfile:", "--win32icon:", "--win32res:", "--win32manifest:", "--appconfig:", "--doc:", "--sourcelink:", "--pdb:"];
    // Options that would produce secondary outputs we do not want next to the original build.
    private static readonly string[] DroppedOptions = ["--refout:", "--refonly", "--doc:", "--embed:"];

    private static readonly FSharpChecker Checker = FSharpChecker.Create(null, null, null, null, null, null, null, null, null, null, null, null, null, null);

    private string ProjectDir => fileSystem.Path.GetDirectoryName(sourceProject.ProjectFilePath)!;

    /// <param name="sourceOverrides">Full source path to replacement text; used to compile mutated sources.</param>
    /// <param name="references">Extra assemblies to reference (the compiled mutant-control helper).</param>
    public FsharpCompileResult Compile(IReadOnlyDictionary<string, string>? sourceOverrides = null, IEnumerable<string>? references = null)
    {
        var workDir = fileSystem.Path.Combine(fileSystem.Path.GetTempPath(), "stryker-fsharp-" + Guid.NewGuid().ToString("N"));
        fileSystem.Directory.CreateDirectory(workDir);
        try
        {
            var output = fileSystem.Path.Combine(workDir, sourceProject.GetAssemblyFileName());
            var sourceMap = new Dictionary<string, string>();
            var argv = BuildArguments(output, workDir, sourceOverrides ?? new Dictionary<string, string>(), references ?? [], sourceMap);
            var (diagnostics, exception) = FSharpAsync.RunSynchronously(Checker.Compile(argv, null), null, null).ToValueTuple();
            var hasErrors = diagnostics.Any(d => d.Severity.IsError) || exception != null;
            foreach (var diagnostic in diagnostics.Where(d => d.Severity.IsError))
            {
                logger.LogDebug("F# compile error: {Diagnostic}", diagnostic);
            }

            return hasErrors || !fileSystem.File.Exists(output)
                ? new FsharpCompileResult(false, [], diagnostics) { SourceMap = sourceMap }
                : new FsharpCompileResult(true, fileSystem.File.ReadAllBytes(output), diagnostics) { SourceMap = sourceMap };
        }
        finally
        {
            fileSystem.Directory.Delete(workDir, true);
        }
    }

    /// <summary>Overwrites the source assembly in every test project's output. Backup is done beforehand, as in the C# flow.</summary>
    public void Inject(ITestProjectsInfo testProjects, byte[] assembly) =>
        InjectFile(testProjects, sourceProject.GetAssemblyFileName(), assembly);

    /// <summary>Writes a file (the compiled assembly or the helper assembly) into every test project's output.</summary>
    public void InjectFile(ITestProjectsInfo testProjects, string fileName, byte[] content)
    {
        foreach (var testProject in testProjects.AnalyzerResults)
        {
            var directory = testProject.GetAssemblyDirectoryPath();
            if (!fileSystem.Directory.Exists(directory))
            {
                fileSystem.Directory.CreateDirectory(directory);
            }

            var injectionPath = fileSystem.Path.Combine(directory, fileName);
            fileSystem.File.WriteAllBytes(injectionPath, content);
            logger.LogDebug("Injected {FileName} into {InjectionPath}", fileName, injectionPath);
        }
    }

    private string[] BuildArguments(string output, string workDir, IReadOnlyDictionary<string, string> sourceOverrides, IEnumerable<string> references, Dictionary<string, string> sourceMap)
    {
        var overrides = sourceOverrides.ToDictionary(o => fileSystem.Path.GetFullPath(o.Key), o => o.Value);
        var argv = new List<string> { "fsc.exe" };
        argv.AddRange(references.Select(r => $"-r:{fileSystem.Path.GetFullPath(r)}"));
        var sourceIndex = 0;
        foreach (var argument in sourceProject.CompilerArguments!)
        {
            if (DroppedOptions.Any(o => argument.StartsWith(o, StringComparison.Ordinal)))
            {
                continue;
            }

            if (argument.StartsWith("-o:", StringComparison.Ordinal) || argument.StartsWith("--out:", StringComparison.Ordinal))
            {
                argv.Add($"-o:{output}");
            }
            else if (argument.StartsWith('-'))
            {
                argv.Add(AbsolutiseOption(argument));
            }
            else
            {
                // positional argument: a source file, kept at its position (compile order is semantic)
                var fullPath = fileSystem.Path.GetFullPath(argument, ProjectDir);
                if (overrides.TryGetValue(fullPath, out var text))
                {
                    var overridePath = fileSystem.Path.Combine(workDir, $"{sourceIndex++}-{fileSystem.Path.GetFileName(fullPath)}");
                    fileSystem.File.WriteAllText(overridePath, text);
                    sourceMap[overridePath] = fullPath;
                    fullPath = overridePath;
                }

                argv.Add(fullPath);
            }
        }

        return [.. argv];
    }

    private string AbsolutiseOption(string argument)
    {
        var option = PathOptions.FirstOrDefault(o => argument.StartsWith(o, StringComparison.Ordinal));
        if (option == null)
        {
            return argument;
        }

        var value = argument[option.Length..];
        // --resource:path[,name[,public|private]]: only the path part is relative
        var comma = value.IndexOf(',');
        var path = comma < 0 ? value : value[..comma];
        var rest = comma < 0 ? string.Empty : value[comma..];
        return option + fileSystem.Path.GetFullPath(path, ProjectDir) + rest;
    }
}
