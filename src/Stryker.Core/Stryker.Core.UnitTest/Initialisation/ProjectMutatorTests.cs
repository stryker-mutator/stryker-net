using System;
using System.Collections.Generic;
using System.IO.Abstractions;
using System.IO.Abstractions.TestingHelpers;
using System.Linq;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Shouldly;
using Stryker.Abstractions.Options;
using Stryker.Abstractions.Reporting;
using Stryker.Abstractions.Testing;
using Stryker.Configuration.Options;
using Stryker.Core.Initialisation;
using Stryker.Core.MutationTest;
using Stryker.Core.ProjectComponents;
using Stryker.Core.ProjectComponents.Csharp;
using Stryker.Core.ProjectComponents.SourceProjects;
using Stryker.Core.ProjectComponents.TestProjects;
using Stryker.TestRunner.Results;
using Stryker.TestRunner.Tests;
using Stryker.TestRunner.VsTest;
using TestCase = Microsoft.VisualStudio.TestPlatform.ObjectModel.TestCase;

namespace Stryker.Core.UnitTest.Initialisation;

[TestClass]
public class ProjectMutatorTests : TestBase
{
    private readonly Mock<IMutationTestProcess> _mutationTestProcessMock = new(MockBehavior.Strict);
    private readonly Mock<IReporter> _reporterMock = new(MockBehavior.Strict);
    private readonly Mock<IInitialisationProcess> _initialisationProcessMock = new(MockBehavior.Strict);
    private readonly MutationTestInput _mutationTestInput;
    private readonly IFileSystem _fileSystemMock = new MockFileSystem();
    private readonly string _testFilePath = "c:\\mytestfile.cs";
    private readonly string _testFileContents = @"using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ExtraProject.XUnit
{
    public class UnitTest1
    {
        [TestMethod]
        public void Test1()
        {
            // example test
        }

        [TestMethod]
        public void Test2()
        {
            // example test
        }
    }
}
";

    public ProjectMutatorTests()
    {
        _mutationTestProcessMock.Setup(x => x.Mutate());
        _mutationTestProcessMock.Setup(x => x.Initialize(It.IsAny<MutationTestInput>(), It.IsAny<IStrykerOptions>(), It.IsAny<IReporter>()));
        _fileSystemMock.File.WriteAllText(_testFilePath, _testFileContents);

        var analyzerResult = TestHelper.SetupProjectAnalyzerResult(
            properties: new Dictionary<string, string>
            {
                { "Language", "C#" },
                { "AssemblyName", "TestProject" },
                { "TargetDir", "c:\\bin\\Debug\\netcoreapp3.1" },
                { "TargetFileName", "TestProject.dll" }
            },
            projectFilePath: "c:\\project.csproj",
            targetFramework: "netcoreapp3.1",
            projectReferences: Array.Empty<string>(),
            sourceFiles: Array.Empty<string>()).Object;

        var folder = new FolderComposite();
        folder.Add(new CsharpFileLeaf
        {
            FullPath = "c:\\TestClass.cs",
            SyntaxTree = CSharpSyntaxTree.ParseText("class TestClass { }")
        });

        _mutationTestInput = new MutationTestInput()
        {
            SourceProjectInfo = new Stryker.Core.ProjectComponents.SourceProjects.SourceProjectInfo()
            {
                AnalyzerResult = analyzerResult,
                ProjectContents = folder
            },
            TestProjectsInfo = new TestProjectsInfo(_fileSystemMock)
            {
                TestProjects = new List<TestProject>
                {
                    new(_fileSystemMock, TestHelper.SetupProjectAnalyzerResult(
                        projectFilePath: "c:\\testproject.csproj",
                        targetFramework: "netcoreapp3.1",
                        sourceFiles: new [] { _testFilePath }).Object)
                }
            }
        };
    }

    [TestMethod]
    public void MutateProject_WithNunitFqnId_RegistersTestToFile()
    {
        // arrange
        var fileSystem = new MockFileSystem();
        const string filePath = "C:\\tests\\SampleTests.cs";
        fileSystem.File.WriteAllText(filePath, @"
namespace MyProject.NUnit
{
    public class SampleTests
    {
        public void TestAgeExplicit() { }
    }
}");
        var serviceProviderMock = new Mock<IServiceProvider>();
        var target = new ProjectMutator(TestLoggerFactory.CreateLogger<ProjectMutator>(), serviceProviderMock.Object);
        const string fqn = "MyProject.NUnit.SampleTests.TestAgeExplicit";
        var description = CreateFallbackDescription(id: fqn, name: "TestAgeExplicit", fqn: fqn);
        var input = CreateInput(fileSystem, [description], [filePath]);

        // act
        target.MutateProject(new StrykerOptions(), input, _reporterMock.Object, _mutationTestProcessMock.Object);

        // assert
        var testFile = input.TestProjectsInfo.TestFiles.ShouldHaveSingleItem();
        testFile.Tests.ShouldHaveSingleItem();
        testFile.Tests[0].Id.ShouldBe(fqn);
    }

    [TestMethod]
    public void MutateProject_WithNunitFqnIdAndParameters_StripsParameterListBeforeMatching()
    {
        // arrange
        var fileSystem = new MockFileSystem();
        const string filePath = "C:\\tests\\SampleTests.cs";
        fileSystem.File.WriteAllText(filePath, @"
namespace MyProject.NUnit
{
    public class SampleTests
    {
        public void TestAgeExplicit() { }
    }
}");
        var serviceProviderMock = new Mock<IServiceProvider>();
        var target = new ProjectMutator(TestLoggerFactory.CreateLogger<ProjectMutator>(), serviceProviderMock.Object);
        const string fqnWithParams = "MyProject.NUnit.SampleTests.TestAgeExplicit(29,False)";
        var description = CreateFallbackDescription(id: fqnWithParams, name: "TestAgeExplicit(29,False)", fqn: fqnWithParams);
        var input = CreateInput(fileSystem, [description], [filePath]);

        // act
        target.MutateProject(new StrykerOptions(), input, _reporterMock.Object, _mutationTestProcessMock.Object);

        // assert
        var testFile = input.TestProjectsInfo.TestFiles.ShouldHaveSingleItem();
        testFile.Tests.ShouldHaveSingleItem();
        testFile.Tests[0].Id.ShouldBe(fqnWithParams);
    }

    [TestMethod]
    public void MutateProject_WithMstestGuidId_FindsMethodByDisplayName()
    {
        // arrange
        var fileSystem = new MockFileSystem();
        const string filePath = "C:\\tests\\SampleTests.cs";
        fileSystem.File.WriteAllText(filePath, @"
namespace MyProject.MSTest
{
    public class SampleTests
    {
        public void TestTimeout() { }
    }
}");
        var serviceProviderMock = new Mock<IServiceProvider>();
        var target = new ProjectMutator(TestLoggerFactory.CreateLogger<ProjectMutator>(), serviceProviderMock.Object);
        var guid = Guid.NewGuid().ToString();
        var description = CreateFallbackDescription(id: guid, name: "TestTimeout", fqn: guid);
        var input = CreateInput(fileSystem, [description], [filePath]);

        // act
        target.MutateProject(new StrykerOptions(), input, _reporterMock.Object, _mutationTestProcessMock.Object);

        // assert
        var testFile = input.TestProjectsInfo.TestFiles.ShouldHaveSingleItem();
        testFile.Tests.ShouldHaveSingleItem();
        testFile.Tests[0].Id.ShouldBe(guid);
    }

    [TestMethod]
    public void MutateProject_WithMstestGuidIdAndParameters_StripsParameterListBeforeMatching()
    {
        // arrange
        var fileSystem = new MockFileSystem();
        const string filePath = "C:\\tests\\SampleTests.cs";
        fileSystem.File.WriteAllText(filePath, @"
namespace MyProject.MSTest
{
    public class SampleTests
    {
        public void TestAgeExplicit() { }
    }
}");
        var serviceProviderMock = new Mock<IServiceProvider>();
        var target = new ProjectMutator(TestLoggerFactory.CreateLogger<ProjectMutator>(), serviceProviderMock.Object);
        var guid = Guid.NewGuid().ToString();
        var description = CreateFallbackDescription(id: guid, name: "TestAgeExplicit (29,False)", fqn: guid);
        var input = CreateInput(fileSystem, [description], [filePath]);

        // act
        target.MutateProject(new StrykerOptions(), input, _reporterMock.Object, _mutationTestProcessMock.Object);

        // assert
        var testFile = input.TestProjectsInfo.TestFiles.ShouldHaveSingleItem();
        testFile.Tests.ShouldHaveSingleItem();
        testFile.Tests[0].Id.ShouldBe(guid);
    }

    [TestMethod]
    public void MutateProject_WithMultipleFilesWithSameClassAndMethod_UsesNamespaceToSelectCorrectFile()
    {
        // arrange
        var fileSystem = new MockFileSystem();
        const string fileA = "C:\\tests\\NUnitTests.cs";
        const string fileB = "C:\\tests\\XUnitTests.cs";
        fileSystem.File.WriteAllText(fileA, @"
namespace MyProject.NUnit
{
    public class SampleTests
    {
        public void TestAgeExplicit() { }
    }
}");
        fileSystem.File.WriteAllText(fileB, @"
namespace MyProject.XUnit
{
    public class SampleTests
    {
        public void TestAgeExplicit() { }
    }
}");
        var serviceProviderMock = new Mock<IServiceProvider>();
        var target = new ProjectMutator(TestLoggerFactory.CreateLogger<ProjectMutator>(), serviceProviderMock.Object);
        const string nunitFqn = "MyProject.NUnit.SampleTests.TestAgeExplicit";
        var description = CreateFallbackDescription(id: nunitFqn, name: "TestAgeExplicit", fqn: nunitFqn);
        var input = CreateInput(fileSystem, [description], [fileA, fileB]);

        // act
        target.MutateProject(new StrykerOptions(), input, _reporterMock.Object, _mutationTestProcessMock.Object);

        // assert
        var testFiles = input.TestProjectsInfo.TestFiles.ToList();
        testFiles.ShouldContain(tf => tf.FilePath == fileA);
        testFiles.ShouldContain(tf => tf.FilePath == fileB);
        testFiles.Single(tf => tf.FilePath == fileA).Tests.ShouldHaveSingleItem();
        testFiles.Single(tf => tf.FilePath == fileB).Tests.ShouldBeEmpty();
    }

    [TestMethod]
    public void MutateProject_WhenMethodNotFoundInAnyFile_RegistersNoTest()
    {
        // arrange
        var fileSystem = new MockFileSystem();
        const string filePath = "c:\\tests\\SampleTests.cs";
        fileSystem.File.WriteAllText(filePath, @"
namespace MyProject.Tests
{
    public class SampleTests
    {
        public void ExistingTest() { }
    }
}");
        var serviceProviderMock = new Mock<IServiceProvider>();
        var target = new ProjectMutator(TestLoggerFactory.CreateLogger<ProjectMutator>(), serviceProviderMock.Object);
        const string fqn = "MyProject.Tests.SampleTests.NonExistingTest";
        var description = CreateFallbackDescription(id: fqn, name: "NonExistingTest", fqn: fqn);
        var input = CreateInput(fileSystem, [description], [filePath]);

        // act
        target.MutateProject(new StrykerOptions(), input, _reporterMock.Object, _mutationTestProcessMock.Object);

        // assert
        var testFile = input.TestProjectsInfo.TestFiles.ShouldHaveSingleItem();
        testFile.Tests.ShouldBeEmpty();
    }

    [TestMethod]
    public void MutateProject_ShouldMutateProjectAndEnsureTestsAreRegistered()
    {
        // arrange
        var options = new StrykerOptions();
        var serviceProviderMock = new Mock<IServiceProvider>();
        var target = new ProjectMutator(TestLoggerFactory.CreateLogger<ProjectMutator>(), serviceProviderMock.Object);
        var testCase1 = new VsTestCase(new TestCase("mytestname1", new Uri(_testFilePath), _testFileContents)
        {
            Id = Guid.NewGuid(),
            CodeFilePath = _testFilePath,
            LineNumber = 7,
        });
        var failedTest = testCase1.Id;
        var testCase2 = new VsTestCase(new TestCase("mytestname2", new Uri(_testFilePath), _testFileContents)
        {
            Id = Guid.NewGuid(),
            CodeFilePath = _testFilePath,
            LineNumber = 13,
        });
        var successfulTest = testCase2.Id;
        var tests = new List<VsTestDescription> { new VsTestDescription(testCase1), new VsTestDescription(testCase2) };
        var initialTestRunResult = new TestRunResult(
            vsTestDescriptions: tests,
            executedTests: new TestIdentifierList(failedTest, successfulTest),
            failedTests: new TestIdentifierList(failedTest),
            timedOutTest: TestIdentifierList.NoTest(),
            message: "testrun succesful",
            Enumerable.Empty<string>(),
            timeSpan: TimeSpan.FromSeconds(2));

        var initialTestrun = new InitialTestRun(initialTestRunResult, new TimeoutValueCalculator(500));

        _mutationTestInput.InitialTestRun = initialTestrun;
        // act
        var result = target.MutateProject(options, _mutationTestInput, _reporterMock.Object, _mutationTestProcessMock.Object);

        // assert
        result.ShouldNotBeNull();
        var testFile = _mutationTestInput.TestProjectsInfo.TestFiles.ShouldHaveSingleItem();
        testFile.Tests.Count.ShouldBe(2);
    }

    private static IFrameworkTestDescription CreateFallbackDescription(string id, string name, string fqn)
    {
        var testCaseMock = new Mock<ITestCase>();
        testCaseMock.Setup(x => x.Id).Returns(id);
        testCaseMock.Setup(x => x.Name).Returns(name);
        testCaseMock.Setup(x => x.FullyQualifiedName).Returns(fqn);
        testCaseMock.Setup(x => x.CodeFilePath).Returns(string.Empty);
        testCaseMock.Setup(x => x.LineNumber).Returns(0);

        var descriptionMock = new Mock<IFrameworkTestDescription>();
        descriptionMock.Setup(x => x.Case).Returns(testCaseMock.Object);
        descriptionMock.Setup(x => x.Id).Returns(id);

        return descriptionMock.Object;
    }

    private static MutationTestInput CreateInput(IFileSystem fileSystem, IList<IFrameworkTestDescription> descriptions, string[] testFilePaths)
    {
        var testRunResult = new TestRunResult(
            vsTestDescriptions: descriptions,
            executedTests: TestIdentifierList.EveryTest(),
            failedTests: TestIdentifierList.NoTest(),
            timedOutTest: TestIdentifierList.NoTest(),
            message: "ok",
            messages: [],
            timeSpan: TimeSpan.Zero);

        var sourceAnalyzerResult = TestHelper.SetupProjectAnalyzerResult(
            properties: new Dictionary<string, string>
            {
                { "Language", "C#" }, { "AssemblyName", "SourceProject" },
                { "TargetDir", "c:\\bin\\Debug\\net" }, { "TargetFileName", "SourceProject.dll" }
            },
            projectFilePath: "c:\\source.csproj",
            targetFramework: "net",
            projectReferences: [],
            sourceFiles: []);

        var testAnalyzerResult = TestHelper.SetupProjectAnalyzerResult(
            projectFilePath: "c:\\testproject.csproj",
            targetFramework: "net",
            sourceFiles: testFilePaths);

        var sourceFolder = new FolderComposite();
        sourceFolder.Add(new CsharpFileLeaf { FullPath = "c:\\Source.cs", SyntaxTree = CSharpSyntaxTree.ParseText("class Source { }") });

        return new MutationTestInput
        {
            SourceProjectInfo = new SourceProjectInfo
            {
                AnalyzerResult = sourceAnalyzerResult.Object,
                ProjectContents = sourceFolder
            },
            TestProjectsInfo = new TestProjectsInfo(fileSystem)
            {
                TestProjects = [new TestProject(fileSystem, testAnalyzerResult.Object)]
            },
            InitialTestRun = new InitialTestRun(testRunResult, new TimeoutValueCalculator(500))
        };
    }
}
