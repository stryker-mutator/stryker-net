using System.Linq;
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
        private readonly IInitialisationProcessProvider _initialisationProcessProvider;
        private readonly IMutationTestProcessProvider _mutationTestProcessProvider;

        public ProjectMutator(IInitialisationProcessProvider initialisationProcessProvider = null,
            IMutationTestProcessProvider mutationTestProcessProvider = null)
        {
            _initialisationProcessProvider = initialisationProcessProvider ?? new InitialisationProcessProvider();
            _mutationTestProcessProvider = mutationTestProcessProvider ?? new MutationTestProcessProvider();
        }

        public IMutationTestProcess MutateProject(StrykerOptions options, IReporter reporters)
        {
            // get a new instance of InitialisationProcess for each project
            var initialisationProcess = _initialisationProcessProvider.Provide();
            // initialize
            var dashboardReporter = GetDashboardReporter(reporters);
            var input = initialisationProcess.Initialize(options, dashboardReporter);

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

        private static DashboardReporter GetDashboardReporter(IReporter reporters)
        {
            if (reporters is BroadcastReporter broadcastReporter)
            {
                return broadcastReporter.Reporters.OfType<DashboardReporter>().FirstOrDefault();
            }

            return reporters as DashboardReporter;
        }
    }
}
