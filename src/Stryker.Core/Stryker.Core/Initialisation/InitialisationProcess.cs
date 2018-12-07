using Microsoft.Extensions.Logging;
using Stryker.Core.Logging;
using Stryker.Core.MutationTest;
using Stryker.Core.Options;
using Stryker.Core.TestRunners;
using System.Linq;
using Stryker.Core.Parsers;
using Stryker.Core.Testing;
using Buildalyzer;
using System.IO;
using Microsoft.CodeAnalysis;
using System.Collections.Generic;

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
        private IMetadataReferenceProvider _metadataReference { get; set; }


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
            _metadataReference = new MetadataReferenceProvider();
        }

        public MutationTestInput Initialize(StrykerOptions options)
        {
            // resolve project info
            var projectInfo = _inputFileResolver.ResolveInput(options.BasePath, options.ProjectUnderTestNameFilter);

            AnalyzerManager manager = new AnalyzerManager();
            string path = projectInfo.ProjectUnderTestPath + "\\" + projectInfo.ProjectUnderTestProjectName + ".csproj";
            ProjectAnalyzer analyzer = manager.GetProject(Path.GetFullPath(path));
            var result = analyzer.Build();
            var analyzerResult = result.First();
            var references2 = analyzerResult.References;

            var references3 = new List<PortableExecutableReference>();
            foreach(var refpath in references2)
            {
                references3.Add(_metadataReference.CreateFromFile(refpath));
            }

            // initial build
            _initialBuildProcess.InitialBuild(projectInfo.TestProjectPath, projectInfo.TestProjectFileName);

            // resolve assembly references
            //var references = _assemblyReferenceResolver.ResolveReferences(
            //        projectInfo.TestProjectPath,
            //        projectInfo.TestProjectFileName,
            //        projectInfo.ProjectUnderTestAssemblyName)
            //        .ToList();

            var input = new MutationTestInput()
            {
                ProjectInfo = projectInfo,
                AssemblyReferences = references3,
                TestRunner = _testRunner ?? new TestRunnerFactory().Create("", projectInfo.TestProjectPath)
            };

            // initial test
            var initialTestDuration = _initialTestProcess.InitialTest(input.TestRunner);
            input.TimeoutMS = new TimeoutValueCalculator().CalculateTimeoutValue(initialTestDuration, options.AdditionalTimeoutMS);

            return input;
        }
    }
}
