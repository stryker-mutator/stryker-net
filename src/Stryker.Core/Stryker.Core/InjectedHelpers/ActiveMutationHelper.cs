using System;
using System.Collections.Generic;
using System.IO.Pipes;
using System.Text;

namespace Stryker
{
    using Core.InjectedHelpers.Coverage;

    public static class MutantControl
    {
        private static HashSet<int> _coveredMutants;
        private static bool captureCoverage;
        private static string pipeName;

        public const string EnvironmentPipeName = "Coverage";

        static MutantControl()
        {
            InitCoverage();
            if (captureCoverage)
            {
                AppDomain.CurrentDomain.ProcessExit += CurrentDomain_ProcessExit;
            }
        }

        public static void InitCoverage()
        {
            ActiveMutation = int.Parse(Environment.GetEnvironmentVariable("ActiveMutation") ?? "-1");
            pipeName = Environment.GetEnvironmentVariable(EnvironmentPipeName);
            captureCoverage = !string.IsNullOrEmpty(pipeName);
            if (captureCoverage)
            {
                _coveredMutants = new HashSet<int>();
            }
        }

        private static void CurrentDomain_ProcessExit(object sender, EventArgs e)
        {
            DumpState();
        }

        public static void DumpState()
        {
            var report = new StringBuilder();
            foreach (var coveredMutant in _coveredMutants)
            {
                report.Append($"{coveredMutant},");
            }

            using (var channel = CommunicationChannel.Client(pipeName, 10))
            {
                channel.SendText(report.ToString());
            }
        }
        // check with: Stryker.MutantControl.IsActive(ID)
        public static bool IsActive(int id)
        {
            if (captureCoverage)
            {
                if (!_coveredMutants.Contains(id))
                    _coveredMutants.Add(id);
            }
            return ActiveMutation == id;
        }

        private static int ActiveMutation { get; set;}
    }
}
