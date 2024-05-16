namespace Stryker.TestRunners.MsTest;
internal class Constants
{
    public static class Modes
    {
        public const string Discovery = "--list-tests";
    }

    public static class RunOptions
    {
        public const string NoBanner = "--no-banner";
    }

    public static class ExitCodes
    {
        public const int NoTests = 8;
    }
}
