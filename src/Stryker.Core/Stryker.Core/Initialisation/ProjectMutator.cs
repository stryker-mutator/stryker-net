using System;
using System.IO;
using System.Linq;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.Extensions.Logging;
using Stryker.Abstractions.Logging;
using Stryker.Abstractions;
using Stryker.Abstractions.Reporting;
using Stryker.Core.MutationTest;
using Stryker.Core.ProjectComponents.TestProjects;
using Stryker.Abstractions.Options;
using Stryker.Abstractions.ProjectComponents;

namespace Stryker.Core.Initialisation
{
    public interface IProjectMutator
    {
        IMutationTestProcess MutateProject(IStrykerOptions options, MutationTestInput input, IReporter reporters);
    }

    public class ProjectMutator : IProjectMutator
    {
        private readonly ILogger _logger;
        private readonly IMutationTestProcess _injectedMutationTestProcess;

        public ProjectMutator(IMutationTestProcess mutationTestProcess = null)
        {
            _injectedMutationTestProcess = mutationTestProcess;
            _logger = ApplicationLogging.LoggerFactory.CreateLogger<ProjectOrchestrator>();
        }

        public IMutationTestProcess MutateProject(IStrykerOptions options, MutationTestInput input, IReporter reporters)
        {
            var process = _injectedMutationTestProcess ?? new MutationTestProcess(input, options, reporters,
                new MutationTestExecutor(input.TestRunner));

            // Enrich test projects info with unit tests
            EnrichTestProjectsWithTestInfo(input.InitialTestRun, input.TestProjectsInfo);

            // mutate
            process.Mutate();

            return process;
        }

        private void EnrichTestProjectsWithTestInfo(InitialTestRun initialTestRun, ITestProjectsInfo testProjectsInfo)
        {
            var unitTests =
                initialTestRun.Result.VsTestDescriptions
                .Select(desc => desc.Case)
                // F# has a different syntax tree and would throw further down the line
                .Where(unitTest => Path.GetExtension(unitTest.CodeFilePath) == ".cs").ToList();

            if (!unitTests.Any())
            {
                unitTests = initialTestRun.Result.VsTestDescriptions
                    .Select(desc => desc.Case)
                    // F# has a different syntax tree and would throw further down the line
                    .Where(unitTest => Path.GetExtension(unitTest.CodeFilePath) != ".fs").ToList();
            }

            foreach (var unitTest in unitTests)
            {
                var testFile = testProjectsInfo.TestFiles.SingleOrDefault(testFile => testFile.FilePath == unitTest.CodeFilePath);
                if (testFile is not null)
                {
                    var lineSpan = testFile.SyntaxTree.GetText().Lines[unitTest.LineNumber - 1].Span;
                    var nodesInSpan = testFile.SyntaxTree.GetRoot().DescendantNodes(lineSpan);
                    var node = nodesInSpan.First(n => n is MethodDeclarationSyntax);
                    testFile.AddTest(unitTest.Id, unitTest.FullyQualifiedName, node);
                }
                else
                {
                    //Test if you can find the file by scanning the sources for testcase name
                    var qualifiedNameArray = unitTest.FullyQualifiedName.Split('.');
                    var methodName = qualifiedNameArray[^1];
                    var className = qualifiedNameArray[^2];
                    var nameSpace1 = new ArraySegment<string>(qualifiedNameArray, 0, qualifiedNameArray.Length - 2);
                    var nameSpace = $"namespace {string.Join('.', nameSpace1)}";

                    testFile = testProjectsInfo.TestFiles.Where(tFile => !tFile.FilePath.EndsWith("GlobalSuppressions.cs")).SingleOrDefault(tFile =>
                        tFile.Source.Contains(className) && tFile.Source.Contains(methodName) && tFile.Source.Contains(nameSpace));
                    if (testFile is not null)
                    {
                        var testDescriptions =
                            initialTestRun.Result.VsTestDescriptions.Where(td => td.Description.Name == unitTest.FullyQualifiedName);
                        foreach (var testDescription in testDescriptions)
                        {
                            testDescription.Description.TestFilePath = testFile.FilePath;
                        }

                        var lineNumber = unitTest.LineNumber;
                        if (lineNumber < 1)
                        {
                            var lines = testFile.Source.Split("\r\n");
                            foreach (var line in lines)
                            {
                                if (line.Contains(methodName) && !line.Contains("class"))
                                {
                                    lineNumber = Array.IndexOf(testFile.Source.Split("\r\n"),
                                        testFile.Source.Split("\r\n").First(sourceLine => sourceLine.Contains(methodName) && !sourceLine.Contains("class")));
                                    break;
                                }
                            }
                        }

                        var lineSpan = testFile.SyntaxTree.GetText().Lines[lineNumber].Span;
                        var nodesInSpan = testFile.SyntaxTree.GetRoot().DescendantNodes(lineSpan);
                        var node = nodesInSpan.First(n => n is MethodDeclarationSyntax);
                        testFile.AddTest(unitTest.Id, unitTest.FullyQualifiedName, node);
                    }
                    else
                    {
                        _logger.LogDebug(
                            "Could not locate unit test in any testfile. This should not happen and results in incorrect test reporting.");
                    }
                }
            }
        }
    }
}
