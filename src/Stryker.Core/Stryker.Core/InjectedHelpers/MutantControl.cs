namespace Stryker
{
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
            _cachedCoverageFilePath = System.Environment.GetEnvironmentVariable("STRYKER_COVERAGE_FILE") ?? string.Empty;
            _coverageFilePathCached = true;

            // Enable coverage capture if coverage file path is configured (MTP runner mode)
            if (!string.IsNullOrEmpty(_cachedCoverageFilePath))
            {
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

            if (string.IsNullOrEmpty(_cachedMutantFilePath) || !System.IO.File.Exists(_cachedMutantFilePath))
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

        public static System.Collections.Generic.IList<int>[] GetCoverageData()
        {
            System.Collections.Generic.IList<int>[] result = new System.Collections.Generic.IList<int>[] { _coveredMutants, _coveredStaticMutants };
            ResetCoverage();
            return result;
        }

        /// <summary>
        /// Gets coverage data and writes it to a file for MTP runner IPC.
        /// Called by MTP runner after each test completes.
        /// Format: "coveredMutants;staticMutants" (comma-separated IDs)
        /// </summary>
        public static void FlushCoverageToFile()
        {
            if (!_coverageFilePathCached)
            {
                _cachedCoverageFilePath = System.Environment.GetEnvironmentVariable("STRYKER_COVERAGE_FILE") ?? string.Empty;
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
