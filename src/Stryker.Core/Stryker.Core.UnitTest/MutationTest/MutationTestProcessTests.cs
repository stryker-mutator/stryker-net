﻿using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Moq;
using Shouldly;
using Stryker.Core.Compiling;
using Stryker.Core.Initialisation;
using Stryker.Core.MutantFilters;
using Stryker.Core.Mutants;
using Stryker.Core.MutationTest;
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

    public delegate bool UpdateHandler(IReadOnlyList<Mutant> mutants, TestListDescription ranTests,
        TestListDescription failedTests);

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
            var inputFile = new FileLeaf()
            {
                Name = "Recursive.cs",
                SourceCode = SourceFile,
                SyntaxTree = CSharpSyntaxTree.ParseText(SourceFile)
            };

            var input = new MutationTestInput()
            {
                ProjectInfo = new ProjectInfo()
                {
                    TestProjectAnalyzerResults = new List<ProjectAnalyzerResult> {
                        new ProjectAnalyzerResult(null, null)
                        {
                            AssemblyPath = "/bin/Debug/netcoreapp2.1/TestName.dll",
                            Properties = new Dictionary<string, string>()
                            {
                                { "TargetDir", "/bin/Debug/netcoreapp2.1" },
                                { "AssemblyName", "TestName" }
                            }
                        }
                    },
                    ProjectUnderTestAnalyzerResult = new ProjectAnalyzerResult(null, null)
                    {
                        AssemblyPath = "/bin/Debug/netcoreapp2.1/TestName.dll",
                        Properties = new Dictionary<string, string>()
                        {
                            { "TargetDir", "/bin/Debug/netcoreapp2.1" },
                            { "AssemblyName", "TestName" }
                        }
                    },
                    ProjectContents = new FolderComposite()
                    {
                        Name = Path.Combine(FilesystemRoot, "ExampleProject"),
                        Children = new Collection<ProjectComponent>() {
                            inputFile,
                        }
                    },
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
            var orchestratorMock = new Mock<IMutantOrchestrator>(MockBehavior.Strict);
            var reporterMock = new Mock<IReporter>(MockBehavior.Strict);
            var mutationTestExecutorMock = new Mock<IMutationTestExecutor>(MockBehavior.Strict);
            var compilingProcessMock = new Mock<ICompilingProcess>(MockBehavior.Strict);

            // setup mocks
            reporterMock.Setup(x => x.OnMutantsCreated(It.IsAny<ProjectComponent>()));
            orchestratorMock.Setup(x => x.GetLatestMutantBatch()).Returns(mockMutants);
            orchestratorMock.Setup(x => x.Mutate(It.IsAny<SyntaxNode>())).Returns(CSharpSyntaxTree.ParseText(SourceFile).GetRoot());
            orchestratorMock.SetupAllProperties();
            compilingProcessMock.Setup(x => x.Compile(It.IsAny<IEnumerable<SyntaxTree>>(), It.IsAny<MemoryStream>(), It.IsAny<MemoryStream>(), true))
                .Returns(new CompilingProcessResult()
                {
                    Success = true
                });
            var options = new StrykerOptions(devMode: true, excludedMutations: new string[] { });


            var target = new MutationTestProcess(input,
                reporterMock.Object,
                mutationTestExecutorMock.Object,
                orchestratorMock.Object,
                compilingProcessMock.Object,
                fileSystem,
                options,
                new BroadcastMutantFilter(Enumerable.Empty<IMutantFilter>()));

            // start mutation process
            target.Mutate();

            // verify the right methods were called
            orchestratorMock.Verify(x => x.Mutate(It.IsAny<SyntaxNode>()), Times.Once);
            reporterMock.Verify(x => x.OnMutantsCreated(It.IsAny<ProjectComponent>()), Times.Once);
        }

        [Fact]
        public void MutateShouldCallMutantFilters()
        {
            var inputFile = new FileLeaf()
            {
                Name = "Recursive.cs",
                SourceCode = SourceFile,
                SyntaxTree = CSharpSyntaxTree.ParseText(SourceFile)
            };

            var input = new MutationTestInput()
            {
                ProjectInfo = new ProjectInfo()
                {
                    TestProjectAnalyzerResults = new List<ProjectAnalyzerResult> {
                        new ProjectAnalyzerResult(null, null)
                        {
                            AssemblyPath = "/bin/Debug/netcoreapp2.1/TestName.dll",
                            Properties = new Dictionary<string, string>()
                            {
                                { "TargetDir", "/bin/Debug/netcoreapp2.1" },
                                { "AssemblyName", "TestName" },
                            }
                        }
                    },
                    ProjectUnderTestAnalyzerResult = new ProjectAnalyzerResult(null, null)
                    {
                        AssemblyPath = "/bin/Debug/netcoreapp2.1/TestName.dll",
                        Properties = new Dictionary<string, string>()
                        {
                            { "TargetDir", "/bin/Debug/netcoreapp2.1" },
                            { "AssemblyName", "TestName" },
                        }
                    },
                    ProjectContents = new FolderComposite()
                    {
                        Name = Path.Combine(FilesystemRoot, "ExampleProject"),
                        Children = new Collection<ProjectComponent>() {
                            inputFile,
                        }
                    },
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
            var orchestratorMock = new Mock<IMutantOrchestrator>(MockBehavior.Strict);
            var reporterMock = new Mock<IReporter>(MockBehavior.Strict);
            var mutationTestExecutorMock = new Mock<IMutationTestExecutor>(MockBehavior.Strict);
            var compilingProcessMock = new Mock<ICompilingProcess>(MockBehavior.Strict);
            var mutantFilterMock = new Mock<IMutantFilter>(MockBehavior.Strict);

            // setup mocks
            reporterMock.Setup(x => x.OnMutantsCreated(It.IsAny<ProjectComponent>()));
            orchestratorMock.Setup(x => x.GetLatestMutantBatch()).Returns(mockMutants);
            orchestratorMock.Setup(x => x.Mutate(It.IsAny<SyntaxNode>())).Returns(CSharpSyntaxTree.ParseText(SourceFile).GetRoot());
            orchestratorMock.SetupAllProperties();
            compilingProcessMock.Setup(x => x.Compile(It.IsAny<IEnumerable<SyntaxTree>>(), It.IsAny<MemoryStream>(), It.IsAny<MemoryStream>(), true))
                .Returns(new CompilingProcessResult()
                {
                    Success = true
                });
            mutantFilterMock.SetupGet(x => x.DisplayName).Returns("Mock filter");
            mutantFilterMock.Setup(x => x.FilterMutants(It.IsAny<IEnumerable<Mutant>>(), It.IsAny<FileLeaf>(), It.IsAny<StrykerOptions>()))
                .Returns((IEnumerable<Mutant> mutants, FileLeaf file, StrykerOptions o) => mutants.Take(1));


            var options = new StrykerOptions(devMode: true, excludedMutations: new string[] { });

            var target = new MutationTestProcess(input,
                reporterMock.Object,
                mutationTestExecutorMock.Object,
                orchestratorMock.Object,
                compilingProcessMock.Object,
                fileSystem,
                options,
                new BroadcastMutantFilter(new[] { mutantFilterMock.Object }));

            // start mutation process
            target.Mutate();

            // verify that filtered mutants are skipped
            inputFile.Mutants.ShouldContain(mutantToBeSkipped);
            mutantToBeSkipped.ResultStatus.ShouldBe(MutantStatus.Ignored);
        }

        [Fact]
        public void MutateShouldWriteToDisk_IfCompilationIsSuccessful()
        {
            string basePath = Path.Combine(FilesystemRoot, "ExampleProject.Test");
            var input = new MutationTestInput()
            {
                ProjectInfo = new ProjectInfo()
                {
                    TestProjectAnalyzerResults = new List<ProjectAnalyzerResult> {
                        new ProjectAnalyzerResult(null, null)
                        {
                            AssemblyPath = Path.Combine(basePath, "bin", "Debug", "netcoreapp2.0", "TestName.dll"),
                            Properties = new Dictionary<string, string>()
                            {
                                { "TargetDir", Path.Combine(basePath, "bin", "Debug", "netcoreapp2.0") },
                                { "AssemblyName", "TestName" },
                            }
                        }
                    },
                    ProjectUnderTestAnalyzerResult = new ProjectAnalyzerResult(null, null)
                    {
                        AssemblyPath = Path.Combine(basePath, "bin", "Debug", "netcoreapp2.0", "ExampleProject.dll"),
                        Properties = new Dictionary<string, string>()
                        {
                            { "TargetDir", Path.Combine(basePath, "bin", "Debug", "netcoreapp2.0") },
                            { "AssemblyName", "ExampleProject" }
                        }
                    },
                    ProjectContents = new FolderComposite()
                    {
                        Name = "ProjectRoot",
                        Children = new Collection<ProjectComponent>() {
                            new FileLeaf
                            {
                                Name = "SomeFile.cs",
                                SourceCode = SourceFile,
                                SyntaxTree = CSharpSyntaxTree.ParseText(SourceFile)
                            }
                        }
                    }
                },
                AssemblyReferences = _assemblies
            };
            var mockMutants = new Collection<Mutant>() { new Mutant() { Mutation = new Mutation() } };

            // create mocks
            var orchestratorMock = new Mock<IMutantOrchestrator>(MockBehavior.Strict);
            var reporterMock = new Mock<IReporter>(MockBehavior.Strict);
            var mutationTestExecutorMock = new Mock<IMutationTestExecutor>(MockBehavior.Strict);
            var compilingProcessMock = new Mock<ICompilingProcess>(MockBehavior.Strict);
            var fileSystem = new MockFileSystem(new Dictionary<string, MockFileData>
            {
                { Path.Combine(FilesystemRoot, "SomeFile.cs"), new MockFileData("SomeFile")},
            });
            fileSystem.AddDirectory(Path.Combine(basePath, "bin", "Debug", "netcoreapp2.0"));

            // setup mocks
            orchestratorMock.Setup(x => x.Mutate(It.IsAny<SyntaxNode>())).Returns(CSharpSyntaxTree.ParseText(SourceFile).GetRoot());
            orchestratorMock.SetupAllProperties();
            orchestratorMock.Setup(x => x.GetLatestMutantBatch()).Returns(mockMutants);
            reporterMock.Setup(x => x.OnMutantsCreated(It.IsAny<ProjectComponent>()));
            compilingProcessMock.Setup(x => x.Compile(It.IsAny<IEnumerable<SyntaxTree>>(), It.IsAny<MemoryStream>(), It.IsAny<MemoryStream>(), It.IsAny<bool>()))
                .Returns(new CompilingProcessResult()
                {
                    Success = true
                });

            var options = new StrykerOptions();
            var target = new MutationTestProcess(input,
                reporterMock.Object,
                mutationTestExecutorMock.Object,
                orchestratorMock.Object,
                compilingProcessMock.Object,
                fileSystem,
                options,
                new BroadcastMutantFilter(Enumerable.Empty<IMutantFilter>()));

            target.Mutate();

            // Verify the created assembly is written to disk on the right location
            string expectedPath = Path.Combine(basePath, "bin", "Debug", "netcoreapp2.0", "ExampleProject.dll");
            fileSystem.FileExists(expectedPath)
                .ShouldBeTrue($"The mutated Assembly was not written to disk, or not to the right location ({expectedPath}).");
        }

        [Fact]
        public void ShouldCallExecutorForEveryMutant()
        {
            var mutant = new Mutant { Id = 1 };
            var otherMutant = new Mutant { Id = 2 };
            string basePath = Path.Combine(FilesystemRoot, "ExampleProject.Test");
            var input = new MutationTestInput()
            {
                ProjectInfo = new ProjectInfo()
                {
                    ProjectUnderTestAnalyzerResult = new ProjectAnalyzerResult(null, null)
                    {
                        AssemblyPath = "/bin/Debug/netcoreapp2.1/TestName.dll",
                        Properties = new Dictionary<string, string>()
                        {
                            { "TargetDir", "/bin/Debug/netcoreapp2.1" },
                            { "TargetFileName", "TestName.dll" }
                        }
                    },
                    ProjectContents = new FolderComposite()
                    {
                        Name = "ProjectRoot",
                        Children = new Collection<ProjectComponent>()
                        {
                            new FileLeaf()
                            {
                                Name = "SomeFile.cs",
                                SourceCode = SourceFile,
                                Mutants = new List<Mutant>() { mutant, otherMutant }
                            }
                        }
                    }
                },
                AssemblyReferences = _assemblies
            };
            var reporterMock = new Mock<IReporter>(MockBehavior.Strict);
            reporterMock.Setup(x => x.OnMutantTested(It.IsAny<Mutant>()));
            reporterMock.Setup(x => x.OnAllMutantsTested(It.IsAny<ProjectComponent>()));
            reporterMock.Setup(x => x.OnStartMutantTestRun(It.IsAny<IList<Mutant>>(), It.IsAny<IEnumerable<TestDescription>>()));

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

            target.Test(options);

            executorMock.Verify(x => x.Test(new List<Mutant> { mutant }, It.IsAny<int>(), It.IsAny<TestUpdateHandler>()), Times.Once);
            reporterMock.Verify(x => x.OnStartMutantTestRun(It.Is<IList<Mutant>>(y => y.Count == 2), It.IsAny<IEnumerable<TestDescription>>()), Times.Once);
            reporterMock.Verify(x => x.OnAllMutantsTested(It.IsAny<ProjectComponent>()), Times.Once);
        }

        [Fact]
        public void ShouldNotCallExecutorForNotCoveredMutants()
        {
            var mutant = new Mutant { Id = 1, ResultStatus = MutantStatus.Survived };
            var otherMutant = new Mutant { Id = 2, ResultStatus = MutantStatus.NotRun };
            var basePath = Path.Combine(FilesystemRoot, "ExampleProject.Test");
            var input = new MutationTestInput()
            {
                ProjectInfo = new ProjectInfo()
                {
                    TestProjectAnalyzerResults = new List<ProjectAnalyzerResult> {
                        new ProjectAnalyzerResult(null, null)
                        {
                            AssemblyPath = "/bin/Debug/netcoreapp2.1/TestName.dll",
                            Properties = new Dictionary<string, string>()
                            {
                                { "TargetDir", "/bin/Debug/netcoreapp2.1" },
                                { "TargetFileName", "TestName.dll" }
                            }
                        }
                    },
                    ProjectUnderTestAnalyzerResult = new ProjectAnalyzerResult(null, null)
                    {
                        AssemblyPath = "/bin/Debug/netcoreapp2.1/TestName.dll",
                        Properties = new Dictionary<string, string>()
                        {
                            { "TargetDir", "/bin/Debug/netcoreapp2.1" },
                            { "TargetFileName", "TestName.dll" }
                        }
                    },
                    ProjectContents = new FolderComposite()
                    {
                        Name = "ProjectRoot",
                        Children = new Collection<ProjectComponent>() {
                            new FileLeaf() {
                                Name = "SomeFile.cs",
                                Mutants = new Collection<Mutant>() { mutant, otherMutant }
                            }
                        }
                    },

                },
                AssemblyReferences = new ReferenceProvider().GetReferencedAssemblies()
            };
            var reporterMock = new Mock<IReporter>(MockBehavior.Strict);
            reporterMock.Setup(x => x.OnMutantTested(It.IsAny<Mutant>()));
            reporterMock.Setup(x => x.OnAllMutantsTested(It.IsAny<ProjectComponent>()));
            reporterMock.Setup(x => x.OnStartMutantTestRun(It.IsAny<IList<Mutant>>(), It.IsAny<IEnumerable<TestDescription>>()));

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

            target.Test(options);

            reporterMock.Verify(x => x.OnStartMutantTestRun(It.Is<IList<Mutant>>(y => y.Count == 1), It.IsAny<IEnumerable<TestDescription>>()), Times.Once);
            executorMock.Verify(x => x.Test(new List<Mutant> { otherMutant }, It.IsAny<int>(), It.IsAny<TestUpdateHandler>()), Times.Once);
            reporterMock.Verify(x => x.OnAllMutantsTested(It.IsAny<ProjectComponent>()), Times.Once);
        }

        [Fact]
        public void ShouldNotTest_WhenAllMutationsWereSkipped()
        {
            var mutant = new Mutant() { Id = 1, ResultStatus = MutantStatus.Ignored };
            string basePath = Path.Combine(FilesystemRoot, "ExampleProject.Test");
            var input = new MutationTestInput()
            {
                ProjectInfo = new ProjectInfo()
                {
                    ProjectContents = new FolderComposite()
                    {
                        Name = "ProjectRoot",
                        Children = new Collection<ProjectComponent>()
                        {
                            new FileLeaf() {
                                Name = "SomeFile.cs",
                                Mutants = new Collection<Mutant>() { mutant }
                            }
                        }
                    },
                },
                AssemblyReferences = _assemblies
            };
            var reporterMock = new Mock<IReporter>(MockBehavior.Strict);
            reporterMock.Setup(x => x.OnMutantTested(It.IsAny<Mutant>()));
            reporterMock.Setup(x => x.OnAllMutantsTested(It.IsAny<ProjectComponent>()));
            reporterMock.Setup(x => x.OnStartMutantTestRun(It.IsAny<IList<Mutant>>(), It.IsAny<IEnumerable<TestDescription>>()));

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

            var testResult = target.Test(options);

            executorMock.Verify(x => x.Test(new List<Mutant> { mutant }, It.IsAny<int>(), It.IsAny<TestUpdateHandler>()), Times.Never);
            reporterMock.Verify(x => x.OnStartMutantTestRun(It.IsAny<IList<Mutant>>(), It.IsAny<IEnumerable<TestDescription>>()), Times.Never);
            reporterMock.Verify(x => x.OnMutantTested(mutant), Times.Never);
            reporterMock.Verify(x => x.OnAllMutantsTested(It.IsAny<ProjectComponent>()), Times.Once);
            testResult.MutationScore.ShouldBe(double.NaN);
        }

        [Fact]
        public void ShouldNotTest_WhenThereAreNoMutationsAtAll()
        {
            string basePath = Path.Combine(FilesystemRoot, "ExampleProject.Test");
            var input = new MutationTestInput()
            {
                ProjectInfo = new ProjectInfo()
                {
                    ProjectContents = new FolderComposite()
                    {
                        Name = "ProjectRoot",
                        Children = new Collection<ProjectComponent>() {
                        new FileLeaf() {
                            Name = "SomeFile.cs",
                            Mutants = new Collection<Mutant>() { }
                        }
                    }
                    },
                },
                AssemblyReferences = _assemblies
            };
            var reporterMock = new Mock<IReporter>(MockBehavior.Strict);
            reporterMock.Setup(x => x.OnMutantTested(It.IsAny<Mutant>()));
            reporterMock.Setup(x => x.OnAllMutantsTested(It.IsAny<ProjectComponent>()));
            reporterMock.Setup(x => x.OnStartMutantTestRun(It.IsAny<IList<Mutant>>(), It.IsAny<IEnumerable<TestDescription>>()));

            var executorMock = new Mock<IMutationTestExecutor>(MockBehavior.Strict);
            executorMock.SetupGet(x => x.TestRunner).Returns(Mock.Of<ITestRunner>());
            executorMock.Setup(x => x.Test(It.IsAny<IList<Mutant>>(), It.IsAny<int>(), It.IsAny<TestUpdateHandler>()));

            var mutantFilterMock = new Mock<IMutantFilter>(MockBehavior.Loose);

            var options = new StrykerOptions(fileSystem: new MockFileSystem(), basePath: basePath);

            var target = new MutationTestProcess(input,
                reporterMock.Object,
                executorMock.Object,
                mutantFilter: mutantFilterMock.Object,
                options: options);

            var testResult = target.Test(options);

            executorMock.Verify(x => x.Test(It.IsAny<IList<Mutant>>(), It.IsAny<int>(), It.IsAny<TestUpdateHandler>()), Times.Never);
            reporterMock.Verify(x => x.OnStartMutantTestRun(It.IsAny<IList<Mutant>>(), It.IsAny<IEnumerable<TestDescription>>()), Times.Never);
            reporterMock.Verify(x => x.OnMutantTested(It.IsAny<Mutant>()), Times.Never);
            reporterMock.Verify(x => x.OnAllMutantsTested(It.IsAny<ProjectComponent>()), Times.Never);
            testResult.MutationScore.ShouldBe(double.NaN);
        }

        [Fact]
        public void ShouldNotTest_WhenThereAreNoTestableMutations()
        {
            var mutant = new Mutant() { Id = 1, ResultStatus = MutantStatus.Ignored };
            var mutant2 = new Mutant() { Id = 2, ResultStatus = MutantStatus.CompileError };
            string basePath = Path.Combine(FilesystemRoot, "ExampleProject.Test");
            var input = new MutationTestInput()
            {
                ProjectInfo = new ProjectInfo()
                {
                    ProjectContents = new FolderComposite()
                    {
                        Name = "ProjectRoot",
                        Children = new Collection<ProjectComponent>() {
                        new FileLeaf() {
                            Name = "SomeFile.cs",
                            Mutants = new Collection<Mutant>() { mutant, mutant2 }
                        }
                    }
                    },
                },
                AssemblyReferences = _assemblies
            };
            var reporterMock = new Mock<IReporter>(MockBehavior.Strict);
            reporterMock.Setup(x => x.OnMutantTested(It.IsAny<Mutant>()));
            reporterMock.Setup(x => x.OnAllMutantsTested(It.IsAny<ProjectComponent>()));
            reporterMock.Setup(x => x.OnStartMutantTestRun(It.IsAny<IList<Mutant>>(), It.IsAny<IEnumerable<TestDescription>>()));

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

            var testResult = target.Test(options);

            executorMock.Verify(x => x.Test(It.IsAny<IList<Mutant>>(), It.IsAny<int>(), It.IsAny<TestUpdateHandler>()), Times.Never);
            reporterMock.Verify(x => x.OnStartMutantTestRun(It.IsAny<IList<Mutant>>(), It.IsAny<IEnumerable<TestDescription>>()), Times.Never);
            reporterMock.Verify(x => x.OnMutantTested(It.IsAny<Mutant>()), Times.Never);
            reporterMock.Verify(x => x.OnAllMutantsTested(It.IsAny<ProjectComponent>()), Times.Once);
            testResult.MutationScore.ShouldBe(double.NaN);
        }
    }
}
