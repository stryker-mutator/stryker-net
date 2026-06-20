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
        private static long _lastMutantFileVersion = -1;
        // Initialized to avoid nullable warnings/errors
        private static string _cachedMutantFilePath = string.Empty;
        private static bool _mutantFilePathCached;

        // The MTP runner reuses a single test host process across mutant runs and signals the active
        // mutant by rewriting a file (see SingleMicrosoftTestPlatformRunner). IsActive runs in the hot
        // path of mutated code - potentially millions of calls per test run - so reading that file on
        // every call (the original behaviour) made the MTP runner far slower than the VSTest runner.
        // A FileSystemWatcher flips _mutantFileDirty only when the runner writes a new mutant id
        // between runs; within a run every check is then a cheap bool + int comparison with no I/O.
        // Held as object (initialized to a non-null sentinel) only to root the watcher against
        // garbage collection; it is never read back. Typing it as the watcher would require a
        // nullable-friendly initializer that is not available in the C# v2 feature set this file targets.
        private static object _mutantFileWatcher = new System.Object();
        private static bool _mutantFileWatcherInitialized;
        private static bool _mutantFileWatcherFailed;
        private static volatile bool _mutantFileDirty = true;

        // Coverage file path for MTP runner (file-based IPC)
        private static string _cachedCoverageFilePath = string.Empty;
        private static bool _coverageFilePathCached;
        private static bool _processExitRegistered;

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
                _cachedCoverageFilePath = System.IO.Path.Combine(System.IO.Path.GetTempPath(), coverageFileName);
                _coverageFilePathCached = true;
                CaptureCoverage = true;
                
                // Register for process exit to flush coverage data
                if (!_processExitRegistered)
                {
                    System.AppDomain.CurrentDomain.ProcessExit += delegate { FlushCoverageToFile(); };
                    _processExitRegistered = true;
                }
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

            EnsureMutantFileWatcher();

            if (_mutantFileWatcherFailed)
            {
                // No watcher could be created on this platform; fall back to detecting changes via the
                // file's last-write timestamp. Correct, just slower (a file stat per check).
                return TryReadMutantFromFileByTimestamp(out mutantId);
            }

            // Fast path: nothing has changed since the last successful read, so reuse the cached
            // ActiveMutant without touching the filesystem.
            if (!_mutantFileDirty && ActiveMutant != ActiveMutantNotInitValue)
            {
                return false;
            }

            if (!System.IO.File.Exists(_cachedMutantFilePath))
            {
                return false;
            }

            try
            {
                string content = System.IO.File.ReadAllText(_cachedMutantFilePath).Trim();
                if (int.TryParse(content, out mutantId))
                {
                    // Only clear the dirty flag on a successful read so a partial/mid-write read is
                    // retried on the next call instead of silently caching a stale mutant id.
                    _mutantFileDirty = false;
                    return true;
                }
            }
            catch
            {
                // Ignore file read errors; the dirty flag stays set so the next call retries.
            }
            return false;
        }

        private static bool TryReadMutantFromFileByTimestamp(out int mutantId)
        {
            mutantId = -1;

            if (!System.IO.File.Exists(_cachedMutantFilePath))
            {
                return false;
            }

            try
            {
                System.IO.FileInfo fileInfo = new System.IO.FileInfo(_cachedMutantFilePath);
                long currentVersion = fileInfo.LastWriteTimeUtc.Ticks;

                // Only re-read if file has changed or we haven't read it yet
                if (currentVersion != _lastMutantFileVersion || ActiveMutant == ActiveMutantNotInitValue)
                {
                    string content = System.IO.File.ReadAllText(_cachedMutantFilePath).Trim();
                    if (int.TryParse(content, out mutantId))
                    {
                        _lastMutantFileVersion = currentVersion;
                        return true;
                    }
                }
            }
            catch
            {
                // Ignore file read errors
            }
            return false;
        }

        private static void EnsureMutantFileWatcher()
        {
            if (_mutantFileWatcherInitialized)
            {
                return;
            }
            _mutantFileWatcherInitialized = true;

            try
            {
                string directory = System.IO.Path.GetDirectoryName(_cachedMutantFilePath) ?? string.Empty;
                string fileName = System.IO.Path.GetFileName(_cachedMutantFilePath);
                if (string.IsNullOrEmpty(directory) || string.IsNullOrEmpty(fileName))
                {
                    _mutantFileWatcherFailed = true;
                    return;
                }

                System.IO.FileSystemWatcher watcher = new System.IO.FileSystemWatcher(directory, fileName);
                watcher.NotifyFilter = System.IO.NotifyFilters.LastWrite
                    | System.IO.NotifyFilters.Size
                    | System.IO.NotifyFilters.CreationTime
                    | System.IO.NotifyFilters.FileName;
                // C# 2.0 allows a parameterless anonymous method to match any delegate signature.
                watcher.Changed += delegate { _mutantFileDirty = true; };
                watcher.Created += delegate { _mutantFileDirty = true; };
                watcher.Deleted += delegate { _mutantFileDirty = true; };
                watcher.Renamed += delegate { _mutantFileDirty = true; };
                watcher.EnableRaisingEvents = true;

                // Keep a static reference so the watcher lives for the (reused) test host process and
                // is not collected.
                _mutantFileWatcher = watcher;
            }
            catch
            {
                // FileSystemWatcher may be unavailable in some environments. Fall back to the
                // timestamp-based change detection, which works correctly without a watcher.
                _mutantFileWatcherFailed = true;
            }
        }

        public static System.Collections.Generic.IList<int>[] GetCoverageData()
        {
            System.Collections.Generic.IList<int>[] result = new System.Collections.Generic.IList<int>[] { _coveredMutants, _coveredStaticMutants };
            ResetCoverage();
            return result;
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
                    _cachedCoverageFilePath = System.IO.Path.Combine(System.IO.Path.GetTempPath(), coverageFileName);
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
