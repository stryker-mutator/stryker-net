using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.IO.Abstractions.TestingHelpers;
using System.Linq;
using System.Reflection;
using Buildalyzer;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Moq;
using Shouldly;
using Stryker.Core.Exceptions;
using Stryker.Core.Initialisation;
using Stryker.Core.MutantFilters;
using Stryker.Core.Mutants;
using Stryker.Core.MutationTest;
using Stryker.Core.Mutators;
using Stryker.Core.Options;
using Stryker.Core.ProjectComponents;
using Stryker.Core.ProjectComponents.SourceProjects;
using Stryker.Core.ProjectComponents.TestProjects;
using Stryker.Core.Reporters;
using Xunit;
using Mutation = Stryker.Core.Mutants.Mutation;

namespace Stryker.Core.UnitTest.MutationTest
{
    public class MutationTestProcessTests : TestBase
    {
        private string CurrentDirectory { get; }
        private string FilesystemRoot { get; }
        private string SourceFile { get; }
        private MockFileSystem fileSystemMock { get; } = new MockFileSystem();

        public MutationTestProcessTests()
        {
            CurrentDirectory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            FilesystemRoot = Path.GetPathRoot(CurrentDirectory);
            SourceFile = File.ReadAllText(CurrentDirectory + "/TestResources/ExampleSourceFile.cs");
        }

        [Fact]
        public void ShouldCallMutantionProcess_MutateAndFilterMutants()
        {
            // Arrange
            var input = new MutationTestInput()
            {
                SourceProjectInfo = new SourceProjectInfo()
                {
                    AnalyzerResult = TestHelper.SetupProjectAnalyzerResult(properties: new Dictionary<string, string>()
                        {
                            { "TargetDir", "/bin/Debug/netcoreapp2.1" },
                            { "TargetFileName", "TestName.dll" },
                            { "AssemblyName", "AssemblyName" },
                            { "Language", "C#" }
                        }).Object
                },
                TestProjectsInfo = new TestProjectsInfo(fileSystemMock)
                {
                    TestProjects = new List<TestProject> {
                        new TestProject(fileSystemMock, TestHelper.SetupProjectAnalyzerResult(properties: new Dictionary<string, string>()
                        {
                            { "TargetDir", "/bin/Debug/netcoreapp2.1" },
                            { "TargetFileName", "TestName.dll" },
                            { "AssemblyName", "AssemblyName" },
                            { "Language", "C#" }
                        }).Object)
                    }
                }
            };

            var options = new StrykerOptions()
            {
                ExcludedMutations = new Mutator[] { }
            };
            var mutationTestExecutorMock = new Mock<IMutationTestExecutor>(MockBehavior.Strict);
            var mutantionProcessMock = new Mock<IMutationProcess>(MockBehavior.Strict);
            mutantionProcessMock.Setup(x => x.Mutate());
            mutantionProcessMock.Setup(x => x.FilterMutants());

            var target = new MutationTestProcess(input, options, null, mutationTestExecutorMock.Object, mutantionProcessMock.Object);

            // Act
            target.Mutate();

            target.FilterMutants();

            // Assert
            mutantionProcessMock.Verify(x => x.Mutate(), Times.Once);
            mutantionProcessMock.Verify(x => x.FilterMutants(), Times.Once);
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

            var fileSystem = new MockFileSystem(new Dictionary<string, MockFileData>
            {
                { Path.Combine(FilesystemRoot, "SomeFile.cs"), new MockFileData("SomeFile")},
            });

            var input = new MutationTestInput()
            {
                SourceProjectInfo = new SourceProjectInfo()
                {
                    AnalyzerResult = TestHelper.SetupProjectAnalyzerResult(properties: new Dictionary<string, string>()
                        {
                            { "TargetDir", Path.Combine(FilesystemRoot, "ProjectUnderTest", "bin", "Debug", "netcoreapp2.0") },
                            { "TargetFileName", "TestName.dll" },
                            { "AssemblyName", "AssemblyName" },
                            { "Language", "C#" }
                        }).Object,
                    ProjectContents = folder
                },
                TestProjectsInfo = new TestProjectsInfo(fileSystemMock)
                {
                    TestProjects = new List<TestProject> {
                        new TestProject(fileSystemMock, TestHelper.SetupProjectAnalyzerResult(properties: new Dictionary<string, string>()
                        {
                            { "TargetDir", Path.Combine(FilesystemRoot, "TestProject", "bin", "Debug", "netcoreapp2.0") },
                            { "TargetFileName", "TestName.dll" },
                            { "Language", "C#" }
                        }).Object)
                    }
                }
            };

            var mockMutants = new Collection<Mutant>() { new() { Mutation = new Mutation() } };

            // create mocks
            var options = new StrykerOptions();
            var orchestratorMock = new Mock<BaseMutantOrchestrator<SyntaxNode>>(MockBehavior.Strict, options);
            var mutationTestExecutorMock = new Mock<IMutationTestExecutor>(MockBehavior.Strict);

            fileSystem.AddDirectory(Path.Combine(FilesystemRoot, "TestProject", "bin", "Debug", "netcoreapp2.0"));

            // setup mocks
            orchestratorMock.Setup(x => x.Mutate(It.IsAny<SyntaxNode>())).Returns(CSharpSyntaxTree.ParseText(SourceFile).GetRoot());
            orchestratorMock.SetupAllProperties();
            orchestratorMock.Setup(x => x.GetLatestMutantBatch()).Returns(mockMutants);

            var mutator = new CsharpMutationProcess(input, fileSystem, options, null, orchestratorMock.Object);

            var target = new MutationTestProcess(input, options, null, mutationTestExecutorMock.Object, mutator);

            target.Mutate();

            // Verify the created assembly is written to disk on the right location
            var expectedPath = Path.Combine(FilesystemRoot, "TestProject", "bin", "Debug", "netcoreapp2.0", "ProjectUnderTest.dll");
            fileSystem.ShouldContainFile(expectedPath);
        }

        [Fact]
        public void ShouldCallExecutorForEveryCoveredMutant()
        {
            var scenario = new FullRunScenario();
            scenario.CreateMutants(1, 2);
            // we need at least one test
            scenario.CreateTest(1);
            // and we need to declare that the mutant is covered
            scenario.DeclareCoverageForMutant(1);
            var basePath = Path.Combine(FilesystemRoot, "ExampleProject.Test");

            var folder = new CsharpFolderComposite();
            folder.Add(new CsharpFileLeaf()
            {
                SourceCode = SourceFile,
                Mutants = scenario.GetMutants()
            });

            var input = new MutationTestInput()
            {
                SourceProjectInfo = new SourceProjectInfo()
                {
                    AnalyzerResult = TestHelper.SetupProjectAnalyzerResult(properties: new Dictionary<string, string>()
                        {
                            { "TargetDir", "/bin/Debug/netcoreapp2.1" },
                            { "TargetFileName", "TestName.dll" },
                            { "Language", "C#" }
                        }).Object,
                    ProjectContents = folder
                },
                TestProjectsInfo = new TestProjectsInfo(fileSystemMock)
                {
                    TestProjects = new List<TestProject> {
                        new TestProject(fileSystemMock, TestHelper.SetupProjectAnalyzerResult(properties: new Dictionary<string, string>()
                        {
                            { "TargetDir", Path.Combine(FilesystemRoot, "TestProject", "bin", "Debug", "netcoreapp2.0") },
                            { "TargetFileName", "TestName.dll" },
                            { "Language", "C#" }
                        }).Object)
                    }
                },
                InitialTestRun = new InitialTestRun(scenario.GetInitialRunResult(), new TimeoutValueCalculator(500))
            };

            var mutationExecutor = new MutationTestExecutor(scenario.GetTestRunnerMock().Object);

            var options = new StrykerOptions()
            {
                ProjectPath = basePath,
                Concurrency = 1,
                OptimizationMode = OptimizationModes.CoverageBasedTest
            };

            var target = new MutationTestProcess(input, options, null, mutationExecutor);

            target.GetCoverage();
            target.Test(scenario.GetCoveredMutants());

            scenario.GetMutantStatus(1).ShouldBe(MutantStatus.Survived);
            scenario.GetMutantStatus(2).ShouldBe(MutantStatus.NoCoverage);
        }

        [Fact]
        public void ShouldCallExecutorForEveryMutantWhenNoOptimization()
        {
            var scenario = new FullRunScenario();
            scenario.CreateMutants(1, 2);
            // we need at least one test
            scenario.CreateTest(1);
            // and we need to declare that the mutant is covered
            scenario.DeclareCoverageForMutant(1);
            scenario.SetMode(OptimizationModes.None);
            var basePath = Path.Combine(FilesystemRoot, "ExampleProject.Test");

            var folder = new CsharpFolderComposite();
            folder.Add(new CsharpFileLeaf()
            {
                SourceCode = SourceFile,
                Mutants = scenario.GetMutants()
            });

            var input = new MutationTestInput()
            {
                SourceProjectInfo = new SourceProjectInfo()
                {
                    AnalyzerResult = TestHelper.SetupProjectAnalyzerResult(properties: new Dictionary<string, string>()
                        {
                            { "TargetDir", "/bin/Debug/netcoreapp2.1" },
                            { "TargetFileName", "TestName.dll" },
                            { "Language", "C#" }
                        }).Object,
                    ProjectContents = folder
                },
                TestProjectsInfo = new TestProjectsInfo(fileSystemMock)
                {
                    TestProjects = new List<TestProject> {
                        new TestProject(fileSystemMock, TestHelper.SetupProjectAnalyzerResult(properties: new Dictionary<string, string>()
                        {
                            { "TargetDir", Path.Combine(FilesystemRoot, "TestProject", "bin", "Debug", "netcoreapp2.0") },
                            { "TargetFileName", "TestName.dll" },
                            { "Language", "C#" }
                        }).Object)
                    }
                },
                InitialTestRun = new InitialTestRun(scenario.GetInitialRunResult(), new TimeoutValueCalculator(500))
            };

            var reporterMock = new Mock<IReporter>(MockBehavior.Strict);
            reporterMock.Setup(x => x.OnMutantTested(It.IsAny<Mutant>()));

            var mutationExecutor = new MutationTestExecutor(scenario.GetTestRunnerMock().Object);

            var options = new StrykerOptions()
            {
                OutputPath = basePath,
                Concurrency = 1,
                OptimizationMode = OptimizationModes.None
            };
            var target = new MutationTestProcess(input, options, null, mutationExecutor);

            target.GetCoverage();
            target.Test(scenario.GetMutants());

            scenario.GetMutantStatus(1).ShouldBe(MutantStatus.Survived);
            scenario.GetMutantStatus(2).ShouldBe(MutantStatus.Survived);
        }

        [Fact]
        public void ShouldHandleCoverage()
        {
            var scenario = new FullRunScenario();
            var basePath = Path.Combine(FilesystemRoot, "ExampleProject.Test");
            scenario.CreateMutants(1, 2, 3);

            var folder = new CsharpFolderComposite();
            folder.Add(new CsharpFileLeaf()
            {
                SourceCode = SourceFile,
                Mutants = scenario.GetMutants()
            });
            scenario.CreateTests(1, 2);

            // mutant 1 is covered by both tests
            scenario.DeclareCoverageForMutant(1);
            // mutant 2 is covered only by test 1
            scenario.DeclareCoverageForMutant(2, 1);
            // mutant 3 as no coverage
            // test 1 succeeds, test 2 fails
            scenario.DeclareTestsFailingWhenTestingMutant(1, 2);
            var runnerMock = scenario.GetTestRunnerMock();

            // setup coverage
            var executor = new MutationTestExecutor(runnerMock.Object);

            var input = new MutationTestInput()
            {
                SourceProjectInfo = new SourceProjectInfo()
                {
                    AnalyzerResult = TestHelper.SetupProjectAnalyzerResult(properties: new Dictionary<string, string>()
                        {
                            { "TargetDir", "/bin/Debug/netcoreapp2.1" },
                            { "TargetFileName", "TestName.dll" },
                            { "Language", "C#" }
                        }).Object,
                    ProjectContents = folder
                },
                TestProjectsInfo = new TestProjectsInfo(fileSystemMock)
                {
                    TestProjects = new List<TestProject> {
                        new TestProject(fileSystemMock, TestHelper.SetupProjectAnalyzerResult(properties: new Dictionary<string, string>()
                        {
                            { "TargetDir", Path.Combine(FilesystemRoot, "TestProject", "bin", "Debug", "netcoreapp2.0") },
                            { "TargetFileName", "TestName.dll" },
                            { "Language", "C#" }
                        }).Object)
                    }
                },
                InitialTestRun = new InitialTestRun(scenario.GetInitialRunResult(), new TimeoutValueCalculator(500))
            };

            var options = new StrykerOptions
            {
                ProjectPath = basePath,
                Concurrency = 1,
                OptimizationMode = OptimizationModes.CoverageBasedTest
            };

            var target = new MutationTestProcess(input, options, null, executor);
            // test mutants
            target.GetCoverage();

            target.Test(input.SourceProjectInfo.ProjectContents.Mutants.Where(m => m.ResultStatus == MutantStatus.NotRun));
            // first mutant should be killed by test 2
            scenario.GetMutantStatus(1).ShouldBe(MutantStatus.Killed);
            // other mutant survives
            scenario.GetMutantStatus(2).ShouldBe(MutantStatus.Survived);
            // third mutant appears as no coverage
            scenario.GetMutantStatus(3).ShouldBe(MutantStatus.NoCoverage);
        }

        [Fact]
        public void ShouldNotKillMutantIfOnlyKilledByFailingTest()
        {
            var scenario = new FullRunScenario();
            var basePath = Path.Combine(FilesystemRoot, "ExampleProject.Test");
            scenario.CreateMutants(1);

            var folder = new CsharpFolderComposite();
            folder.Add(new CsharpFileLeaf()
            {
                SourceCode = SourceFile,
                Mutants = scenario.GetMutants()
            });
            scenario.CreateTests(1, 2, 3);

            // mutant 1 is covered by all tests
            scenario.DeclareCoverageForMutant(1);
            // mutant 2 is covered only by test 1
            scenario.DeclareTestsFailingAtInit(1);
            // test 1 succeeds, test 2 fails
            scenario.DeclareTestsFailingWhenTestingMutant(1, 1);
            var runnerMock = scenario.GetTestRunnerMock();

            // setup coverage
            var executor = new MutationTestExecutor(runnerMock.Object);

            var input = new MutationTestInput()
            {
                SourceProjectInfo = new SourceProjectInfo()
                {
                    AnalyzerResult = TestHelper.SetupProjectAnalyzerResult(properties: new Dictionary<string, string>()
                        {
                            { "TargetDir", "/bin/Debug/netcoreapp2.1" },
                            { "TargetFileName", "TestName.dll" },
                            { "Language", "C#" }
                        }).Object,
                    ProjectContents = folder
                },
                TestProjectsInfo = new TestProjectsInfo(fileSystemMock)
                {
                    TestProjects = new List<TestProject> {
                        new TestProject(fileSystemMock, TestHelper.SetupProjectAnalyzerResult(properties: new Dictionary<string, string>()
                        {
                            { "TargetDir", Path.Combine(FilesystemRoot, "TestProject", "bin", "Debug", "netcoreapp2.0") },
                            { "TargetFileName", "TestName.dll" },
                            { "Language", "C#" }
                        }).Object)
                    }
                },
                InitialTestRun = new InitialTestRun(scenario.GetInitialRunResult(), new TimeoutValueCalculator(500))
            };

            var options = new StrykerOptions
            {
                ProjectPath = basePath,
                Concurrency = 1,
                OptimizationMode = OptimizationModes.CoverageBasedTest
            };

            var target = new MutationTestProcess(input, options, null, executor);

            // test mutants
            target.GetCoverage();

            target.Test(input.SourceProjectInfo.ProjectContents.Mutants);
            // first mutant should be marked as survived
            scenario.GetMutantStatus(1).ShouldBe(MutantStatus.Survived);
        }

        [Fact]
        public void ShouldNotKillMutantIfOnlyCoveredByFailingTest()
        {
            var scenario = new FullRunScenario();
            var basePath = Path.Combine(FilesystemRoot, "ExampleProject.Test");
            scenario.CreateMutants(1);

            var folder = new CsharpFolderComposite();
            folder.Add(new CsharpFileLeaf()
            {
                SourceCode = SourceFile,
                Mutants = scenario.GetMutants()
            });
            scenario.CreateTests(1, 2, 3);

            // mutant 1 is covered by both tests
            scenario.DeclareCoverageForMutant(1, 1, 2, 3);
            // mutant 2 is covered only by test 1
            scenario.DeclareTestsFailingAtInit(1, 2, 3);
            // test 1 succeeds, test 2 fails
            scenario.DeclareTestsFailingWhenTestingMutant(1, 1, 2, 3);
            var runnerMock = scenario.GetTestRunnerMock();

            // setup coverage
            var executor = new MutationTestExecutor(runnerMock.Object);

            var input = new MutationTestInput()
            {
                SourceProjectInfo = new SourceProjectInfo()
                {
                    AnalyzerResult = TestHelper.SetupProjectAnalyzerResult(properties: new Dictionary<string, string>()
                        {
                            { "TargetDir", "/bin/Debug/netcoreapp2.1" },
                            { "TargetFileName", "TestName.dll" },
                            { "Language", "C#" }
                        }).Object,
                    ProjectContents = folder
                },
                TestProjectsInfo = new TestProjectsInfo(fileSystemMock)
                {
                    TestProjects = new List<TestProject> {
                        new TestProject(fileSystemMock, TestHelper.SetupProjectAnalyzerResult(properties: new Dictionary<string, string>()
                        {
                            { "TargetDir", Path.Combine(FilesystemRoot, "TestProject", "bin", "Debug", "netcoreapp2.0") },
                            { "TargetFileName", "TestName.dll" },
                            { "Language", "C#" }
                        }).Object)
                    }
                },
                InitialTestRun = new InitialTestRun(scenario.GetInitialRunResult(), new TimeoutValueCalculator(500))
            };

            var options = new StrykerOptions
            {
                ProjectPath = basePath,
                Concurrency = 1,
                OptimizationMode = OptimizationModes.CoverageBasedTest
            };

            var target = new MutationTestProcess(input, options, null, executor);
            // test mutants
            target.GetCoverage();

            // first mutant should be marked as survived without any test
            scenario.GetMutantStatus(1).ShouldBe(MutantStatus.Survived);
        }

        [Fact]
        public void ShouldKillMutantKilledByFailingTestAndNormalTest()
        {
            var scenario = new FullRunScenario();
            var basePath = Path.Combine(FilesystemRoot, "ExampleProject.Test");
            scenario.CreateMutants(1);

            var folder = new CsharpFolderComposite();
            folder.Add(new CsharpFileLeaf()
            {
                SourceCode = SourceFile,
                Mutants = scenario.GetMutants()
            });
            scenario.CreateTests(1, 2, 3);

            // mutant 1 is covered by both tests
            scenario.DeclareCoverageForMutant(1);
            // mutant 2 is covered only by test 1
            scenario.DeclareTestsFailingAtInit(1);
            // test 1 succeeds, test 2 fails
            scenario.DeclareTestsFailingWhenTestingMutant(1, 1, 2);
            var runnerMock = scenario.GetTestRunnerMock();

            // setup coverage
            var executor = new MutationTestExecutor(runnerMock.Object);

            var input = new MutationTestInput()
            {
                SourceProjectInfo = new SourceProjectInfo()
                {
                    AnalyzerResult = TestHelper.SetupProjectAnalyzerResult(properties: new Dictionary<string, string>()
                        {
                            { "TargetDir", "/bin/Debug/netcoreapp2.1" },
                            { "TargetFileName", "TestName.dll" },
                            { "Language", "C#" }
                        }).Object,
                    ProjectContents = folder
                },
                TestProjectsInfo = new TestProjectsInfo(fileSystemMock)
                {
                    TestProjects = new List<TestProject> {
                        new TestProject(fileSystemMock, TestHelper.SetupProjectAnalyzerResult(properties: new Dictionary<string, string>()
                        {
                            { "TargetDir", Path.Combine(FilesystemRoot, "TestProject", "bin", "Debug", "netcoreapp2.0") },
                            { "TargetFileName", "TestName.dll" },
                            { "Language", "C#" }
                        }).Object)
                    }
                },
                InitialTestRun = new InitialTestRun(scenario.GetInitialRunResult(), new TimeoutValueCalculator(500))
            };

            var options = new StrykerOptions
            {
                ProjectPath = basePath,
                Concurrency = 1,
                OptimizationMode = OptimizationModes.CoverageBasedTest
            };

            var target = new MutationTestProcess(input, options, null, executor);

            // test mutants
            target.GetCoverage();

            target.Test(input.SourceProjectInfo.ProjectContents.Mutants);
            // first mutant should be killed by test 2
            scenario.GetMutantStatus(1).ShouldBe(MutantStatus.Killed);
        }

        [Theory]
        [InlineData(MutantStatus.Ignored)]
        [InlineData(MutantStatus.CompileError)]
        public void ShouldThrowExceptionWhenOtherStatusThanNotRunIsPassed(MutantStatus status)
        {
            var mutants = new List<Mutant> { new Mutant { Id = 1, ResultStatus = status } };
            Should.Throw<GeneralStrykerException>(() => new MutationTestProcess(null, null, null, null).Test(mutants));
        }

        [Fact]
        public void ShouldNotTest_WhenThereAreNoMutations()
        {
            var reporter = Mock.Of<IReporter>();
            var mutationTestExecutor = Mock.Of<IMutationTestExecutor>();
            var result = new MutationTestProcess(null, null, reporter, mutationTestExecutor).Test(Enumerable.Empty<Mutant>());

            Mock.Get(reporter).VerifyNoOtherCalls();
            Mock.Get(mutationTestExecutor).VerifyNoOtherCalls();
            result.MutationScore.ShouldBe(double.NaN);
        }
    }
}
