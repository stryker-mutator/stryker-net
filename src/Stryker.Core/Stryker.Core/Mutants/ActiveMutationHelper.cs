using System;

namespace Stryker
{
    public static class ActiveMutationHelper
    {
        
        static ActiveMutationHelper()
        {
            ActiveMutation = int.Parse(System.Environment.GetEnvironmentVariable("ActiveMutation") ?? "-1");
            AppDomain.CurrentDomain.ProcessExit += CurrentDomain_ProcessExit;
        }

        private static void CurrentDomain_ProcessExit(object sender, EventArgs e)
        {
            //DumpState();
        }

        public static bool Check(int id)
        {
            return ActiveMutation == id;
        }

        public static int ActiveMutation { get; private set;}
    }
}
