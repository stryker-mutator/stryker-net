using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.IO.Abstractions.TestingHelpers;
using System.Reflection;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Moq;
using Stryker.Core.Mutants;
using Stryker.Core.MutationTest;
using Stryker.Core.Options;
using Stryker.Core.ProjectComponents;
using Stryker.Core.ProjectComponents.SourceProjects;
using Stryker.Core.ProjectComponents.TestProjects;
using Xunit;
using Mutation = Stryker.Core.Mutants.Mutation;

namespace Stryker.Core.UnitTest.MutationTest
{
    public class CSharpMutationTestProcessTests : TestBase
    {
        private string CurrentDirectory { get; }
        private string FilesystemRoot { get; }
        private string SourceFile { get; }

        public CSharpMutationTestProcessTests()
        {
            CurrentDirectory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            FilesystemRoot = Path.GetPathRoot(CurrentDirectory);
            SourceFile = File.ReadAllText(CurrentDirectory + "/TestResources/ExampleSourceFile.cs");
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
                    AnalyzerResult = TestHelper.SetupProjectAnalyzerResult(
                        projectFilePath: "/c/ProjectUnderTest/ProjectUnderTest.csproj",
                        properties: new Dictionary<string, string>()
                        {
                            { "TargetDir", Path.Combine(FilesystemRoot, "ProjectUnderTest", "bin", "Debug", "netcoreapp2.0") },
                            { "TargetFileName", "ProjectUnderTest.dll" },
                            { "AssemblyName", "AssemblyName" },
                            { "Language", "C#" }
                        },
                        references: new[] { typeof(object).Assembly.Location }).Object,
                    ProjectContents = folder,
                    TestProjectsInfo = new TestProjectsInfo(fileSystem)
                    {
                        TestProjects = new List<TestProject> {
                            new(fileSystem, TestHelper.SetupProjectAnalyzerResult(properties: new Dictionary<string, string>()
                            {
                                { "TargetDir", Path.Combine(FilesystemRoot, "TestProject", "bin", "Debug", "netcoreapp2.0") },
                                { "TargetFileName", "TestName.dll" },
                                { "Language", "C#" }
                            }).Object)
                        }
                    }
                }
            };

            var mockMutants = new Collection<Mutant>() { new() { Mutation = new Mutation() } };

            // create mocks
            var options = new StrykerOptions();
            var orchestratorMock = new Mock<BaseMutantOrchestrator<SyntaxTree, SemanticModel>>(MockBehavior.Strict, options);

            fileSystem.AddDirectory(Path.Combine(FilesystemRoot, "TestProject", "bin", "Debug", "netcoreapp2.0"));

            // setup mocks
            orchestratorMock.Setup(x => x.Mutate(It.IsAny<SyntaxTree>(), It.IsAny<SemanticModel>())).Returns(CSharpSyntaxTree.ParseText(SourceFile));
            orchestratorMock.SetupAllProperties();
            orchestratorMock.Setup(x => x.GetLatestMutantBatch()).Returns(mockMutants);

            var target = new CsharpMutationProcess( fileSystem, options, null, orchestratorMock.Object);

            target.Mutate(input);

            // Verify the created assembly is written to disk on the right location
            var expectedPath = Path.Combine(FilesystemRoot, "TestProject", "bin", "Debug", "netcoreapp2.0", "ProjectUnderTest.dll");
            fileSystem.ShouldContainFile(expectedPath);
        }
    }
}
