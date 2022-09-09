using System;
using System.Collections.Generic;
using System.Threading;

namespace Stryker
{
    public static class MutantControl
    {
        private static HashSet<int> _coveredMutants;
        private static HashSet<int> _coveredStaticdMutants;
        private static List<int> _hitTrace = new List<int>(10000);
        private static object _coverageLock = new object();

        public static bool CaptureCoverage;
        public static bool CaptureTrace;
        // this attribute will be set by the Stryker Data Collector before each test
        private static int _activeMutant = -2;
        public static int ActiveMutantSeen;
        public const int ActiveMutantNotInitValue = -2;
        
        static MutantControl()
        {
            InitCoverage();
        }

        public static void InitCoverage()
        {
            ResetCoverage();
        }

        public static void ResetCoverage()
        {
            _coveredMutants = new HashSet<int>();
            _coveredStaticdMutants = new HashSet<int>();
        }

        public static ISet<int>[] GetCoverageData()
        {
            ISet<int>[] result = new ISet<int>[]{_coveredMutants, _coveredStaticdMutants};
            ResetCoverage();
            return result;
        }

        public static IList<int> GetTrace()
        {
            return Interlocked.Exchange(ref _hitTrace, new List<int>(10000));
        }

        public static void SetActiveMutant(int mutant)
        {
            _activeMutant = mutant;
            ActiveMutantSeen = -1;
        }

        // check with: Stryker.MutantControl.IsActive(ID)
        public static bool IsActive(int id)
        {
            if (CaptureCoverage)
            {
                RegisterCoverage(id);
                return false;
            }
            if (_activeMutant == ActiveMutantNotInitValue)
            {
                string environmentVariable = Environment.GetEnvironmentVariable("ActiveMutation");
                if (string.IsNullOrEmpty(environmentVariable))
                {
                    _activeMutant = -1;
                }
                else
                {
                    _activeMutant = int.Parse(environmentVariable);
                }
            }

            if (CaptureTrace)
            {
                _hitTrace.Add(id);
            }

            if (id == _activeMutant)
            {
                ActiveMutantSeen = _activeMutant;
                return true;
            }

            return false;
        }

        private static void RegisterCoverage(int id)
        {
            lock (_coverageLock)
            {
                _coveredMutants.Add(id);
                if (MutantContext.InStatic())
                {
                    _coveredStaticdMutants.Add(id);
                }
            }
        }
    }
}
