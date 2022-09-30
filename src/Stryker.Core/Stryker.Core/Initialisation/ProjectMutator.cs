using System.Collections.Generic;
using System.Linq;
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
        private readonly IInitialisationProcessProvider _initialisationProcessProvider;
        private readonly IMutationTestProcessProvider _mutationTestProcessProvider;

        public ProjectMutator(IInitialisationProcessProvider initialisationProcessProvider = null,
            IMutationTestProcessProvider mutationTestProcessProvider = null)
        {
            _initialisationProcessProvider = initialisationProcessProvider ?? new InitialisationProcessProvider();
            _mutationTestProcessProvider = mutationTestProcessProvider ?? new MutationTestProcessProvider();
        }

        public IMutationTestProcess MutateProject(StrykerOptions options, IReporter reporters, IEnumerable<IAnalyzerResult> solutionProjects = null)
        {
            // get a new instance of InitialisationProcess for each project
            var initialisationProcess = _initialisationProcessProvider.Provide();
            // initialize
            var input = initialisationProcess.Initialize(options, solutionProjects);

            var process = _mutationTestProcessProvider.Provide(
                mutationTestInput: input,
                reporter: reporters,
                mutationTestExecutor: new MutationTestExecutor(input.TestRunner),
                options: options);

            // initial test
            input.InitialTestRun = initialisationProcess.InitialTest(options);

            // mutate
            process.Mutate();

            return process;
        }
    }
}
