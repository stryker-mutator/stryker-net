using System;
using System.Collections.Generic;
using System.Threading;

namespace Stryker
{
    /// <summary>
    /// This static class hosts mutant (runtime) switching helper functions.
    /// As it is injected in every mutated project, it needs to remain compatible with old C# syntax.
    /// So please refrain from using modern short form (e.g. arrow expression) or syntactic sugar as it may break compatibility in various ways.
    /// </summary>
    public static class MutantControl
    {
        private static HashSet<int> _coveredMutants;
        private static HashSet<int> _coveredStaticdMutants;
        private static List<int> _hitTrace = new List<int>(10000);
        private static object _coverageLock = new object();
        private static int _activeMutant = -2;

        private static bool _captureCoverage;
        private static bool _captureTrace;
        // this attribute will be set by the Stryker Data Collector before each test
        private static bool _activeMutantSeen;
        
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
            _activeMutantSeen = false;
        }

        public static bool ActiveMutantSeen()
        {
            return _activeMutantSeen;
        }

        public static void CaptureCoverage(bool mode)
        {
            _captureCoverage = mode;
        }

        public static void CaptureTrace(bool mode)
        {
            _captureTrace = mode;
        }

        // check with: Stryker.MutantControl.IsActive(ID)
        public static bool IsActive(int id)
        {
            if (_captureCoverage)
            {
                RegisterCoverage(id);
                return false;
            }

            if (_captureTrace)
            {
                _hitTrace.Add(id);
            }

            if (id != _activeMutant)
            {
                return false;
            }
            _activeMutantSeen = true;
            return true;

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
