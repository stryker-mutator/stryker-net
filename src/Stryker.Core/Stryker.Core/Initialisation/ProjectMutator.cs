using Stryker.Core.MutationTest;
using Stryker.Core.Options;
using Stryker.Core.Reporters;

namespace Stryker.Core.Initialisation
{
    public interface IProjectMutator
    {
        IMutationTestProcess MutateProject(StrykerOptions options, MutationTestInput input, IReporter reporters);
    }

    public class ProjectMutator : IProjectMutator
    {
        private readonly IMutationTestProcess _injectedMutationTestProcess;

        public ProjectMutator(IMutationTestProcess mutationTestProcess = null) => _injectedMutationTestProcess = mutationTestProcess;

        public IMutationTestProcess MutateProject(StrykerOptions options, MutationTestInput input, IReporter reporters)
        {
            var process = _injectedMutationTestProcess ?? new MutationTestProcess(input, options, reporters,
                new MutationTestExecutor(input.TestRunner));
            
            // mutate
            process.Mutate();

            return process;
        }
    }
}
