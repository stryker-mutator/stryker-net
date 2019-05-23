using System;
using System.Collections.Generic;

namespace Stryker
{
    using Core.InjectedHelpers.Coverage;

    public static class MutantControl
    {
        private static HashSet<int> _coveredMutants;
        private static bool usePipe;
        private static string pipeName;
        private static string envName;
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
            if (channel != null)
            {
                channel?.Dispose();
                channel = null;
            }
            var coverageMode = Environment.GetEnvironmentVariable(EnvironmentPipeName)??string.Empty;
            if (coverageMode.StartsWith("pipe:"))
            {
                pipeName = coverageMode.Substring(5);
                usePipe = true;
                captureCoverage = true;
                channel = CommunicationChannel.Client(pipeName, 100);
                channel.RaiseReceivedMessage += Channel_RaiseReceivedMessage;
                channel.Start();
            }
            else if (coverageMode.StartsWith("env:"))
            {
                envName = coverageMode.Substring(4);
                captureCoverage = true;
                usePipe = false;
            }
            if (captureCoverage)
            {
                _coveredMutants = new HashSet<int>();
            }
        }

        private static void Channel_RaiseReceivedMessage(object sender, string args)
        {
            if (!args.StartsWith("DUMP"))
            {
                return;
            }
            HashSet<int> temp;
            lock (_coveredMutants)
            {
                temp = _coveredMutants;
                _coveredMutants = new HashSet<int>();
            }
            DumpState(temp);
        }

        private static void CurrentDomain_ProcessExit(object sender, EventArgs e)
        {
            Console.WriteLine($"AppDomainUnload");
            DumpState(_coveredMutants);
            GC.KeepAlive(_coveredMutants);
        }

        public static void DumpState(HashSet<int> state = null)
        {
            string report;
            state = state ?? _coveredMutants;
            lock (state)
            {
                report = string.Join(",", state);
            }

            Console.WriteLine($"DumpState {state.Count}");
            channel.SendText(report);
        }

        // check with: Stryker.MutantControl.IsActive(ID)
        public static bool IsActive(int id)
        {
            if (captureCoverage)
            {
                lock (_coveredMutants)
                {
                    if (_coveredMutants.Add(id))
                    {
                        if (!usePipe)
                        {
                            Environment.SetEnvironmentVariable(envName, string.Join(",", _coveredMutants));
                        }
                    }
                }
            }
            return ActiveMutation == id;
        }

        private static int ActiveMutation { get; set;}
    }
}
