using Microsoft.Extensions.Logging;
using Stryker.Core.Logging;
using Stryker.Core.MutationTest;
using Stryker.Core.Options;
using Stryker.Core.TestRunners;
using System.Linq;
using Language = Stryker.Core.LanguageFactory.Language;

namespace Stryker.Core.Initialisation
{
    public interface IInitialisationProcess
    {
        (MutationTestInput, Language) Initialize(StrykerOptions options);
        int InitialTest(StrykerOptions option, out int nbTests);
    }

    public class InitialisationProcess : IInitialisationProcess
    {
        private readonly IInputFileResolver _inputFileResolver;
        private readonly IInitialBuildProcess _initialBuildProcess;
        private readonly IInitialTestProcess _initialTestProcess;
        private readonly IAssemblyReferenceResolver _assemblyReferenceResolver;
        private ITestRunner _testRunner;
        private readonly ILogger _logger;

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
            _assemblyReferenceResolver = assemblyReferenceResolver ?? new AssemblyReferenceResolver();
            _logger = ApplicationLogging.LoggerFactory.CreateLogger<InitialisationProcess>();
        }

        public (MutationTestInput, Language) Initialize(StrykerOptions options)
        {
            // resolve project infov
            var tuple = _inputFileResolver.ResolveInput(options);
            var projectInfo = tuple.Item1;
            Language language = tuple.Item2;

            // initial build
            var testProjects = projectInfo.TestProjectAnalyzerResults.ToList();
            for (var i = 0; i < testProjects.Count; i++)
            {
                _logger.LogInformation(
                    "Building test project {ProjectFilePath} ({CurrentTestProject}/{OfTotalTestProjects})",
                    testProjects[i].ProjectFilePath, i + 1,
                    projectInfo.TestProjectAnalyzerResults.Count());

                _initialBuildProcess.InitialBuild(
                    testProjects[i].TargetFramework == Framework.DotNetClassic,
                    testProjects[i].ProjectFilePath,
                    options.SolutionPath);
            }

            if (_testRunner == null)
            {
                _testRunner = new TestRunnerFactory().Create(options, options.Optimizations, projectInfo);
            }

            var input = new MutationTestInput()
            {
                ProjectInfo = projectInfo,
                AssemblyReferences = _assemblyReferenceResolver.LoadProjectReferences(projectInfo.ProjectUnderTestAnalyzerResult.References).ToList(),
                TestRunner = _testRunner
            };

            return (input, language);
        }

        public int InitialTest(StrykerOptions options, out int nbTests)
        {
            // initial test
            var initialTestDuration = _initialTestProcess.InitialTest(_testRunner);

            nbTests = _initialTestProcess.TotalNumberOfTests;
            return new TimeoutValueCalculator().CalculateTimeoutValue(initialTestDuration, options.AdditionalTimeoutMS);
        }
    }
}
