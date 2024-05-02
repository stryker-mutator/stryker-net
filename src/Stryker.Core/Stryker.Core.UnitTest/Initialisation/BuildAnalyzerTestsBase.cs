using System;
using System.Collections.Generic;
using System.IO.Abstractions.TestingHelpers;
using System.Linq;
using System.Reflection;
using Buildalyzer;
using Buildalyzer.Construction;
using Moq;
using Stryker.Core.Initialisation.Buildalyzer;
using Stryker.Core.Testing;

namespace Stryker.Core.UnitTest.Initialisation;

public class BuildAnalyzerTestsBase : TestBase
{
    protected readonly MockFileSystem FileSystem = new();
    protected string ProjectPath;
    private readonly Dictionary<string, IAnalyzerResult> _projectCache = new();
    protected readonly Mock<IBuildalyzerProvider> _buildalyzerProviderMock = new(MockBehavior.Strict);

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
    protected Mock<IProjectAnalyzer> SourceProjectAnalyzerMock(string csprojPathName, string[] sourceFiles, IEnumerable<string> projectReferences = null, string framework = "net6.0", Func<bool> success = null)
    {
        var properties = new Dictionary<string, string>
            { { "IsTestProject", "False" }, { "ProjectTypeGuids", "not testproject" }, { "Language", "C#" } };
        projectReferences??= new List<string>();

        return BuildProjectAnalyzerMock(csprojPathName, sourceFiles, properties, projectReferences, framework, success);
    }

    /// <summary>
    /// Build a simple test project
    /// </summary>
    /// <param name="testCsprojPathName">test project pathname</param>
    /// <param name="csProj">production code project pathname</param>
    /// <param name="framework"></param>
    /// <param name="success"></param>
    /// <returns>a mock project analyzer</returns>
    /// <remarks>the test project references the production code project and contains no source file</remarks>
    protected Mock<IProjectAnalyzer> TestProjectAnalyzerMock(string testCsprojPathName, string csProj, string framework = "net6.0", bool success = true)
    {
        var properties = new Dictionary<string, string>{ { "IsTestProject", "True" }, { "Language", "C#" } };

        var projectReferences =  _projectCache[csProj].ProjectReferences.Append(csProj).ToList();
        return BuildProjectAnalyzerMock(testCsprojPathName, [], properties, projectReferences, framework, () => success);
    }

    /// <summary>
    /// build a project analyzer mock
    /// </summary>
    /// <param name="csprojPathName">project file name</param>
    /// <param name="sourceFiles">source files to return</param>
    /// <param name="properties">project properties</param>
    /// <param name="projectReferences">project references</param>
    /// <param name="framework"></param>
    /// <param name="success">analysis success</param>
    /// <returns>a mock project analyzer</returns>
    /// <remarks>
    /// 1. project and source files will be created (empty) in the file system
    /// 2. the project analyzer mock returns a single project result</remarks>
    internal Mock<IProjectAnalyzer> BuildProjectAnalyzerMock(string csprojPathName,
        string[] sourceFiles, Dictionary<string, string> properties,
        IEnumerable<string> projectReferences = null,
        string framework = "net6.0",    
        Func<bool> success = null)
    {
        var sourceProjectAnalyzerMock = new Mock<IProjectAnalyzer>(MockBehavior.Strict);
        var sourceProjectAnalyzerResultMock = new Mock<IAnalyzerResult>(MockBehavior.Strict);
        var sourceProjectFileMock = new Mock<IProjectFile>(MockBehavior.Strict);
        success??= () => true;
        projectReferences ??= [];

        // create dummy project and source files
        FileSystem.AddFile(csprojPathName, new MockFileData(""));
        foreach (var file in sourceFiles)
        {
            FileSystem.AddFile(file, new MockFileData(""));
        }

        // create bin folder
        var projectUnderTestBin = FileSystem.Path.Combine(ProjectPath, "bin");
        FileSystem.AddDirectory(projectUnderTestBin);

        var projectBin =
            FileSystem.Path.Combine(projectUnderTestBin, FileSystem.Path.GetFileNameWithoutExtension(csprojPathName)+".dll");
        FileSystem.AddFile(FileSystem.Path.Combine(projectUnderTestBin, projectBin), new MockFileData(""));
        sourceProjectAnalyzerResultMock.Setup(x => x.ProjectReferences).Returns(projectReferences);
        sourceProjectAnalyzerResultMock.Setup(x => x.References).Returns(projectReferences.
            Where (_projectCache.ContainsKey).
            Select( iar => _projectCache[iar].GetAssemblyPath()).ToArray());
        sourceProjectAnalyzerResultMock.Setup(x => x.SourceFiles).Returns(sourceFiles);
        sourceProjectAnalyzerResultMock.Setup(x => x.PreprocessorSymbols).Returns(["NET"]);
        properties.Add("TargetRefPath", projectBin);
        properties.Add("TargetDir", projectUnderTestBin);
        properties.Add("TargetFileName", projectBin);

        sourceProjectAnalyzerResultMock.Setup(x => x.Properties).Returns(properties);
        sourceProjectAnalyzerResultMock.Setup(x => x.ProjectFilePath).Returns(csprojPathName);
        sourceProjectAnalyzerResultMock.Setup(x => x.TargetFramework).Returns(framework);
        sourceProjectAnalyzerResultMock.Setup(x => x.Succeeded).Returns(success);
        _projectCache[csprojPathName] = sourceProjectAnalyzerResultMock.Object;

        var sourceProjectAnalyzerResultsMock = BuildAnalyzerResultsMock(sourceProjectAnalyzerResultMock.Object);
        sourceProjectAnalyzerMock.Setup(x => x.Build(It.IsAny<string[]>())).Returns(sourceProjectAnalyzerResultsMock);

        sourceProjectAnalyzerMock.Setup(x => x.ProjectFile).Returns(sourceProjectFileMock.Object);

        sourceProjectFileMock.Setup(x => x.Path).Returns(csprojPathName);
        sourceProjectFileMock.Setup(x => x.Name).Returns(FileSystem.Path.GetFileName(csprojPathName));
        sourceProjectFileMock.Setup(x=> x.TargetFrameworks).Returns([framework]);
        return sourceProjectAnalyzerMock;
    }

    internal static IAnalyzerResults BuildAnalyzerResultsMock(IAnalyzerResult sourceProjectAnalyzerResult)
    {
        IEnumerable<IAnalyzerResult> analyzerResults = [ sourceProjectAnalyzerResult];
        var sourceProjectAnalyzerResultsMock = new Mock<IAnalyzerResults>(MockBehavior.Strict);
        sourceProjectAnalyzerResultsMock.Setup(x => x.OverallSuccess).Returns(() => analyzerResults.All(r=> r.Succeeded));
        sourceProjectAnalyzerResultsMock.Setup(x => x.Results).Returns(analyzerResults);
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

        _buildalyzerProviderMock.Setup(x => x.Provide(It.IsAny<string>(), It.IsAny<AnalyzerManagerOptions>()))
            .Returns(buildalyzerAnalyzerManagerMock.Object);
        _buildalyzerProviderMock.Setup(x => x.Provide(It.IsAny<AnalyzerManagerOptions>()))
            .Returns(buildalyzerAnalyzerManagerMock.Object);
        return buildalyzerAnalyzerManagerMock;
    }
}
