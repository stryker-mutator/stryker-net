using System;
using System.Collections.Generic;
using Buildalyzer;
using Stryker.Abstractions;
using Stryker.Abstractions.ProjectComponents;
using Stryker.Core.InjectedHelpers;

namespace Stryker.Core.ProjectComponents.SourceProjects;

public class SourceProjectInfo(IAnalyzerResult analyzerResult, ITestProjectsInfo testProjectsInfo)
    : IProjectAndTests
{
    private readonly List<string> _warnings = [];

    public Action OnProjectBuilt { get; set; }

    public IAnalyzerResult AnalyzerResult { get; init; } = analyzerResult;

    public ITestProjectsInfo? TestProjectsInfo { get; init; } = testProjectsInfo;

    /// <summary>
    /// The Folder/File structure found in the project under test.
    /// </summary>
    public IReadOnlyProjectComponent ProjectContents { get; set; }

    public string HelperNamespace => CodeInjector.HelperNamespace;

    public CodeInjection CodeInjector { get; } = new();

    public IReadOnlyCollection<string> Warnings => _warnings;

    public IReadOnlyList<string> GetTestAssemblies() =>
        TestProjectsInfo?.GetTestAssemblies() ?? [];

    public void LogError(string error) => _warnings.Add(error);

    public void BackupOriginalAssembly(IAnalyzerResult analyzerResult) => TestProjectsInfo?.BackupOriginalAssembly(analyzerResult);

    public void RestoreOriginalAssembly(IAnalyzerResult analyzerResult) => TestProjectsInfo?.RestoreOriginalAssembly(analyzerResult);
}
