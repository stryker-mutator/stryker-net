using System.Collections.Generic;
using Microsoft.Extensions.Logging;
using Stryker.Core.Logging;
using Stryker.Core.MutationTest;
using Stryker.Core.Options;
using Stryker.Core.TestRunners;
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
        private ProjectInfo projectInfo;

        // these flags control various optimisation techniques
        private const OptimizationFlags Flags = false ? 0 : OptimizationFlags.CoverageBasedTest | OptimizationFlags.SkipUncoveredMutants | OptimizationFlags.AbortTestOnKill;

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
            projectInfo = _inputFileResolver.ResolveInput(options.BasePath, options.ProjectUnderTestNameFilter, options.FilesToExclude.ToList());

            if (_testRunner == null)
            {
                _testRunner = new TestRunnerFactory().Create(options, Flags, projectInfo);
            }
            // initial build
            _initialBuildProcess.InitialBuild(projectInfo.TestProjectPath, projectInfo.TestProjectFileName);

            // resolve assembly references
            var references = _assemblyReferenceResolver.ResolveReferences(
                    projectInfo.TestProjectPath,
                    projectInfo.TestProjectFileName,
                    projectInfo.ProjectUnderTestAssemblyName)
                    .ToList();

            var input = new MutationTestInput()
            {
                ProjectInfo = projectInfo,
                AssemblyReferences = references,
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
