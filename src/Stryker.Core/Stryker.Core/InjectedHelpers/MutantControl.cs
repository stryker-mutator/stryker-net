using System;
using System.Collections.Generic;
using System.Text;

namespace Stryker
{
    public static class MutantControl
    {
        private static List<int> _coveredMutants;
        private static List<int> _coveredStaticdMutants;
        private static string envName;
        private static Object _coverageLock = new Object();

        public static bool CaptureCoverage;
        // this attribute will be set by the Stryker Data Collector before each test
        public static int ActiveMutant = -2;
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
            _coveredMutants = new List<int>();
            _coveredStaticdMutants = new List<int>();
        }

        public static IList<int>[] GetCoverageData()
        {
            IList<int>[] result = new IList<int>[]{_coveredMutants, _coveredStaticdMutants};
            ResetCoverage();
            return result;
        }

        // check with: Stryker.MutantControl.IsActive(ID)
        public static bool IsActive(int id)
        {
            if (CaptureCoverage)
            {
                RegisterCoverage(id);
                return false;
            }
            if (ActiveMutant == ActiveMutantNotInitValue)
            {
                string environmentVariable = Environment.GetEnvironmentVariable("ActiveMutation");
                if (string.IsNullOrEmpty(environmentVariable))
                {
                    ActiveMutant = -1;
                }
                else
                {
                    ActiveMutant = int.Parse(environmentVariable);
                }
            }

            if (id == ActiveMutant)
            {
                ActiveMutantSeen = ActiveMutant;
                return true;
            }
            return false;
        }

        private static void RegisterCoverage(int id)
        {
            lock (_coverageLock)
            {
                if (!_coveredMutants.Contains(id))
                {
                    _coveredMutants.Add(id);
                }
                if (MutantContext.InStatic() && !_coveredStaticdMutants.Contains(id))
                {
                    _coveredStaticdMutants.Add(id);
                }
            }
        }
    }
}
