using System.Collections.Generic;
using System.Linq;
using Buildalyzer;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.Extensions.Logging;
using Stryker.Core.Logging;
using Stryker.Core.MutationTest;
using Stryker.Core.Options;
using Stryker.Core.ProjectComponents.TestProjects;
using Stryker.Core.Reporters;

namespace Stryker.Core.Initialisation
{
    public interface IProjectMutator
    {
        IMutationTestProcess MutateProject(StrykerOptions options, IReporter reporters, IEnumerable<IAnalyzerResult> solutionProjects = null);
    }

    public class ProjectMutator : IProjectMutator
    {
        private readonly ILogger _logger;
        private readonly IMutationTestProcess _injectedMutationtestProcess;
        private readonly IInitialisationProcess _injectedInitialisationProcess;

        public ProjectMutator(IInitialisationProcess initialisationProcess = null,
            IMutationTestProcess mutationTestProcess = null)
        {
            _injectedInitialisationProcess = initialisationProcess;
            _injectedMutationtestProcess = mutationTestProcess;
            _logger = ApplicationLogging.LoggerFactory.CreateLogger<ProjectOrchestrator>();
        }

        public IMutationTestProcess MutateProject(StrykerOptions options, IReporter reporters, IEnumerable<IAnalyzerResult> solutionProjects = null)
        {
            // get a new instance of InitialisationProcess for each project
            var initialisationProcess = _injectedInitialisationProcess ?? new InitialisationProcess();
            // initialize
            var input = initialisationProcess.Initialize(options, solutionProjects);

            var process = _injectedMutationtestProcess ?? new MutationTestProcess(input, options, reporters,
                new MutationTestExecutor(input.TestRunner));

            // initial test
            input.InitialTestRun = initialisationProcess.InitialTest(options);

            // Enrich test projects info with unit tests
            EnrichTestProjectsWithTestInfo(input.InitialTestRun, input.TestProjectsInfo);

            // mutate
            process.Mutate();

            return process;
        }

        private void EnrichTestProjectsWithTestInfo(InitialTestRun initialTestRun, TestProjectsInfo testProjectsInfo)
        {
            foreach (var unitTest in initialTestRun.Result.VsTestDescriptions.Select(desc => desc.Case))
            {
                var testFile = testProjectsInfo.TestFiles.SingleOrDefault(testFile => testFile.FilePath == unitTest.CodeFilePath);
                if (testFile is not null)
                {
                    var lineSpan = testFile.SyntaxTree.GetText().Lines[unitTest.LineNumber - 1].Span;
                    var nodes = testFile.SyntaxTree.GetRoot().DescendantNodes(lineSpan)
                        .Where(n => n.GetType() == typeof(MethodDeclarationSyntax));
                    var node = nodes.First(n => n.Span.Contains(lineSpan));
                    testFile.AddTest(unitTest.Id, unitTest.FullyQualifiedName, node);
                }
                else
                {
                    _logger.LogDebug("Could not locate unit test in any testfile. This should not happen and results in incorrect test reporting.");
                }
            }
        }
    }
}
