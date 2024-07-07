using System.Reflection;

namespace Stryker.TestRunner.MSTest.Testing.Options;
internal static class RunOptions
{
    public const string NoConsole = "--internal-vstest-adapter";
    public const string NoBanner = "--no-banner";

    public static string DiscoverySettings => $"--settings {GetSettingsPath("discovery.runsettings")}";
    public static string RunSettings => $"--settings {GetSettingsPath("run.runsettings")}";
    public static string CoverageSettings => $"--settings {GetSettingsPath("coverage.runsettings")}";

    private const string RunSettingsDirectory = "RunSettings";

    private static string GetSettingsPath(string fileName) =>
        Path.Combine(CurrentExecutingDirectory, RunSettingsDirectory, fileName);


    private static string CurrentExecutingDirectory =>
        Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) ?? string.Empty;
}
