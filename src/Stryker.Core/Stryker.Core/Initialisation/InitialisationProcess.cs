using Microsoft.Extensions.Logging;
using Stryker.Core.Logging;
using Stryker.Core.MutationTest;
using Stryker.Core.Options;
using Stryker.Core.TestRunners;
using Stryker.Core.TestRunners.VsTest;
using System.IO;
using System.Linq;

namespace Stryker.Core.Initialisation
{
    public interface IInitialisationProcess
    {
        MutationTestInput Initialize(StrykerOptions options);
    }

    public class InitialisationProcess : IInitialisationProcess
    {
        private readonly IInputFileResolver _inputFileResolver;
        private readonly IInitialBuildProcess _initialBuildProcess;
        private readonly IInitialTestProcess _initialTestProcess;
        private readonly ITestRunner _testRunner;
        private readonly ILogger _logger;
        private readonly IAssemblyReferenceResolver _assemblyReferenceResolver;
        private readonly ITestRunner _testCaseDiscoverer;

        public InitialisationProcess(
            IInputFileResolver inputFileResolver = null,
            IInitialBuildProcess initialBuildProcess = null,
            IInitialTestProcess initialTestProcess = null,
            ITestRunner testRunner = null,
            IAssemblyReferenceResolver assemblyReferenceResolver = null,
            ITestRunner testCaseDiscoverer = null)
        {
            _inputFileResolver = inputFileResolver ?? new InputFileResolver();
            _initialBuildProcess = initialBuildProcess ?? new InitialBuildProcess();
            _initialTestProcess = initialTestProcess ?? new InitialTestProcess();
            _testRunner = testRunner;
            _logger = ApplicationLogging.LoggerFactory.CreateLogger<InitialisationProcess>();
            _assemblyReferenceResolver = assemblyReferenceResolver ?? new AssemblyReferenceResolver();
            _testCaseDiscoverer = testCaseDiscoverer;
        }

        public MutationTestInput Initialize(StrykerOptions options)
        {
            // resolve project info
            var projectInfo = _inputFileResolver.ResolveInput(options);

            // initial build
            _initialBuildProcess.InitialBuild(projectInfo.FullFramework, options.BasePath, options.SolutionPath, Path.GetFileName(projectInfo.TestProjectAnalyzerResult.ProjectFilePath));

            var input = new MutationTestInput()
            {
                ProjectInfo = projectInfo,
                AssemblyReferences = _assemblyReferenceResolver.LoadProjectReferences(projectInfo.ProjectUnderTestAnalyzerResult.References).ToList(),
                TestRunner = _testRunner ?? new TestRunnerFactory().Create(options, projectInfo)
            };

            using (var testDiscoverer = _testCaseDiscoverer ?? new VsTestRunner(options, projectInfo))
            {
                _logger.LogInformation("Total number of tests found: {0}", testDiscoverer.DiscoverTests());
            }

            // initial test
            var initialTestDuration = _initialTestProcess.InitialTest(input.TestRunner);
            input.TimeoutMS = new TimeoutValueCalculator().CalculateTimeoutValue(initialTestDuration, options.AdditionalTimeoutMS);

            return input;
        }
    }
}
