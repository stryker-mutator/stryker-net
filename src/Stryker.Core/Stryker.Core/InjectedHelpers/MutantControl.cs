namespace Stryker
{

    public static class MutantControl
    {
        private static System.Collections.Generic.List<int> _coveredMutants = new System.Collections.Generic.List<int>();
        private static System.Collections.Generic.List<int> _coveredStaticMutants = new System.Collections.Generic.List<int>();
        private static string envName = string.Empty;
        private static System.Object _coverageLock = new System.Object();
        private static long _lastMutantFileVersion = -1;

        // this attribute will be set by the Stryker Data Collector before each test
        public static bool CaptureCoverage;
        public static int ActiveMutant = -2;
        public const int ActiveMutantNotInitValue = -2;

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
            string environmentVariableName = System.Environment.GetEnvironmentVariable("STRYKER_MUTANT_ID_CONTROL_VAR");
            if (environmentVariableName != null)
            {
                System.Environment.SetEnvironmentVariable(environmentVariableName, mutantId.ToString());
            }
            ActiveMutant = ActiveMutantNotInitValue;
        }

        private static bool TryReadMutantFromFile(out int mutantId)
        {
            mutantId = -1;
            string mutantFilePath = System.Environment.GetEnvironmentVariable("STRYKER_MUTANT_FILE");
            if (string.IsNullOrEmpty(mutantFilePath) || !System.IO.File.Exists(mutantFilePath))
            {
                return false;
            }

            try
            {
                var fileInfo = new System.IO.FileInfo(mutantFilePath);
                long currentVersion = fileInfo.LastWriteTimeUtc.Ticks;

                // Only re-read if file has changed or we haven't read it yet
                if (currentVersion != _lastMutantFileVersion || ActiveMutant == ActiveMutantNotInitValue)
                {
                    string content = System.IO.File.ReadAllText(mutantFilePath).Trim();
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
            string mutantFilePath = System.Environment.GetEnvironmentVariable("STRYKER_MUTANT_FILE");
            if (!string.IsNullOrEmpty(mutantFilePath))
            {
                if (TryReadMutantFromFile(out int fileMutantId))
                {
                    ActiveMutant = fileMutantId;
                }
                return id == ActiveMutant;
            }

            // lazy load the active mutant id from the environment variable (used by VSTest runner)
            if (ActiveMutant == ActiveMutantNotInitValue)
            {
#pragma warning disable CS8600
                // get the environment variable storing the mutation id
                string environmentVariableName = System.Environment.GetEnvironmentVariable("STRYKER_MUTANT_ID_CONTROL_VAR");
                if (environmentVariableName != null)
                {
                    string environmentVariable = System.Environment.GetEnvironmentVariable(environmentVariableName);
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
