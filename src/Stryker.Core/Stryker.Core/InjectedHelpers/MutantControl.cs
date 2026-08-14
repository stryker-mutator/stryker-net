namespace Stryker
{
    /// <summary>
    /// A static class used for controlling mutant activation and coverage tracking at runtime.
    /// It supports both environment variable-based control (for VSTest runner) and file-based control (for MTP runner with process reuse).
    /// It should only use C# features up to v2 to ensure compatibility with the widest range of projects it is injected into.
    /// </summary>
    public static class MutantControl
    {
        private static System.Collections.Generic.List<int> _coveredMutants = new System.Collections.Generic.List<int>();
        private static System.Collections.Generic.List<int> _coveredStaticMutants = new System.Collections.Generic.List<int>();
        private static string envName = string.Empty;
        private static System.Object _coverageLock = new System.Object();
        // Initialized to avoid nullable warnings/errors
        private static string _cachedMutantFilePath = string.Empty;
        private static bool _mutantFilePathCached;

        // Memory-mapped view of the mutant-id file used by the MTP runner. The runner writes the active
        // mutant id (a 4-byte int) to the file between runs; reading it through a memory-mapped view is a
        // plain memory access (no syscall), so it is cheap enough for the IsActive hot path while still
        // always reflecting the latest value the runner wrote. The test host process is reused across
        // mutant runs and has no per-run reset hook, so reading every call (rather than caching) is what
        // keeps this correct: any cached or event-based scheme would race the start of the next run.
        // _mutantMmf / _mutantAccessor are typed as object and initialized to a non-null sentinel only to
        // root them (and avoid nullable warnings); the accessor is cast back to its real type when read.
        private static object _mutantMmf = new System.Object();
        private static object _mutantAccessor = new System.Object();
        private static bool _mutantMmfReady;
        private static bool _mutantMmfFailed;

        // Coverage file path for MTP runner (file-based IPC)
        private static string _cachedCoverageFilePath = string.Empty;
        private static bool _coverageFilePathCached;
        private static bool _processExitRegistered;

        // Epoch relay for MTP per-test coverage capture with a reused process (no restart between
        // tests). The runner runs one test at a time and, right after each test completes, bumps the
        // request half of this 8-byte memory-mapped file (offset 0). This background thread notices the
        // change, flushes the coverage accumulated for the test that just ran to the coverage file, resets
        // for the next test, and writes the ack half (offset 4) so the runner knows it is safe to read the
        // file. Polling (rather than a wait handle) is used because named cross-process synchronization
        // primitives are unreliable on Unix; the same tradeoff already governs the mutant-id file above.
        private static string _cachedEpochFilePath = string.Empty;
        private static object _epochMmf = new System.Object();
        private static object _epochAccessor = new System.Object();
        private static bool _epochMmfReady;
        private static bool _epochMmfFailed;
        private static bool _epochPollerStarted;
        // Must match the request value the runner writes via InitializeEpochFile (0), NOT a sentinel
        // distinct from it. The poller thread only starts once MutantControl is first touched, which is
        // the first IsActive() call from mutated code - i.e., only once the first test has already begun
        // running, with unpredictable OS thread-start latency before this thread gets its first time
        // slice. A sentinel that guarantees "the first observed value looks new" would flush+reset
        // whatever that already-running test registered before the runner ever requests epoch 1,
        // discarding real coverage into a flush nobody reads. Starting equal to the file's true initial
        // value means the poller does nothing until the runner's first genuine request (>= 1) arrives.
        private static int _lastHandledEpoch = 0;

        // this attribute will be set by the Stryker Data Collector before each test
        public static bool CaptureCoverage;
        public static int ActiveMutant = -2;
        public const int ActiveMutantNotInitValue = -2;

        static MutantControl()
        {
            // Check for MTP file-based coverage mode at class initialization
            // Environment variable contains only the filename, not the full path
            string coverageFileName = System.Environment.GetEnvironmentVariable("STRYKER_COVERAGE_FILE") ?? string.Empty;

            if (!string.IsNullOrEmpty(coverageFileName))
            {
                // Construct full path using temp directory
                _cachedCoverageFilePath = BuildCoverageFilePath(coverageFileName);
                _coverageFilePathCached = true;
                CaptureCoverage = true;

                // Register for process exit to flush coverage data
                if (!_processExitRegistered)
                {
                    System.AppDomain.CurrentDomain.ProcessExit += delegate { FlushCoverageToFile(); };
                    _processExitRegistered = true;
                }
            }

            string epochFileName = System.Environment.GetEnvironmentVariable("STRYKER_COVERAGE_EPOCH_FILE") ?? string.Empty;
            if (!string.IsNullOrEmpty(epochFileName))
            {
                // Suffixed per mutated assembly, like the coverage file: this copy of the class relays
                // only its own coverage, and the runner waits for every relay before reading
                _cachedEpochFilePath = BuildCoverageFilePath(epochFileName);
                EnsureEpochFileExists();
                EnsureEpochPollerStarted();
            }
        }

        public static void InitCoverage()
        {
            ResetCoverage();
        }

        public static void ResetCoverage()
        {
            _coveredMutants = new System.Collections.Generic.List<int>();
            _coveredStaticMutants = new System.Collections.Generic.List<int>();
        }

        public static void ResetActiveMutant()
        {
            ActiveMutant = ActiveMutantNotInitValue;
        }

        public static void SetActiveMutantViaEnvironmentVariable(int mutantId)
        {
            // Ensure we never assign null to a non-nullable string
            string environmentVariableName = System.Environment.GetEnvironmentVariable("STRYKER_MUTANT_ID_CONTROL_VAR") ?? string.Empty;
            if (environmentVariableName.Length > 0)
            {
                System.Environment.SetEnvironmentVariable(environmentVariableName, mutantId.ToString());
            }
            ActiveMutant = ActiveMutantNotInitValue;
        }

        private static bool TryReadMutantFromFile(out int mutantId)
        {
            mutantId = -1;

            // Cache the mutant file path to avoid repeated environment variable lookups
            if (!_mutantFilePathCached)
            {
                // coalesce null to empty string so _cachedMutantFilePath is never null
                _cachedMutantFilePath = System.Environment.GetEnvironmentVariable("STRYKER_MUTANT_FILE") ?? string.Empty;
                _mutantFilePathCached = true;
            }

            if (string.IsNullOrEmpty(_cachedMutantFilePath))
            {
                return false;
            }

            // Fast path: read the active mutant id from a memory-mapped view of the file (see the field
            // comments above). This is a plain memory read - no filesystem stat or content read per call.
            if (!_mutantMmfFailed)
            {
                if (!_mutantMmfReady)
                {
                    EnsureMutantMmf();
                }

                if (_mutantMmfReady)
                {
                    try
                    {
                        mutantId = ((System.IO.MemoryMappedFiles.MemoryMappedViewAccessor)_mutantAccessor).ReadInt32(0);
                        return true;
                    }
                    catch
                    {
                        // The mapping became unusable; fall back to reading the file directly from now on.
                        _mutantMmfFailed = true;
                    }
                }
            }

            // Fallback (no memory-mapped view could be created): read the 4-byte mutant id straight from
            // the file on every call. Correct but slower; only used if memory mapping is unavailable.
            return TryReadMutantFromFileDirect(out mutantId);
        }

        private static void EnsureMutantMmf()
        {
            if (_mutantMmfReady || _mutantMmfFailed)
            {
                return;
            }

            // The runner creates the file (with the initial -1) before the test host starts, so it
            // normally exists on the first call. If it is not there yet, leave things unmapped and retry
            // on a later call rather than giving up permanently.
            if (!System.IO.File.Exists(_cachedMutantFilePath))
            {
                return;
            }

            try
            {
                // FileShare.ReadWrite lets the runner keep writing the file while the test host keeps it mapped.
                // leaveOpen: false means the mapping owns and disposes the stream with it.
                System.IO.FileStream stream = new System.IO.FileStream(
                    _cachedMutantFilePath,
                    System.IO.FileMode.Open,
                    System.IO.FileAccess.Read,
                    System.IO.FileShare.ReadWrite);

                System.IO.MemoryMappedFiles.MemoryMappedFile mmf = System.IO.MemoryMappedFiles.MemoryMappedFile.CreateFromFile(
                    stream,
                    null,
                    4,
                    System.IO.MemoryMappedFiles.MemoryMappedFileAccess.Read,
                    System.IO.HandleInheritability.None,
                    false);

                System.IO.MemoryMappedFiles.MemoryMappedViewAccessor accessor = mmf.CreateViewAccessor(0, 4, System.IO.MemoryMappedFiles.MemoryMappedFileAccess.Read);

                _mutantMmf = mmf;
                _mutantAccessor = accessor;
                _mutantMmfReady = true;
            }
            catch
            {
                // Memory mapping is unavailable in this environment; the direct-read fallback keeps the active mutant correct (just slower).
                _mutantMmfFailed = true;
            }
        }

        private static bool TryReadMutantFromFileDirect(out int mutantId)
        {
            mutantId = -1;

            if (!System.IO.File.Exists(_cachedMutantFilePath))
            {
                return false;
            }

            try
            {
                byte[] bytes = System.IO.File.ReadAllBytes(_cachedMutantFilePath);
                if (bytes.Length >= 4)
                {
                    mutantId = System.BitConverter.ToInt32(bytes, 0);
                    return true;
                }
            }
            catch
            {
                // Ignore file read errors
            }
            return false;
        }

        public static System.Collections.Generic.IList<int>[] GetCoverageData()
        {
            System.Collections.Generic.IList<int>[] result = new System.Collections.Generic.IList<int>[] { _coveredMutants, _coveredStaticMutants };
            ResetCoverage();
            return result;
        }

        /// <summary>
        /// Resolves the coverage file this copy of the class writes to, from the file name the runner
        /// handed out through STRYKER_COVERAGE_FILE.
        /// A test host loads one copy of this class per mutated assembly and every copy reads the same
        /// environment variable, so the name is suffixed with the assembly this copy belongs to: without
        /// it, the copies overwrite each other's coverage and only the last flush survives. The runner
        /// reads every file whose name starts with the name it handed out (minus the extension).
        /// </summary>
        private static string BuildCoverageFilePath(string coverageFileName)
        {
            string directory = System.IO.Path.GetTempPath();
            string discriminator = GetOwningAssemblyDiscriminator();
            if (discriminator.Length == 0)
            {
                return System.IO.Path.Combine(directory, coverageFileName);
            }

            string nameWithoutExtension = System.IO.Path.GetFileNameWithoutExtension(coverageFileName);
            string extension = System.IO.Path.GetExtension(coverageFileName);
            return System.IO.Path.Combine(directory, nameWithoutExtension + "-" + discriminator + extension);
        }

        /// <summary>
        /// Identifies the assembly this copy of the class was compiled into, uniquely among the copies a
        /// test host loads. The assembly name alone cannot do that: characters that a file name cannot hold
        /// are replaced, so names such as A.B and A-B would collide, and the name has to be trimmed to keep
        /// the path within length limits, so names sharing a long prefix would collide too. Two colliding
        /// copies would write to the same file and hide each other's coverage, which is exactly what this
        /// naming exists to prevent, so the module id - assigned per compiled module - is what separates
        /// them; the trimmed name is only kept to make the file recognizable while debugging.
        /// Returns an empty string when the identity cannot be read.
        /// </summary>
        private static string GetOwningAssemblyDiscriminator()
        {
            string name;
            System.Guid moduleId;
            try
            {
                name = typeof(MutantControl).Assembly.GetName().Name ?? string.Empty;
                moduleId = typeof(MutantControl).Module.ModuleVersionId;
            }
            catch (System.Exception)
            {
                // Never fail the tests over coverage bookkeeping
                return string.Empty;
            }

            int length = name.Length < 16 ? name.Length : 16;
            char[] safeName = new char[length];
            for (int i = 0; i < length; i++)
            {
                char current = name[i];
                safeName[i] = char.IsLetterOrDigit(current) ? current : '-';
            }

            return new string(safeName) + "-" + moduleId.ToString("N");
        }

        /// <summary>
        /// Writes accumulated coverage data to a file for MTP runner IPC.
        /// Called automatically on process exit to capture all coverage from tests run in this process.
        /// Format: "coveredMutants;staticMutants" (comma-separated IDs)
        /// </summary>
        public static void FlushCoverageToFile()
        {
            if (!_coverageFilePathCached)
            {
                // Environment variable contains only the filename
                string coverageFileName = System.Environment.GetEnvironmentVariable("STRYKER_COVERAGE_FILE") ?? string.Empty;
                if (!string.IsNullOrEmpty(coverageFileName))
                {
                    // Construct full path using temp directory
                    _cachedCoverageFilePath = BuildCoverageFilePath(coverageFileName);
                }
                _coverageFilePathCached = true;
            }

            if (string.IsNullOrEmpty(_cachedCoverageFilePath))
            {
                return;
            }

            try
            {
                lock (_coverageLock)
                {
                    string covered = string.Join(",", _coveredMutants);
                    string staticMutants = string.Join(",", _coveredStaticMutants);
                    string content = covered + ";" + staticMutants;
                    System.IO.File.WriteAllText(_cachedCoverageFilePath, content);
                    ResetCoverage();
                }
            }
            catch (System.Exception ex)
            {
                // Do not fail tests due to coverage write issues; log for diagnostics instead.
                System.Diagnostics.Debug.WriteLine(string.Format("[Stryker] Failed to flush coverage to file '{0}': {1}", _cachedCoverageFilePath, ex));
            }
        }

        private static void EnsureEpochPollerStarted()
        {
            if (_epochPollerStarted)
            {
                return;
            }
            _epochPollerStarted = true;

            System.Threading.Thread thread = new System.Threading.Thread(EpochPollerLoop);
            thread.IsBackground = true;
            thread.Name = "StrykerCoverageEpochRelay";
            thread.Start();
        }

        /// <summary>
        /// Creates this copy's epoch relay file, initialized to request=0/ack=0 to match the poller's
        /// starting state. Done synchronously from the static constructor, not on the poller thread: the
        /// runner discovers a test host's relays by listing files right after a test finishes, and has to
        /// find a relay for every assembly whose mutated code ran during that test.
        /// </summary>
        private static void EnsureEpochFileExists()
        {
            try
            {
                if (System.IO.File.Exists(_cachedEpochFilePath))
                {
                    return;
                }

                System.IO.FileStream stream = new System.IO.FileStream(
                    _cachedEpochFilePath,
                    System.IO.FileMode.Create,
                    System.IO.FileAccess.ReadWrite,
                    System.IO.FileShare.ReadWrite);
                try
                {
                    byte[] emptyRelay = new byte[8];
                    stream.Write(emptyRelay, 0, emptyRelay.Length);
                    stream.Flush();
                }
                finally
                {
                    stream.Close();
                }
            }
            catch
            {
                // Mapping is retried by the poller; if that fails too the runner's ack wait times out and
                // the affected test's coverage is reported as Dubious rather than hanging.
            }
        }

        private static void EnsureEpochMmf()
        {
            if (_epochMmfReady || _epochMmfFailed)
            {
                return;
            }

            try
            {
                System.IO.FileStream stream = new System.IO.FileStream(
                    _cachedEpochFilePath,
                    System.IO.FileMode.OpenOrCreate,
                    System.IO.FileAccess.ReadWrite,
                    System.IO.FileShare.ReadWrite);

                System.IO.MemoryMappedFiles.MemoryMappedFile mmf = System.IO.MemoryMappedFiles.MemoryMappedFile.CreateFromFile(
                    stream,
                    null,
                    8,
                    System.IO.MemoryMappedFiles.MemoryMappedFileAccess.ReadWrite,
                    System.IO.HandleInheritability.None,
                    false);

                System.IO.MemoryMappedFiles.MemoryMappedViewAccessor accessor = mmf.CreateViewAccessor(0, 8, System.IO.MemoryMappedFiles.MemoryMappedFileAccess.ReadWrite);

                _epochMmf = mmf;
                _epochAccessor = accessor;
                _epochMmfReady = true;
            }
            catch
            {
                // Memory mapping unavailable; the runner's ack-wait will time out and the affected test's
                // coverage is reported as Dubious rather than hanging indefinitely.
                _epochMmfFailed = true;
            }
        }

        private static void EpochPollerLoop()
        {
            while (true)
            {
                try
                {
                    if (!_epochMmfReady && !_epochMmfFailed)
                    {
                        EnsureEpochMmf();
                    }

                    if (_epochMmfReady)
                    {
                        System.IO.MemoryMappedFiles.MemoryMappedViewAccessor accessor =
                            (System.IO.MemoryMappedFiles.MemoryMappedViewAccessor)_epochAccessor;
                        int requestedEpoch = accessor.ReadInt32(0);
                        if (requestedEpoch != _lastHandledEpoch)
                        {
                            // The previous test (or nothing, on the very first iteration) has finished;
                            // publish what it covered and reset before the next test starts.
                            FlushCoverageToFile();
                            _lastHandledEpoch = requestedEpoch;
                            accessor.Write(4, requestedEpoch);
                            accessor.Flush();
                        }
                    }
                }
                catch
                {
                    // Best-effort relay; a transient failure here surfaces as an ack timeout on the
                    // runner side for the current test, not a crash of the test host.
                }

                System.Threading.Thread.Sleep(1);
            }
        }

        private static void CurrentDomain_ProcessExit(object sender, System.EventArgs e)
        {
            System.GC.KeepAlive(_coveredMutants);
            System.GC.KeepAlive(_coveredStaticMutants);
        }

        // check with: Stryker.MutantControl.IsActive(ID)
        public static bool IsActive(int id)
        {
            if (CaptureCoverage)
            {
                RegisterCoverage(id);
                return false;
            }

            // Check for file-based mutant control (used by MTP runner for process reuse)
            // Cache check: only call TryReadMutantFromFile if we might be using file-based control
            if (!_mutantFilePathCached || !string.IsNullOrEmpty(_cachedMutantFilePath))
            {
                int fileMutantId;
                if (TryReadMutantFromFile(out fileMutantId))
                {
                    ActiveMutant = fileMutantId;
                }

                // If we cached the file path and it's set, always use file-based control
                if (_mutantFilePathCached && !string.IsNullOrEmpty(_cachedMutantFilePath))
                {
                    return id == ActiveMutant;
                }
            }

            // lazy load the active mutant id from the environment variable (used by VSTest runner)
            if (ActiveMutant == ActiveMutantNotInitValue)
            {
                // coalesce null to empty string to avoid null-to-non-nullable conversion
                string environmentVariableName = System.Environment.GetEnvironmentVariable("STRYKER_MUTANT_ID_CONTROL_VAR") ?? string.Empty;
                if (environmentVariableName.Length > 0)
                {
                    string environmentVariable = System.Environment.GetEnvironmentVariable(environmentVariableName) ?? string.Empty;
                    if (string.IsNullOrEmpty(environmentVariable))
                    {
                        ActiveMutant = -1;
                    }
                    else
                    {
                        ActiveMutant = int.Parse(environmentVariable);
                    }
                }
                else
                {
                    ActiveMutant = -1;
                }
            }

            return id == ActiveMutant;
        }

        private static void RegisterCoverage(int id)
        {
            lock (_coverageLock)
            {
                if (!_coveredMutants.Contains(id))
                {
                    _coveredMutants.Add(id);
                }
                if (MutantContext.InStatic() && !_coveredStaticMutants.Contains(id))
                {
                    _coveredStaticMutants.Add(id);
                }
            }
        }
    }
}
