namespace Stryker
{
    public static class Helper
    {
        private static readonly int activeMutant;
        static Helper()
        {
            activeMutant = int.Parse(System.Environment.GetEnvironmentVariable("ActiveMutation") ?? string.Empty);
        }
        public static int ActiveMutant => activeMutant;
    }
}
