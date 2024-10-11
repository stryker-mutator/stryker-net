using System;
using System.Collections.Generic;
using System.IO.Abstractions.TestingHelpers;
using System.Linq;
using System.Reflection;
using Buildalyzer;
using Buildalyzer.Construction;
using Buildalyzer.Environment;
using Moq;
using Stryker.Core.Initialisation.Buildalyzer;
using Stryker.Core.Testing;
using Stryker.Core.UnitTest;

namespace Stryker.Core.UnitTest.Initialisation;

public class BuildAnalyzerTestsBase : TestBase
{
    protected internal const string DefaultFramework = "net6.0";
    protected readonly MockFileSystem FileSystem = new();
    protected string ProjectPath;
    private readonly Dictionary<string, Dictionary<string, IAnalyzerResult>> _projectCache = new();
    protected readonly Mock<IBuildalyzerProvider> BuildalyzerProviderMock = new(MockBehavior.Strict);

    public BuildAnalyzerTestsBase()
    {
        var currentDirectory = FileSystem.Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
        var filesystemRoot = FileSystem.Path.GetPathRoot(currentDirectory);

        ProjectPath = FileSystem.Path.Combine(filesystemRoot, "sourceproject");
    }

    /// <summary>
    /// Build a simple production project
    /// </summary>
    /// <param name="csprojPathName">project pathname</param>
    /// <param name="sourceFiles">project source files</param>
    /// <param name="projectReferences">project references</param>
    /// <param name="framework">framework version</param>
    /// <param name="success">Predicate to control when analysis is successful. Default is always success.</param>
    protected Mock<IProjectAnalyzer> SourceProjectAnalyzerMock(string csprojPathName, string[] sourceFiles,
        IEnumerable<string> projectReferences = null, string framework = DefaultFramework, Func<bool> success = null)
    {
        var properties = GetSourceProjectDefaultProperties();
        projectReferences ??= new List<string>();

        return BuildProjectAnalyzerMock(csprojPathName, sourceFiles, properties, projectReferences, [framework], success);
    }

    /// <summary>
    /// Build a simple production project
    /// </summary>
    /// <param name="csprojPathName">project pathname</param>
    /// <param name="sourceFiles">project source files</param>
    /// <param name="projectReferences">project references</param>
    /// <param name="framework">framework version</param>
    /// <param name="success">Predicate to control when analysis is successful. Default is always success.</param>
    protected Mock<IProjectAnalyzer> SourceProjectAnalyzerMock(string csprojPathName, string[] sourceFiles,
        IEnumerable<string> projectReferences , IEnumerable<string> frameworks, Func<bool> success = null)
    {
        var properties = GetSourceProjectDefaultProperties();
        projectReferences??= new List<string>();

        return BuildProjectAnalyzerMock(csprojPathName, sourceFiles, properties, projectReferences, frameworks, success);
    }

    public static Dictionary<string, string> GetSourceProjectDefaultProperties()
    {
        var properties = new Dictionary<string, string>
            { { "IsTestProject", "False" }, { "ProjectTypeGuids", "not testproject" }, { "Language", "C#" } };
        return properties;
    }

    /// <summary>
    /// Build a simple test project
    /// </summary>
    /// <param name="testCsprojPathName">test project pathname</param>
    /// <param name="csProj">production code project pathname</param>
    /// <param name="frameworks"></param>
    /// <param name="success"></param>
    /// <returns>a mock project analyzer</returns>
    /// <remarks>the test project references the production code project and contains no source file</remarks>
    protected Mock<IProjectAnalyzer> TestProjectAnalyzerMock(string testCsprojPathName, string csProj, IEnumerable<string> frameworks = null, bool success = true)
    {
        frameworks??=new []{DefaultFramework};
        var properties = new Dictionary<string, string>{ { "IsTestProject", "True" }, { "Language", "C#" } };
        var projectReferences =  string.IsNullOrEmpty(csProj) ? [] : GetProjectResult(csProj, frameworks.First()).ProjectReferences.Append(csProj).ToList();
        return BuildProjectAnalyzerMock(testCsprojPathName, [], properties, projectReferences, frameworks, () => success);
    }

    private IAnalyzerResult GetProjectResult(string projectFile, string expectedFramework, bool returnDefaultIfNotFound = true)
    {
        if (!_projectCache.TryGetValue(projectFile, out var project))
        {
            return null;
        }

        var target= PickCompatibleFramework(expectedFramework, project.Keys);
        if (target == null)
        {
            return returnDefaultIfNotFound ? project.Values.First() : null;
        }
        return project[target];
    }

    /// <summary>
    /// Parse a net moniker. WARNING: use this for tests only, as it only supports netcore, netstandard and net monikers.
    /// This is a very naive implementation which does not fully respect the official rules
    /// </summary>
    /// <param name="framework">moniker to parse</param>
    /// <returns>a tuple with the framework kind first and the version next</returns>
    protected static (FrameworkKind kind, decimal version) ParseFramework(string framework)
    {
        FrameworkKind kind;
        decimal version;

        if (framework.StartsWith("netcoreapp"))
        {
            if (!decimal.TryParse(framework[10..], out version))
            {
                version = 1.0m;
            }
            return (FrameworkKind.NetCore, version);
        }
        if (framework.StartsWith("netstandard"))
        {
            if (!decimal.TryParse(framework[11..], out version))
            {
                version = 1.0m;
            }
            return (FrameworkKind.NetStandard, version);
        }
        if (framework.StartsWith("net") && decimal.TryParse(framework[3..], out version))
        {
            return framework.Contains('.') ? (FrameworkKind.NetCore, version) : (FrameworkKind.Net, version/100);
        }
        return (FrameworkKind.Other, 1.1m);
    }

    /// <summary>
    /// Framework families
    /// </summary>
    protected enum FrameworkKind
    {
        Net, NetCore, NetStandard, Other
    }

    /// <summary>
    /// Picks the best compatible framework from a list. WARNING: use this for tests only, as it only supports netcore and net monikers.
    /// netstandard is not properly implemented. This is a basic implementation
    /// </summary>
    /// <param name="framework">target framework</param>
    /// <param name="frameworks">list of available frameworks</param>
    /// <returns><paramref name="framework"/> if the framework is among the target, the best match if available, null otherwise.</returns>
    protected static string PickCompatibleFramework(string framework, IEnumerable<string> frameworks)
    {
        var parsed = ParseFramework(framework);

        string bestCandidate = null;
        var bestVersion = 1.0m;
        foreach(var candidate in frameworks)
        {
            if (candidate == framework)
            {
                return framework;
            }
            var parsedCandidate = ParseFramework(candidate);
            if (parsedCandidate.kind != parsed.kind)
            {
                continue;
            }

            if (parsedCandidate.version > parsed.version || parsedCandidate.version <= bestVersion)
            {
                continue;
            }

            bestVersion = parsedCandidate.version;
            bestCandidate = candidate;
        }
        return bestCandidate;
    }

    /// <summary>
    /// build a project analyzer mock
    /// </summary>
    /// <param name="csprojPathName">project file name</param>
    /// <param name="sourceFiles">source files to return</param>
    /// <param name="properties">project properties</param>
    /// <param name="projectReferences">project references</param>
    /// <param name="frameworks">list of frameworks (multitargeting)</param>
    /// <param name="success">analysis success</param>
    /// <param name="rawReferences">assembly references</param>
    /// <returns>a mock project analyzer</returns>
    /// <remarks>
    /// 1. project and source files will be created (empty) in the file system
    /// 2. the project analyzer mock returns a single project result</remarks>
    internal Mock<IProjectAnalyzer> BuildProjectAnalyzerMock(string csprojPathName,
        string[] sourceFiles, Dictionary<string, string> properties,
        IEnumerable<string> projectReferences= null,
        IEnumerable<string> frameworks = null,
        Func<bool> success = null,
        IEnumerable<string> rawReferences = null)
    {
        var projectFileMock = new Mock<IProjectFile>(MockBehavior.Strict);
        success ??= () => true;
        projectReferences ??= [];
        frameworks ??=[DefaultFramework];
        // create dummy project and source files
        FileSystem.AddFile(csprojPathName, new MockFileData(""));
        foreach (var file in sourceFiles)
        {
            FileSystem.AddFile(file, new MockFileData(""));
        }
        rawReferences ??= ["System"];

        var projectAnalyzerResults = new Dictionary<string, IAnalyzerResult>();
        foreach(var framework in frameworks)
        {
            var specificProperties = new Dictionary<string, string>(properties);
            // create bin folders
            var projectUnderTestBin = FileSystem.Path.Combine(ProjectPath, "bin", framework);
            FileSystem.AddDirectory(projectUnderTestBin);

            var projectBin =
                FileSystem.Path.Combine(projectUnderTestBin, FileSystem.Path.GetFileNameWithoutExtension(csprojPathName)+".dll");
            FileSystem.AddFile(FileSystem.Path.Combine(projectUnderTestBin, projectBin), new MockFileData(""));
            var projectAnalyzerResultMock = new Mock<IAnalyzerResult>(MockBehavior.Strict);
            projectAnalyzerResultMock.Setup(x => x.ProjectReferences).Returns(projectReferences);
            projectAnalyzerResultMock.Setup(x => x.References).Returns(projectReferences.
                Where ( p => p !=null && _projectCache.ContainsKey(p)).
                Select( iar => GetProjectResult(iar, framework).GetAssemblyPath()).Union(rawReferences).ToArray());
            projectAnalyzerResultMock.Setup(x => x.SourceFiles).Returns(sourceFiles);
            projectAnalyzerResultMock.Setup(x => x.PreprocessorSymbols).Returns(["NET"]);
            specificProperties.Add("TargetRefPath", projectBin);
            specificProperties.Add("TargetDir", projectUnderTestBin);
            specificProperties.Add("TargetFileName", projectBin);

            projectAnalyzerResultMock.Setup(x => x.Properties).Returns(specificProperties);
            projectAnalyzerResultMock.Setup(x => x.GetProperty(It.IsAny<string>())).Returns((string p) => specificProperties.GetValueOrDefault(p, null));
            projectAnalyzerResultMock.Setup(x => x.ProjectFilePath).Returns(csprojPathName);
            projectAnalyzerResultMock.Setup(x => x.TargetFramework).Returns(framework);
            projectAnalyzerResultMock.Setup(x => x.Succeeded).Returns(success);

            projectAnalyzerResultMock.Setup(x => x.Analyzer).Returns<AnalyzerManager>(null);
            projectAnalyzerResults[framework] = projectAnalyzerResultMock.Object;
        }

        _projectCache[csprojPathName] = projectAnalyzerResults;

        var sourceProjectAnalyzerResultsMock = BuildAnalyzerResultsMock(projectAnalyzerResults);

        var projectAnalyzerMock = new Mock<IProjectAnalyzer>(MockBehavior.Strict);
        projectAnalyzerMock.Setup(x => x.Build(It.IsAny<string[]>())).Returns(sourceProjectAnalyzerResultsMock);
        projectAnalyzerMock.Setup(x => x.Build(It.IsAny<string[]>(), It.IsAny<EnvironmentOptions>())).Returns(sourceProjectAnalyzerResultsMock);
        projectAnalyzerMock.Setup(x => x.Build(It.IsAny<EnvironmentOptions>())).Returns(sourceProjectAnalyzerResultsMock);
        projectAnalyzerMock.Setup(x => x.Build()).Returns(sourceProjectAnalyzerResultsMock);

        projectAnalyzerMock.Setup(x => x.ProjectFile).Returns(projectFileMock.Object);
        projectAnalyzerMock.Setup(x => x.EnvironmentFactory).Returns<EnvironmentFactory>(null);

        projectFileMock.Setup(x => x.Path).Returns(csprojPathName);
        projectFileMock.Setup(x => x.Name).Returns(FileSystem.Path.GetFileName(csprojPathName));
        projectFileMock.Setup(x=> x.TargetFrameworks).Returns(frameworks.ToArray() );
        return projectAnalyzerMock;
    }

    private IAnalyzerResults BuildAnalyzerResultsMock(IDictionary<string, IAnalyzerResult> projectAnalyzerResults)
    {
        var analyzerResults = projectAnalyzerResults.Values.ToList();
        var sourceProjectAnalyzerResultsMock = new Mock<IAnalyzerResults>(MockBehavior.Strict);
        sourceProjectAnalyzerResultsMock.Setup(x => x.OverallSuccess).Returns(() => analyzerResults.All(r => r.Succeeded));
        sourceProjectAnalyzerResultsMock.Setup(x => x.Results).Returns(analyzerResults);
        sourceProjectAnalyzerResultsMock.Setup(x => x.Count).Returns(analyzerResults.Count);
        sourceProjectAnalyzerResultsMock.Setup(x => x.GetEnumerator()).Returns(() => analyzerResults.GetEnumerator());
        return sourceProjectAnalyzerResultsMock.Object;
    }

    protected Mock<IAnalyzerManager> BuildBuildAnalyzerMock(Dictionary<string, IProjectAnalyzer> analyzerResults)
    {
        var buildalyzerAnalyzerManagerMock = new Mock<IAnalyzerManager>(MockBehavior.Strict);
        buildalyzerAnalyzerManagerMock.Setup(x => x.Projects)
            .Returns(analyzerResults);
        buildalyzerAnalyzerManagerMock.Setup(x => x.SetGlobalProperty(It.IsAny<string>(), It.IsAny<string>()));
        buildalyzerAnalyzerManagerMock.Setup(x => x.SolutionFilePath).Returns((string)null);

        foreach (var analyzerResult in analyzerResults)
        {
            var filename = analyzerResult.Value.Build([string.Empty]).Results.First().ProjectFilePath;
            buildalyzerAnalyzerManagerMock.Setup(x => x.GetProject(filename)).Returns(analyzerResult.Value);
        }

        BuildalyzerProviderMock.Setup(x => x.Provide(It.IsAny<string>(), It.IsAny<AnalyzerManagerOptions>()))
            .Returns(buildalyzerAnalyzerManagerMock.Object);
        BuildalyzerProviderMock.Setup(x => x.Provide(It.IsAny<AnalyzerManagerOptions>()))
            .Returns(buildalyzerAnalyzerManagerMock.Object);
        return buildalyzerAnalyzerManagerMock;
    }
}
