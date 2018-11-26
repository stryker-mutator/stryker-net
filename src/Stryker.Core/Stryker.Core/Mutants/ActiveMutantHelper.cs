namespace Stryker
{
    public static class Helper
    {
        private static readonly int activeMutant;
        static Helper()
        {
            activeMutant = int.Parse(System.Environment.GetEnvironmentVariable("ActiveMutation") ?? "-1");
        }
        public static int ActiveMutation => activeMutant;
    }
}
