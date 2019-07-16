using Stryker.Core.InjectedHelpers.Coverage;
using System;
using System.Collections.Generic;

namespace Stryker
{
    public class MutantControl
    {
        private static HashSet<int> _coveredMutants;
        private static HashSet<int> _staticMutants;
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
                channel.Dispose();
                channel = null;
            }
            string coverageMode = Environment.GetEnvironmentVariable(EnvironmentPipeName) ?? string.Empty;
            if (coverageMode.StartsWith("pipe:"))
            {
                pipeName = coverageMode.Substring(5);
                usePipe = true;
                captureCoverage = true;
                channel = CommunicationChannel.Client(pipeName, 100);
                channel.SetLogger(Log);
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
                _staticMutants = new HashSet<int>();
            }
        }

        private static void Channel_RaiseReceivedMessage(object sender, string args)
        {
            if (!args.StartsWith("DUMP"))
            {
                return;
            }
            HashSet<int> temp, tempStatic;
            lock (_coveredMutants)
            {
                temp = _coveredMutants;
                tempStatic = _staticMutants;
                _coveredMutants = new HashSet<int>();
                _staticMutants = new HashSet<int>();
            }
            DumpState(temp, tempStatic);
        }

        private static void CurrentDomain_ProcessExit(object sender, EventArgs e)
        {
            DumpState();
            GC.KeepAlive(_coveredMutants);
        }

        public static void DumpState()
        {
            DumpState(_coveredMutants, _staticMutants);
        }

        public static void DumpState(HashSet<int> state, HashSet<int> staticMutants)
        {
            string report;
            lock (state)
            {
                report = string.Join(",", state)+";"+string.Join(",", staticMutants);
            }

            channel.SendText(report);
        }

        private static void Log(string message)
        {
            Console.WriteLine("["+DateTime.Now.ToString(":HH:mm:ss.fff") + " DBG]"+  message);
        }

        // check with: Stryker.MutantControl.IsActive(ID)
        public static bool IsActive(int id)
        {
            if (captureCoverage)
            {
                lock (_coveredMutants)
                {
                    if (!usePipe)
                    {
                        if (string.IsNullOrEmpty(Environment.GetEnvironmentVariable(envName)))
                        {
                            _coveredMutants = new HashSet<int>();
                            _staticMutants = new HashSet<int>();
                        }
                    }
                    if (_coveredMutants.Add(id))
                    {
                        if (!usePipe)
                        {
                            Environment.SetEnvironmentVariable(envName, string.Join(",", _coveredMutants));
                        }
                    }

                    if (StaticContext.InStatic())
                    {
                        _staticMutants.Add(id);
                    }
                }
            }
            return ActiveMutation == id;
        }

        public static int ActiveMutation;
    }
}
