using System;
using System.Collections.Generic;
using System.IO;

namespace Stryker
{
    public static class MutantControl
    {
        private static readonly HashSet<int> _coveredMutants;
        private static readonly bool captureCoverage;
        private static readonly string logFileName;

        static MutantControl()
        {
            ActiveMutation = int.Parse(Environment.GetEnvironmentVariable("ActiveMutation") ?? "-1");
            logFileName = Environment.GetEnvironmentVariable("CoverageFileName");
            captureCoverage = !string.IsNullOrEmpty(logFileName);
            if (captureCoverage)
            {
                AppDomain.CurrentDomain.ProcessExit += CurrentDomain_ProcessExit;
                _coveredMutants = new HashSet<int>();
            }
        }

        private static void CurrentDomain_ProcessExit(object sender, EventArgs e)
        {
            DumpState();
        }

        private static void DumpState()
        {
            using (var file = File.OpenWrite(logFileName))
            {
                using (var writer = new StreamWriter(file))
                {
                    foreach (var coveredMutant in _coveredMutants)
                    {
                        writer.Write($"{coveredMutant},");
                    }

                }
            }
        }
        // check with: Stryker.MutantControl.IsActive(ID)
        public static bool IsActive(int id)
        {
            if (captureCoverage)
            {
                _coveredMutants.Add(id);
            }
            return ActiveMutation == id;
        }

        private static int ActiveMutation { get; set;}
    }
}
