using System;
using System.Collections.Generic;
using System.IO.Abstractions;
using System.Linq;
using Buildalyzer;
using Microsoft.Extensions.Logging;
using Stryker.Abstractions;
using Stryker.Abstractions.Options;
using Stryker.Core.ProjectComponents.SourceProjects;
using Stryker.Core.ProjectComponents.TestProjects;
using Stryker.Utilities.Buildalyzer;

namespace Stryker.Core.Initialisation;

/// <summary>
/// Keep track of a project and the test projects covering it
/// </summary>
/// <param name="target">analyzer result for target project</param>
/// <param name="logger">logger used for reporting</param>
internal class MutableProjectTarget(IAnalyzerResult target, ILogger logger)
{
    public IAnalyzerResult ProjectTarget { get; } = target;

    public HashSet<IAnalyzerResult> TestProjects { get; } = [];

    public bool IsValidTarget => ProjectTarget.IsValid() && TestProjects.Count>0 && TestProjects.Any(tp => tp.IsValid());

    private static readonly string[] FoldersToExclude = ["obj", "bin", "node_modules", "StrykerOutput"];

    /// <summary>
    /// Builds a <see cref="SourceProjectInfo"/> instance describing a project its associated test project(s)
    /// </summary>
    /// <param name="options">Stryker options</param>
    /// <param name="targetsForMutation"></param>
    /// <param name="fileSystem">filesystem</param>
    /// <returns></returns>
    public SourceProjectInfo BuildSourceProjectInfo(IStrykerOptions options,
        TargetsForMutation targetsForMutation, IFileSystem fileSystem )
    {
        var targetProjectInfo = new SourceProjectInfo
        {
            AnalyzerResult = ProjectTarget
        };

        var language = targetProjectInfo.AnalyzerResult.GetLanguage();

        ProjectComponentsBuilder builder = language == Language.Csharp
            ? new CsharpProjectComponentsBuilder(targetProjectInfo, options, FoldersToExclude, logger,
                fileSystem)
            : throw new NotSupportedException($"Language not supported: {language}");

        var inputFiles = builder.Build();
        builder.InjectHelpers(inputFiles);
        targetProjectInfo.OnProjectBuilt = builder.PostBuildAction();
        targetProjectInfo.ProjectContents = inputFiles;
        targetProjectInfo.TargetsForMutation = targetsForMutation;
        logger.LogInformation("Found project {ProjectFileName} to mutate.", ProjectTarget.ProjectFilePath);
        targetProjectInfo.TestProjectsInfo = new TestProjectsInfo(fileSystem)
        {
            TestProjects = TestProjects.Select(testProjectAnalyzerResult => new TestProject(fileSystem, testProjectAnalyzerResult)).ToList()
        };
        return targetProjectInfo;
    }

    public void DumpForAnalysis()
    {
        logger.LogInformation(" target {TargetFramework} analysis {Result}, simulated build {BuildResult}.",
            ProjectTarget.TargetFramework,
            ProjectTarget.IsValid() ? "succeeded" : "failed",
            ProjectTarget.Succeeded ? "succeeded" : "failed"
        );
        if (TestProjects.Count == 0)
        {
            logger.LogWarning(
                "  can't be mutated because no test project references it. If this is a test project, " +
                "ensure it has the property: <IsTestProject>true</IsTestProject> in its project file.");
            return;
        }

        // dump info on test projects
        foreach (var testProject in TestProjects)
        {
            logger.LogInformation("  referenced by test project {ProjectName}, analysis {Result}.",
                testProject.ProjectFilePath,
                testProject.IsValid() ? "succeeded" : "failed");

        }

        // dump associated test projects
        // provide synthetic status
        if (TestProjects.Any(r => r.IsValid()))
        {
            logger.LogInformation("  can be mutated.");
        }
        else
        {
            logger.LogWarning("  can't be mutated because all referencing test projects' analysis failed.");
        }
    }
}
