using System.IO;
using System.Linq;
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
        IMutationTestProcess MutateProject(StrykerOptions options, MutationTestInput input, IReporter reporters);
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

        public IMutationTestProcess MutateProject(StrykerOptions options, MutationTestInput input, IReporter reporters)
        {
            var process = _injectedMutationTestProcess ?? new MutationTestProcess(input, options, reporters,
                new MutationTestExecutor(input.TestRunner));
            
            // Enrich test projects info with unit tests
            EnrichTestProjectsWithTestInfo(input.InitialTestRun, input.TestProjectsInfo);

            // mutate
            process.Mutate();

            return process;
        }

        private void EnrichTestProjectsWithTestInfo(InitialTestRun initialTestRun, TestProjectsInfo testProjectsInfo)
        {
            var unitTests =
                initialTestRun.Result.VsTestDescriptions
                .Select(desc => desc.Case)
                // F# has a different syntax tree and would throw further down the line
                .Where(unitTest => Path.GetExtension(unitTest.CodeFilePath) == ".cs");

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
                    _logger.LogDebug("Could not locate unit test in any testfile. This should not happen and results in incorrect test reporting.");
                }
            }
        }
    }
}
