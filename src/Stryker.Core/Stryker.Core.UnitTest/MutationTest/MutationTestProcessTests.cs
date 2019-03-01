using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Moq;
using Shouldly;
using Stryker.Core.Compiling;
using Stryker.Core.Initialisation;
using Stryker.Core.Initialisation.ProjectComponent;
using Stryker.Core.Mutants;
using Stryker.Core.MutationTest;
using Stryker.Core.Options;
using Stryker.Core.Reporters;
using Stryker.Core.TestRunners;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.IO.Abstractions.TestingHelpers;
using System.Reflection;
using Xunit;

namespace Stryker.Core.UnitTest.MutationTest
{
    public class MutationTestProcessTests
    {
        private string _currentDirectory { get; set; }
        private string _filesystemRoot { get; set; }
        private string _sourceFile { get; set; }

        public MutationTestProcessTests()
        {
            _currentDirectory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            _filesystemRoot = Path.GetPathRoot(_currentDirectory);
            _sourceFile = File.ReadAllText(_currentDirectory + "/TestResources/ExampleSourceFile.cs");
        }

        [Fact]
        public void MutationTestProcess_MutateShouldCallMutantOrchestrator()
        {
            var input = new MutationTestInput() {
                ProjectInfo = new ProjectInfo() {
                    TestProjectAnalyzerResult = new ProjectAnalyzerResult(null)
                    {
                        Properties = new Dictionary<string, string>()
                        {
                            { "OutputPath", "/bin/Debug/netcoreapp2.1" },
                            { "TargetFileName", "TestName.dll" }
                        }
                    },
                    ProjectUnderTestAnalyzerResult = new ProjectAnalyzerResult(null)
                    {
                        Properties = new Dictionary<string, string>()
                        {
                            { "OutputPath", "/bin/Debug/netcoreapp2.1" },
                            { "TargetFileName", "TestName.dll" }
                        }
                    },
                    ProjectContents = new FolderComposite() {
                        Name = Path.Combine(_filesystemRoot, "ExampleProject"),
                        Children = new Collection<ProjectComponent>() {
                            new FileLeaf() {
                                Name = "Recursive.cs",
                                SourceCode = _sourceFile
                            }
                        }
                    },
                },
                AssemblyReferences = new ReferenceProvider().GetReferencedAssemblies()
            };

            var fileSystem = new MockFileSystem(new Dictionary<string, MockFileData>
            {
                { Path.Combine(_filesystemRoot, "ExampleProject","Recursive.cs"), new MockFileData(_sourceFile)},
                { Path.Combine(_filesystemRoot, "ExampleProject.Test", "bin", "Debug", "netcoreapp2.0", "ExampleProject.dll"), new MockFileData("Bytecode") },
                { Path.Combine(_filesystemRoot, "ExampleProject.Test", "obj", "Release", "netcoreapp2.0", "ExampleProject.dll"), new MockFileData("Bytecode") }
            });
            var mockMutants = new Collection<Mutant>() { new Mutant() { Mutation = new Mutation() } };

            // create mocks
            var orchestratorMock = new Mock<IMutantOrchestrator>(MockBehavior.Strict);
            var reporterMock = new Mock<IReporter>(MockBehavior.Strict);
            var mutationTestExecutorMock = new Mock<IMutationTestExecutor>(MockBehavior.Strict);
            var compilingProcessMock = new Mock<ICompilingProcess>(MockBehavior.Strict);

            // setup mocks
            reporterMock.Setup(x => x.OnMutantsCreated(It.IsAny<ProjectComponent>()));
            orchestratorMock.Setup(x => x.GetLatestMutantBatch()).Returns(mockMutants);
            orchestratorMock.Setup(x => x.Mutate(It.IsAny<SyntaxNode>())).Returns(CSharpSyntaxTree.ParseText(_sourceFile).GetRoot());
            orchestratorMock.SetupAllProperties();
            compilingProcessMock.Setup(x => x.Compile(It.IsAny<IEnumerable<SyntaxTree>>(), It.IsAny<MemoryStream>(), true))
                .Returns(new CompilingProcessResult()
                {
                    Success = true
                });

            var target = new MutationTestProcess(input,
                reporterMock.Object,
                mutationTestExecutorMock.Object,
                orchestratorMock.Object,
                compilingProcessMock.Object,
                fileSystem);

            // start mutation process
            target.Mutate(new StrykerOptions(devMode: true, excludedMutations: new string[] { }));

            // verify the right methods were called
            orchestratorMock.Verify(x => x.Mutate(It.IsAny<SyntaxNode>()), Times.Once);
            reporterMock.Verify(x => x.OnMutantsCreated(It.IsAny<ProjectComponent>()), Times.Once);
        }

        [Fact]
        public void MutationTestProcess_ExcludeFilesToMutate_MutationCalledOnce()
        {
            string sourceFile2 = _sourceFile.Replace("Recursive.cs", "Recursive2.cs");
            string sourceFile3 = _sourceFile.Replace("Recursive.cs", "Recursive3.cs");

            var input = new MutationTestInput() {
                ProjectInfo = new ProjectInfo() {
                    TestProjectAnalyzerResult = new ProjectAnalyzerResult(null)
                    {
                        Properties = new Dictionary<string, string>()
                        {
                            { "OutputPath", "/bin/Debug/netcoreapp2.1" },
                            { "TargetFileName", "TestName.dll" }
                        }
                    },
                    ProjectUnderTestAnalyzerResult = new ProjectAnalyzerResult(null)
                    {
                        Properties = new Dictionary<string, string>()
                        {
                            { "OutputPath", "/bin/Debug/netcoreapp2.1" },
                            { "TargetFileName", "TestName.dll" }
                        }
                    },
                    ProjectContents = new FolderComposite() {
                        Name = Path.Combine(_filesystemRoot, "ExampleProject"),
                        Children = new Collection<ProjectComponent>() {
                            new FileLeaf() {
                                Name = "Recursive.cs",
                                SourceCode = _sourceFile
                            },
                            new FileLeaf() {
                                Name = "Recursive2.cs",
                                SourceCode = sourceFile2,
                                IsExcluded = true
                            },
                            new FileLeaf() {
                                Name = "Recursive3.cs",
                                SourceCode = sourceFile3
                            }
                        }
                    },
                },
                AssemblyReferences = new ReferenceProvider().GetReferencedAssemblies()
            };

            var fileSystem = new MockFileSystem(new Dictionary<string, MockFileData>
            {
                { Path.Combine(_filesystemRoot, "ExampleProject","Recursive.cs"), new MockFileData(_sourceFile)},
                { Path.Combine(_filesystemRoot, "ExampleProject","Recursive2.cs"), new MockFileData(sourceFile2)},
                { Path.Combine(_filesystemRoot, "ExampleProject","Recursive3.cs"), new MockFileData(sourceFile3)},
                { Path.Combine(_filesystemRoot, "ExampleProject.Test", "bin", "Debug", "netcoreapp2.0", "ExampleProject.dll"), new MockFileData("Bytecode") },
                { Path.Combine(_filesystemRoot, "ExampleProject.Test", "obj", "Release", "netcoreapp2.0", "ExampleProject.dll"), new MockFileData("Bytecode") }
            });
            var mockMutants = new Collection<Mutant>() { new Mutant() { Mutation = new Mutation() } };

            // create mocks
            var orchestratorMock = new Mock<IMutantOrchestrator>(MockBehavior.Strict);
            var reporterMock = new Mock<IReporter>(MockBehavior.Strict);
            var mutationTestExecutorMock = new Mock<IMutationTestExecutor>(MockBehavior.Strict);
            var compilingProcessMock = new Mock<ICompilingProcess>(MockBehavior.Strict);

            // setup mocks
            reporterMock.Setup(x => x.OnMutantsCreated(It.IsAny<ProjectComponent>()));
            orchestratorMock.Setup(x => x.GetLatestMutantBatch()).Returns(mockMutants);
            orchestratorMock.Setup(x => x.Mutate(It.IsAny<SyntaxNode>())).Returns(CSharpSyntaxTree.ParseText(_sourceFile).GetRoot());
            orchestratorMock.SetupAllProperties();
            compilingProcessMock.Setup(x => x.Compile(It.IsAny<IEnumerable<SyntaxTree>>(), It.IsAny<MemoryStream>(), It.IsAny<bool>()))
                .Returns(new CompilingProcessResult()
                {
                    Success = true
                });

            var target = new MutationTestProcess(input,
                reporterMock.Object,
                mutationTestExecutorMock.Object,
                orchestratorMock.Object,
                compilingProcessMock.Object,
                fileSystem);

            var options = new StrykerOptions();
            // start mutation process
            target.Mutate(options);

            // verify the right methods were called
            orchestratorMock.Verify(x => x.Mutate(It.IsAny<SyntaxNode>()), Times.Exactly(2));
            reporterMock.Verify(x => x.OnMutantsCreated(It.IsAny<ProjectComponent>()), Times.Once);
        }

        [Fact]
        public void MutationTestProcess_MutateShouldWriteToDisk_IfCompilationIsSuccessful()
        {
            string basePath = Path.Combine(_filesystemRoot, "ExampleProject.Test");
            var input = new MutationTestInput() {
                ProjectInfo = new ProjectInfo() {
                    TestProjectAnalyzerResult = new ProjectAnalyzerResult(null)
                    {
                        Properties = new Dictionary<string, string>()
                        {
                            { "OutputPath", Path.Combine(basePath, "bin", "Debug", "netcoreapp2.0") },
                            { "TargetFileName", "TestName.dll" }
                        }
                    },
                    ProjectUnderTestAnalyzerResult = new ProjectAnalyzerResult(null)
                    {
                        Properties = new Dictionary<string, string>()
                        {
                            { "OutputPath", Path.Combine(basePath, "bin", "Debug", "netcoreapp2.0") },
                            { "TargetFileName", "ExampleProject.dll" }
                        }
                    },
                    ProjectContents = new FolderComposite() {
                        Name = "ProjectRoot",
                        Children = new Collection<ProjectComponent>() {
                            new FileLeaf() {
                                Name = "SomeFile.cs",
                                SourceCode = _sourceFile
                            }
                        }
                    }
                },
                AssemblyReferences = new ReferenceProvider().GetReferencedAssemblies()
            };
            var mockMutants = new Collection<Mutant>() { new Mutant() { Mutation = new Mutation() } };

            // create mocks
            var orchestratorMock = new Mock<IMutantOrchestrator>(MockBehavior.Strict);
            var reporterMock = new Mock<IReporter>(MockBehavior.Strict);
            var mutationTestExecutorMock = new Mock<IMutationTestExecutor>(MockBehavior.Strict);
            var compilingProcessMock = new Mock<ICompilingProcess>(MockBehavior.Strict);
            var fileSystem = new MockFileSystem(new Dictionary<string, MockFileData>
            {
                { Path.Combine(_filesystemRoot, "SomeFile.cs"), new MockFileData("SomeFile")},
            });
            fileSystem.AddDirectory(Path.Combine(basePath, "bin", "Debug", "netcoreapp2.0"));

            // setup mocks
            orchestratorMock.Setup(x => x.Mutate(It.IsAny<SyntaxNode>())).Returns(CSharpSyntaxTree.ParseText(_sourceFile).GetRoot());
            orchestratorMock.SetupAllProperties();
            orchestratorMock.Setup(x => x.GetLatestMutantBatch()).Returns(mockMutants);
            reporterMock.Setup(x => x.OnMutantsCreated(It.IsAny<ProjectComponent>()));
            compilingProcessMock.Setup(x => x.Compile(It.IsAny<IEnumerable<SyntaxTree>>(), It.IsAny<MemoryStream>(), It.IsAny<bool>()))
                .Returns(new CompilingProcessResult()
                {
                    Success = true
                });

            var target = new MutationTestProcess(input,
                reporterMock.Object,
                mutationTestExecutorMock.Object,
                orchestratorMock.Object,
                compilingProcessMock.Object,
                fileSystem);

            var options = new StrykerOptions();
            target.Mutate(options);

            // Verify the created assembly is written to disk on the right location
            string expectedPath = Path.Combine(basePath, "bin", "Debug", "netcoreapp2.0", "ExampleProject.dll");
            fileSystem.FileExists(expectedPath)
                .ShouldBeTrue($"The mutated Assembly was not written to disk, or not to the right location ({expectedPath}).");
        }

        [Fact]
        public void MutationTestProcess_ShouldCallExecutorForEveryMutant()
        {
            var mutant = new Mutant() { Id = 1 };
            string basePath = Path.Combine(_filesystemRoot, "ExampleProject.Test");
            var input = new MutationTestInput()
            {
                ProjectInfo = new ProjectInfo()
                {
                    ProjectUnderTestAnalyzerResult = new ProjectAnalyzerResult(null)
                    {
                        Properties = new Dictionary<string, string>()
                        {
                            { "OutputPath", "/bin/Debug/netcoreapp2.1" },
                            { "TargetFileName", "TestName.dll" }
                        }
                    },
                    ProjectContents = new FolderComposite()
                    {
                        Name = "ProjectRoot",
                        Children = new Collection<ProjectComponent>() {
                            new FileLeaf() {
                                Name = "SomeFile.cs",
                                SourceCode = _sourceFile,
                                Mutants = new List<Mutant>() { mutant }
                            }
                        }
                    }
                },
                AssemblyReferences = new ReferenceProvider().GetReferencedAssemblies()
            };
            var reporterMock = new Mock<IReporter>(MockBehavior.Strict);
            reporterMock.Setup(x => x.OnMutantTested(It.IsAny<Mutant>()));
            reporterMock.Setup(x => x.OnAllMutantsTested(It.IsAny<ProjectComponent>()));
            reporterMock.Setup(x => x.OnStartMutantTestRun(It.IsAny<IList<Mutant>>()));

            var executorMock = new Mock<IMutationTestExecutor>(MockBehavior.Strict);
            executorMock.SetupGet(x => x.TestRunner).Returns(Mock.Of<ITestRunner>());
            executorMock.Setup(x => x.Test(It.IsAny<Mutant>()));

            var options = new StrykerOptions(basePath: Path.Combine(_filesystemRoot, "test"));

            var target = new MutationTestProcess(input,
                reporterMock.Object,
                executorMock.Object);

            target.Test(options);

            executorMock.Verify(x => x.Test(mutant), Times.Once);
            reporterMock.Verify(x => x.OnStartMutantTestRun(It.Is<IList<Mutant>>(y => y.Count == 1)), Times.Once);
            reporterMock.Verify(x => x.OnMutantTested(mutant), Times.Once);
            reporterMock.Verify(x => x.OnAllMutantsTested(It.IsAny<ProjectComponent>()), Times.Once);
        }
    }
}
