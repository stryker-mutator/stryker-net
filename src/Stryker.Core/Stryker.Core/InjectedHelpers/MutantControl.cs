using Stryker.Core.InjectedHelpers.Coverage;
using System;
using System.Collections.Generic;
using System.Text;

namespace Stryker
{
    public static class MutantControl
    {
        private static List<int> _coveredMutants;
        private static List<int> _covereStaticdMutants;
        private static bool usePipe;
        private static string pipeName;
        private static string envName;
        private static Object _coverageLock = new Object();

        // this attributs will be set by Stryker Data Collector before eachtest
        public static bool CaptureCoverage;
        public static int ActiveMutant = -2;
        public const int ActiveMutantNotInitValue = -2;
#if !STRYKER_NO_PIPE
        private static CommunicationChannel channel;
#endif
        public const string EnvironmentPipeName = "Coverage";

        static MutantControl()
        {
            InitCoverage();
            if (usePipe)
            {
#if !STRYKER_NO_DOMAIN
                AppDomain.CurrentDomain.ProcessExit += CurrentDomain_ProcessExit;
#endif
            }
        }

        public static void InitCoverage()
        {
            string coverageMode = Environment.GetEnvironmentVariable(EnvironmentPipeName) ?? string.Empty;
#if !STRYKER_NO_PIPE

            if (channel != null)
            {
                channel.Dispose();
                channel = null;
            }

            if (coverageMode.StartsWith("pipe:"))
            {
                Log("Use pipe for data transmission");
                pipeName = coverageMode.Substring(5);
                usePipe = true;
                CaptureCoverage = true;
                channel = CommunicationChannel.Client(pipeName, 100);
                channel.SetLogger(Log);
                channel.Start();
            }
#endif
            ResetCoverage();
        }

        public static void ResetCoverage()
        {
            _coveredMutants = new List<int>();
            _covereStaticdMutants = new List<int>();
        }

        public static IList<int>[] GetCoverageData()
        {
            IList<int>[] result = new IList<int>[]{_coveredMutants, _covereStaticdMutants};
            ResetCoverage();
            return result;
        }

        private static void CurrentDomain_ProcessExit(object sender, EventArgs e)
        {
            DumpState();
            GC.KeepAlive(_coveredMutants);
        }

        private static string BuildReport()
        {
            return string.Format("{0};{1}", string.Join(",", _coveredMutants), string.Join(",", _covereStaticdMutants));
        }

        public static void DumpState()
        {
            DumpState(BuildReport());
        }

        public static void DumpState(string report)
        {
#if !STRYKER_NO_PIPE
            channel.SendText(report);
#endif
        }

        private static void Log(string message)
        {
            // uncomment when you need to debug this component
            //Console.WriteLine("[" + DateTime.Now.ToString("HH:mm:ss.fff") + " DBG] " + message);
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
                if (MutantContext.InStatic() && !_covereStaticdMutants.Contains(id))
                {
                    _covereStaticdMutants.Add(id);
                }
            }
        }
    }
}