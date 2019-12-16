using Stryker.Core.InjectedHelpers.Coverage;
using System;
using System.Collections.Generic;
using System.Text;

namespace Stryker
{
    public class MutantControl
    {
        private static Dictionary<int, bool> _coveredMutants;
        private static StringBuilder _mutantsAsString;
        private static StringBuilder _staticMutantsAsStrings;
        private static bool usePipe;
        private static string pipeName;
        private static string envName;
        private static bool captureCoverage;
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
            ActiveMutation = int.Parse(Environment.GetEnvironmentVariable("ActiveMutation") ?? "-1");
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
                captureCoverage = true;
                channel = CommunicationChannel.Client(pipeName, 100);
                channel.SetLogger(Log);
                channel.RaiseReceivedMessage += Channel_RaiseReceivedMessage;
                channel.Start();
            }
#else
            if (coverageMode.StartsWith("env:"))
            {
                Log("Use env for data transmission");
                envName = coverageMode.Substring(4);
                captureCoverage = true;
                usePipe = false;
            }
#endif
            if (captureCoverage)
            {
                ResetCoverage();
            }
        }
                
        private static void ResetCoverage()
        {
            _coveredMutants = new Dictionary<int, bool>();
            _mutantsAsString = new StringBuilder();
            _staticMutantsAsStrings = new StringBuilder();
        }

        private static void Channel_RaiseReceivedMessage(object sender, string args)
        {
            if (!args.StartsWith("DUMP"))
            {
                return;
            }

            string temp;
            lock (_coveredMutants)
            {
                temp = BuildReport();
                ResetCoverage();
            }
            DumpState(temp);
        }

        private static void CurrentDomain_ProcessExit(object sender, EventArgs e)
        {
            DumpState();
            GC.KeepAlive(_coveredMutants);
        }

        private static string BuildReport()
        {
            return string.Format("{0};{1}",_mutantsAsString, _staticMutantsAsStrings);
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
            Console.WriteLine("["+DateTime.Now.ToString("HH:mm:ss.fff") + " DBG] "+  message);
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
                            ResetCoverage();
                        }
                    }

                    bool add = false; 
                    if (!_coveredMutants.ContainsKey(id))
                    {
                        _coveredMutants.Add(id, false);
                        if (_mutantsAsString.Length > 0)
                        {
                            _mutantsAsString.Append(',');
                        }
                        _mutantsAsString.Append(id.ToString());
                        add = true;
                    }
                    if (MutantContext.InStatic() && !_coveredMutants[id])
                    {
                        if (_staticMutantsAsStrings.Length > 0)
                        {
                            _staticMutantsAsStrings.Append(',');
                        }
                        _staticMutantsAsStrings.Append(id.ToString());
                        add = true;
                    }

                    if (add)
                    {
                        if (!usePipe)
                        {
                            Environment.SetEnvironmentVariable(envName, BuildReport());
                        }
                    }

                }
            }
            return ActiveMutation == id;
        }

        public static int ActiveMutation;
    }
}
