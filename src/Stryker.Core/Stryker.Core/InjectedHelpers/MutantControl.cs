using System;
using System.Collections.Generic;
using System.Text;

namespace Stryker
{
    internal static class MutantControl
    {
        private static List<int> _coveredMutants;
        private static List<int> _coveredStaticdMutants;
        private static string envName;
        private static Object _coverageLock = new Object();

        // this attribute will be set by the Stryker Data Collector before each test
        public static bool CaptureCoverage;
        public static int ActiveMutant = -2;
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

        private static void CurrentDomain_ProcessExit(object sender, EventArgs e)
        {
            GC.KeepAlive(_coveredMutants);
            GC.KeepAlive(_coveredStaticdMutants);
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
                if (MutantContext.InStatic() && !_coveredStaticdMutants.Contains(id))
                {
                    _coveredStaticdMutants.Add(id);
                }
            }
        }
    }
}
