using System;
using System.Collections.Generic;
using Buildalyzer;
using Stryker.Abstractions.Initialisation;
using Stryker.Abstractions.ProjectComponents;
using Stryker.Core.Initialisation.Buildalyzer;
using Stryker.Core.InjectedHelpers;
using Stryker.Core.ProjectComponents.TestProjects;

namespace Stryker.Core.ProjectComponents.SourceProjects;

public class SourceProjectInfo : IProjectAndTests
{
    private readonly List<string> _warnings = [];

    public Action OnProjectBuilt { get; set; }

    public IAnalyzerResult AnalyzerResult { get; set; }

    /// <summary>
    /// The Folder/File structure found in the project under test.
    /// </summary>
    public IReadOnlyProjectComponent ProjectContents { get; set; }

    public bool IsFullFramework => AnalyzerResult.TargetsFullFramework();

    public string HelperNamespace => CodeInjector.HelperNamespace;

    public CodeInjection CodeInjector { get; } = new();

    public ITestProjectsInfo TestProjectsInfo { get; set; }

    public IReadOnlyCollection<string> Warnings => _warnings;

    public IReadOnlyList<string> GetTestAssemblies() =>
        TestProjectsInfo.GetTestAssemblies();

    public string LogError(string error)
    {
        _warnings.Add(error);
        return error;
    }
}
