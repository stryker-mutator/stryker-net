using System;
using System.Collections.Generic;
using System.Text;

namespace Stryker.Core.Options
{
    public class DashboardReporterOptions
    {
        public string DashboardUrl { get; } = "https://dashboard.stryker-mutator.io";
        public string DashboardApiKey { get; }
        public string ProjectName { get; }
        public string ModuleName { get; }
        public string ProjectVersion { get; }

        public DashboardReporterOptions(string dashboardApiKey, string projectName, string moduleName, string projectVersion)
        {
            DashboardApiKey = dashboardApiKey;
            ProjectName = projectName;
            ModuleName = moduleName;
            ProjectVersion = projectVersion;
        }
    }
}
