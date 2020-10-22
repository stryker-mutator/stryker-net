using Stryker.Core.Exceptions;
using Stryker.Core.Reporters;
using System;
using System.Collections.Generic;
using System.Text;

namespace Stryker.Core.Options
{
    public class DiffOptions
    {
        public bool DiffEnabled { get; }
        public string DashboardUrl { get; } = "https://dashboard.stryker-mutator.io";
        public string DashboardApiKey { get; }
        public string ProjectName { get; }
        public string ModuleName { get; }
        public string ProjectVersion { get; }
        public string FallbackVersion { get; }
        public bool CompareToDashboard { get; }
        public string GitSource { get; }

        public DiffOptions(bool hasDashboardReporter = false,
            bool compareToDashboard = false,
            bool diff = false,
            string gitSource = "master",
            string dashboardApiKey = null,
            string projectName = null,
            string moduleName = null,
            string projectVersion = null,
            string fallbackVersion = null,
            string dashboardUrl = null)
        {
            GitSource = ValidateGitSource(gitSource);
            DiffEnabled = diff;
            CompareToDashboard = compareToDashboard;
            ModuleName = hasDashboardReporter ? moduleName : null;
            (DashboardApiKey, ProjectName) = ValidateDashboardReporter(hasDashboardReporter, dashboardApiKey, projectName);
            (ProjectVersion, FallbackVersion) = ValidateCompareToDashboard(projectVersion, fallbackVersion);
            DashboardUrl = dashboardUrl ?? DashboardUrl;
        }

        private (string DashboardApiKey, string ProjectName) ValidateDashboardReporter(bool hasDashboardReporter, string dashboadApiKey, string projectName)
        {
            if (!hasDashboardReporter)
            {
                return (null, null);
            }

            var errorStrings = new StringBuilder();
            if (string.IsNullOrWhiteSpace(dashboadApiKey))
            {
                var environmentApiKey = Environment.GetEnvironmentVariable("STRYKER_DASHBOARD_API_KEY");
                if (!string.IsNullOrWhiteSpace(environmentApiKey))
                {
                    dashboadApiKey = environmentApiKey;
                }
                else
                {
                    errorStrings.AppendLine($"An API key is required when the {Reporter.Dashboard} reporter is turned on! You can get an API key at {DashboardUrl}");
                }
            }

            if (string.IsNullOrWhiteSpace(projectName))
            {
                errorStrings.AppendLine($"A project name is required when the {Reporter.Dashboard} reporter is turned on!");
            }

            if (errorStrings.Length > 0)
            {
                throw new StrykerInputException(errorStrings.ToString());
            }

            return (dashboadApiKey, projectName);
        }

        private (string ProjectVersion, string FallbackVersion) ValidateCompareToDashboard(string projectVersion, string fallbackVersion)
        {
            if (string.IsNullOrEmpty(fallbackVersion))
            {
                fallbackVersion = GitSource;
            }

            if (CompareToDashboard)
            {
                var errorStrings = new StringBuilder();
                if (string.IsNullOrEmpty(projectVersion))
                {
                    errorStrings.Append("When the compare to dashboard feature is enabled, dashboard-version cannot be empty, please provide a dashboard-version");
                }

                if (fallbackVersion == projectVersion)
                {
                    errorStrings.Append("Fallback version cannot be set to the same value as the dashboard-version, please provide a different fallback version");
                }

                if (errorStrings.Length > 0)
                {
                    throw new StrykerInputException(errorStrings.ToString());
                }
            }

            return (projectVersion, fallbackVersion);
        }

        private IEnumerable<FilePattern> ValidateDiffIgnoreFiles(IEnumerable<string> diffIgnoreFiles)
        {
            var mappedDiffIgnoreFiles = new List<FilePattern>();
            if (diffIgnoreFiles != null)
            {
                foreach (var pattern in diffIgnoreFiles)
                {
                    mappedDiffIgnoreFiles.Add(FilePattern.Parse(FilePathUtils.NormalizePathSeparators(pattern)));
                }
            }
            return mappedDiffIgnoreFiles;
        }

        private string ValidateGitSource(string gitSource)
        {
            if (string.IsNullOrEmpty(gitSource))
            {
                throw new StrykerInputException("GitSource may not be empty, please provide a valid git branch name");
            }
            return gitSource;
        }
    }
}
