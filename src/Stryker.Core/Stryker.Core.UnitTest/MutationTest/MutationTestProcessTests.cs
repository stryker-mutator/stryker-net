using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Moq;
using Stryker.Core.Compiling;
using Stryker.Core.Initialisation.ProjectComponent;
using Stryker.Core.Mutants;
using Stryker.Core.MutationTest;
using Stryker.Core.Options;
using Stryker.Core.Reporters;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.IO.Abstractions.TestingHelpers;
using System.Reflection;
using System.Runtime.InteropServices;
using Xunit;

namespace Stryker.Core.UnitTest.MutationTest
{
    public class MutationTestProcessTests
    {
        private static readonly bool RunningOnWindows = RuntimeInformation.IsOSPlatform(OSPlatform.Windows);
        private string _currentDirectory { get; set; }

        public MutationTestProcessTests()
        {
            _currentDirectory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
        }

        [SkippableFact]
        public void MutationTestProcess_MutateShouldCallMutantOrchestrator()
        {
            Skip.IfNot(RunningOnWindows);

            string file1 = @"using System;

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
            var input = new MutationTestInput()
            {
                ProjectInfo = new Core.Initialisation.ProjectInfo()
                {
                    TestProjectPath = @"c:\ExampleProject.Test",
                    ProjectUnderTestPath = @"c:\ExampleProject",
                    ProjectUnderTestAssemblyName = "ExampleProject",
                    TargetFramework = "netcoreapp2.0",
                    ProjectContents = new FolderComposite()
                    {
                        Name = @"c:\ExampleProject",
                        Children = new Collection<ProjectComponent>()
                    {
                        new FileLeaf()
                        {
                            Name = "Recursive.cs",
                            SourceCode = file1
                        }
                    }
                    },
                },
                AssemblyReferences = new ReferenceProvider().GetReferencedAssemblies()
            };

            var fileSystem = new MockFileSystem(new Dictionary<string, MockFileData>
            {
                { @"c:\ExampleProject\Recursive.cs", new MockFileData(file1)},
                { @"c:\ExampleProject.Test\bin\Debug\netcoreapp2.0\ExampleProject.dll", new MockFileData("Bytecode") },
                { @"c:\ExampleProject.Test\obj\Release\netcoreapp2.0\ExampleProject.dll", new MockFileData("Bytecode") }
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
            orchestratorMock.Setup(x => x.Mutate(It.IsAny<SyntaxNode>())).Returns(CSharpSyntaxTree.ParseText(file1).GetRoot());
            orchestratorMock.SetupAllProperties();
            compilingProcessMock.Setup(x => x.Compile(It.IsAny<IEnumerable<SyntaxTree>>(), It.IsAny<MemoryStream>()))
                .Returns(new CompilingProcessResult() {
                    Success = true
                });

            var options = new StrykerOptions("c:/test", "debug", "", 2000, null, false);

            var target = new MutationTestProcess(input, 
                reporterMock.Object,
                null,
                mutationTestExecutorMock.Object,
                orchestratorMock.Object,
                compilingProcessMock.Object,
                fileSystem);

            // start mutation process
            target.Mutate();

            // verify the right methods were called
            orchestratorMock.Verify(x => x.Mutate(It.IsAny<SyntaxNode>()), Times.Once);
            reporterMock.Verify(x => x.OnMutantsCreated(It.IsAny<ProjectComponent>()), Times.Once);
        }

        [SkippableFact]
        public void MutationTestProcess_MutateShouldWriteToDisk_IfCompilationIsSuccessful()
        {
            Skip.IfNot(RunningOnWindows);

            string file1 = @"using System;

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
            string basePath = @"c\ExampleProject.Test";
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
                            SourceCode = file1
                        }
                    }
                    },
                    ProjectUnderTestAssemblyName = "ExampleProject",
                    ProjectUnderTestPath = @"c:\ExampleProject\",
                    TargetFramework = "netcoreapp2.0"
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
                { @"c:\SomeFile.cs", new MockFileData("SomeFile")},
            });

            // setup mocks
            orchestratorMock.Setup(x => x.Mutate(It.IsAny<SyntaxNode>())).Returns(CSharpSyntaxTree.ParseText(file1).GetRoot());
            orchestratorMock.SetupAllProperties();
            orchestratorMock.Setup(x => x.GetLatestMutantBatch()).Returns(mockMutants);
            reporterMock.Setup(x => x.OnMutantsCreated(It.IsAny<ProjectComponent>()));
            compilingProcessMock.Setup(x => x.Compile(It.IsAny<IEnumerable<SyntaxTree>>(), It.IsAny<MemoryStream>()))
                .Returns(new CompilingProcessResult()
                {
                    Success = true
                });

            var options = new StrykerOptions("c:/test", "debug", "", 2000, null, false);

            var target = new MutationTestProcess(input,
                reporterMock.Object,
                null,
                mutationTestExecutorMock.Object,
                orchestratorMock.Object,
                compilingProcessMock.Object,
                fileSystem);

            target.Mutate();

            // Verify the created assembly is written to disk on the right location
            Assert.True(fileSystem.FileExists($@"{basePath}\bin\Debug\netcoreapp2.0\ExampleProject.dll"), 
                "The mutated Assembly was not written to disk, or not to the right location.");
        }

        [Fact]
        public void MutationTestProcess_ShouldCallExecutorForEveryMutant()
        {
            var mutant = new Mutant() { Id = 1 };
            string basePath = @"c\ExampleProject.Test";
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
                    ProjectUnderTestPath = @"c:\ExampleProject\",
                    TargetFramework = "netcoreapp2.0",
                },                
                AssemblyReferences = new ReferenceProvider().GetReferencedAssemblies()
            };
            var reporterMock = new Mock<IReporter>(MockBehavior.Strict);
            reporterMock.Setup(x => x.OnMutantTested(It.IsAny<Mutant>()));
            reporterMock.Setup(x => x.OnAllMutantsTested(It.IsAny<ProjectComponent>()));

            var executorMock = new Mock<IMutationTestExecutor>(MockBehavior.Strict);
            executorMock.Setup(x => x.Test(It.IsAny<Mutant>()));

            var options = new StrykerOptions("c:/test", "debug", "", 2000, null, false);
            var target = new MutationTestProcess(input, reporterMock.Object, null, executorMock.Object);

            target.Test();

            executorMock.Verify(x => x.Test(mutant), Times.Once);
            reporterMock.Verify(x => x.OnMutantTested(mutant), Times.Once);
            reporterMock.Verify(x => x.OnAllMutantsTested(It.IsAny<ProjectComponent>()), Times.Once);
        }
    }
}
