using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Abstractions;
using System.Linq;
using Buildalyzer;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Stryker.Abstractions;
using Stryker.Abstractions.Exceptions;
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

    public bool IsValidTarget => ProjectTarget.IsValid() && TestProjects.Count > 0 && TestProjects.Any(tp => tp.IsValid());

    private static readonly string[] FoldersToExclude = ["obj", "bin", "node_modules", "StrykerOutput"];

    /// <summary>
    /// Add a new test project
    /// </summary>
    /// <param name="testProject"></param>
    /// <returns>true if the project was added, false if it was already present</returns>
    /// <remarks>This method filters duplicate on project name (not assembly name)</remarks>
    public bool AddTestProject(IAnalyzerResult testProject)
    {
        // this is a workaround for multitargeting projects
        if (TestProjects.Any(tp => tp.ProjectFilePath == testProject.ProjectFilePath))
        {
            return false;
        }
        TestProjects.Add(testProject);
        return true;
    }

    private static ProjectComponentsBuilder DefaultCsharpBuilderFactory(SourceProjectInfo projectInfo, IStrykerOptions options,
        string[] foldersToExclude, ILogger logger, IFileSystem fileSystem) =>
        new CsharpProjectComponentsBuilder(projectInfo, options, foldersToExclude, logger, fileSystem);

    private static string LanguageNotEnabledMessage(Language language, string projectPath) =>
        language == Language.Fsharp
            ? $"F# support is experimental and not yet enabled. Project '{projectPath}' cannot be mutated."
            : $"Project '{projectPath}' uses a language Stryker cannot mutate ({language}). Only C# projects can be mutated.";

    /// <summary>
    /// Builds a <see cref="SourceProjectInfo"/> instance describing a project its associated test project(s)
    /// </summary>
    /// <param name="options">Stryker options</param>
    /// <param name="fileSystem">filesystem</param>
    /// <returns></returns>
    public SourceProjectInfo BuildSourceProjectInfo(IStrykerOptions options, IFileSystem fileSystem, IServiceProvider serviceProvider = null)
    {
        var testProjectInfo = new TestProjectsInfo(fileSystem)
        {
            TestProjects = TestProjects.Select(testProjectAnalyzerResult => new TestProject(fileSystem, testProjectAnalyzerResult)).ToList()
        };
        var targetProjectInfo = new SourceProjectInfo(ProjectTarget , testProjectInfo);

        var language = targetProjectInfo.AnalyzerResult.GetLanguage();

        // Languages other than C# are enabled by registering their keyed builder factory (see Stryker.Fsharp)
        var builderFactory = (serviceProvider as IKeyedServiceProvider)?.GetKeyedService<ProjectComponentsBuilderFactory>(language)
            ?? (language == Language.Csharp ? DefaultCsharpBuilderFactory : null);
        if (builderFactory is null)
        {
            throw new InputException(LanguageNotEnabledMessage(language, ProjectTarget.ProjectFilePath));
        }

        var builder = builderFactory(targetProjectInfo, options, FoldersToExclude, logger, fileSystem);

        var inputFiles = builder.Build();
        builder.InjectHelpers(inputFiles);
        targetProjectInfo.OnProjectBuilt = builder.PostBuildAction();
        targetProjectInfo.ProjectContents = inputFiles;
        logger.LogInformation("Found project {ProjectFileName} to mutate.", ProjectTarget.ProjectFilePath);
        return targetProjectInfo;
    }

    public void LogAnalysisSummary()
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
                Path.GetFileName(testProject.ProjectFilePath),
                testProject.IsValid() ? "succeeded" : "failed");
        }

        // dump associated test projects
        // provide synthetic status
        if (TestProjects.Any(r => r.IsValid()))
        {
            logger.LogInformation(ProjectTarget.IsValid() ? "  can be mutated." : " can't be mutated because its simulated build failed.");
        }
        else
        {
            logger.LogWarning("  can't be mutated because all referencing test projects' simulated build failed.");
        }
    }
}
