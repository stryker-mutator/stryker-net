using System.Collections.Generic;
using Microsoft.Extensions.Logging;
using Stryker.Core.Exceptions;
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
        
        int InitialTest(StrykerOptions option);

        IEnumerable<int> CoveredMutants { get; }

    }

    public class InitialisationProcess : IInitialisationProcess
    {
        private IInputFileResolver _inputFileResolver { get; set; }
        private IInitialBuildProcess _initialBuildProcess { get; set; }
        private IInitialTestProcess _initialTestProcess { get; set; }
        private ITestRunner _testRunner { get; set; }
        private ILogger Logger { get; }
        private IAssemblyReferenceResolver _assemblyReferenceResolver { get; set; }

        // these flags control various optimization techniques
        // TODO: expose these options through the CLI (once features is stable)
        private const OptimizationFlags Flags = OptimizationFlags.CoverageBasedTest | OptimizationFlags.SkipUncoveredMutants | OptimizationFlags.AbortTestOnKill | OptimizationFlags.CaptureCoveragePerTest;


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
            Logger = ApplicationLogging.LoggerFactory.CreateLogger<InitialisationProcess>();
            _assemblyReferenceResolver = assemblyReferenceResolver ?? new AssemblyReferenceResolver();
        }

        public IEnumerable<int> CoveredMutants => _testRunner.CoveredMutants;

        public MutationTestInput Initialize(StrykerOptions options)
        {
            // resolve project info
            var projectInfo = _inputFileResolver.ResolveInput(options);

            // initial build
            _initialBuildProcess.InitialBuild(projectInfo.FullFramework, options.BasePath, options.SolutionPath, Path.GetFileName(projectInfo.TestProjectAnalyzerResult.ProjectFilePath));

            if (_testRunner == null)
            {
                _testRunner = new TestRunnerFactory().Create(options, Flags, projectInfo);
            }

            var input = new MutationTestInput()
            {
                ProjectInfo = projectInfo,
                AssemblyReferences = _assemblyReferenceResolver.LoadProjectReferences(projectInfo.ProjectUnderTestAnalyzerResult.References).ToList(),
                TestRunner = _testRunner
            };

            return input;
        }

        public int InitialTest(StrykerOptions options)
        {
            // initial test
            var initialTestDuration = _initialTestProcess.InitialTest(_testRunner);
            
            return new TimeoutValueCalculator().CalculateTimeoutValue(initialTestDuration, options.AdditionalTimeoutMS);
        }
    }
}
