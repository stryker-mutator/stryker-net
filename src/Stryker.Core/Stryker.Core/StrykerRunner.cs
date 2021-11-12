using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Microsoft.Extensions.Logging;
using Stryker.Core.Exceptions;
using Stryker.Core.Initialisation;
using Stryker.Core.Logging;
using Stryker.Core.Mutants;
using Stryker.Core.MutationTest;
using Stryker.Core.Options;
using Stryker.Core.ProjectComponents;
using Stryker.Core.Reporters;
using Stryker.Core.Reporters.Progress;

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
        /// <param name="inputs"></param>
        /// <param name="loggerFactory">This loggerfactory will be used to create loggers during the stryker run</param>
        /// <param name="projectOrchestrator"></param>
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

            var reporters = inputs.MutantToDiagnose.SuppliedInput.HasValue ? new BroadcastReporter(Enumerable.Empty<IReporter>()) : _reporterFactory.Create(options);

            try
            {
                // Mutate
                _mutationTestProcesses = projectOrchestrator.MutateProjects(options, reporters).ToList();

                IReadOnlyProjectComponent rootComponent = AddRootFolderIfMultiProject(_mutationTestProcesses.Select(x => x.Input.ProjectInfo.ProjectContents).ToList(), options);

                _logger.LogInformation("{0} mutants created", rootComponent.Mutants.Count());

                AnalyseCoverage(options);

                // Filter
                foreach (var project in _mutationTestProcesses)
                {
                    project.FilterMutants();
                }

                // Report
                reporters.OnMutantsCreated(rootComponent);

                var allMutants = rootComponent.Mutants;
                var mutantsNotRun = rootComponent.NotRunMutants().ToList();

                if (!mutantsNotRun.Any())
                {
                    if (allMutants.Any(x => x.ResultStatus == MutantStatus.Ignored))
                    {
                        _logger.LogWarning("It looks like all mutants with tests were ignored. Try a re-run with less ignoring!");
                    }
                    if (allMutants.Any(x => x.ResultStatus == MutantStatus.NoCoverage))
                    {
                        _logger.LogWarning("It looks like all non-ignored mutants are not covered by a test. Go add some tests!");
                    }
                    if (allMutants.Any(x => x.ResultStatus == MutantStatus.CompileError))
                    {
                        _logger.LogWarning("It looks like all mutants resulted in compile errors. Mutants sure are strange!");
                    }
                    if (!allMutants.Any())
                    {
                        _logger.LogWarning("It\'s a mutant-free world, nothing to test.");
                    }

                    reporters.OnAllMutantsTested(rootComponent);
                    return new StrykerRunResult(options, rootComponent.GetMutationScore());
                }

                if (!inputs.MutantToDiagnose.SuppliedInput.HasValue)
                {
                    // normal test run
                    TestMutants(reporters, mutantsNotRun, readOnlyInputComponent);
                }
                else
                {
                    DiagnoseMutant(inputs, reporters, mutantsNotRun, readOnlyInputComponent);
                }

                reporters.OnAllMutantsTested(rootComponent);
                return new StrykerRunResult(options, rootComponent.GetMutationScore());
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

        private void DiagnoseMutant(IStrykerInputs inputs, IReporter reporters, List<Mutant> mutantsNotRun,
            IReadOnlyProjectComponent readOnlyInputComponent)
        {
            var mutant = inputs.MutantToDiagnose.SuppliedInput!.Value;

            var monitoredProject = _mutationTestProcesses.FirstOrDefault(p =>
                p.Input.ProjectInfo.ProjectContents.Mutants.Any(m => m.Id == mutant));
            _logger.LogWarning("*** Mutant Diagnostic mode enabled ***");
            if (monitoredProject == null)
            {
                // we were given an invalid mutant ID
                _logger.LogError($"Unable to find mutant {mutant}. Please check that it exists in the project(s).");
                return;
            }
            var monitoredMutant = monitoredProject.Input.ProjectInfo.ProjectContents.Mutants.First(m => m.Id == mutant);
            // diagnostic run
            _logger.LogWarning($"Diagnosing mutant {mutant}.");
            if (monitoredMutant.CoveringTests.IsEveryTest)
            {
                _logger.LogWarning("This mutant is run against every test, unable to automatically diagnose issue.");
                return;
            }
            _logger.LogInformation("Mutant is covered by the following tests: ");
            var testNames = monitoredProject.GetTestNames(monitoredMutant.CoveringTests);
            _logger.LogInformation(string.Join(',', testNames));
            
            _logger.LogInformation("*** Step 1 normal run ***");
            var step1 = PerformStep(reporters, mutantsNotRun, readOnlyInputComponent, monitoredProject, monitoredMutant);
            // clean up status
            _logger.LogInformation("*** Step 2 solo run ***");

            mutantsNotRun = new List<Mutant> { monitoredMutant };
            var step2 = PerformStep(reporters, mutantsNotRun, readOnlyInputComponent, monitoredProject, monitoredMutant);
            // clean up status
            _logger.LogInformation("*** Step 3 run against all tests ***");
            monitoredMutant.CoveringTests = TestsGuidList.EveryTest();
            var step3 = PerformStep(reporters, new List<Mutant> { monitoredMutant }, readOnlyInputComponent, monitoredProject, monitoredMutant);
            _logger.LogInformation("*** Step 3 solo run against all ***");
            if (step1 == step2 && step1 == step3)
            {
                _logger.LogWarning($"All runs lead to the same status ({step1}), no problem was detected.");
            }
            else
            {
                _logger.LogInformation($"Run results are not consistent!");
                if (step1 == MutantStatus.Survived)
                {
                    // false positive
                    if (step2 == MutantStatus.Survived)
                    {
                        _logger.LogInformation("Coverage analysis dit not properly capture coverage for this mutant.");
                        // TODO: dump the test that killed the mutant (and that was not part of the covering test
                    }
                    else
                    {
                        _logger.LogInformation("There have been an unexpected interaction between two mutations.");
                        // TODO: perform a binary search on the original mutant group to find the problematic mutant. This one has probably coverage issue
                    }
                }
                else if (step1 == MutantStatus.NoCoverage)
                {
                    //TO DO handle situation where a mutant is killed while not having coverage
                }
                else
                {
                    _logger.LogWarning("Stryker.NET is not able to produce a diagnostic for false negative.");
                }
            }
            _logger.LogWarning("*** Mutant Diagnostic mode end ***");
        }

        private MutantStatus PerformStep(IReporter reporters, IReadOnlyCollection<Mutant> mutants,
            IReadOnlyProjectComponent readOnlyInputComponent, IMutationTestProcess monitoredProject, Mutant monitoredMutant)
        {
            reporters.OnStartMutantTestRun(mutants);
            monitoredMutant.ResultStatus = MutantStatus.NotRun;
            monitoredProject!.Diagnostic(mutants, monitoredMutant.Id);

            reporters.OnAllMutantsTested(readOnlyInputComponent);
            var step2 = monitoredMutant.ResultStatus;
            _logger.LogInformation($"Mutant {monitoredMutant.Id} is {step2}.");
            if (step2 == MutantStatus.Killed)
            {
                _logger.LogInformation("Mutant is killed by the following tests: ");
                var testNames = monitoredProject.GetTestNames(monitoredMutant.KillingTests);
                _logger.LogInformation(string.Join(',', testNames));
            }

            return step2;
        }

        private void TestMutants(IReporter reporters, IEnumerable<Mutant> mutantsNotRun, IReadOnlyProjectComponent readOnlyInputComponent)
        {
            // Report
            reporters.OnStartMutantTestRun(mutantsNotRun);

            // Test
            foreach (var project in _mutationTestProcesses)
            {
                project.Test(project.Input.ProjectInfo.ProjectContents.Mutants.Where(x => x.ResultStatus == MutantStatus.NotRun)
                    .ToList());
                project.Restore();
            }

            reporters.OnAllMutantsTested(readOnlyInputComponent);
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
                var rootComponent = new Solution
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
