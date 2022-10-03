using System.Linq;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Stryker.Core.MutationTest;
using Stryker.Core.Options;
using Stryker.Core.ProjectComponents.TestProjects;
using Stryker.Core.Reporters;

namespace Stryker.Core.Initialisation
{
    public interface IProjectMutator
    {
        IMutationTestProcess MutateProject(StrykerOptions options, IReporter reporters);
    }

    public class ProjectMutator : IProjectMutator
    {
        private readonly IMutationTestProcess _injectedMutationtestProcess;
        private readonly IInitialisationProcess _injectedInitialisationProcess;

        public ProjectMutator(IInitialisationProcess initialisationProcess = null,
            IMutationTestProcess mutationTestProcess = null)
        {
            _injectedInitialisationProcess = initialisationProcess ;
            _injectedMutationtestProcess = mutationTestProcess;
        }

        public IMutationTestProcess MutateProject(StrykerOptions options, IReporter reporters)
        {
            // get a new instance of InitialisationProcess for each project
            var initialisationProcess = _injectedInitialisationProcess ?? new InitialisationProcess();
            // initialize
            var input = initialisationProcess.Initialize(options);

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

        private static void EnrichTestProjectsWithTestInfo(InitialTestRun initialTestRun, TestProjectsInfo testProjectsInfo)
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
                    // Log debug testfile not found for unit test
                }
            }
        }
    }
}
