using System.Collections.Generic;
using Buildalyzer;
using Stryker.Core.MutationTest;
using Stryker.Core.Options;
using Stryker.Core.Reporters;

namespace Stryker.Core.Initialisation
{
    public interface IProjectMutator
    {
        IMutationTestProcess MutateProject(StrykerOptions options, IReporter reporters, IEnumerable<IAnalyzerResult> solutionProjects = null);
    }

    public class ProjectMutator : IProjectMutator
    {
        private readonly IMutationTestProcess _injectedMutationTestProcess;
        private readonly IInitialisationProcess _injectedInitializationProcess;

        public ProjectMutator(IInitialisationProcess initializationProcess = null,
            IMutationTestProcess mutationTestProcess = null)
        {
            _injectedInitializationProcess = initializationProcess ;
            _injectedMutationTestProcess = mutationTestProcess;
        }

        public IMutationTestProcess MutateProject(StrykerOptions options, IReporter reporters, IEnumerable<IAnalyzerResult> solutionProjects = null)
        {
            // get a new instance of InitializationProcess for each project
            var initializationProcess = _injectedInitializationProcess ?? new InitialisationProcess();

            // initialize
            var input = initializationProcess.Initialize(options, solutionProjects);

            var process = _injectedMutationTestProcess ?? new MutationTestProcess(input, options, reporters,
                new MutationTestExecutor(input.TestRunner));

            // initial test
            input.InitialTestRun = initializationProcess.InitialTest(options);

            // mutate
            process.Mutate();

            return process;
        }
    }
}
