namespace Stryker
{
    public static class ActiveMutationHelper
    {
        static ActiveMutationHelper()
        {
            ActiveMutation = int.Parse(System.Environment.GetEnvironmentVariable("ActiveMutation") ?? "-1");
        }

        public static int ActiveMutation { get; private set;}
    }
}
