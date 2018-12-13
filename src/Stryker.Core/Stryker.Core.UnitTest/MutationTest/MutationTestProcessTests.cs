using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Moq;
using Stryker.Core.Compiling;
using Stryker.Core.Initialisation.ProjectComponent;
using Stryker.Core.Mutants;
using Stryker.Core.MutationTest;
using Stryker.Core.Mutators;
using Stryker.Core.Options;
using Stryker.Core.Reporters;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.IO.Abstractions.TestingHelpers;
using System.Linq;
using System.Reflection;
using Xunit;

namespace Stryker.Core.UnitTest.MutationTest
{
    public class MutationTestProcessTests
    {
        private string _currentDirectory { get; set; }
        private string _filesystemRoot { get; set; }
        private string _exampleFileContents = @"using System;

                namespace ExampleProject
                {
                    public class Recursive
                    {
                        public int Fibinacci(int len)
                        {
                            return Fibonacci(0, 1, 1, len);
                        }

                        private int Fibonacci(int a, int b, int counter, int len)
                        {
                            if (counter <= len)
                            {
                                Console.Write(""{0} "", a);
                                return Fibonacci(b, a + b, counter + 1, len);
                            }
                            return 0;
                        }
                    }
                }
                ";

        public MutationTestProcessTests()
        {
            _currentDirectory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            _filesystemRoot = Path.GetPathRoot(_currentDirectory);
        }

        [Fact]
        public void MutationTestProcess_MutateShouldCallMutantOrchestrator()
        {
            var input = new MutationTestInput()
            {
                ProjectInfo = new Core.Initialisation.ProjectInfo()
                {
                    TestProjectPath = Path.Combine(_filesystemRoot, "ExampleProject.Test"),
                    ProjectUnderTestPath = Path.Combine(_filesystemRoot, "ExampleProject"),
                    ProjectUnderTestAssemblyName = "ExampleProject",
                    TargetFramework = "netcoreapp2.0",
                    ProjectContents = new FolderComposite()
                    {
                        Name = Path.Combine(_filesystemRoot, "ExampleProject"),
                        Children = new Collection<ProjectComponent>()
                    {
                        new FileLeaf()
                        {
                            Name = "Recursive.cs",
                            SourceCode = _exampleFileContents
                        }
                    }
                    },
                },
                AssemblyReferences = new ReferenceProvider().GetReferencedAssemblies()
            };

            var fileSystem = new MockFileSystem(new Dictionary<string, MockFileData>
            {
                { Path.Combine(_filesystemRoot, "ExampleProject","Recursive.cs"), new MockFileData(_exampleFileContents)},
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
            orchestratorMock.Setup(x => x.Mutate(It.IsAny<SyntaxNode>())).Returns(CSharpSyntaxTree.ParseText(_exampleFileContents).GetRoot());
            orchestratorMock.SetupAllProperties();
            compilingProcessMock.Setup(x => x.Compile(It.IsAny<IEnumerable<SyntaxTree>>(), It.IsAny<MemoryStream>(), It.IsAny<bool>()))
                .Returns(new CompilingProcessResult()
                {
                    Success = true
                });

            var target = new MutationTestProcess(input,
                reporterMock.Object,
                Enumerable.Empty<MutatorType>(),
                mutationTestExecutorMock.Object,
                orchestratorMock.Object,
                compilingProcessMock.Object,
                fileSystem);

            var options = new StrykerOptions();
            // start mutation process
            target.Mutate(options);

            // verify the right methods were called
            orchestratorMock.Verify(x => x.Mutate(It.IsAny<SyntaxNode>()), Times.Once);
            reporterMock.Verify(x => x.OnMutantsCreated(It.IsAny<ProjectComponent>()), Times.Once);
        }

        [Fact]
        public void MutationTestProcess_MutateShouldWriteToDisk_IfCompilationIsSuccessful()
        {
            string basePath = Path.Combine(_filesystemRoot, "ExampleProject.Test");
            var input = new MutationTestInput()
            {
                ProjectInfo = new Core.Initialisation.ProjectInfo()
                {
                    TestProjectPath = basePath,
                    ProjectContents = new FolderComposite()
                    {
                        Name = "ProjectRoot",
                        Children = new Collection<ProjectComponent>() {
                        new FileLeaf() {
                            Name = "SomeFile.cs",
                            SourceCode = _exampleFileContents
                        }
                    }
                    },
                    ProjectUnderTestAssemblyName = "ExampleProject",
                    ProjectUnderTestPath = Path.Combine(_filesystemRoot, "ExampleProject"),
                    TargetFramework = "netcoreapp2.0",
                    AppendTargetFrameworkToOutputPath = true
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
            fileSystem.AddDirectory(Path.Combine(_filesystemRoot, "ExampleProject.Test", "bin", "Debug", "netcoreapp2.0"));

            // setup mocks
            orchestratorMock.Setup(x => x.Mutate(It.IsAny<SyntaxNode>())).Returns(CSharpSyntaxTree.ParseText(_exampleFileContents).GetRoot());
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
                Enumerable.Empty<MutatorType>(),
                mutationTestExecutorMock.Object,
                orchestratorMock.Object,
                compilingProcessMock.Object,
                fileSystem);

            var options = new StrykerOptions();
            target.Mutate(options);

            // Verify the created assembly is written to disk on the right location
            Assert.True(fileSystem.FileExists(Path.Combine(basePath, "bin", "Debug", "netcoreapp2.0", "ExampleProject.dll")),
                "The mutated Assembly was not written to disk, or not to the right location.");
        }

        [Fact]
        public void MutationTestProcess_ShouldCallExecutorForEveryMutant()
        {
            var mutant = new Mutant() { Id = 1 };
            string basePath = Path.Combine(_filesystemRoot, "ExampleProject.Test");
            var input = new MutationTestInput()
            {
                ProjectInfo = new Core.Initialisation.ProjectInfo()
                {
                    TestProjectPath = basePath,
                    ProjectContents = new FolderComposite()
                    {
                        Name = "ProjectRoot",
                        Children = new Collection<ProjectComponent>() {
                        new FileLeaf() {
                            Name = "SomeFile.cs",
                            Mutants = new Collection<Mutant>() { mutant }
                        }
                    }
                    },
                    ProjectUnderTestAssemblyName = "ExampleProject",
                    ProjectUnderTestPath = Path.Combine(_filesystemRoot, "ExampleProject"),
                    TargetFramework = "netcoreapp2.0",
                },
                AssemblyReferences = new ReferenceProvider().GetReferencedAssemblies()
            };
            var reporterMock = new Mock<IReporter>(MockBehavior.Strict);
            reporterMock.Setup(x => x.OnMutantTested(It.IsAny<Mutant>()));
            reporterMock.Setup(x => x.OnAllMutantsTested(It.IsAny<ProjectComponent>()));
            reporterMock.Setup(x => x.OnStartMutantTestRun(It.IsAny<IList<Mutant>>()));

            var executorMock = new Mock<IMutationTestExecutor>(MockBehavior.Strict);
            executorMock.Setup(x => x.Test(It.IsAny<Mutant>()));

            var options = new StrykerOptions(basePath: Path.Combine(_filesystemRoot, "test"));

            var target = new MutationTestProcess(input,
                reporterMock.Object,
                Enumerable.Empty<MutatorType>(),
                executorMock.Object,
                null,
                null,
                null);

            target.Test(options);

            executorMock.Verify(x => x.Test(mutant), Times.Once);
            reporterMock.Verify(x => x.OnStartMutantTestRun(It.Is<IList<Mutant>>(y => y.Count == 1)), Times.Once);
            reporterMock.Verify(x => x.OnMutantTested(mutant), Times.Once);
            reporterMock.Verify(x => x.OnAllMutantsTested(It.IsAny<ProjectComponent>()), Times.Once);
        }
    }
}
