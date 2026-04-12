using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Logging;
using Stryker.Core.Initialisation;
using Stryker.Utilities.Buildalyzer;

namespace Stryker.Core.ProjectComponents.SourceProjects;

/// <summary>
/// Simple class aggregating mutable projects and the associated overarching context: platform/framework... settings
/// and solution file when relevant
/// </summary>
public class RelatedSourceProjectsInfo(
    ProjectsTracker projectsTracker,
    IReadOnlyCollection<SourceProjectInfo> sourceProjectInfos,
    ILogger logger = null)
{
    public IReadOnlyCollection<SourceProjectInfo> SourceProjectInfos { get; } = sourceProjectInfos;

    public ProjectsTracker Tracker { get; } = projectsTracker;

    public bool BuildTestProjects(IInitialBuildProcess buildProcess)
    {
        if (!string.IsNullOrEmpty(Tracker.SolutionFilePath))
        {
            Tracker.BuildSolution(buildProcess, SourceProjectInfos.Select(p => p.AnalyzerResult));

            return true;
        }
        var testProjects = SourceProjectInfos.SelectMany(p => p.TestProjectsInfo.AnalyzerResults)
            .Distinct().ToList();
        for (var i = 0; i < testProjects.Count; i++)
        {
            logger?.LogInformation(
                "Building test project {ProjectFilePath} ({CurrentTestProject}/{OfTotalTestProjects})",
                testProjects[i].ProjectFilePath, i + 1,
                testProjects.Count);

            buildProcess.InitialBuild(
                false,
                testProjects[i].ProjectFilePath,
                null,
                testProjects[i].GetProperty("Configuration"),
                testProjects[i].GetProperty("Platform"),
                msbuildPath: testProjects[i].MsBuildPath());
        }

        return true;
    }
}
