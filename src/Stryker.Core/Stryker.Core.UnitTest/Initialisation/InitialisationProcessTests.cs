using Microsoft.CodeAnalysis;
using Moq;
using Shouldly;
using Stryker.Core.Exceptions;
using Stryker.Core.Initialisation;
using Stryker.Core.Options;
using Stryker.Core.ProjectComponents;
using Stryker.Core.TestRunners;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Xunit;

namespace Stryker.Core.UnitTest.Initialisation
{
    public class InitialisationProcessTests
    {
        [Fact]
        public void InitialisationProcess_ShouldCallNeededResolvers()
        {
            var testRunnerMock = new Mock<ITestRunner>(MockBehavior.Strict);
            var inputFileResolverMock = new Mock<IInputFileResolver>(MockBehavior.Strict);
            var initialBuildProcessMock = new Mock<IInitialBuildProcess>(MockBehavior.Strict);
            var initialTestProcessMock = new Mock<IInitialTestProcess>(MockBehavior.Strict);
            var assemblyReferenceResolverMock = new Mock<IAssemblyReferenceResolver>(MockBehavior.Strict);

            testRunnerMock.Setup(x => x.RunAll(It.IsAny<int>(), It.IsAny<int>()))
                .Returns(new TestRunResult { Success = true }); // testrun is successful
            inputFileResolverMock.Setup(x => x.ResolveInput(It.IsAny<StrykerOptions>()))
                .Returns(new ProjectInfo
                {
                    TestProjectAnalyzerResult = new ProjectAnalyzerResult(null, null)
                    {
                        ProjectFilePath = "C://Example/Dir/ProjectFolder",
                        References = new string[0]
                    },
                    ProjectContents = new FolderComposite
                    {
                        Name = "ProjectRoot",
                        Children = new Collection<ProjectComponent>
                        {
                            new FileLeaf
                            {
                                Name = "SomeFile.cs"
                            }
                        }
                    }
                });
            initialTestProcessMock.Setup(x => x.InitialTest(It.IsAny<ITestRunner>())).Returns(999);
            initialBuildProcessMock.Setup(x => x.InitialBuild(It.IsAny<bool>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()));
            assemblyReferenceResolverMock.Setup(x => x.ResolveReferences(It.IsAny<ProjectAnalyzerResult>()))
                .Returns(Enumerable.Empty<PortableExecutableReference>());

            var target = new InitialisationProcess(
                inputFileResolverMock.Object,
                initialBuildProcessMock.Object,
                initialTestProcessMock.Object,
                testRunnerMock.Object,
                assemblyReferenceResolverMock.Object);

            var options = new StrykerOptions();

            var result = target.Initialize(options);

            inputFileResolverMock.Verify(x => x.ResolveInput(It.IsAny<StrykerOptions>()), Times.Once);
        }

        [Fact]
        public void InitialisationProcess_ShouldThrowOnFailedInitialTestRun()
        {
            var testRunnerMock = new Mock<ITestRunner>(MockBehavior.Strict);
            var inputFileResolverMock = new Mock<IInputFileResolver>(MockBehavior.Strict);
            var initialBuildProcessMock = new Mock<IInitialBuildProcess>(MockBehavior.Strict);
            var initialTestProcessMock = new Mock<IInitialTestProcess>(MockBehavior.Strict);
            var assemblyReferenceResolverMock = new Mock<IAssemblyReferenceResolver>(MockBehavior.Strict);

            testRunnerMock.Setup(x => x.RunAll(It.IsAny<int>(), It.IsAny<int>()));
            inputFileResolverMock.Setup(x => x.ResolveInput(It.IsAny<StrykerOptions>())).Returns(
                new ProjectInfo
                {
                    TestProjectAnalyzerResult = new ProjectAnalyzerResult(null, null)
                    {
                        ProjectFilePath = "C://Example/Dir/ProjectFolder",
                        References = new string[0]
                    },
                    ProjectContents = new FolderComposite
                    {
                        Name = "ProjectRoot",
                        Children = new Collection<ProjectComponent>
                        {
                            new FileLeaf
                            {
                                Name = "SomeFile.cs"
                            }
                        }
                    }
                });
            initialBuildProcessMock.Setup(x => x.InitialBuild(It.IsAny<bool>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()));
            initialTestProcessMock.Setup(x => x.InitialTest(It.IsAny<ITestRunner>())).Throws(new StrykerInputException("")); // failing test
            assemblyReferenceResolverMock.Setup(x => x.ResolveReferences(It.IsAny<ProjectAnalyzerResult>()))
                .Returns(Enumerable.Empty<PortableExecutableReference>())
                .Verifiable();

            var target = new InitialisationProcess(
                inputFileResolverMock.Object,
                initialBuildProcessMock.Object,
                initialTestProcessMock.Object,
                testRunnerMock.Object,
                assemblyReferenceResolverMock.Object);
            var options = new StrykerOptions();

            var exception = Assert.Throws<StrykerInputException>(() => target.Initialize(options));

            inputFileResolverMock.Verify(x => x.ResolveInput(It.IsAny<StrykerOptions>()), Times.Once);
            assemblyReferenceResolverMock.Verify();
            initialTestProcessMock.Verify(x => x.InitialTest(testRunnerMock.Object), Times.Once);
        }

        [Fact]
        public void InitialisationProcess_ShouldThrowOnMsTestV1Detected()
        {
            var testRunnerMock = new Mock<ITestRunner>(MockBehavior.Strict);
            var inputFileResolverMock = new Mock<IInputFileResolver>(MockBehavior.Strict);
            var initialBuildProcessMock = new Mock<IInitialBuildProcess>(MockBehavior.Strict);
            var initialTestProcessMock = new Mock<IInitialTestProcess>(MockBehavior.Strict);
            var assemblyReferenceResolverMock = new Mock<IAssemblyReferenceResolver>(MockBehavior.Strict);

            inputFileResolverMock.Setup(x => x.ResolveInput(It.IsAny<StrykerOptions>())).Returns(
                new ProjectInfo
                {
                    TestProjectAnalyzerResult = new ProjectAnalyzerResult(null, null)
                    {
                        ProjectFilePath = "C://Example/Dir/ProjectFolder",
                        References = new string[] { "Microsoft.VisualStudio.QualityTools.UnitTestFramework" }
                    },
                    ProjectContents = new FolderComposite
                    {
                        Name = "ProjectRoot",
                        Children = new Collection<ProjectComponent>
                        {
                            new FileLeaf
                            {
                                Name = "SomeFile.cs"
                            }
                        }
                    }
                });
            initialBuildProcessMock.Setup(x => x.InitialBuild(It.IsAny<bool>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()));
            initialTestProcessMock.Setup(x => x.InitialTest(It.IsAny<ITestRunner>())).Throws(new StrykerInputException("")); // failing test
            assemblyReferenceResolverMock.Setup(x => x.ResolveReferences(It.IsAny<ProjectAnalyzerResult>()))
                .Returns(Enumerable.Empty<PortableExecutableReference>())
                .Verifiable();

            var target = new InitialisationProcess(
                inputFileResolverMock.Object,
                initialBuildProcessMock.Object,
                initialTestProcessMock.Object,
                testRunnerMock.Object,
                assemblyReferenceResolverMock.Object);
            var options = new StrykerOptions();

            Assert.Throws<StrykerInputException>(() => target.Initialize(options));
        }

        [Fact]
        public void InitialisationProcess_ShouldKeepDotnetTestIfIsTestProjectSet()
        {
            var testRunnerMock = new Mock<ITestRunner>(MockBehavior.Strict);
            var inputFileResolverMock = new Mock<IInputFileResolver>(MockBehavior.Strict);
            var initialBuildProcessMock = new Mock<IInitialBuildProcess>(MockBehavior.Strict);
            var initialTestProcessMock = new Mock<IInitialTestProcess>(MockBehavior.Strict);
            var assemblyReferenceResolverMock = new Mock<IAssemblyReferenceResolver>(MockBehavior.Strict);

            inputFileResolverMock.Setup(x => x.ResolveInput(It.IsAny<StrykerOptions>())).Returns(
                new ProjectInfo
                {
                    FullFramework = true,
                    TestProjectAnalyzerResult = new ProjectAnalyzerResult(null, null)
                    {
                        ProjectFilePath = "C://Example/Dir/ProjectFolder",
                        Properties = new ReadOnlyDictionary<string, string>(new Dictionary<string, string> { { "IsTestProject", "true" } }),
                        References = new string[0]
                    },
                    ProjectContents = new FolderComposite
                    {
                        Name = "ProjectRoot",
                        Children = new Collection<ProjectComponent>
                        {
                            new FileLeaf
                            {
                                Name = "SomeFile.cs"
                            }
                        }
                    }
                });
            initialBuildProcessMock.Setup(x => x.InitialBuild(It.IsAny<bool>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()));
            initialTestProcessMock.Setup(x => x.InitialTest(It.IsAny<ITestRunner>())).Returns(0);
            assemblyReferenceResolverMock.Setup(x => x.ResolveReferences(It.IsAny<ProjectAnalyzerResult>()))
                .Returns(Enumerable.Empty<PortableExecutableReference>())
                .Verifiable();

            var target = new InitialisationProcess(
                inputFileResolverMock.Object,
                initialBuildProcessMock.Object,
                initialTestProcessMock.Object,
                testRunnerMock.Object,
                assemblyReferenceResolverMock.Object);
            var options = new StrykerOptions();

            target.Initialize(options);

            options.TestRunner.ShouldBe(TestRunner.DotnetTest);
        }

        [Fact]
        public void InitialisationProcess_ShouldKeepDotnetTestIfNotFullFramework()
        {
            var testRunnerMock = new Mock<ITestRunner>(MockBehavior.Strict);
            var inputFileResolverMock = new Mock<IInputFileResolver>(MockBehavior.Strict);
            var initialBuildProcessMock = new Mock<IInitialBuildProcess>(MockBehavior.Strict);
            var initialTestProcessMock = new Mock<IInitialTestProcess>(MockBehavior.Strict);
            var assemblyReferenceResolverMock = new Mock<IAssemblyReferenceResolver>(MockBehavior.Strict);

            inputFileResolverMock.Setup(x => x.ResolveInput(It.IsAny<StrykerOptions>())).Returns(
                new ProjectInfo
                {
                    FullFramework = false,
                    TestProjectAnalyzerResult = new ProjectAnalyzerResult(null, null)
                    {
                        ProjectFilePath = "C://Example/Dir/ProjectFolder",
                        References = new string[0]
                    },
                    ProjectContents = new FolderComposite
                    {
                        Name = "ProjectRoot",
                        Children = new Collection<ProjectComponent>
                        {
                            new FileLeaf
                            {
                                Name = "SomeFile.cs"
                            }
                        }
                    }
                });
            initialBuildProcessMock.Setup(x => x.InitialBuild(It.IsAny<bool>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()));
            initialTestProcessMock.Setup(x => x.InitialTest(It.IsAny<ITestRunner>())).Returns(0);
            assemblyReferenceResolverMock.Setup(x => x.ResolveReferences(It.IsAny<ProjectAnalyzerResult>()))
                .Returns(Enumerable.Empty<PortableExecutableReference>())
                .Verifiable();

            var target = new InitialisationProcess(
                inputFileResolverMock.Object,
                initialBuildProcessMock.Object,
                initialTestProcessMock.Object,
                testRunnerMock.Object,
                assemblyReferenceResolverMock.Object);
            var options = new StrykerOptions();
            target.Initialize(options);

            options.TestRunner.ShouldBe(TestRunner.DotnetTest);
        }

        [Fact]
        public void InitialisationProcess_ShouldForceVsTestIfIsTestProjectNotSetAndFullFramework()
        {
            var testRunnerMock = new Mock<ITestRunner>(MockBehavior.Strict);
            var inputFileResolverMock = new Mock<IInputFileResolver>(MockBehavior.Strict);
            var initialBuildProcessMock = new Mock<IInitialBuildProcess>(MockBehavior.Strict);
            var initialTestProcessMock = new Mock<IInitialTestProcess>(MockBehavior.Strict);
            var assemblyReferenceResolverMock = new Mock<IAssemblyReferenceResolver>(MockBehavior.Strict);

            inputFileResolverMock.Setup(x => x.ResolveInput(It.IsAny<StrykerOptions>())).Returns(
                new ProjectInfo
                {
                    FullFramework = true,
                    TestProjectAnalyzerResult = new ProjectAnalyzerResult(null, null)
                    {
                        ProjectFilePath = "C://Example/Dir/ProjectFolder",
                        Properties = new ReadOnlyDictionary<string, string>(new Dictionary<string, string>()),
                        References = new string[0]
                    },
                    ProjectContents = new FolderComposite
                    {
                        Name = "ProjectRoot",
                        Children = new Collection<ProjectComponent>
                        {
                            new FileLeaf
                            {
                                Name = "SomeFile.cs"
                            }
                        }
                    }
                });
            initialBuildProcessMock.Setup(x => x.InitialBuild(It.IsAny<bool>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()));
            initialTestProcessMock.Setup(x => x.InitialTest(It.IsAny<ITestRunner>())).Returns(0);
            assemblyReferenceResolverMock.Setup(x => x.ResolveReferences(It.IsAny<ProjectAnalyzerResult>()))
                .Returns(Enumerable.Empty<PortableExecutableReference>())
                .Verifiable();

            var target = new InitialisationProcess(
                inputFileResolverMock.Object,
                initialBuildProcessMock.Object,
                initialTestProcessMock.Object,
                testRunnerMock.Object,
                assemblyReferenceResolverMock.Object);
            var options = new StrykerOptions();
            target.Initialize(options);

            options.TestRunner.ShouldBe(TestRunner.VsTest);
        }

        [Fact]
        public void InitialisationProcess_ShouldForceVsTestIfIsTestProjectSetFalseAndFullFramework()
        {
            var testRunnerMock = new Mock<ITestRunner>(MockBehavior.Strict);
            var inputFileResolverMock = new Mock<IInputFileResolver>(MockBehavior.Strict);
            var initialBuildProcessMock = new Mock<IInitialBuildProcess>(MockBehavior.Strict);
            var initialTestProcessMock = new Mock<IInitialTestProcess>(MockBehavior.Strict);
            var assemblyReferenceResolverMock = new Mock<IAssemblyReferenceResolver>(MockBehavior.Strict);

            inputFileResolverMock.Setup(x => x.ResolveInput(It.IsAny<StrykerOptions>())).Returns(
                new ProjectInfo
                {
                    FullFramework = true,
                    TestProjectAnalyzerResult = new ProjectAnalyzerResult(null, null)
                    {
                        ProjectFilePath = "C://Example/Dir/ProjectFolder",
                        Properties = new ReadOnlyDictionary<string, string>(new Dictionary<string, string> { { "IsTestProject", "false" } }),
                        References = new string[0]
                    },
                    ProjectContents = new FolderComposite
                    {
                        Name = "ProjectRoot",
                        Children = new Collection<ProjectComponent>
                        {
                            new FileLeaf
                            {
                                Name = "SomeFile.cs"
                            }
                        }
                    }
                });
            initialBuildProcessMock.Setup(x => x.InitialBuild(It.IsAny<bool>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()));
            initialTestProcessMock.Setup(x => x.InitialTest(It.IsAny<ITestRunner>())).Returns(0);
            assemblyReferenceResolverMock.Setup(x => x.ResolveReferences(It.IsAny<ProjectAnalyzerResult>()))
                .Returns(Enumerable.Empty<PortableExecutableReference>())
                .Verifiable();

            var target = new InitialisationProcess(
                inputFileResolverMock.Object,
                initialBuildProcessMock.Object,
                initialTestProcessMock.Object,
                testRunnerMock.Object,
                assemblyReferenceResolverMock.Object);
            var options = new StrykerOptions();
            target.Initialize(options);

            options.TestRunner.ShouldBe(TestRunner.VsTest);
        }
    }
}
