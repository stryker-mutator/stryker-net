namespace StrykerNamespace
{
    public class ActiveMutationHelper
    {
        static ActiveMutationHelper()
        {
            ActiveMutation = int.Parse(System.Environment.GetEnvironmentVariable("ActiveMutation") ?? "-1");
        }

        public static int ActiveMutation;
    }
}
