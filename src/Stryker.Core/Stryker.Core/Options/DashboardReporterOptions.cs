namespace Stryker.Core.Options
{
    public class DashboardReporterOptions
    {
        public string DashboardUrl { get; }
        public string DashboardApiKey { get; }
        public string ProjectName { get; }
        public string ModuleName { get; }
        public string ProjectVersion { get; }
        public string FallbackVersion { get; }

        public DashboardReporterOptions(string dashboardApiKey, string projectName, string moduleName, string projectVersion, string fallbackVersion, string dashboardUrl = "https://dashboard.stryker-mutator.io")
        {
            DashboardApiKey = dashboardApiKey;
            ProjectName = projectName;
            ModuleName = moduleName;
            ProjectVersion = projectVersion;
            FallbackVersion = fallbackVersion;
            DashboardUrl = dashboardUrl;
        }
    }
}
