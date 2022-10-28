using System.CodeDom.Compiler;
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
using Stryker.Core.Reporters;
using Stryker.Core.TestRunners;
using Xunit;
using Mutation = Stryker.Core.Mutants.Mutation;

namespace Stryker.Core.UnitTest.MutationTest
{
    public class MutationTestProcessTests : TestBase
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
            var fileSystem = new MockFileSystem(new Dictionary<string, MockFileData>
            {
                { Path.Combine(FilesystemRoot, "ExampleProject","Recursive.cs"), new MockFileData(SourceFile)},
                { Path.Combine(FilesystemRoot, "ExampleProject.Test", "bin", "Debug", "netcoreapp2.0", "ExampleProject.dll"), new MockFileData("Bytecode") },
                { Path.Combine(FilesystemRoot, "ExampleProject.Test", "obj", "Release", "netcoreapp2.0", "ExampleProject.dll"), new MockFileData("Bytecode") }
            });

            var input = new MutationTestInput()
            {
                ProjectInfo = new ProjectInfo(fileSystem)
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

            var mutantToBeSkipped = new Mutant() { Mutation = new Mutation() };
            var mockMutants = new Collection<Mutant>() { new() { Mutation = new Mutation() }, mutantToBeSkipped };

            // create mocks
            var options = new StrykerOptions()
            {
                DevMode = true,
                ExcludedMutations = new Mutator[] { }
            };
            var orchestratorMock = new Mock<BaseMutantOrchestrator<SyntaxNode>>(MockBehavior.Strict, options);
            var mutationTestExecutorMock = new Mock<IMutationTestExecutor>(MockBehavior.Strict);

            // setup mocks
            orchestratorMock.Setup(x => x.GetLatestMutantBatch()).Returns(mockMutants);
            orchestratorMock.Setup(x => x.Mutate(It.IsAny<SyntaxNode>())).Returns(CSharpSyntaxTree.ParseText(SourceFile).GetRoot());
            orchestratorMock.SetupAllProperties();
            var mutator = new CsharpMutationProcess(input, fileSystem, options, null, orchestratorMock.Object);

            var target = new MutationTestProcess(input, options, null, mutationTestExecutorMock.Object, mutator);

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
            
            var fileSystem = new MockFileSystem(new Dictionary<string, MockFileData>
            {
                { Path.Combine(FilesystemRoot, "ExampleProject","Recursive.cs"), new MockFileData(SourceFile)},
                { Path.Combine(FilesystemRoot, "ExampleProject.Test", "bin", "Debug", "netcoreapp2.0", "ExampleProject.dll"), new MockFileData("Bytecode") },
                { Path.Combine(FilesystemRoot, "ExampleProject.Test", "obj", "Release", "netcoreapp2.0", "ExampleProject.dll"), new MockFileData("Bytecode") }
            });

            var input = new MutationTestInput()
            {
                ProjectInfo = new ProjectInfo(fileSystem)
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

            var mutantToBeSkipped = new Mutant() { Mutation = new Mutation() };
            var compileErrorMutant = new Mutant() { Mutation = new Mutation(), ResultStatus = MutantStatus.CompileError };
            var mockMutants = new Collection<Mutant>() { new Mutant() { Mutation = new Mutation() }, mutantToBeSkipped, compileErrorMutant };

            // create mocks
            var options = new StrykerOptions()
            {
                DevMode = true,
                ExcludedMutations = new Mutator[] { }
            };

            var orchestratorMock = new Mock<BaseMutantOrchestrator<SyntaxNode>>(MockBehavior.Strict, options);
            var mutationTestExecutorMock = new Mock<IMutationTestExecutor>(MockBehavior.Strict);
            var mutantFilterMock = new Mock<IMutantFilter>(MockBehavior.Strict);

            // setup mocks
            orchestratorMock.Setup(x => x.GetLatestMutantBatch()).Returns(mockMutants);
            orchestratorMock.Setup(x => x.Mutate(It.IsAny<SyntaxNode>())).Returns(CSharpSyntaxTree.ParseText(SourceFile).GetRoot());
            orchestratorMock.SetupAllProperties();
            mutantFilterMock.SetupGet(x => x.DisplayName).Returns("Mock filter");
            IEnumerable<Mutant> mutantsPassedToFilter = null;
            mutantFilterMock.Setup(x => x.FilterMutants(It.IsAny<IEnumerable<Mutant>>(), It.IsAny<IReadOnlyFileLeaf>(), It.IsAny<StrykerOptions>()))
                .Callback<IEnumerable<Mutant>, IReadOnlyFileLeaf, StrykerOptions>((mutants, _, __) => mutantsPassedToFilter = mutants)
                .Returns((IEnumerable<Mutant> mutants, IReadOnlyFileLeaf file, StrykerOptions o) => mutants.Take(1));


            var mutator = new CsharpMutationProcess(input, fileSystem, options, new BroadcastMutantFilter(new[] { mutantFilterMock.Object }), orchestratorMock.Object);

            var target = new MutationTestProcess(input, options, null, mutationTestExecutorMock.Object, mutator);

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

            var fileSystem = new MockFileSystem(new Dictionary<string, MockFileData>
            {
                { Path.Combine(FilesystemRoot, "SomeFile.cs"), new MockFileData("SomeFile")},
            });

            var input = new MutationTestInput()
            {
                ProjectInfo = new ProjectInfo(fileSystem)
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
            string expectedPath = Path.Combine(FilesystemRoot, "TestProject", "bin", "Debug", "netcoreapp2.0", "ProjectUnderTest.dll");
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
                ProjectInfo = new ProjectInfo(new MockFileSystem())
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
                AssemblyReferences = _assemblies,
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
                ProjectInfo = new ProjectInfo(new MockFileSystem())
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
                AssemblyReferences = _assemblies,
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

            var input = new MutationTestInput
            {
                ProjectInfo = new ProjectInfo(new MockFileSystem())
                {
                    ProjectUnderTestAnalyzerResult = TestHelper.SetupProjectAnalyzerResult(properties: new Dictionary<string, string>()
                    {
                        { "TargetDir", "/bin/Debug/netcoreapp2.1" },
                        { "TargetFileName", "TestName.dll" },
                        { "Language", "C#" }
                    }).Object,
                    ProjectContents = folder
                },
                AssemblyReferences = _assemblies,
                InitialTestRun =  new InitialTestRun(scenario.GetInitialRunResult(), new TimeoutValueCalculator(500))
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
            
            target.Test(input.ProjectInfo.ProjectContents.Mutants.Where(m=> m.ResultStatus == MutantStatus.NotRun));
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

            var input = new MutationTestInput
            {
                ProjectInfo = new ProjectInfo(new MockFileSystem())
                {
                    ProjectUnderTestAnalyzerResult = TestHelper.SetupProjectAnalyzerResult(properties: new Dictionary<string, string>()
                    {
                        { "TargetDir", "/bin/Debug/netcoreapp2.1" },
                        { "TargetFileName", "TestName.dll" },
                        { "Language", "C#" }
                    }).Object,
                    ProjectContents = folder
                },
                AssemblyReferences = _assemblies,
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
            
            target.Test(input.ProjectInfo.ProjectContents.Mutants);
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

            var input = new MutationTestInput
            {
                ProjectInfo = new ProjectInfo(new MockFileSystem())
                {
                    ProjectUnderTestAnalyzerResult = TestHelper.SetupProjectAnalyzerResult(properties: new Dictionary<string, string>()
                    {
                        { "TargetDir", "/bin/Debug/netcoreapp2.1" },
                        { "TargetFileName", "TestName.dll" },
                        { "Language", "C#" }
                    }).Object,
                    ProjectContents = folder
                },
                AssemblyReferences = _assemblies,
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

            var input = new MutationTestInput
            {
                ProjectInfo = new ProjectInfo(new MockFileSystem())
                {
                    ProjectUnderTestAnalyzerResult = TestHelper.SetupProjectAnalyzerResult(properties: new Dictionary<string, string>()
                    {
                        { "TargetDir", "/bin/Debug/netcoreapp2.1" },
                        { "TargetFileName", "TestName.dll" },
                        { "Language", "C#" }
                    }).Object,
                    ProjectContents = folder
                },
                AssemblyReferences = _assemblies,
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
            
            target.Test(input.ProjectInfo.ProjectContents.Mutants);
            // first mutant should be killed by test 2
            scenario.GetMutantStatus(1).ShouldBe(MutantStatus.Killed);
        }

        [Theory]
        [InlineData(MutantStatus.Ignored)]
        [InlineData(MutantStatus.CompileError)]
        public void ShouldThrowExceptionWhenOtherStatusThanNotRunIsPassed(MutantStatus status)
        {
            var mutant = new Mutant { Id = 1, ResultStatus = status };
            var basePath = Path.Combine(FilesystemRoot, "ExampleProject.Test");

            var folder = new CsharpFolderComposite();
            folder.Add(new CsharpFileLeaf
            {
                Mutants = new Collection<Mutant> { mutant }
            });

            var input = new MutationTestInput()
            {
                ProjectInfo = new ProjectInfo(new MockFileSystem())
                {
                    ProjectUnderTestAnalyzerResult = TestHelper.SetupProjectAnalyzerResult(properties: new Dictionary<string, string>()
                        {
                            { "TargetDir", "/bin/Debug/netcoreapp2.1" },
                            { "TargetFileName", "TestName.dll" },
                            { "Language", "C#" }
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
            runnerMock.Setup(x => x.DiscoverTests()).Returns(new TestSet());
            var executorMock = new Mock<IMutationTestExecutor>(MockBehavior.Strict);
            executorMock.SetupGet(x => x.TestRunner).Returns(runnerMock.Object);
            executorMock.Setup(x => x.Test(It.IsAny<IList<Mutant>>(),
                It.IsAny<ITimeoutValueCalculator>(),
                It.IsAny<TestUpdateHandler>()));

            var options = new StrykerOptions()
            {
                ProjectPath = basePath
            };

            var target = new MutationTestProcess(input, options, null, executorMock.Object);

            Should.Throw<GeneralStrykerException>(() => target.Test(input.ProjectInfo.ProjectContents.Mutants));
        }

        [Fact]
        public void ShouldNotTest_WhenThereAreNoMutationsAtAll()
        {
            string basePath = Path.Combine(FilesystemRoot, "ExampleProject.Test");
            var scenario = new FullRunScenario();
            var folder = new CsharpFolderComposite();
            folder.Add(new CsharpFileLeaf()
            {
                Mutants = scenario.GetMutants()
            });

            var projectUnderTest = TestHelper.SetupProjectAnalyzerResult(
                    properties: new Dictionary<string, string>() { { "Language", "C#" } }).Object;
            var input = new MutationTestInput()
            {
                ProjectInfo = new ProjectInfo(new MockFileSystem())
                {
                    ProjectContents = folder,
                    ProjectUnderTestAnalyzerResult = projectUnderTest
                },
                AssemblyReferences = _assemblies
            };

            var executorMock = new Mock<IMutationTestExecutor>(MockBehavior.Strict);
            executorMock.SetupGet(x => x.TestRunner).Returns(scenario.GetTestRunnerMock().Object);
            executorMock.Setup(x => x.Test(It.IsAny<IList<Mutant>>(), It.IsAny<ITimeoutValueCalculator>(), It.IsAny<TestUpdateHandler>()));

            var options = new StrykerOptions()
            {
                ProjectPath = basePath
            };

            var reporterMock = new Mock<IReporter>(MockBehavior.Strict);
            reporterMock.Setup(x => x.OnMutantTested(It.IsAny<Mutant>()));
            var target = new MutationTestProcess(input, options, null, executorMock.Object);

            var testResult = target.Test(input.ProjectInfo.ProjectContents.Mutants);

            executorMock.Verify(x => x.Test(It.IsAny<IList<Mutant>>(), It.IsAny<ITimeoutValueCalculator>(), It.IsAny<TestUpdateHandler>()), Times.Never);
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
                    properties: new Dictionary<string, string>() { { "Language", "C#" } }).Object;
            var input = new MutationTestInput()
            {
                ProjectInfo = new ProjectInfo(new MockFileSystem())
                {
                    ProjectContents = folder,
                    ProjectUnderTestAnalyzerResult = projectUnderTest
                },
                AssemblyReferences = _assemblies
            };
            var reporterMock = new Mock<IReporter>(MockBehavior.Strict);
            reporterMock.Setup(x => x.OnMutantTested(It.IsAny<Mutant>()));

            var runnerMock = new Mock<ITestRunner>();
            var executorMock = new Mock<IMutationTestExecutor>(MockBehavior.Strict);
            executorMock.SetupGet(x => x.TestRunner).Returns(runnerMock.Object);
            executorMock.Setup(x => x.Test(It.IsAny<IList<Mutant>>(), It.IsAny<ITimeoutValueCalculator>(), It.IsAny<TestUpdateHandler>()));

            var options = new StrykerOptions()
            {
                ProjectPath = basePath
            };

            var target = new MutationTestProcess(input, options, null, executorMock.Object);

            var testResult = target.Test(folder.Mutants);

            executorMock.Verify(x => x.Test(It.IsAny<IList<Mutant>>(), It.IsAny<ITimeoutValueCalculator>(), It.IsAny<TestUpdateHandler>()), Times.Never);
            reporterMock.Verify(x => x.OnMutantTested(It.IsAny<Mutant>()), Times.Never);
            testResult.MutationScore.ShouldBe(double.NaN);
        }
    }
}
