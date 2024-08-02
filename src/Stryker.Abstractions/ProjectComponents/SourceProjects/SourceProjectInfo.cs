using System;
using System.Collections.Generic;
using Buildalyzer;
using Stryker.Configuration.Initialisation;
using Stryker.Configuration.Initialisation.Buildalyzer;
using Stryker.Configuration.InjectedHelpers;
using Stryker.Configuration.ProjectComponents.TestProjects;

namespace Stryker.Configuration.ProjectComponents.SourceProjects;

public class SourceProjectInfo : IProjectAndTests
{
    private readonly List<string> _warnings = [];

    public Action OnProjectBuilt {get;set;}

    public IAnalyzerResult AnalyzerResult { get; set; }

    /// <summary>
    /// The Folder/File structure found in the project under test.
    /// </summary>
    public IProjectComponent ProjectContents { get; set; }

    public bool IsFullFramework => AnalyzerResult.TargetsFullFramework();

    public string HelperNamespace => CodeInjector.HelperNamespace;

    public CodeInjection CodeInjector { get;} = new();

    public TestProjectsInfo TestProjectsInfo { get; set; }

    public IReadOnlyCollection<string> Warnings => _warnings;

    public IReadOnlyList<string> GetTestAssemblies() =>
        TestProjectsInfo.GetTestAssemblies();

    public string LogError(string error)
    {
        _warnings.Add(error);
        return error;
    }
}
