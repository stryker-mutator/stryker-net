using System.Collections.Concurrent;
using System.Diagnostics;
using System.IO.MemoryMappedFiles;
using System.Security.Cryptography;
using System.Text;
using Microsoft.Extensions.Logging;
using Stryker.Abstractions;
using Stryker.Abstractions.Exceptions;
using Stryker.Abstractions.Options;
using Stryker.Abstractions.Testing;
using Stryker.TestRunner.MicrosoftTestPlatform.Models;
using Stryker.TestRunner.Results;
using Stryker.TestRunner.Tests;
using static Stryker.Abstractions.Testing.ITestRunner;

namespace Stryker.TestRunner.MicrosoftTestPlatform;

/// <summary>
/// Individual test runner instance that handles test execution with mutation-specific
/// environment variables. Used by MicrosoftTestPlatformRunnerPool.
/// Maintains persistent test server connections per assembly to reduce process startup overhead.
/// Uses file-based mutant control to allow changing the active mutant without restarting processes.
/// </summary>
public class MicrosoftTestingPlatformRunner : IDisposable
{
    // One coverage file per test assembly. The injected MutantControl flushes coverage with an
    // unconditional overwrite on process exit, so with test hosts sharing a single file the last
    // flush to land replaces all the others: only one assembly's coverage survives, and which one
    // depends on server stop order and process shutdown timing. Giving every assembly's host its
    // own file and unioning them at read time makes coverage independent of both.
    internal readonly ConcurrentDictionary<string, string> _coverageFilePaths = new();
    internal readonly Dictionary<string, AssemblyTestServer> _assemblyServers = new();
    internal bool _disposed;
    internal bool _coverageMode;
    internal bool _perTestCoverageMode;

    private readonly int _id;
    private readonly Dictionary<string, List<TestNode>> _testsByAssembly;
    private readonly Dictionary<string, MtpTestDescription> _testDescriptions;
    private List<MtpTestDescription>? _testDescriptionsSnapshot;
    private readonly TestSet _testSet;
    private readonly object _discoveryLock;
    private readonly ILogger _logger;
    private readonly string _mutantFilePath;
    private readonly string _reachedFilePath;
    private readonly string _runIdentity;
    private readonly string _coverageFilePathBase;
    private readonly IStrykerOptions? _options;
    private readonly object _serverLock = new();
    private readonly SemaphoreSlim _sessionSemaphore = new(1, 1);
    private readonly object _mutantFileLock = new();
    private FileStream? _mutantFileStream;
    private readonly object _reachedFileLock = new();
    private FileStream? _reachedFileStream;
    private int _retestCount;
    private int _retestedKilledCount;
    private readonly HashSet<string> _initializedPerTestFiles = new();
    private readonly Dictionary<string, int> _perTestEpochCounters = new();

    private string RunnerId => $"MtpRunner-{_id}";

    /// <summary>
    /// Path of the mutant-id control file this runner shares with its test hosts. Exposed for unit testing.
    /// </summary>
    internal string MutantFilePath => _mutantFilePath;

    /// <summary>
    /// Path of the reached-mutant relay file this runner shares with its test hosts. Exposed for unit testing.
    /// </summary>
    internal string ReachedFilePath => _reachedFilePath;

    public MicrosoftTestingPlatformRunner(
        int id,
        Dictionary<string, List<TestNode>> testsByAssembly,
        Dictionary<string, MtpTestDescription> testDescriptions,
        TestSet testSet,
        object discoveryLock,
        ILogger logger,
        IStrykerOptions? options = null)
    {
        _id = id;
        _testsByAssembly = testsByAssembly;
        _testDescriptions = testDescriptions;
        _testSet = testSet;
        _discoveryLock = discoveryLock;
        _logger = logger;
        _options = options;

        // Create unique file paths for this runner to communicate with the test process.
        // The names embed the process id plus a per-instance nonce: coverage files are only deleted
        // once their path has been assigned, so a predictable name could let a run read a stale file
        // left behind by a crashed earlier run (same runner id, same assembly), and concurrent
        // Stryker processes could clobber each other's files. The nonce covers what the process id
        // alone does not (pid reuse, several runner instances with the same id in one process).
        // Shared by the mutant-id control file and all coverage files. Runner ids are pool-local,
        // so the control file also needs the full identity to prevent another runner's construction,
        // writes or disposal from changing this runner's active mutant.
        _runIdentity = $"{Environment.ProcessId}-{_id}-{Guid.NewGuid().ToString("N")[..8]}";
        _mutantFilePath = Path.Combine(Path.GetTempPath(), $"stryker-mutant-{_runIdentity}.txt");
        _reachedFilePath = Path.Combine(Path.GetTempPath(), $"stryker-reached-{_runIdentity}.txt");
        _coverageFilePathBase = Path.Combine(Path.GetTempPath(), $"stryker-coverage-{_runIdentity}");

        // Initialize with no active mutation. A failure is already logged and the next session
        // write fails the run, so the constructor stays non-throwing.
        try
        {
            WriteMutantIdToFile(-1);
        }
        catch (InvalidOperationException)
        {
        }

        // Size the reached-signal file up front so the test host can memory-map it on its first
        // ReportReached call. A failure is already logged and only costs the reached signal.
        ResetReachedFile();
    }

    public Task<bool> DiscoverTestsAsync(string assembly)
    {
        return DiscoverTestsInternalAsync(assembly);
    }

    public Task<ITestRunResult> InitialTestAsync(IProjectAndTests project)
    {
        var assemblies = project.GetTestAssemblies();
        return RunAllTestsAsync(assemblies, mutantId: -1, mutants: null, update: null);
    }

    public Task<ITestRunResult> TestMultipleMutantsAsync(
        IProjectAndTests project,
        ITimeoutValueCalculator? timeoutCalc,
        IReadOnlyList<IMutant> mutants,
        TestUpdateHandler? update)
    {
        var assemblies = project.GetTestAssemblies();

        // Determine which mutant to activate
        // When testing a single mutant, activate it; otherwise use -1 (no mutation)
        var mutantId = mutants.Count == 1 ? mutants[0].Id : -1;

        _logger.LogDebug("{RunnerId}: Testing mutant(s) [{Mutants}] with active mutation ID: {MutantId}",
            RunnerId, string.Join(",", mutants.Select(m => m.Id)), mutantId);

        return RunAllTestsAsync(assemblies, mutantId, mutants, update, timeoutCalc);
    }

    public async Task ResetServerAsync()
    {
        _logger.LogDebug("{RunnerId}: Resetting test servers to reload assemblies", RunnerId);
        
        lock (_serverLock)
        {
            foreach (var server in _assemblyServers.Values)
            {
                server.Dispose();
            }
            _assemblyServers.Clear();
        }
        
        _logger.LogDebug("{RunnerId}: Test servers reset complete", RunnerId);
        await Task.CompletedTask;
    }

    internal int ActiveMutantId { get; set; } = -1;

    /// <summary>
    /// Publishes the active mutant id to the control file. A failed write throws: continuing would run
    /// the tests with a different mutant active than the one being judged.
    /// </summary>
    private void WriteMutantIdToFile(int mutantId)
    {
        try
        {
            // Keep the control file open for the life of the runner. Reopening it for every mutant session
            // can invalidate memory-mapped views in reused MTP hosts; MutantControl maps the file (or reads it
            // directly as a fallback) with FileShare.ReadWrite (stryker-mutator/stryker-net#3832).
            lock (_mutantFileLock)
            {
                _mutantFileStream ??= OpenMutantControlFile();
                _mutantFileStream.Seek(0, SeekOrigin.Begin);
                Span<byte> bytes = stackalloc byte[sizeof(int)];
                BitConverter.TryWriteBytes(bytes, mutantId);
                _mutantFileStream.Write(bytes);
                _mutantFileStream.Flush();
            }

            _logger.LogDebug("{RunnerId}: Wrote mutant ID {MutantId} to mutant control file {FilePath}",
                RunnerId, mutantId, _mutantFilePath);
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "{RunnerId}: Failed to write mutant ID to mutant control file {FilePath}",
                RunnerId, _mutantFilePath);
            throw new InvalidOperationException($"Could not publish mutant id {mutantId} to the control file {_mutantFilePath}.", ex);
        }
    }

    private FileStream OpenMutantControlFile() => OpenMutantControlFile(_mutantFilePath, stream => stream.SetLength(sizeof(int)));

    /// <summary>
    /// Opens the control file and sizes it with <paramref name="resize"/> when it is too short. The stream is
    /// released when sizing fails, since the caller never receives it and a retry would otherwise leak a handle.
    /// </summary>
    internal static FileStream OpenMutantControlFile(string path, Action<FileStream> resize)
    {
        var stream = new FileStream(path, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.ReadWrite);
        try
        {
            if (stream.Length < sizeof(int))
            {
                resize(stream);
            }
        }
        catch
        {
            stream.Dispose();
            throw;
        }

        return stream;
    }

    private Dictionary<string, string?> BuildEnvironmentVariables(string assembly)
    {
        var envVars = new Dictionary<string, string?>
        {
            ["STRYKER_MUTANT_FILE"] = _mutantFilePath,
            ["STRYKER_REACHED_FILE"] = _reachedFilePath
        };

        ExternalEnvironmentVariables.Add(envVars);

        // Add coverage filename when in coverage mode (MutantControl will combine with temp path).
        // Per-test (reused-process) capture uses its own assembly-scoped coverage/epoch file pair so
        // that other assembly servers kept warm on this runner don't race this one's flush.
        if (_perTestCoverageMode)
        {
            envVars["STRYKER_COVERAGE_FILE"] = Path.GetFileName(GetPerTestCoverageFilePath(assembly));
            envVars["STRYKER_COVERAGE_EPOCH_FILE"] = Path.GetFileName(GetPerTestEpochFilePath(assembly));
        }
        else if (_coverageMode)
        {
            envVars["STRYKER_COVERAGE_FILE"] = Path.GetFileName(GetCoverageFilePath(assembly));
        }

        return envVars;
    }

    /// <summary>
    /// Returns the coverage file path assigned to the given test assembly, assigning one on first
    /// use. The base embeds the process id, runner id and a per-instance nonce (files from other
    /// processes, runners and runner instances must not collide); the hash of the assembly path
    /// distinguishes assemblies in different directories that share a file name. The assembly name
    /// itself is only included, truncated, to keep the file recognizable when debugging.
    /// </summary>
    internal string GetCoverageFilePath(string assembly) =>
        _coverageFilePaths.GetOrAdd(assembly, static (path, basePath) =>
        {
            var name = new string(Path.GetFileNameWithoutExtension(path)
                .Select(c => char.IsLetterOrDigit(c) ? c : '-')
                .Take(32)
                .ToArray());
            var hash = Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(path)))[..8];
            return $"{basePath}-{name}-{hash}.txt";
        }, _coverageFilePathBase);

    /// <summary>
    /// Enables or disables coverage capture mode. When enabled, the test process will track
    /// which mutations are covered and write the data to a file on process exit.
    /// </summary>
    public void SetCoverageMode(bool enabled)
    {
        lock (_serverLock)
        {
            if (_coverageMode == enabled)
            {
                // Already in the desired state; no action needed
                return;
            }

            _coverageMode = enabled;
            _logger.LogDebug("{RunnerId}: Coverage mode {Status}", RunnerId, enabled ? "enabled" : "disabled");

            // Reset servers to apply the new environment variables
            foreach (var server in _assemblyServers.Values)
            {
                server.Dispose();
            }
            _assemblyServers.Clear();
        }

        // Clean up any existing coverage files, even when enabling, to ensure we start fresh
        DeleteCoverageFiles();
    }

    /// <summary>
    /// Enables or disables per-test coverage capture mode ("perTest": tests run one at a time against a
    /// reused process, with coverage relayed after each test via <see cref="RunSingleTestForCoverageInReusedProcessAsync"/>).
    /// </summary>
    public void SetPerTestCoverageMode(bool enabled)
    {
        lock (_serverLock)
        {
            if (_perTestCoverageMode == enabled)
            {
                return;
            }

            _perTestCoverageMode = enabled;
            _logger.LogDebug("{RunnerId}: Per-test coverage mode {Status}", RunnerId, enabled ? "enabled" : "disabled");

            foreach (var server in _assemblyServers.Values)
            {
                server.Dispose();
            }
            _assemblyServers.Clear();

            DeletePerTestFiles();

            _initializedPerTestFiles.Clear();
            _perTestEpochCounters.Clear();
        }
    }

    /// <summary>
    /// Deletes the coverage and epoch files of every assembly this runner set up for per-test capture.
    /// Both come as one file per mutated assembly the host loaded, named after the path handed out, so
    /// each one is matched by prefix rather than by that exact path.
    /// Callers hold <see cref="_serverLock"/>.
    /// </summary>
    private void DeletePerTestFiles()
    {
        foreach (var assembly in _initializedPerTestFiles)
        {
            foreach (var coverageFilePath in EnumerateCoverageFiles(GetPerTestCoverageFilePath(assembly)))
            {
                DeleteFileIfExists(coverageFilePath);
            }

            var epochFilePath = GetPerTestEpochFilePath(assembly);
            foreach (var relayFilePath in EnumerateEpochRelayFiles(epochFilePath))
            {
                DeleteFileIfExists(relayFilePath);
            }
            DeleteFileIfExists(epochFilePath);
        }
    }

    private static string SanitizeAssemblyName(string assembly) =>
        $"{Path.GetFileNameWithoutExtension(assembly)}-{(uint)assembly.GetHashCode()}";

    internal string GetPerTestCoverageFilePath(string assembly) =>
        Path.Combine(Path.GetTempPath(), $"stryker-coverage-pt-{_runIdentity}-{SanitizeAssemblyName(assembly)}.txt");

    internal string GetPerTestEpochFilePath(string assembly) =>
        Path.Combine(Path.GetTempPath(), $"stryker-epoch-{_runIdentity}-{SanitizeAssemblyName(assembly)}.txt");

    /// <summary>
    /// Reads coverage data from the coverage files written by the test processes, unioned across all
    /// test assemblies this runner started a server for and, within each of them, across every mutated
    /// assembly the host loaded (see <see cref="EnumerateCoverageFiles"/>).
    /// Returns the covered mutants and static mutants as separate lists.
    /// </summary>
    public (IReadOnlyList<int> CoveredMutants, IReadOnlyList<int> StaticMutants) ReadCoverageData()
    {
        var coveredMutants = new HashSet<int>();
        var staticMutants = new HashSet<int>();

        foreach (var (assembly, handedOutPath) in _coverageFilePaths)
        {
            var (covered, statics) = ReadCoverageForHandedOutPath(handedOutPath, assembly);
            coveredMutants.UnionWith(covered);
            staticMutants.UnionWith(statics);
        }

        return (coveredMutants.ToList(), staticMutants.ToList());
    }

    /// <summary>
    /// Reads the coverage written for one path handed out through STRYKER_COVERAGE_FILE, unioned across
    /// every mutated assembly the test host loaded (see <see cref="EnumerateCoverageFiles"/>).
    /// </summary>
    private (IReadOnlyList<int> CoveredMutants, IReadOnlyList<int> StaticMutants) ReadCoverageForHandedOutPath(
        string handedOutPath, string? assembly = null)
    {
        var coverageFilePaths = EnumerateCoverageFiles(handedOutPath);
        if (coverageFilePaths.Count == 0)
        {
            _logger.LogDebug("{RunnerId}: No coverage file{ForAssembly} found at {Path}",
                RunnerId, assembly is null ? string.Empty : $" for {Path.GetFileName(assembly)}", handedOutPath);
            return (Array.Empty<int>(), Array.Empty<int>());
        }

        var coveredMutants = new HashSet<int>();
        var staticMutants = new HashSet<int>();

        foreach (var coverageFilePath in coverageFilePaths)
        {
            var (covered, statics) = ReadCoverageDataFrom(coverageFilePath, assembly);
            coveredMutants.UnionWith(covered);
            staticMutants.UnionWith(statics);
        }

        return (coveredMutants.ToList(), staticMutants.ToList());
    }

    /// <summary>
    /// Returns the coverage files written for a path handed out by <see cref="GetCoverageFilePath"/> or
    /// <see cref="GetPerTestCoverageFilePath"/>. A test host loads one copy of the injected MutantControl
    /// per mutated assembly, and each copy suffixes the name it was given with its own assembly name so
    /// the copies do not overwrite each other, so a single handed-out path can yield several files.
    /// </summary>
    private IReadOnlyList<string> EnumerateCoverageFiles(string handedOutPath)
    {
        var directory = Path.GetDirectoryName(handedOutPath);
        if (string.IsNullOrEmpty(directory) || !Directory.Exists(directory))
        {
            return Array.Empty<string>();
        }

        var searchPattern = Path.GetFileNameWithoutExtension(handedOutPath) + "*" + Path.GetExtension(handedOutPath);
        try
        {
            return Directory.GetFiles(directory, searchPattern);
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "{RunnerId}: Failed to list coverage files matching {Pattern} in {Directory}",
                RunnerId, searchPattern, directory);
            return Array.Empty<string>();
        }
    }

    /// <summary>
    /// Reads coverage data from a single coverage file written by a test process. Shared by the
    /// per-assembly union in <see cref="ReadCoverageData"/> and the single-test read used by
    /// <see cref="RunSingleTestForCoverageInReusedProcessAsync"/>.
    /// </summary>
    private (IReadOnlyList<int> CoveredMutants, IReadOnlyList<int> StaticMutants) ReadCoverageDataFrom(string coverageFilePath, string? assembly = null)
    {
        if (!File.Exists(coverageFilePath))
        {
            _logger.LogDebug("{RunnerId}: Coverage file{ForAssembly} not found at {Path}",
                RunnerId, assembly is null ? string.Empty : $" for {Path.GetFileName(assembly)}", coverageFilePath);
            return (Array.Empty<int>(), Array.Empty<int>());
        }

        try
        {
            var content = File.ReadAllText(coverageFilePath).Trim();
            _logger.LogDebug("{RunnerId}: Read coverage data{ForAssembly}: {Content}",
                RunnerId, assembly is null ? string.Empty : $" for {Path.GetFileName(assembly)}", content);

            if (string.IsNullOrEmpty(content))
            {
                return (Array.Empty<int>(), Array.Empty<int>());
            }

            var parts = content.Split(';');
            var coveredMutants = ParseMutantIds(parts.Length > 0 ? parts[0] : string.Empty);
            var staticMutants = ParseMutantIds(parts.Length > 1 ? parts[1] : string.Empty);

            return (coveredMutants, staticMutants);
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "{RunnerId}: Failed to read coverage file at {Path}", RunnerId, coverageFilePath);
            return (Array.Empty<int>(), Array.Empty<int>());
        }
    }

    private static IReadOnlyList<int> ParseMutantIds(string idString)
    {
        if (string.IsNullOrWhiteSpace(idString))
        {
            return Array.Empty<int>();
        }

        return idString
            .Split(',', StringSplitOptions.RemoveEmptyEntries)
            .Select(s => int.TryParse(s.Trim(), out var id) ? id : (int?)null)
            .Where(id => id.HasValue)
            .Select(id => id.Value)
            .ToList();
    }

    private void DeleteFileIfExists(string path)
    {
        try
        {
            if (File.Exists(path))
            {
                File.Delete(path);
            }
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "{RunnerId}: Failed to delete file at {Path}", RunnerId, path);
        }
    }

    private void DeleteCoverageFiles()
    {
        foreach (var coverageFilePath in _coverageFilePaths.Values.SelectMany(EnumerateCoverageFiles))
        {
            DeleteFileIfExists(coverageFilePath);
        }
    }

    /// <summary>
    /// Creates the 8-byte coverage epoch relay file (see <see cref="MutantControl"/>'s epoch poller) for
    /// an assembly if it doesn't already exist, initialized to request=0/ack=0 to match the poller's
    /// starting state. Idempotent so it is safe to call before every per-test run.
    /// </summary>
    internal void InitializeEpochFile(string epochFilePath)
    {
        if (File.Exists(epochFilePath))
        {
            return;
        }

        try
        {
            using var stream = new FileStream(epochFilePath, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.ReadWrite);
            stream.SetLength(8);
            using var mmf = MemoryMappedFile.CreateFromFile(stream, null, 8, MemoryMappedFileAccess.ReadWrite, HandleInheritability.None, leaveOpen: true);
            using var accessor = mmf.CreateViewAccessor(0, 8, MemoryMappedFileAccess.ReadWrite);
            accessor.Write(0, 0);
            accessor.Write(4, 0);
            accessor.Flush();
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "{RunnerId}: Failed to initialize coverage epoch file {Path}", RunnerId, epochFilePath);
        }
    }

    internal void WriteEpochRequest(string epochFilePath, int epoch)
    {
        try
        {
            using var stream = new FileStream(epochFilePath, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.ReadWrite);
            using var mmf = MemoryMappedFile.CreateFromFile(stream, null, 8, MemoryMappedFileAccess.ReadWrite, HandleInheritability.None, leaveOpen: true);
            using var accessor = mmf.CreateViewAccessor(0, 8, MemoryMappedFileAccess.ReadWrite);
            accessor.Write(0, epoch);
            accessor.Flush();
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "{RunnerId}: Failed to write coverage epoch request to {Path}", RunnerId, epochFilePath);
        }
    }

    /// <summary>
    /// Returns the epoch relay files owned by a test host's copies of the injected MutantControl for a
    /// path handed out through STRYKER_COVERAGE_EPOCH_FILE: one per mutated assembly the host loaded,
    /// named after the handed-out path the same way the coverage files are (see
    /// <see cref="EnumerateCoverageFiles"/>). The handed-out path itself is excluded - no copy relays
    /// through it, so waiting for an ack on it would never return.
    /// </summary>
    private IReadOnlyList<string> EnumerateEpochRelayFiles(string basePath)
    {
        var directory = Path.GetDirectoryName(basePath);
        if (string.IsNullOrEmpty(directory) || !Directory.Exists(directory))
        {
            return Array.Empty<string>();
        }

        var baseFileName = Path.GetFileName(basePath);
        var searchPattern = Path.GetFileNameWithoutExtension(basePath) + "*" + Path.GetExtension(basePath);
        try
        {
            return Directory.GetFiles(directory, searchPattern)
                .Where(path => !string.Equals(Path.GetFileName(path), baseFileName, StringComparison.OrdinalIgnoreCase))
                .ToList();
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "{RunnerId}: Failed to list epoch relay files matching {Pattern} in {Directory}",
                RunnerId, searchPattern, directory);
            return Array.Empty<string>();
        }
    }

    /// <summary>
    /// Asks every mutated assembly's relay in the test host to flush the coverage of the test that just
    /// ran, and returns the relays it reached. One request per relay: a host with several mutated
    /// assemblies runs one relay per assembly, each with its own coverage file, and each has to be told
    /// separately. The returned set is what <see cref="WaitForAllEpochAcksAsync"/> then waits for, so
    /// that a relay created after this point is not waited on: it never received the request, so it
    /// could never acknowledge it, and its coverage is flushed on the next epoch instead.
    /// </summary>
    internal IReadOnlyList<string> BroadcastEpochRequest(string basePath, int epoch)
    {
        // Keep the handed-out path in step so a relay that attaches to it later starts from this epoch
        WriteEpochRequest(basePath, epoch);

        var relayFilePaths = EnumerateEpochRelayFiles(basePath);
        foreach (var relayFilePath in relayFilePaths)
        {
            WriteEpochRequest(relayFilePath, epoch);
        }

        return relayFilePaths;
    }

    /// <summary>
    /// Waits until every relay the request reached has acknowledged the epoch, which is when all of the
    /// coverage for that test is on disk. Waiting for a single ack would let the coverage files be read
    /// while another mutated assembly's relay has not flushed yet, attributing that assembly's coverage
    /// to the next test.
    /// </summary>
    internal async Task<bool> WaitForAllEpochAcksAsync(
        IReadOnlyList<string> relayFilePaths, int expectedEpoch, TimeSpan timeout)
    {
        if (relayFilePaths.Count == 0)
        {
            // The test touched no mutated code, so no relay exists and there is nothing to flush
            _logger.LogDebug("{RunnerId}: No coverage epoch relay to wait for", RunnerId);
            return true;
        }

        var deadline = DateTime.UtcNow + timeout;
        while (true)
        {
            if (relayFilePaths.All(path => TryReadEpochAck(path, out var ack) && ack == expectedEpoch))
            {
                return true;
            }

            if (DateTime.UtcNow >= deadline)
            {
                return false;
            }

            await Task.Delay(1).ConfigureAwait(false);
        }
    }

    internal static bool TryReadEpochAck(string epochFilePath, out int ack)
    {
        ack = -1;
        if (!File.Exists(epochFilePath))
        {
            return false;
        }

        try
        {
            using var stream = new FileStream(epochFilePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
            using var mmf = MemoryMappedFile.CreateFromFile(stream, null, 8, MemoryMappedFileAccess.Read, HandleInheritability.None, leaveOpen: true);
            using var accessor = mmf.CreateViewAccessor(0, 8, MemoryMappedFileAccess.Read);
            ack = accessor.ReadInt32(4);
            return true;
        }
        catch
        {
            return false;
        }
    }

    internal static async Task<bool> WaitForEpochAckAsync(string epochFilePath, int expectedEpoch, TimeSpan timeout)
    {
        var deadline = DateTime.UtcNow + timeout;
        while (DateTime.UtcNow < deadline)
        {
            if (TryReadEpochAck(epochFilePath, out var ack) && ack == expectedEpoch)
            {
                return true;
            }
            await Task.Delay(1).ConfigureAwait(false);
        }

        return TryReadEpochAck(epochFilePath, out var finalAck) && finalAck == expectedEpoch;
    }

    /// <summary>
    /// Captures coverage for a single test without restarting the test host: runs the test on the
    /// (possibly already warm) server for its assembly, then asks the injected <see cref="MutantControl"/>'s
    /// background epoch relay to flush what that test covered and reset for the next one. Used for the
    /// "perTest" coverage mode; see MutantControl's epoch poller for the other side of this handshake.
    /// </summary>
    internal virtual async Task<ICoverageRunResult> RunSingleTestForCoverageInReusedProcessAsync(
        string assembly, TestNode test, string testId)
    {
        var coverageFilePath = GetPerTestCoverageFilePath(assembly);
        var epochFilePath = GetPerTestEpochFilePath(assembly);

        lock (_serverLock)
        {
            if (_initializedPerTestFiles.Add(assembly))
            {
                // Relays left behind by an earlier run would be waited on and never acknowledge, or worse
                // already show this epoch, so clear them before the host creates its own
                foreach (var staleRelayFilePath in EnumerateEpochRelayFiles(epochFilePath))
                {
                    DeleteFileIfExists(staleRelayFilePath);
                }
                InitializeEpochFile(epochFilePath);
                foreach (var staleCoverageFilePath in EnumerateCoverageFiles(coverageFilePath))
                {
                    DeleteFileIfExists(staleCoverageFilePath);
                }
            }
        }

        // A crashed test host tears down the RPC connection mid-request (rather than timing out), the
        // same failure mode RunAssemblyTestsInternalAsync already retries once for. Without discarding
        // the dead server here, every later test on this runner+assembly would keep trying to reuse it
        // and fail too; retrying once on a freshly started server recovers this test's real coverage
        // instead of settling for Dubious on the first hiccup.
        const int maxRunAttempts = 2;
        Exception? lastRunException = null;

        for (var attempt = 1; attempt <= maxRunAttempts; attempt++)
        {
            try
            {
                var server = await GetOrCreateServerAsync(assembly).ConfigureAwait(false);
                var (_, timedOut) = await server.RunTestsAsync(new[] { test }, CalculateSingleTestTimeout(test)).ConfigureAwait(false);
                if (timedOut)
                {
                    _logger.LogWarning(
                        "{RunnerId}: Test run timed out while capturing per-test coverage for {TestId}; marking as Dubious",
                        RunnerId, testId);
                    await DiscardServerAsync(assembly).ConfigureAwait(false);
                    return CoverageRunResult.Create(testId, CoverageConfidence.Dubious,
                        Array.Empty<int>(), Array.Empty<int>(), Array.Empty<int>());
                }

                int epoch;
                lock (_serverLock)
                {
                    _perTestEpochCounters.TryGetValue(assembly, out var current);
                    epoch = current + 1;
                    _perTestEpochCounters[assembly] = epoch;
                }

                var requestedRelays = BroadcastEpochRequest(epochFilePath, epoch);

                var acked = await WaitForAllEpochAcksAsync(requestedRelays, epoch, TimeSpan.FromSeconds(10)).ConfigureAwait(false);
                if (!acked)
                {
                    _logger.LogWarning(
                        "{RunnerId}: Timed out waiting for coverage relay ack for test {TestId}; marking as Dubious",
                        RunnerId, testId);
                    return CoverageRunResult.Create(testId, CoverageConfidence.Dubious,
                        Array.Empty<int>(), Array.Empty<int>(), Array.Empty<int>());
                }

                var (covered, staticMutants) = ReadCoverageForHandedOutPath(coverageFilePath);
                return CoverageRunResult.Create(testId, CoverageConfidence.Normal, covered, staticMutants, Array.Empty<int>());
            }
            catch (Exception ex)
            {
                lastRunException = ex;
                _logger.LogDebug(ex,
                    "{RunnerId}: Per-test coverage capture for {TestId} failed on attempt {Attempt}/{MaxAttempts}; discarding crashed server",
                    RunnerId, testId, attempt, maxRunAttempts);

                // The server most likely crashed; drop it so the next attempt (or the next test on this
                // runner) starts a fresh one instead of reusing a dead RPC connection.
                await DiscardServerAsync(assembly).ConfigureAwait(false);
            }
        }

        _logger.LogWarning(lastRunException,
            "{RunnerId}: Failed to capture per-test coverage for {TestId} after {MaxAttempts} attempts",
            RunnerId, testId, maxRunAttempts);
        return CoverageRunResult.Create(testId, CoverageConfidence.Dubious,
            Array.Empty<int>(), Array.Empty<int>(), Array.Empty<int>());
    }

    /// <summary>
    /// Builds the result of an isolated capture from the coverage file the test host flushed on exit. Mutants seen
    /// running in a static context are kept so they are later run on a fresh host.
    /// </summary>
    internal ICoverageRunResult BuildIsolatedCoverageResult(string testId, string coverageFilePath, string assembly)
    {
        var (covered, staticMutants) = ReadCoverageForHandedOutPath(coverageFilePath, assembly);
        _logger.LogDebug("{RunnerId}: Isolated coverage of {TestId}: {CoveredCount} mutant(s) covered, {StaticCount} of them in a static context",
            RunnerId, testId, covered.Count, staticMutants.Count);
        return CoverageRunResult.Create(testId, CoverageConfidence.Exact, covered, staticMutants, Array.Empty<int>());
    }

    /// <summary>
    /// Captures coverage for a single test with full process isolation: the test host is discarded and
    /// restarted before the test runs and stopped again right after, so no static state or coverage from
    /// another test can leak in. That is what lets the result be trusted with
    /// <see cref="CoverageConfidence.Exact"/> - unlike the reused-process "perTest" capture in
    /// <see cref="RunSingleTestForCoverageInReusedProcessAsync"/>, which can only ever report
    /// <see cref="CoverageConfidence.Normal"/> since other tests may have already touched shared state
    /// on the same warm process. The mutants seen running in a static context are kept, unlike
    /// VsTestRunnerPool, which starts a new process for every mutant run: mutation runs here reuse test
    /// hosts, and only a mutant flagged as static is run on a fresh host
    /// (see <see cref="RequiresFreshHost"/>), so dropping the flag would let the warm-up cache the original
    /// value of a static initializer and the mutant could never take effect.
    /// </summary>
    internal virtual async Task<ICoverageRunResult> RunSingleTestForCoverageInIsolatedProcessAsync(
        string assembly, TestNode test, string testId)
    {
        var coverageFilePath = GetCoverageFilePath(assembly);

        try
        {
            // Discard any server left over from a previous isolated test (or another mode) so this
            // test starts in a fresh process rather than one that already ran other code.
            await DiscardServerAsync(assembly).ConfigureAwait(false);

            // Clear what an earlier test left on disk. Coverage files are written per mutated assembly,
            // so a host that does not load one of them never rewrites its file, and reading it would
            // credit this test with the earlier one's coverage - at Exact confidence, which the pool
            // trusts enough to drop mutants no test covers. A single shared file gave this for free,
            // since every host rewrote it whole.
            var staleCoverageFilePaths = EnumerateCoverageFiles(coverageFilePath);
            if (staleCoverageFilePaths.Count > 0)
            {
                _logger.LogDebug("{RunnerId}: Removing {Count} stale coverage file(s) before capturing {TestId}", RunnerId, staleCoverageFilePaths.Count, testId);
            }

            foreach (var staleCoverageFilePath in staleCoverageFilePaths)
            {
                DeleteFileIfExists(staleCoverageFilePath);
            }

            var server = await GetOrCreateServerAsync(assembly).ConfigureAwait(false);
            var (_, timedOut) = await server.RunTestsAsync(new[] { test }, CalculateSingleTestTimeout(test)).ConfigureAwait(false);
            if (timedOut)
            {
                _logger.LogWarning(
                    "{RunnerId}: Test run timed out while capturing isolated coverage for {TestId}; marking as Dubious",
                    RunnerId, testId);
                await DiscardServerAsync(assembly).ConfigureAwait(false);
                return CoverageRunResult.Create(testId, CoverageConfidence.Dubious,
                    Array.Empty<int>(), Array.Empty<int>(), Array.Empty<int>());
            }

            // Stop gracefully (not force) so the injected MutantControl's ProcessExit handler flushes
            // this test's (and only this test's) coverage to file before the process goes away.
            await server.StopAsync().ConfigureAwait(false);

            return BuildIsolatedCoverageResult(testId, coverageFilePath, assembly);
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex,
                "{RunnerId}: Failed to capture isolated coverage for {TestId}; marking as Dubious",
                RunnerId, testId);

            await DiscardServerAsync(assembly).ConfigureAwait(false);
            return CoverageRunResult.Create(testId, CoverageConfidence.Dubious,
                Array.Empty<int>(), Array.Empty<int>(), Array.Empty<int>());
        }
    }

    private async Task<AssemblyTestServer> GetOrCreateServerAsync(string assembly)
    {
        AssemblyTestServer? deadServer = null;
        lock (_serverLock)
        {
            if (_assemblyServers.TryGetValue(assembly, out var existing))
            {
                if (existing.IsAlive)
                {
                    return existing;
                }

                // The server process is no longer alive (e.g. it crashed during a previous run).
                // Drop it so a fresh server is started rather than reusing a dead RPC connection,
                // which would fail every subsequent test run instantly.
                _logger.LogDebug("{RunnerId}: Test server for {Assembly} is no longer alive; recreating", RunnerId, assembly);
                _assemblyServers.Remove(assembly);
                deadServer = existing;
            }
        }

        if (deadServer is not null)
        {
            await deadServer.StopAsync(force: true).ConfigureAwait(false);
        }

        var environmentVariables = BuildEnvironmentVariables(assembly);
        var server = new AssemblyTestServer(assembly, environmentVariables, _logger, RunnerId, _options);

        var started = await server.StartAsync().ConfigureAwait(false);
        if (!started)
        {
            throw new InvalidOperationException($"Failed to start test server for {assembly}");
        }

        lock (_serverLock)
        {
            _assemblyServers[assembly] = server;
        }

        return server;
    }

    /// <summary>
    /// Force-stops and removes the server for the given assembly so the next run starts a fresh one.
    /// Used after a run fails because the test host crashed and tore down the RPC connection.
    /// </summary>
    private async Task DiscardServerAsync(string assembly)
    {
        AssemblyTestServer? server;
        lock (_serverLock)
        {
            _assemblyServers.TryGetValue(assembly, out server);
            _assemblyServers.Remove(assembly);
        }

        if (server is not null)
        {
            _logger.LogDebug("{RunnerId}: Discarding the test server for {Assembly} (warmed up: {WarmedUp})",
                RunnerId, Path.GetFileName(assembly), server.IsWarmedUp);
            await server.StopAsync(force: true).ConfigureAwait(false);
        }
    }

    private async Task<bool> DiscoverTestsInternalAsync(string assembly)
    {
        try
        {
            var server = await GetOrCreateServerAsync(assembly).ConfigureAwait(false);
            var tests = await server.DiscoverTestsAsync().ConfigureAwait(false);

            lock (_discoveryLock)
            {
                _testsByAssembly[assembly] = tests;

                foreach (var test in tests.Where(t => !_testDescriptions.ContainsKey(t.Uid)))
                {
                    var mtpTestDescription = new MtpTestDescription(test);
                    _testDescriptions[test.Uid] = mtpTestDescription;
                    _testSet.RegisterTest(mtpTestDescription.Description);
                }
            }

            _logger.LogDebug("{RunnerId}: Discovered {TestCount} tests in {Assembly}", RunnerId, tests.Count, assembly);
            return tests.Count > 0;
        }
        catch (Exception ex) when (ex is not InputException && ex is not OperationCanceledException)
        {
            _logger.LogError(ex, "{RunnerId}: Failed to discover tests in {Assembly}", RunnerId, assembly);
            throw new InputException($"Test discovery failed for assembly '{assembly}'.", ex.Message);
        }
    }

    internal List<TestNode>? GetDiscoveredTests(string assembly)
    {
        lock (_discoveryLock)
        {
            return _testsByAssembly.TryGetValue(assembly, out var tests) ? tests : null;
        }
    }

    internal TimeSpan? CalculateAssemblyTimeout(List<TestNode> discoveredTests, ITimeoutValueCalculator timeoutCalc, string assembly, IReadOnlyList<IMutant>? mutants = null)
    {
        // Estimates the run time of a set of tests based on their initial (unmutated) run time.
        int EstimateTime(IEnumerable<TestNode> tests) => (int)tests.Sum(t =>
        {
            lock (_discoveryLock)
            {
                return _testDescriptions.TryGetValue(t.Uid, out var desc)
                    ? desc.InitialRunTime.TotalMilliseconds
                    : 0;
            }
        });

        int estimatedTimeMs;
        // When no mutants are known, or any mutant must be assessed by every test, base the timeout
        // on the full test run. Otherwise base it only on the tests covering the mutant(s) in this
        // run. Mutants are grouped so their covering tests are disjoint, so the union of those tests
        // is exactly what this run needs to execute.
        if (mutants is null || mutants.Count == 0 || mutants.Any(m => m.AssessingTests.IsEveryTest))
        {
            estimatedTimeMs = EstimateTime(discoveredTests);
        }
        else
        {
            var assessingTests = mutants
                .SelectMany(m => m.AssessingTests.GetIdentifiers())
                .ToHashSet();
            estimatedTimeMs = EstimateTime(discoveredTests.Where(t => assessingTests.Contains(t.Uid)));
        }

        var timeoutMs = timeoutCalc.CalculateTimeoutValue(estimatedTimeMs);
        _logger.LogDebug("{RunnerId}: Using {TimeoutMs} ms as test run timeout for {Assembly}",
            RunnerId, timeoutMs, Path.GetFileName(assembly));

        return TimeSpan.FromMilliseconds(timeoutMs);
    }

    // Coverage capture has no ITimeoutValueCalculator of its own (CaptureCoverage isn't handed one),
    // so a single test's own initial run time is used instead, with the same 1.5x margin the assembly
    // timeout above uses. The floor guards fast tests, whose RPC/JIT overhead dwarfs their measured
    // duration - without it, a well-behaved test could be flagged as timed out before it even had a
    // chance to run.
    private static readonly TimeSpan _minimumSingleTestCoverageTimeout = TimeSpan.FromSeconds(30);
    private const double SingleTestCoverageTimeoutRatio = 1.5;

    internal TimeSpan CalculateSingleTestTimeout(TestNode test)
    {
        var additionalTimeout = TimeSpan.FromMilliseconds(_options?.AdditionalTimeout ?? 0);
        var estimatedRunTime = _testDescriptions.TryGetValue(test.Uid, out var description)
            ? description.InitialRunTime
            : TimeSpan.Zero;

        var calculated = (estimatedRunTime * SingleTestCoverageTimeoutRatio) + additionalTimeout;
        return calculated > _minimumSingleTestCoverageTimeout ? calculated : _minimumSingleTestCoverageTimeout;
    }

    internal async Task HandleAssemblyTimeoutAsync(string assembly, List<TestNode> discoveredTests, List<string> allTimedOutTests)
    {
        _logger.LogDebug("{RunnerId}: Test run timed out for {Assembly}", RunnerId, Path.GetFileName(assembly));

        allTimedOutTests.AddRange(discoveredTests.Select(t => t.Uid));
        
        AssemblyTestServer? server;
        lock (_serverLock)
        {
            _assemblyServers.TryGetValue(assembly, out server);
        }
        
        if (server is not null)
        {
            _logger.LogDebug("{RunnerId}: Restarting test server for {Assembly} after timeout", RunnerId, Path.GetFileName(assembly));
            try
            {
                await server.RestartAsync(force: true).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                _logger.LogDebug(ex, "{RunnerId}: Failed to restart test server for {Assembly} after timeout. Creating a new server on next use.", RunnerId, Path.GetFileName(assembly));
                lock (_serverLock)
                {
                    _assemblyServers.Remove(assembly);
                }
            }
        }
    }

    private sealed class TestRunAccumulator
    {
        private readonly List<string> _executedTests = [];
        private readonly List<string> _failedTests = [];
        private readonly List<string> _messages = [];
        private readonly List<string> _errorMessages = [];
        private int _totalDiscoveredTests;
        private int _totalExecutedTests;

        public List<string> TimedOutTests { get; } = [];
        public bool HasTimeout { get; set; }
        public bool HasError { get; private set; }
        public TimeSpan TotalDuration { get; private set; }

        public void Aggregate(TestRunResult result, List<TestNode>? discoveredTests)
        {
            // A crash sentinel (FailingTests == EveryTest, produced only by the TestRunResult(false)
            // path when an assembly run crashes) must NOT be folded into the executed/failed sets:
            // EveryTest.GetIdentifiers() is empty, so doing so would record "every test ran, none
            // failed" and report otherwise-untested mutants as Survived. Flag it as an error instead;
            // RunAllTestsAsync then returns a RuntimeError result so the affected mutants are
            // classified as RuntimeError (excluded from the score) rather than Survived or Killed.
            if (result.FailingTests.IsEveryTest)
            {
                HasError = true;
                if (!string.IsNullOrWhiteSpace(result.ResultMessage))
                {
                    _errorMessages.Add(result.ResultMessage);
                }
                TotalDuration += result.Duration;
                return;
            }

            if (result.ExecutedTests.IsEveryTest)
            {
                // "Every test" is scoped to one assembly, but this accumulator spans all of them, so the
                // identifiers have to be recorded and not just counted. Counting alone loses which tests
                // ran, and with a test filter active the aggregate count stays below the total discovered
                // count, so BuildExecutedTests cannot collapse back to EveryTest either: the mutant's
                // assessing tests would not be included in the ran set and it would stay Pending.
                _executedTests.AddRange(discoveredTests?.Select(t => t.Uid) ?? []);
                _totalExecutedTests += discoveredTests?.Count ?? 0;
            }
            else
            {
                var executedIds = result.ExecutedTests.GetIdentifiers().ToList();
                _executedTests.AddRange(executedIds);
                _totalExecutedTests += executedIds.Count;
            }

            _failedTests.AddRange(result.FailingTests.GetIdentifiers());
            TotalDuration += result.Duration;
            _messages.AddRange(result.Messages ?? []);

            if (!string.IsNullOrWhiteSpace(result.ResultMessage))
            {
                _errorMessages.Add(result.ResultMessage);
            }
        }

        public void AddDiscoveredCount(int count) => _totalDiscoveredTests += count;

        public ITestIdentifiers BuildExecutedTests() =>
            _totalDiscoveredTests > 0 && _totalExecutedTests >= _totalDiscoveredTests
                ? TestIdentifierList.EveryTest()
                : new TestIdentifierList(_executedTests);

        public ITestIdentifiers BuildFailedTests() => new TestIdentifierList(_failedTests);

        public ITestIdentifiers BuildTimedOutTests() => new TestIdentifierList(TimedOutTests);

        public string BuildErrorMessage() => string.Join(Environment.NewLine, _errorMessages);

        public IEnumerable<string> Messages => _messages;
    }

    /// <summary>
    /// Builds a filter restricting a test run to the tests that can actually kill the given mutant(s)
    /// (<see cref="IMutant.AssessingTests"/>), so a covered mutant is tested against its covering tests
    /// only instead of the whole suite. Returns <c>null</c> (run every test) when <paramref name="mutants"/>
    /// is null/empty (initial/coverage runs), when a mutant is missing coverage data (defensive fallback),
    /// or when any mutant must be tested against every test (e.g. static mutants).
    /// </summary>
    internal static Func<TestNode, bool>? BuildTestUidFilter(IReadOnlyList<IMutant>? mutants)
    {
        if (mutants is null || mutants.Count == 0)
        {
            return null;
        }

        if (mutants.Any(m => m.AssessingTests is null || m.AssessingTests.IsEveryTest))
        {
            return null;
        }

        var testIds = new HashSet<string>(mutants.SelectMany(m => m.AssessingTests.GetIdentifiers()));
        return testIds.Count == 0 ? null : node => testIds.Contains(node.Uid);
    }

    /// <summary>
    /// Returns the test descriptions as a list that is safe to enumerate without the discovery lock.
    /// Descriptions are only ever added, so the cached copy is reused until the count changes instead
    /// of being rebuilt for every mutant.
    /// </summary>
    private List<MtpTestDescription> GetTestDescriptionsSnapshot()
    {
        lock (_discoveryLock)
        {
            if (_testDescriptionsSnapshot is null || _testDescriptionsSnapshot.Count != _testDescriptions.Count)
            {
                _testDescriptionsSnapshot = _testDescriptions.Values.ToList();
            }

            return _testDescriptionsSnapshot;
        }
    }

    internal async Task<ITestRunResult> RunAllTestsAsync(
        IReadOnlyList<string> assemblies,
        int mutantId,
        IReadOnlyList<IMutant>? mutants,
        TestUpdateHandler? update,
        ITimeoutValueCalculator? timeoutCalc = null)
    {
        await _sessionSemaphore.WaitAsync().ConfigureAwait(false);
        try
        {
            ActiveMutantId = mutantId;
            WriteMutantIdToFile(mutantId);

            var testUidFilter = BuildTestUidFilter(mutants);
            var accumulator = new TestRunAccumulator();

            foreach (var assembly in assemblies)
            {
                var (result, timedOut, discoveredTests) = await RunAssemblyTestsAsync(assembly, timeoutCalc, mutants, testUidFilter).ConfigureAwait(false);

                if (discoveredTests is not null)
                {
                    accumulator.AddDiscoveredCount(discoveredTests.Count);

                    if (timedOut)
                    {
                        accumulator.HasTimeout = true;
                        await HandleAssemblyTimeoutAsync(assembly, discoveredTests, accumulator.TimedOutTests).ConfigureAwait(false);
                    }
                }

                if (result is not null)
                {
                    accumulator.Aggregate(result, discoveredTests);
                }
            }

            var executedTests = accumulator.BuildExecutedTests();
            var failedTestIds = accumulator.BuildFailedTests();
            var timedOutTestIds = accumulator.BuildTimedOutTests();

            var testDescriptionValues = GetTestDescriptionsSnapshot();

            if (update is not null && mutants is not null)
            {
                update.Invoke(mutants, failedTestIds, executedTests, timedOutTestIds);
            }

            if (accumulator.HasError)
            {
                // The test host crashed (e.g. a mutation caused a fatal fault). Signal a runtime error
                // so the affected mutants are classified as RuntimeError (excluded from the score)
                // rather than reported as survived or logged as a test failure.
                _logger.LogDebug("{RunnerId}: A test host crashed during this run; reporting a runtime error for the affected mutant(s).", RunnerId);
                return TestRunResult.RuntimeError(
                    testDescriptionValues,
                    executedTests,
                    failedTestIds,
                    timedOutTestIds,
                    accumulator.BuildErrorMessage(),
                    accumulator.Messages,
                    accumulator.TotalDuration);
            }

            if (accumulator.HasTimeout)
            {
                return TestRunResult.TimedOut(
                    testDescriptionValues,
                    executedTests,
                    failedTestIds,
                    timedOutTestIds,
                    accumulator.BuildErrorMessage(),
                    accumulator.Messages,
                    accumulator.TotalDuration);
            }

            return new TestRunResult(
                testDescriptionValues,
                executedTests,
                failedTestIds,
                timedOutTestIds,
                accumulator.BuildErrorMessage(),
                accumulator.Messages,
                accumulator.TotalDuration);
        }
        catch (Exception ex)
        {
            _logger.LogDebug(ex, "{RunnerId}: Failed to run tests for mutant ID {MutantId}", RunnerId, mutantId);
            return new TestRunResult(false, ex.Message);
        }
        finally
        {
            // A host started outside a mutant session must not be warmed up under a stale mutant id.
            ActiveMutantId = -1;
            _sessionSemaphore.Release();
        }
    }

    internal virtual async Task<(TestRunResult? Result, bool TimedOut, List<TestNode>? DiscoveredTests)> RunAssemblyTestsAsync(
        string assembly,
        ITimeoutValueCalculator? timeoutCalc,
        IReadOnlyList<IMutant>? mutants = null,
        Func<TestNode, bool>? testUidFilter = null)
    {
        if (!File.Exists(assembly))
        {
            return (null, false, null);
        }

        var discoveredTests = GetDiscoveredTests(assembly);
        var anyCoveringTests = testUidFilter is not null && discoveredTests is not null && !discoveredTests.Any(testUidFilter);

        // When no tests in this assembly cover the mutant(s) under test, skip the assembly entirely.
        if (anyCoveringTests)
        {
            _logger.LogDebug("{RunnerId}: No test in {Assembly} covers the mutant(s) under test; skipping this assembly",
                RunnerId, Path.GetFileName(assembly));
            return (null, false, null);
        }

        TimeSpan? timeout = null;
        if (timeoutCalc is not null && discoveredTests is not null)
        {
            timeout = CalculateAssemblyTimeout(discoveredTests, timeoutCalc, assembly, mutants);
        }

        var (testResults, timedOut) = await RunAssemblyTestsInternalAsync(
            assembly,
            testUidFilter,
            timeout,
            () => CalculateWarmUpTimeout(discoveredTests, timeoutCalc, assembly),
            RequiresFreshHost(mutants)).ConfigureAwait(false);

        return (testResults as TestRunResult, timedOut, discoveredTests);
    }

    /// <summary>
    /// True when a mutant sits in static code (a static initializer or static constructor). Such a mutant
    /// only takes effect if it is active when the host first initializes that code, so it needs a host
    /// that has not run anything yet, and nothing else may reuse the host afterwards.
    /// </summary>
    internal static bool RequiresFreshHost(IReadOnlyList<IMutant>? mutants) =>
        mutants is not null && mutants.Any(mutant => mutant.IsStaticValue);

    /// <summary>
    /// Whether a fresh host runs all tests once with no mutant active before its first mutant. Without
    /// the warm-up, the first session on a host initializes every static with the current mutant active,
    /// and that value stays for the host's life: a mutant that corrupts a static silently poisons the
    /// host and every later mutant on it can be falsely killed. The warm-up used to be skipped without
    /// coverage-based testing because a mutant in code reached only from a static initializer is not
    /// flagged as static and could never take effect on a warmed host; the fresh-host retest now covers
    /// exactly that case, so every covered mode warms up. <c>off</c> has no coverage data, so no retest
    /// is possible and the warm-up stays off there. Unknown options keep the warm-up on.
    /// </summary>
    internal bool WarmUpEnabled =>
        _options is null
        || _options.OptimizationMode.HasFlag(OptimizationModes.CoverageBasedTest)
        || _options.OptimizationMode.HasFlag(OptimizationModes.SkipUncoveredMutants);

    /// <summary>
    /// The warm-up runs every test, so it cannot borrow the per-mutant timeout, which is sized for the
    /// tests covering one mutant. It uses the timeout of a full run instead, which scales with the suite.
    /// Returns <c>null</c> (no cap) when no timeout calculator or no discovered tests are available.
    /// </summary>
    internal TimeSpan? CalculateWarmUpTimeout(List<TestNode>? discoveredTests, ITimeoutValueCalculator? timeoutCalc, string assembly)
    {
        if (timeoutCalc is null || discoveredTests is null)
        {
            return null;
        }

        return CalculateAssemblyTimeout(discoveredTests, timeoutCalc, assembly);
    }

    /// <summary>
    /// Runs the tests once with no mutant active on a host that has not run any yet, then restores the
    /// active mutant id. Returns false, after discarding the host, when the warm-up timed out.
    /// </summary>
    internal async Task<bool> WarmUpServerAsync(AssemblyTestServer server, string assembly, List<TestNode>? tests, TimeSpan? timeout = null)
    {
        if (ActiveMutantId >= 0)
        {
            var mutantId = ActiveMutantId;
            WriteMutantIdToFile(-1);
            bool timedOut;
            try
            {
                timedOut = await RunWarmUpTestsAsync(server, tests?.ToArray(), timeout).ConfigureAwait(false);
            }
            finally
            {
                WriteMutantIdToFile(mutantId);
            }

            if (timedOut)
            {
                _logger.LogDebug("{RunnerId}: Warm-up run of {Assembly} timed out; discarding the test server",
                    RunnerId, Path.GetFileName(assembly));
                await DiscardServerAsync(assembly).ConfigureAwait(false);
                return false;
            }
        }

        server.IsWarmedUp = true;
        return true;
    }

    /// <summary>
    /// Discards the host when a test reported a type that failed to initialize or load, so the next mutant
    /// starts on a fresh one. Returns true when the host was recycled.
    /// </summary>
    internal async Task<bool> RecycleIfHostPoisonedAsync(string assembly, IEnumerable<TestNodeUpdate> testResults)
    {
        if (!testResults.Any(update => TestNodeStates.IsHostPoisoning(update.Node)))
        {
            return false;
        }

        _logger.LogDebug("{RunnerId}: A type initializer failed in {Assembly}; recycling the test server before the next mutant",
            RunnerId, Path.GetFileName(assembly));
        await DiscardServerAsync(assembly).ConfigureAwait(false);
        return true;
    }

    internal virtual async Task<bool> RunWarmUpTestsAsync(AssemblyTestServer server, TestNode[]? tests, TimeSpan? timeout)
    {
        var (_, timedOut) = await server.RunTestsAsync(tests, timeout).ConfigureAwait(false);
        return timedOut;
    }

    internal async Task<(ITestRunResult Result, bool TimedOut)> RunAssemblyTestsInternalAsync(
        string assembly,
        Func<TestNode, bool>? testUidFilter,
        TimeSpan? timeout = null,
        Func<TimeSpan?>? warmUpTimeout = null,
        bool freshHost = false)
    {
        // A crashed test host tears down the RPC connection, so the run throws (rather than timing out).
        // Retry once on a freshly started server: a crash caused by a *previous* mutant then self-heals
        // for the current mutant instead of corrupting its result.
        const int maxRunAttempts = 2;
        Exception? lastRunException = null;

        for (var attempt = 1; attempt <= maxRunAttempts; attempt++)
        {
            AssemblyTestServer server;
            try
            {
                if (freshHost && attempt == 1)
                {
                    // Statics the reused host already initialized would hide this mutant.
                    await DiscardServerAsync(assembly).ConfigureAwait(false);
                }

                // Get or create the server for this assembly (reuses an existing, live server)
                server = await GetOrCreateServerAsync(assembly).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                // The server could not be started at all; retrying immediately would not help.
                return (new TestRunResult(false, ex.Message), false);
            }

            if (_logger.IsEnabled(LogLevel.Debug))
            {
                _logger.LogDebug("{RunnerId}: Running {Assembly} (attempt {Attempt}/{MaxAttempts}); active mutant: {MutantId}, fresh host required: {FreshHost}, warm-up enabled: {WarmUpEnabled}, host already warmed up: {WarmedUp}, runs already executed on this host: {RunCount}",
                    RunnerId, Path.GetFileName(assembly), attempt, maxRunAttempts, ActiveMutantId, freshHost, WarmUpEnabled, server.IsWarmedUp, server.RunCount);
            }

            var startTime = DateTime.UtcNow;
            try
            {
                List<TestNode>? tests = null;
                lock (_discoveryLock)
                {
                    if (_testsByAssembly.TryGetValue(assembly, out var assemblyTests))
                    {
                        tests = assemblyTests;
                    }
                }

                var testsToRun = tests?.Where(t => testUidFilter is null || testUidFilter(t)).ToArray();

                // A fresh host runs static initializers under whichever mutant is active first, and keeps that
                // value for its whole life: the mutant is then judged differently than on a reused host and
                // every later mutant on this host inherits it. Initializing under "no mutant" first makes the
                // outcome independent of which runner or host a mutant lands on (stryker-mutator/stryker-net#3832).
                if (!freshHost && WarmUpEnabled && !server.IsWarmedUp && !await WarmUpServerAsync(server, assembly, tests, warmUpTimeout?.Invoke()).ConfigureAwait(false))
                {
                    lastRunException = new TimeoutException($"The warm-up run of {Path.GetFileName(assembly)} timed out");
                    continue;
                }

                // The host reports reaching the active mutant through the relay file; clear any signal
                // from an earlier run so what we read after this run belongs to it alone.
                ResetReachedFile();

                var (testResults, timedOut) = await server.RunTestsAsync(testsToRun, timeout).ConfigureAwait(false);

                var duration = DateTime.UtcNow - startTime;
                var result = BuildTestRunResult(testResults, tests?.Count ?? 0, duration);

                // The runtime caches a failed type initializer for the life of the process, so this host would
                // fail every later mutant the same way and each would be reported as killed. This mutant stays
                // killed; the next one starts on a fresh host (stryker-mutator/stryker-net#3832).
                if (freshHost)
                {
                    // This host initialized its statics under a static mutant: no other mutant may reuse it.
                    await DiscardServerAsync(assembly).ConfigureAwait(false);
                }
                else
                {
                    await RecycleIfHostPoisonedAsync(assembly, testResults).ConfigureAwait(false);
                }

                // A survivor that never reached its mutated code may have been hidden by reused-host state.
                // Retest once on a fresh host; the fresh-host result is final (no loops).
                var reached = ReadReachedFile();
                if (_logger.IsEnabled(LogLevel.Debug))
                {
                    _logger.LogDebug("{RunnerId}: Test run used reached signal: {Reached}", RunnerId, reached);
                }

                // No active mutant (coverage analysis, warm-up bookkeeping) has nothing to retest, and
                // a retest there would spin a fresh host for a session that judges no mutant.
                if (!freshHost && !timedOut && RetestHiddenSurvivorsEnabled && ShouldRetestHiddenSurvivor(result, reached, ActiveMutantId))
                {
                    _logger.LogDebug("{RunnerId}: Retesting mutant {MutantId} on a fresh host: it survived on a reused host without reaching its mutated code",
                        RunnerId, ActiveMutantId);
                    _retestCount++;

                    var (retestResult, retestTimedOut) = await RunAssemblyTestsInternalAsync(
                        assembly, testUidFilter, timeout, warmUpTimeout, freshHost: true).ConfigureAwait(false);
                    if (!retestResult.FailingTests.IsEmpty && !retestResult.FailingTests.IsEveryTest)
                    {
                        _retestedKilledCount++;
                    }

                    return (retestResult, retestTimedOut);
                }

                return (result, timedOut);
            }
            catch (Exception ex)
            {
                lastRunException = ex;
                _logger.LogDebug(ex, "{RunnerId}: Test run for {Assembly} failed on attempt {Attempt}/{MaxAttempts}; discarding crashed server",
                    RunnerId, Path.GetFileName(assembly), attempt, maxRunAttempts);

                // The server most likely crashed; drop it so the next attempt starts a fresh one.
                await DiscardServerAsync(assembly).ConfigureAwait(false);
            }
        }

        // Every attempt failed. Return the crash sentinel; the accumulator recognises it and flags the
        // run as crashed, so the affected mutants are reported as RuntimeError rather than Survived.
        return (new TestRunResult(false, lastRunException!.Message), false);
    }

    /// <summary>
    /// True when a run's outcome may have been distorted by reused-host state and a fresh-host retest is
    /// worthwhile: the mutant was not reached while every executed test passed on a non-fresh host. A run
    /// with no active mutant (coverage analysis) judges no mutant, so it is never retested.
    /// Exposed for unit testing. Coverage-mode gating is separate (<see cref="RetestHiddenSurvivorsEnabled"/>).
    /// </summary>
    internal static bool ShouldRetestHiddenSurvivor(TestRunResult result, bool reached, int activeMutantId) =>
        activeMutantId >= 0
        && !reached
        && !result.ExecutedTests.IsEmpty
        && result.FailingTests.IsEmpty
        && result.TimedOutTests.IsEmpty
        && !result.SessionTimedOut
        && !result.SessionHadRuntimeIssue;

    /// <summary>
    /// True when the mode has coverage data to compare against, so a not-reached survivor can be told
    /// apart from a simply uncovered one. In <c>off</c> mode there is no coverage, so a survivor says
    /// nothing about reaching the code and a retest is skipped (the user chose the mode; hidden mutants
    /// remain a known limitation there).
    /// </summary>
    internal bool RetestHiddenSurvivorsEnabled =>
        _options is null
        || _options.OptimizationMode.HasFlag(OptimizationModes.CoverageBasedTest)
        || _options.OptimizationMode.HasFlag(OptimizationModes.SkipUncoveredMutants);

    /// <summary>
    /// Number of mutants this runner retested on a fresh host because reused-host state hid them, and
    /// how many of those retests killed the mutant. Exposed for unit testing and the pool summary.
    /// </summary>
    internal (int Retested, int Killed) RetestStatistics => (_retestCount, _retestedKilledCount);

    /// <summary>
    /// Reads the reached signal the test host wrote during the run. The host either memory-maps the file
    /// (offset 0: flag, offset 4: mutant id) or, as a fallback, writes the same binary pair; both spell
    /// "reached". The signal only counts when the recorded id is the active mutant: a reset that failed
    /// would otherwise leave a previous run's flag set and hide a not-reached survivor from the retest.
    /// Any failure reads as "not reached", which can only cost an extra fresh-host retest, never a wrong
    /// verdict.
    /// </summary>
    internal bool ReadReachedFile()
    {
        try
        {
            var bytes = new byte[2 * sizeof(int)];
            int read;
            lock (_reachedFileLock)
            {
                var stream = _reachedFileStream;
                if (stream is null)
                {
                    // The persistent stream could not be opened; fall back to a shared read.
                    if (!File.Exists(_reachedFilePath))
                    {
                        return false;
                    }

                    using var diskStream = new FileStream(_reachedFilePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
                    read = 0;
                    while (read < bytes.Length)
                    {
                        var chunk = diskStream.Read(bytes, read, bytes.Length - read);
                        if (chunk == 0)
                        {
                            break;
                        }

                        read += chunk;
                    }
                }
                else
                {
                    stream.Seek(0, SeekOrigin.Begin);
                    read = 0;
                    while (read < bytes.Length)
                    {
                        var chunk = stream.Read(bytes, read, bytes.Length - read);
                        if (chunk == 0)
                        {
                            break;
                        }

                        read += chunk;
                    }
                }
            }

            if (read < 2 * sizeof(int))
            {
                return false;
            }

            return BitConverter.ToInt32(bytes, 0) != 0 && BitConverter.ToInt32(bytes, 4) == ActiveMutantId;
        }
        catch (Exception ex)
        {
            _logger.LogDebug(ex, "{RunnerId}: Failed to read the reached-signal file {FilePath}",
                RunnerId, _reachedFilePath);
            return false;
        }
    }

    /// <summary>
    /// Clears the reached signal before a run so the flag the host writes belongs to this run only.
    /// The file is sized to 8 bytes (flag + mutant id) so the host can memory-map it.
    /// </summary>
    private void ResetReachedFile()
    {
        try
        {
            lock (_reachedFileLock)
            {
                // Keep the file open for the life of the runner: reopening it can invalidate the memory-mapped
                // view the reused host holds, exactly like the mutant control file.
                _reachedFileStream ??= OpenMutantControlFile(_reachedFilePath, stream => stream.SetLength(2 * sizeof(int)));
                var zeros = new byte[2 * sizeof(int)];
                _reachedFileStream.Seek(0, SeekOrigin.Begin);
                _reachedFileStream.Write(zeros);
                _reachedFileStream.Flush();
            }
        }
        catch (Exception ex)
        {
            _logger.LogDebug(ex, "{RunnerId}: Failed to reset the reached-signal file {FilePath}",
                RunnerId, _reachedFilePath);
        }
    }

    /// <summary>
    /// Maps a list of <see cref="TestNodeUpdate"/>s returned by the MTP server
    /// to a <see cref="TestRunResult"/>. Exposed for unit testing.
    /// </summary>
    /// <remarks>
    /// Classification of execution states goes through <see cref="TestNodeStates"/>
    /// so that failure attribution (the bug this adapter originally had) stays in
    /// one place:
    /// <list type="bullet">
    ///   <item><description><c>failed</c>/<c>error</c>/<c>cancelled</c> → failing tests (mutant killed)</description></item>
    ///   <item><description><c>timed-out</c> → timed-out tests (mutant timeout)</description></item>
    ///   <item><description><c>passed</c>/<c>skipped</c> → executed but neither failing nor timed-out</description></item>
    ///   <item><description><c>in-progress</c>/<c>discovered</c> → excluded from executed tests</description></item>
    /// </list>
    /// </remarks>
    internal TestRunResult BuildTestRunResult(
        IReadOnlyCollection<TestNodeUpdate> testResults,
        int totalDiscoveredTests,
        TimeSpan duration)
    {
        var finishedTests = TestNodeStates.CollapseFinishedUpdates(testResults).ToList();

        var failedTests = new List<string>();
        var timedOutTests = new List<string>();
        var errorMessages = new List<string>();
        var messages = new List<string>(finishedTests.Count);
        foreach (var finished in finishedTests)
        {
            var state = finished.Node.ExecutionState;
            var message = $"{finished.Node.DisplayName}{Environment.NewLine}{Environment.NewLine}State: {state}";
            messages.Add(message);
            if (TestNodeStates.IsFailure(state))
            {
                failedTests.Add(finished.Node.Uid);
                errorMessages.Add(message);
            }
            else if (TestNodeStates.IsTimeout(state))
            {
                timedOutTests.Add(finished.Node.Uid);
                errorMessages.Add(message);
            }
        }

        lock (_discoveryLock)
        {
            // MTP doesn't report per-test timing, so approximate with the average
            var perTestDuration = finishedTests.Count > 0
                ? TimeSpan.FromTicks(duration.Ticks / finishedTests.Count)
                : TimeSpan.Zero;

            foreach (var testResult in finishedTests.Where(tr => _testDescriptions.ContainsKey(tr.Node.Uid)))
            {
                var testDescription = _testDescriptions[testResult.Node.Uid];
                testDescription.RegisterInitialTestResult(new MtpTestResult(perTestDuration));
            }
        }

        var errorMessagesStr = string.Join(Environment.NewLine, errorMessages);

        var executedTestCount = finishedTests.Count;
        var executedTests = totalDiscoveredTests > 0 && executedTestCount >= totalDiscoveredTests
            ? TestIdentifierList.EveryTest()
            : new TestIdentifierList(finishedTests.Select(x => x.Node.Uid));

        var failedTestIds = new TestIdentifierList(failedTests);
        var timedOutTestIds = timedOutTests.Count == 0
            ? TestIdentifierList.NoTest()
            : new TestIdentifierList(timedOutTests);

        return new TestRunResult(
            GetTestDescriptionsSnapshot(),
            executedTests,
            failedTestIds,
            timedOutTestIds,
            errorMessagesStr,
            messages,
            duration);
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    public virtual void Dispose(bool disposing)
    {
        if (_disposed)
        {
            return;
        }

        if (disposing)
        {
            lock (_serverLock)
            {
                foreach (var server in _assemblyServers.Values)
                {
                    server.Dispose();
                }
                _assemblyServers.Clear();
            }

            // Clean up temp files
            try
            {
                lock (_mutantFileLock)
                {
                    _mutantFileStream?.Dispose();
                    _mutantFileStream = null;
                }

                lock (_reachedFileLock)
                {
                    _reachedFileStream?.Dispose();
                    _reachedFileStream = null;
                }

                if (File.Exists(_mutantFilePath))
                {
                    File.Delete(_mutantFilePath);
                }

                if (File.Exists(_reachedFilePath))
                {
                    File.Delete(_reachedFilePath);
                }
                // Only has anything to do when the runner is disposed while still in per-test mode,
                // leaving the mode deletes these files and forgets the assemblies they belong to
                DeletePerTestFiles();
            }
            catch (Exception ex)
            {
                // Ignore cleanup errors
                _logger.LogWarning(ex, "{RunnerId}: Failed to clean up temp files", RunnerId);
            }
            DeleteCoverageFiles();
        }
        _disposed = true;
    }
}
