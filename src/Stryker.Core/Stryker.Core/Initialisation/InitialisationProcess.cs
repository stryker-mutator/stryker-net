using Microsoft.Extensions.Logging;
using Stryker.Core.Logging;
using Stryker.Core.MutationTest;
using Stryker.Core.Options;
using Stryker.Core.TestRunners;
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
        private IInputFileResolver _inputFileResolver { get; set; }
        private IInitialBuildProcess _initialBuildProcess { get; set; }
        private IInitialTestProcess _initialTestProcess { get; set; }
        private ITestRunner _testRunner { get; set; }
        private ILogger _logger { get; set; }
        private IAssemblyReferenceResolver _assemblyReferenceResolver { get; set; }

        public InitialisationProcess(
            IInputFileResolver inputFileResolver = null,
            IInitialBuildProcess initialBuildProcess = null,
            IInitialTestProcess initialTestProcess = null,
            ITestRunner testRunner = null,
            IAssemblyReferenceResolver assemblyReferenceResolver = null)
        {
            _inputFileResolver = inputFileResolver ?? new InputFileResolver();
            _initialBuildProcess = initialBuildProcess ?? new InitialBuildProcess();
            _initialTestProcess = initialTestProcess ?? new InitialTestProcess();
            _testRunner = testRunner;
            _logger = ApplicationLogging.LoggerFactory.CreateLogger<InitialisationProcess>();
            _assemblyReferenceResolver = assemblyReferenceResolver ?? new AssemblyReferenceResolver();
        }

        public MutationTestInput Initialize(StrykerOptions options)
        {
            // resolve project info
            var projectInfo = _inputFileResolver.ResolveInput(options);

            // initial build
            _initialBuildProcess.InitialBuild(projectInfo.FullFramework, options.BasePath, options.SolutionPath, Path.GetFileName(projectInfo.TestProjectAnalyzerResult.ProjectFilePath));

            // resolve assembly references
            var assemblyReferences = _assemblyReferenceResolver
                .ResolveReferences(projectInfo.ProjectUnderTestAnalyzerResult)
                .ToList();

            var input = new MutationTestInput()
            {
                ProjectInfo = projectInfo,
                AssemblyReferences = assemblyReferences,
                TestRunner = _testRunner ?? new TestRunnerFactory().Create(options, projectInfo)
            };

            // initial test
            var initialTestDuration = _initialTestProcess.InitialTest(input.TestRunner);
            input.TimeoutMS = new TimeoutValueCalculator().CalculateTimeoutValue(initialTestDuration, options.AdditionalTimeoutMS);

            return input;
        }
    }
}
