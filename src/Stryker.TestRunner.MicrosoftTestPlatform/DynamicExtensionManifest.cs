using System.Diagnostics;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using Stryker.TestRunner.MicrosoftTestPlatform.Extension;

namespace Stryker.TestRunner.MicrosoftTestPlatform;

internal sealed class DynamicExtensionManifest : IDisposable
{
    private static readonly object SyncRoot = new();
    private static readonly Dictionary<string, SharedManifest> Manifests = new(StringComparer.OrdinalIgnoreCase);

    private readonly string _testApplicationDirectory;
    private bool _disposed;

    private DynamicExtensionManifest(string testApplicationDirectory, string path)
    {
        _testApplicationDirectory = testApplicationDirectory;
        Path = path;
    }

    public string Path { get; }

    public static DynamicExtensionManifest Acquire(string testApplication)
    {
        var normalizedApplication = System.IO.Path.GetFullPath(testApplication);
        var directory = System.IO.Path.GetDirectoryName(normalizedApplication)
            ?? throw new InvalidOperationException($"Could not determine the directory of test application '{testApplication}'.");
        lock (SyncRoot)
        {
            if (Manifests.TryGetValue(directory, out var sharedManifest))
            {
                sharedManifest.ReferenceCount++;
                return new DynamicExtensionManifest(directory, sharedManifest.Path);
            }

            var path = CreateManifest(directory);
            Manifests.Add(directory, new SharedManifest(path));
            return new DynamicExtensionManifest(directory, path);
        }
    }

    public void Dispose()
    {
        lock (SyncRoot)
        {
            if (_disposed)
            {
                return;
            }

            _disposed = true;
            var sharedManifest = Manifests[_testApplicationDirectory];
            sharedManifest.ReferenceCount--;
            if (sharedManifest.ReferenceCount == 0)
            {
                Manifests.Remove(_testApplicationDirectory);
                DeleteManifest(sharedManifest.Path);
            }
        }
    }

    private static string CreateManifest(string directory)
    {
        CleanupStaleManifests(directory);

        using var currentProcess = Process.GetCurrentProcess();
        var processStartTicks = currentProcess.StartTime.ToUniversalTime().Ticks;
        var manifestPath = System.IO.Path.Combine(
            directory,
            $"stryker-{Environment.ProcessId}-{processStartTicks}.testingplatformextensions.json");
        var temporaryPath = $"{manifestPath}.{Guid.NewGuid():N}.tmp";
        var extensionAssembly = typeof(BlockingCoverageTestingPlatformBuilderHook).Assembly.Location;

        var manifest = new
        {
            extensions = new[]
            {
                new
                {
                    id = GetExtensionId(extensionAssembly),
                    displayName = "Stryker blocking coverage consumer",
                    assemblyPath = extensionAssembly,
                    typeFullName = typeof(BlockingCoverageTestingPlatformBuilderHook).FullName
                }
            }
        };

        try
        {
            File.WriteAllText(temporaryPath, JsonSerializer.Serialize(manifest));
            File.Move(temporaryPath, manifestPath, overwrite: true);
            return manifestPath;
        }
        finally
        {
            File.Delete(temporaryPath);
        }
    }

    private static string GetExtensionId(string extensionAssembly)
    {
        var hash = SHA256.HashData(Encoding.UTF8.GetBytes(extensionAssembly));
        return $"Stryker.BlockingCoverageConsumer.{Convert.ToHexString(hash[..8])}";
    }

    private static void CleanupStaleManifests(string directory)
    {
        foreach (var manifestPath in Directory.EnumerateFiles(
                     directory,
                     "stryker-*-*.testingplatformextensions.json"))
        {
            var nameParts = System.IO.Path.GetFileName(manifestPath).Split('-');
            if (nameParts.Length < 3
                || !int.TryParse(nameParts[1], out var processId)
                || !long.TryParse(nameParts[2].Split('.')[0], out var processStartTicks)
                || IsProcessInstanceRunning(processId, processStartTicks))
            {
                continue;
            }

            DeleteManifest(manifestPath);
        }
    }

    private static bool IsProcessInstanceRunning(int processId, long processStartTicks)
    {
        try
        {
            using var process = Process.GetProcessById(processId);
            return process.StartTime.ToUniversalTime().Ticks == processStartTicks;
        }
        catch (ArgumentException)
        {
            return false;
        }
        catch (InvalidOperationException)
        {
            return false;
        }
        catch (System.ComponentModel.Win32Exception)
        {
            return true;
        }
    }

    private static void DeleteManifest(string path)
    {
        try
        {
            File.Delete(path);
        }
        catch (IOException exception)
        {
            System.Diagnostics.Debug.WriteLine(
                $"[Stryker] Could not delete MTP dynamic extension manifest '{path}': {exception}");
        }
        catch (UnauthorizedAccessException exception)
        {
            System.Diagnostics.Debug.WriteLine(
                $"[Stryker] Could not delete MTP dynamic extension manifest '{path}': {exception}");
        }
    }

    private sealed class SharedManifest(string path)
    {
        public string Path { get; } = path;

        public int ReferenceCount { get; set; } = 1;
    }
}
