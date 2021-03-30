using Buildalyzer;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.Extensions.Logging;
using Moq;
using Shouldly;
using Stryker.Core.Compiling;
using Stryker.Core.CoverageAnalysis;
using Stryker.Core.Exceptions;
using Stryker.Core.Initialisation;
using Stryker.Core.Logging;
using Stryker.Core.MutantFilters;
using Stryker.Core.Mutants;
using Stryker.Core.MutationTest;
using Stryker.Core.Mutators;
using Stryker.Core.Options;
using Stryker.Core.ProjectComponents;
using Stryker.Core.Reporters;
using Stryker.Core.TestRunners;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.IO.Abstractions.TestingHelpers;
using System.Linq;
using System.Reflection;
using Xunit;

namespace Stryker.Core.UnitTest.MutationTest
{

    public delegate bool UpdateHandler(IReadOnlyList<Mutant> mutants, TestsGuidList ranTestsGuids,
        TestsGuidList failedTestsGuids);

    public class MutationTestProcessTests
    {
        private string CurrentDirectory { get; }
        private string FilesystemRoot { get; }
        private string SourceFile { get; }
        private readonly IEnumerable<PortableExecutableReference> _assemblies;

        public MutationTestProcessTests()
        {
            CurrentDirectory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            FilesystemRoot = Path.GetPathRoot(CurrentDirectory);
            SourceFile = File.ReadAllText(CurrentDirectory + "/TestResources/ExampleSourceFile.cs");
            _assemblies = new ReferenceProvider().GetReferencedAssemblies();
        }

        [Fact]
        public void ShouldCallMutantOrchestratorAndReporter()
        {
            var inputFile = new CsharpFileLeaf()
            {
                SourceCode = SourceFile,
                SyntaxTree = CSharpSyntaxTree.ParseText(SourceFile)
            };
            var folder = new CsharpFolderComposite();
            folder.Add(inputFile);
            var input = new MutationTestInput()
            {
                ProjectInfo = new ProjectInfo()
                {
                    ProjectUnderTestAnalyzerResult = TestHelper.SetupProjectAnalyzerResult(properties: new Dictionary<string, string>()
                        {
                            { "TargetDir", "/bin/Debug/netcoreapp2.1" },
                            { "TargetFileName", "TestName.dll" },
                            { "AssemblyName", "AssemblyName" },
                            { "Language", "C#" }
                        }).Object,
                    TestProjectAnalyzerResults = new List<IAnalyzerResult> { TestHelper.SetupProjectAnalyzerResult(properties: new Dictionary<string, string>()
                        {
                            { "TargetDir", "/bin/Debug/netcoreapp2.1" },
                            { "TargetFileName", "TestName.dll" },
                            { "AssemblyName", "AssemblyName" },
                            { "Language", "C#" }
                        }).Object
                    },
                    ProjectContents = folder
                },
                AssemblyReferences = _assemblies
            };

            var fileSystem = new MockFileSystem(new Dictionary<string, MockFileData>
            {
                { Path.Combine(FilesystemRoot, "ExampleProject","Recursive.cs"), new MockFileData(SourceFile)},
                { Path.Combine(FilesystemRoot, "ExampleProject.Test", "bin", "Debug", "netcoreapp2.0", "ExampleProject.dll"), new MockFileData("Bytecode") },
                { Path.Combine(FilesystemRoot, "ExampleProject.Test", "obj", "Release", "netcoreapp2.0", "ExampleProject.dll"), new MockFileData("Bytecode") }
            });

            var mutantToBeSkipped = new Mutant() { Mutation = new Mutation() };
            var mockMutants = new Collection<Mutant>() { new Mutant() { Mutation = new Mutation() }, mutantToBeSkipped };

            // create mocks
            var orchestratorMock = new Mock<MutantOrchestrator<SyntaxNode>>(MockBehavior.Strict);
            var reporterMock = new Mock<IReporter>(MockBehavior.Strict);
            var mutationTestExecutorMock = new Mock<IMutationTestExecutor>(MockBehavior.Strict);
            var coverageAnalyzerMock = new Mock<ICoverageAnalyser>(MockBehavior.Strict);

            // setup mocks
            orchestratorMock.Setup(x => x.GetLatestMutantBatch()).Returns(mockMutants);
            orchestratorMock.Setup(x => x.Mutate(It.IsAny<SyntaxNode>())).Returns(CSharpSyntaxTree.ParseText(SourceFile).GetRoot());
            orchestratorMock.SetupAllProperties();
            var options = new StrykerOptions(devMode: true, excludedMutations: new string[] { }, fileSystem: new MockFileSystem());

            var target = new MutationTestProcess(input,
                reporterMock.Object,
                mutationTestExecutorMock.Object,
                orchestratorMock.Object,
                fileSystem,
                new BroadcastMutantFilter(Enumerable.Empty<IMutantFilter>()),
                coverageAnalyzerMock.Object,
                options);

            // start mutation process
            target.Mutate();

            target.FilterMutants();

            // verify the right methods were called
            orchestratorMock.Verify(x => x.Mutate(It.IsAny<SyntaxNode>()), Times.Once);
        }

        [Fact]
        public void FilterMutantsShouldCallMutantFilters()
        {
            var inputFile = new CsharpFileLeaf()
            {
                SourceCode = SourceFile,
                SyntaxTree = CSharpSyntaxTree.ParseText(SourceFile)
            };

            var folder = new CsharpFolderComposite();
            folder.Add(inputFile);

            var input = new MutationTestInput()
            {
                ProjectInfo = new ProjectInfo()
                {
                    ProjectUnderTestAnalyzerResult = TestHelper.SetupProjectAnalyzerResult(properties: new Dictionary<string, string>()
                        {
                            { "TargetDir", "/bin/Debug/netcoreapp2.1" },
                            { "TargetFileName", "TestName.dll" },
                            { "AssemblyName", "AssemblyName" },
                            { "Language", "C#" }
                        }).Object,
                    TestProjectAnalyzerResults = new List<IAnalyzerResult> { TestHelper.SetupProjectAnalyzerResult(properties: new Dictionary<string, string>()
                        {
                            { "TargetDir", "/bin/Debug/netcoreapp2.1" },
                            { "TargetFileName", "TestName.dll" },
                            { "AssemblyName", "AssemblyName" },
                            { "Language", "C#" }
                        }).Object
                    },
                    ProjectContents = folder
                },
                AssemblyReferences = _assemblies
            };

            var fileSystem = new MockFileSystem(new Dictionary<string, MockFileData>
            {
                { Path.Combine(FilesystemRoot, "ExampleProject","Recursive.cs"), new MockFileData(SourceFile)},
                { Path.Combine(FilesystemRoot, "ExampleProject.Test", "bin", "Debug", "netcoreapp2.0", "ExampleProject.dll"), new MockFileData("Bytecode") },
                { Path.Combine(FilesystemRoot, "ExampleProject.Test", "obj", "Release", "netcoreapp2.0", "ExampleProject.dll"), new MockFileData("Bytecode") }
            });

            var mutantToBeSkipped = new Mutant() { Mutation = new Mutation() };
            var compileErrorMutant = new Mutant() { Mutation = new Mutation(), ResultStatus = MutantStatus.CompileError };
            var mockMutants = new Collection<Mutant>() { new Mutant() { Mutation = new Mutation() }, mutantToBeSkipped, compileErrorMutant };

            // create mocks
            var orchestratorMock = new Mock<MutantOrchestrator<SyntaxNode>>(MockBehavior.Strict);
            var reporterMock = new Mock<IReporter>(MockBehavior.Strict);
            var mutationTestExecutorMock = new Mock<IMutationTestExecutor>(MockBehavior.Strict);
            var mutantFilterMock = new Mock<IMutantFilter>(MockBehavior.Strict);
            var coverageAnalyzerMock = new Mock<ICoverageAnalyser>(MockBehavior.Strict);

            // setup mocks
            reporterMock.Setup(x => x.OnMutantsCreated(It.IsAny<ReadOnlyProjectComponent>()));
            orchestratorMock.Setup(x => x.GetLatestMutantBatch()).Returns(mockMutants);
            orchestratorMock.Setup(x => x.Mutate(It.IsAny<SyntaxNode>())).Returns(CSharpSyntaxTree.ParseText(SourceFile).GetRoot());
            orchestratorMock.SetupAllProperties();
            mutantFilterMock.SetupGet(x => x.DisplayName).Returns("Mock filter");
            IEnumerable<Mutant> mutantsPassedToFilter = null;
            mutantFilterMock.Setup(x => x.FilterMutants(It.IsAny<IEnumerable<Mutant>>(), It.IsAny<ReadOnlyFileLeaf>(), It.IsAny<IStrykerOptions>()))
                .Callback<IEnumerable<Mutant>, ReadOnlyFileLeaf, IStrykerOptions>((mutants, _, __) => mutantsPassedToFilter = mutants)
                .Returns((IEnumerable<Mutant> mutants, ReadOnlyFileLeaf file, IStrykerOptions o) => mutants.Take(1));

            var options = new StrykerOptions(devMode: true, excludedMutations: new string[] { }, fileSystem: new MockFileSystem());

            var target = new MutationTestProcess(input,
                reporterMock.Object,
                mutationTestExecutorMock.Object,
                orchestratorMock.Object,
                fileSystem,
                new BroadcastMutantFilter(new[] { mutantFilterMock.Object }),
                coverageAnalyzerMock.Object,
                options);

            // start mutation process
            target.Mutate();

            target.FilterMutants();

            // verify that compiler error mutants are not passed to filter
            mutantsPassedToFilter.ShouldNotContain(compileErrorMutant);

            // verify that filtered mutants are skipped
            inputFile.Mutants.ShouldContain(mutantToBeSkipped);
            mutantToBeSkipped.ResultStatus.ShouldBe(MutantStatus.Ignored);
        }

        [Fact]
        public void MutateShouldWriteToDisk_IfCompilationIsSuccessful()
        {
            var folder = new CsharpFolderComposite();
            folder.Add(new CsharpFileLeaf
            {
                SourceCode = SourceFile,
                SyntaxTree = CSharpSyntaxTree.ParseText(SourceFile)
            });

            var input = new MutationTestInput()
            {
                ProjectInfo = new ProjectInfo()
                {
                    ProjectUnderTestAnalyzerResult = TestHelper.SetupProjectAnalyzerResult(properties: new Dictionary<string, string>()
                        {
                            { "TargetDir", Path.Combine(FilesystemRoot, "ProjectUnderTest", "bin", "Debug", "netcoreapp2.0") },
                            { "TargetFileName", "ProjectUnderTest.dll" },
                            { "AssemblyName", "ProjectUnderTest.dll" },
                            { "Language", "C#" }
                        }).Object,
                    TestProjectAnalyzerResults = new List<IAnalyzerResult> { TestHelper.SetupProjectAnalyzerResult(properties: new Dictionary<string, string>()
                        {
                            { "TargetDir", Path.Combine(FilesystemRoot, "TestProject", "bin", "Debug", "netcoreapp2.0") },
                            { "TargetFileName", "TestProject.dll" },
                            { "Language", "C#" }
                        }).Object
                    },
                    ProjectContents = folder
                },
                AssemblyReferences = _assemblies
            };
            var mockMutants = new Collection<Mutant>() { new Mutant() { Mutation = new Mutation() } };

            // create mocks
            var orchestratorMock = new Mock<MutantOrchestrator<SyntaxNode>>(MockBehavior.Strict);
            var reporterMock = new Mock<IReporter>(MockBehavior.Strict);
            var mutationTestExecutorMock = new Mock<IMutationTestExecutor>(MockBehavior.Strict);
            var compilingProcessMock = new Mock<ICompilingProcess>(MockBehavior.Strict);
            var coverageAnalyzerMock = new Mock<ICoverageAnalyser>(MockBehavior.Strict);
            var fileSystem = new MockFileSystem(new Dictionary<string, MockFileData>
            {
                { Path.Combine(FilesystemRoot, "SomeFile.cs"), new MockFileData("SomeFile")},
            });
            fileSystem.AddDirectory(Path.Combine(FilesystemRoot, "TestProject", "bin", "Debug", "netcoreapp2.0"));

            // setup mocks
            orchestratorMock.Setup(x => x.Mutate(It.IsAny<SyntaxNode>())).Returns(CSharpSyntaxTree.ParseText(SourceFile).GetRoot());
            orchestratorMock.SetupAllProperties();
            orchestratorMock.Setup(x => x.GetLatestMutantBatch()).Returns(mockMutants);
            reporterMock.Setup(x => x.OnMutantsCreated(It.IsAny<ReadOnlyProjectComponent>()));

            var options = new StrykerOptions();
            var target = new MutationTestProcess(input,
                reporterMock.Object,
                mutationTestExecutorMock.Object,
                orchestratorMock.Object,
                fileSystem,
                new BroadcastMutantFilter(Enumerable.Empty<IMutantFilter>()),
                coverageAnalyzerMock.Object,
                options);

            target.Mutate();

            // Verify the created assembly is written to disk on the right location
            string expectedPath = Path.Combine(FilesystemRoot, "TestProject", "bin", "Debug", "netcoreapp2.0", "ProjectUnderTest.dll");
            fileSystem.ShouldContainFile(expectedPath);
        }

        [Fact]
        public void ShouldCallExecutorForEveryMutant()
        {
            var mutant = new Mutant { Id = 1, CoveringTests = TestsGuidList.EveryTest() };
            var otherMutant = new Mutant { Id = 2, CoveringTests = TestsGuidList.EveryTest() };
            string basePath = Path.Combine(FilesystemRoot, "ExampleProject.Test");

            var folder = new CsharpFolderComposite();
            folder.Add(new CsharpFileLeaf()
            {
                SourceCode = SourceFile,
                Mutants = new List<Mutant>() { mutant, otherMutant }
            });

            var input = new MutationTestInput()
            {
                ProjectInfo = new ProjectInfo()
                {
                    ProjectUnderTestAnalyzerResult = TestHelper.SetupProjectAnalyzerResult(properties: new Dictionary<string, string>()
                        {
                            { "TargetDir", "/bin/Debug/netcoreapp2.1" },
                            { "TargetFileName", "TestName.dll" },
                            { "Language", "C#" }
                        }).Object
                    ,
                    ProjectContents = folder
                },
                AssemblyReferences = _assemblies
            };
            var reporterMock = new Mock<IReporter>(MockBehavior.Strict);
            reporterMock.Setup(x => x.OnMutantTested(It.IsAny<Mutant>()));

            var runnerMock = new Mock<ITestRunner>();
            runnerMock.Setup(x => x.DiscoverNumberOfTests()).Returns(1);
            var executorMock = new Mock<IMutationTestExecutor>(MockBehavior.Strict);
            executorMock.SetupGet(x => x.TestRunner).Returns(runnerMock.Object);
            executorMock.Setup(x => x.Test(It.IsAny<IList<Mutant>>(), It.IsAny<int>(), It.IsAny<TestUpdateHandler>()));

            var mutantFilterMock = new Mock<IMutantFilter>(MockBehavior.Loose);

            var options = new StrykerOptions(basePath: basePath, fileSystem: new MockFileSystem());

            var target = new MutationTestProcess(input,
                reporterMock.Object,
                executorMock.Object,
                mutantFilter: mutantFilterMock.Object,
                options: options);

            target.Test(input.ProjectInfo.ProjectContents.Mutants);

            executorMock.Verify(x => x.Test(new List<Mutant> { mutant }, It.IsAny<int>(), It.IsAny<TestUpdateHandler>()), Times.Once);
        }

        [Theory]
        [InlineData(MutantStatus.Ignored)]
        [InlineData(MutantStatus.CompileError)]
        public void ShouldThrowExceptionWhenOtherStatusThanNotRunIsPassed(MutantStatus status)
        {
            var mutant = new Mutant { Id = 1, ResultStatus = status };
            var basePath = Path.Combine(FilesystemRoot, "ExampleProject.Test");

            var folder = new CsharpFolderComposite();
            folder.Add(new CsharpFileLeaf()
            {
                Mutants = new Collection<Mutant>() { mutant }
            });

            var input = new MutationTestInput()
            {
                ProjectInfo = new ProjectInfo()
                {
                    ProjectUnderTestAnalyzerResult = TestHelper.SetupProjectAnalyzerResult(properties: new Dictionary<string, string>()
                        {
                            { "TargetDir", "/bin/Debug/netcoreapp2.1" },
                            { "TargetFileName", "TestName.dll" }
                        }).Object,
                    TestProjectAnalyzerResults = new List<IAnalyzerResult> { TestHelper.SetupProjectAnalyzerResult(properties: new Dictionary<string, string>()
                        {
                            { "TargetDir", "/bin/Debug/netcoreapp2.1" },
                            { "TargetFileName", "TestName.dll" },
                            { "Language", "C#" }
                        }).Object
                    },
                    ProjectContents = folder
                },
                AssemblyReferences = new ReferenceProvider().GetReferencedAssemblies()
            };
            var reporterMock = new Mock<IReporter>(MockBehavior.Strict);
            reporterMock.Setup(x => x.OnMutantTested(It.IsAny<Mutant>()));

            var runnerMock = new Mock<ITestRunner>();
            runnerMock.Setup(x => x.DiscoverNumberOfTests()).Returns(1);
            var executorMock = new Mock<IMutationTestExecutor>(MockBehavior.Strict);
            executorMock.SetupGet(x => x.TestRunner).Returns(runnerMock.Object);
            executorMock.Setup(x => x.Test(It.IsAny<IList<Mutant>>(), It.IsAny<int>(), It.IsAny<TestUpdateHandler>()));

            var mutantFilterMock = new Mock<IMutantFilter>(MockBehavior.Loose);

            var options = new StrykerOptions(basePath: basePath, fileSystem: new MockFileSystem());

            var target = new MutationTestProcess(input,
                reporterMock.Object,
                executorMock.Object,
                mutantFilter: mutantFilterMock.Object,
                options: options);

            Should.Throw<GeneralStrykerException>(() => target.Test(input.ProjectInfo.ProjectContents.Mutants));
        }

        [Fact]
        public void ShouldNotTest_WhenThereAreNoMutationsAtAll()
        {
            string basePath = Path.Combine(FilesystemRoot, "ExampleProject.Test");

            var folder = new CsharpFolderComposite();
            folder.Add(new CsharpFileLeaf()
            {
                Mutants = new Collection<Mutant>() { }
            });

            var projectUnderTest = TestHelper.SetupProjectAnalyzerResult(
                    properties: new Dictionary<string, string>() { { "Language", "F#" } }).Object;
            var input = new MutationTestInput()
            {
                ProjectInfo = new ProjectInfo()
                {
                    ProjectContents = folder,
                    ProjectUnderTestAnalyzerResult = projectUnderTest
                },
                AssemblyReferences = _assemblies
            };
            var reporterMock = new Mock<IReporter>(MockBehavior.Strict);
            reporterMock.Setup(x => x.OnMutantTested(It.IsAny<Mutant>()));

            var executorMock = new Mock<IMutationTestExecutor>(MockBehavior.Strict);
            executorMock.SetupGet(x => x.TestRunner).Returns(Mock.Of<ITestRunner>());
            executorMock.Setup(x => x.Test(It.IsAny<IList<Mutant>>(), It.IsAny<int>(), It.IsAny<TestUpdateHandler>()));

            var mutantFilterMock = new Mock<IMutantFilter>(MockBehavior.Loose);

            var options = new StrykerOptions(basePath: basePath, fileSystem: new MockFileSystem());

            var target = new MutationTestProcess(input,
                reporterMock.Object,
                executorMock.Object,
                mutantFilter: mutantFilterMock.Object,
                options: options);

            var testResult = target.Test(input.ProjectInfo.ProjectContents.Mutants);

            executorMock.Verify(x => x.Test(It.IsAny<IList<Mutant>>(), It.IsAny<int>(), It.IsAny<TestUpdateHandler>()), Times.Never);
            reporterMock.Verify(x => x.OnStartMutantTestRun(It.IsAny<IList<Mutant>>()), Times.Never);
            reporterMock.Verify(x => x.OnMutantTested(It.IsAny<Mutant>()), Times.Never);
            testResult.MutationScore.ShouldBe(double.NaN);
        }

        [Fact]
        public void ShouldNotTest_WhenThereAreNoTestableMutations()
        {
            string basePath = Path.Combine(FilesystemRoot, "ExampleProject.Test");

            var folder = new CsharpFolderComposite();
            folder.Add(new CsharpFileLeaf()
            {
                Mutants = new Collection<Mutant>() { }
            });

            var projectUnderTest = TestHelper.SetupProjectAnalyzerResult(
                    properties: new Dictionary<string, string>() { { "Language", "F#" } }).Object;
            var input = new MutationTestInput()
            {
                ProjectInfo = new ProjectInfo()
                {
                    ProjectContents = folder,
                    ProjectUnderTestAnalyzerResult = projectUnderTest
                },
                AssemblyReferences = _assemblies
            };
            var reporterMock = new Mock<IReporter>(MockBehavior.Strict);
            reporterMock.Setup(x => x.OnMutantTested(It.IsAny<Mutant>()));

            var runnerMock = new Mock<ITestRunner>();
            runnerMock.Setup(x => x.DiscoverNumberOfTests()).Returns(1);
            var executorMock = new Mock<IMutationTestExecutor>(MockBehavior.Strict);
            executorMock.SetupGet(x => x.TestRunner).Returns(runnerMock.Object);
            executorMock.Setup(x => x.Test(It.IsAny<IList<Mutant>>(), It.IsAny<int>(), It.IsAny<TestUpdateHandler>()));

            var mutantFilterMock = new Mock<IMutantFilter>(MockBehavior.Loose);

            var options = new StrykerOptions(fileSystem: new MockFileSystem(), basePath: basePath);

            var target = new MutationTestProcess(input,
                reporterMock.Object,
                executorMock.Object,
                mutantFilter: mutantFilterMock.Object,
                options: options);

            var testResult = target.Test(folder.Mutants);

            executorMock.Verify(x => x.Test(It.IsAny<IList<Mutant>>(), It.IsAny<int>(), It.IsAny<TestUpdateHandler>()), Times.Never);
            reporterMock.Verify(x => x.OnMutantTested(It.IsAny<Mutant>()), Times.Never);
            testResult.MutationScore.ShouldBe(double.NaN);
        }
    }
}
