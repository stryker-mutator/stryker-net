using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using Microsoft.Extensions.Logging;
using Stryker.Core.Exceptions;
using Stryker.Core.Initialisation;
using Stryker.Core.Logging;
using Stryker.Core.Mutants;
using Stryker.Core.MutationTest;
using Stryker.Core.Options;
using Stryker.Core.ProjectComponents;
using Stryker.Core.ProjectComponents.TestProjects;
using Stryker.Core.Reporters;
using Stryker.Core.TestRunners.VsTest;

namespace Stryker.Core
{
    public interface IStrykerRunner
    {
        StrykerRunResult RunMutationTest(IStrykerInputs inputs, ILoggerFactory loggerFactory, IProjectOrchestrator projectOrchestrator = null);
    }

    public class StrykerRunner : IStrykerRunner
    {
        private IEnumerable<IMutationTestProcess> _mutationTestProcesses;
        private ILogger _logger;
        private readonly IReporterFactory _reporterFactory;

        public StrykerRunner(IEnumerable<IMutationTestProcess> mutationTestProcesses = null,
            IReporterFactory reporterFactory = null)
        {
            _mutationTestProcesses = mutationTestProcesses ?? new List<IMutationTestProcess>();
            _reporterFactory = reporterFactory ?? new ReporterFactory();
        }

        /// <summary>
        /// Starts a mutation test run
        /// </summary>
        /// <param name="options">The user options</param>
        /// <param name="loggerFactory">This loggerfactory will be used to create loggers during the stryker run</param>
        /// <exception cref="InputException">For managed exceptions</exception>
        public StrykerRunResult RunMutationTest(IStrykerInputs inputs, ILoggerFactory loggerFactory, IProjectOrchestrator projectOrchestrator = null)
        {
            var stopwatch = new Stopwatch();
            stopwatch.Start();

            SetupLogging(loggerFactory);

            // Setup project orchestrator can't be done sooner since it needs logging
            projectOrchestrator ??= new ProjectOrchestrator();

            var options = inputs.ValidateAll();
            _logger.LogDebug("Stryker started with options: {@Options}", options);

            var reporters = _reporterFactory.Create(options);


            try
            {
                // Mutate
                _mutationTestProcesses = projectOrchestrator.MutateProjects(options, reporters).ToList();

                var rootComponent = AddRootFolderIfMultiProject(_mutationTestProcesses.Select(x => x.Input.SourceProjectInfo.ProjectContents).ToList(), options);

                _logger.LogInformation("{0} mutants created", rootComponent.Mutants.Count());

                AnalyseCoverage(options);
                var readOnlyInputComponent = rootComponent.ToReadOnlyInputComponent();

                // Filter
                foreach (var project in _mutationTestProcesses)
                {
                    project.FilterMutants();
                }

                // Report
                reporters.OnMutantsCreated(readOnlyInputComponent);

                var allMutants = rootComponent.Mutants.ToList();
                var mutantsNotRun = allMutants.Where(x => x.ResultStatus == MutantStatus.NotRun).ToList();

                if (!mutantsNotRun.Any())
                {
                    if (allMutants.Any(x => x.ResultStatus == MutantStatus.Ignored))
                    {
                        _logger.LogWarning("It looks like all mutants with tests were excluded. Try a re-run with less exclusion!");
                    }
                    if (allMutants.Any(x => x.ResultStatus == MutantStatus.NoCoverage))
                    {
                        _logger.LogWarning("It looks like all non-excluded mutants are not covered by a test. Go add some tests!");
                    }
                    if (!allMutants.Any())
                    {
                        _logger.LogWarning("It\'s a mutant-free world, nothing to test.");
                    }
                    return new StrykerRunResult(options, double.NaN);
                }

                // Report
                reporters.OnStartMutantTestRun(mutantsNotRun);

                // Test
                foreach (var project in _mutationTestProcesses)
                {
                    project.Test(project.Input.SourceProjectInfo.ProjectContents.Mutants.Where(x => x.ResultStatus == MutantStatus.NotRun).ToList());
                    project.Restore();
                }

                var combinedTestProjectsInfo = _mutationTestProcesses.Select(mtp => mtp.Input.TestProjectsInfo).Aggregate((a, b) => a + b);
                var testRunnerPools = _mutationTestProcesses.Select(mtp => mtp.Input.TestRunner).Cast<VsTestRunnerPool>();

                foreach(var testRunner in testRunnerPools)
                {
                    foreach(var (_, vsTestDescription) in testRunner.VsTests)
                    {
                        var vsTestCase = vsTestDescription.Case;
                        var testFile = combinedTestProjectsInfo.TestFiles.SingleOrDefault(t => t.FilePath == vsTestCase.CodeFilePath);
                        if(testFile is not null)
                        {
                            testFile.Tests.Add(new TestCase
                            {
                                Id = vsTestCase.Id,
                                Name = vsTestCase.FullyQualifiedName,
                                Source = null,
                                Line = vsTestCase.LineNumber
                            });
                        }
                    }
                }

                reporters.OnAllMutantsTested(readOnlyInputComponent, combinedTestProjectsInfo);

                return new StrykerRunResult(options, readOnlyInputComponent.GetMutationScore());
            }
#if !DEBUG
            catch (Exception ex) when (!(ex is InputException))
            // let the exception be caught by the debugger when in debug
            {
                _logger.LogError(ex, "An error occurred during the mutation test run ");
                throw;
            }
            
#endif
            finally
            {
                // log duration
                stopwatch.Stop();
                _logger.LogInformation("Time Elapsed {0}", stopwatch.Elapsed);
            }
        }

        private void SetupLogging(ILoggerFactory loggerFactory)
        {
            // setup logging
            ApplicationLogging.LoggerFactory = loggerFactory;
            _logger = ApplicationLogging.LoggerFactory.CreateLogger<StrykerRunner>();
        }

        private void AnalyseCoverage(StrykerOptions options)
        {
            if (options.OptimizationMode.HasFlag(OptimizationModes.SkipUncoveredMutants) || options.OptimizationMode.HasFlag(OptimizationModes.CoverageBasedTest))
            {
                _logger.LogInformation($"Capture mutant coverage using '{options.OptimizationMode}' mode.");

                foreach (var project in _mutationTestProcesses)
                {
                    project.GetCoverage();
                }
            }
        }

        /// <summary>
        /// In the case of multiple projects we wrap them inside a wrapper root component. Otherwise the only project root will be the root component.
        /// </summary>
        /// <param name="projectComponents">A list of all project root components</param>
        /// <param name="options">The current stryker options</param>
        /// <returns>The root folder component</returns>
        private IProjectComponent AddRootFolderIfMultiProject(IEnumerable<IProjectComponent> projectComponents, StrykerOptions options)
        {
            if (projectComponents.Count() > 1)
            {
                var rootComponent = new FolderComposite
                {
                    FullPath = options.BasePath // in case of a solution run the basePath will be where the solution file is
                };
                rootComponent.AddRange(projectComponents);
                return rootComponent;
            }
            else
            {
                return projectComponents.FirstOrDefault();
            }
        }
    }
}
