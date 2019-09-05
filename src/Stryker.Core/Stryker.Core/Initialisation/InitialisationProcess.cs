using System.Collections.Generic;
using Stryker.Core.MutationTest;
using Stryker.Core.Options;
using Stryker.Core.TestRunners;
using System.Linq;
using Stryker.Core.Mutants;

namespace Stryker.Core.Initialisation
{
    public interface IInitialisationProcess
    {
        MutationTestInput Initialize(StrykerOptions options);
        int InitialTest(StrykerOptions option, out int nbTests);
        void GetCoverage(StrykerOptions options, IEnumerable<Mutant> projectContentsMutants);
    }

    public class InitialisationProcess : IInitialisationProcess
    {
        private readonly IInputFileResolver _inputFileResolver;
        private readonly IInitialBuildProcess _initialBuildProcess;
        private readonly IInitialTestProcess _initialTestProcess;
        private readonly IAssemblyReferenceResolver _assemblyReferenceResolver;
        private ITestRunner _testRunner;

        // these flags control various optimization techniques
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
        }

        public MutationTestInput Initialize(StrykerOptions options)
        {
            // resolve project info
            var projectInfo = _inputFileResolver.ResolveInput(options);

            // initial build
            _initialBuildProcess.InitialBuild(projectInfo.FullFramework, projectInfo.TestProjectAnalyzerResult.ProjectFilePath, options.SolutionPath);

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

            return input;
        }

        public int InitialTest(StrykerOptions options, out int nbTests)
        {
            // initial test
            var initialTestDuration = _initialTestProcess.InitialTest(_testRunner);

            nbTests = _initialTestProcess.TotalNumberOfTests;
            return new TimeoutValueCalculator().CalculateTimeoutValue(initialTestDuration, options.AdditionalTimeoutMS);
        }

        public void GetCoverage(StrykerOptions options, IEnumerable<Mutant> mutants)
        {
            _initialTestProcess.GetCoverage(_testRunner, mutants);
        }
    }
}
