using Stryker.Core.MutationTest;
using Stryker.Core.Options;
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

            // mutate
            process.Mutate();

            return process;
        }
    }
}
