using System;
using System.Collections.Generic;
using System.Text;

namespace Stryker
{
    using Core.InjectedHelpers.Coverage;

    public static class MutantControl
    {
        private static HashSet<int> _coveredMutants;
        private static bool captureCoverage;
        private static string pipeName;
        private static CommunicationChannel channel;

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
            if (channel != null)
            {
                channel?.Dispose();
                channel = null;
            }
            if (captureCoverage)
            {
                _coveredMutants = new HashSet<int>();
                channel = CommunicationChannel.Client(pipeName, 10);
                channel.RaiseReceivedMessage += Channel_RaiseReceivedMessage;
                channel.Start();
            }
        }

        private static void Channel_RaiseReceivedMessage(object sender, string args)
        {
            if (args == "DUMP")
            {
                var temp = _coveredMutants;
                _coveredMutants = new HashSet<int>();
                DumpState(temp);
            }
        }

        private static void CurrentDomain_ProcessExit(object sender, EventArgs e)
        {
            DumpState(_coveredMutants);
        }

        public static void DumpState(HashSet<int> state = null)
        {
            var report = new StringBuilder();
            var firstTime = true;

            foreach (var coveredMutant in state??_coveredMutants)
            {
                if (firstTime)
                {
                    firstTime = false;
                }
                else
                {
                    report.Append(',');
                }
                report.Append($"{coveredMutant}");
            }

            channel.SendText(report.ToString());
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
