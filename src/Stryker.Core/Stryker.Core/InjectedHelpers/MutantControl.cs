using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Stryker
{
    using Core.InjectedHelpers.Coverage;

    public static class MutantControl
    {
        private static HashSet<int> _coveredMutants;
        private static bool usePipe;
        private static string pipeName;
        private static bool captureCoverage;
        private static CommunicationChannel channel;

        public const string EnvironmentPipeName = "Coverage";

        static MutantControl()
        {
            InitCoverage();
            if (usePipe)
            {
                AppDomain.CurrentDomain.ProcessExit += CurrentDomain_ProcessExit;
            }
        }

        public static void InitCoverage()
        {
            ActiveMutation = int.Parse(Environment.GetEnvironmentVariable("ActiveMutation") ?? "-1");
            pipeName = Environment.GetEnvironmentVariable(EnvironmentPipeName);
            usePipe = !string.IsNullOrEmpty(pipeName);
            if (channel != null)
            {
                channel?.Dispose();
                channel = null;
            }

            captureCoverage = usePipe;
            if (captureCoverage)
            {
                _coveredMutants = new HashSet<int>();
            }

            if (usePipe)
            {
                channel = CommunicationChannel.Client(pipeName, 100);
                channel.RaiseReceivedMessage += Channel_RaiseReceivedMessage;
                channel.Start();
            }
        }

        private static void Channel_RaiseReceivedMessage(object sender, string args)
        {
            if (args == "DUMP")
            {
                HashSet<int> temp;
                lock (_coveredMutants)
                {
                    temp = _coveredMutants;
                    _coveredMutants = new HashSet<int>();
                }
                DumpState(temp);
            }
        }

        private static void CurrentDomain_ProcessExit(object sender, EventArgs e)
        {
            DumpState(_coveredMutants);
            GC.KeepAlive(_coveredMutants);
        }

        public static void DumpState(HashSet<int> state = null)
        {
            string report;
            state = state ?? _coveredMutants;
            lock (state)
            {
                report = string.Join(',', state);
            }

            channel.SendText(report);
        }

        // check with: Stryker.MutantControl.IsActive(ID)
        public static bool IsActive(int id)
        {
            if (usePipe)
            {
                lock (_coveredMutants)
                {
                    if (Environment.GetEnvironmentVariable("CoverageReset") != null)
                    {
                        Environment.SetEnvironmentVariable("CoverageReset", null);
                        _coveredMutants.Clear();
                    }
                    if (_coveredMutants.Add(id))
                    {
                        Environment.SetEnvironmentVariable("Coverage", string.Join(',', _coveredMutants));
                    }
                }
            }
            return ActiveMutation == id;
        }

        private static int ActiveMutation { get; set;}
    }
}
