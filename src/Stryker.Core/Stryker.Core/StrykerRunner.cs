using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using Microsoft.Extensions.Logging;
using Stryker.Core.Exceptions;
using Stryker.Core.Initialisation;
using Stryker.Core.Logging;
using Stryker.Core.Mutants;
using Stryker.Core.MutationTest;
using Stryker.Core.Options;
using Stryker.Core.ProjectComponents;
using Stryker.Core.Reporters;

namespace Stryker.Core
{
    public interface IStrykerRunner
    {
        void SetupLogging(ILoggerFactory loggerFactory);
        StrykerRunResult RunMutationTest(IStrykerInputs inputs, IProjectOrchestrator projectOrchestrator = null);
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
        /// <param name="projectOrchestrator"></param>
        /// <exception cref="InputException">For managed exceptions</exception>
        public StrykerRunResult RunMutationTest(IStrykerInputs inputs, IProjectOrchestrator projectOrchestrator = null)
        {
            var stopwatch = new Stopwatch();
            stopwatch.Start();

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
                    reporters.OnAllMutantsTested(rootComponent);
                    return new StrykerRunResult(options, rootComponent.GetMutationScore());
                }

                if (!inputs.MutantToDiagnose.SuppliedInput.HasValue)
                {
                    // normal test run
                    TestMutants(reporters, mutantsNotRun, rootComponent);
                }
                else
                {
                    DiagnoseMutant(inputs, mutantsNotRun);
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

        private void DiagnoseMutant(IStrykerInputs inputs, IList<IReadOnlyMutant> mutantsNotRun)
        {
            var mutant = inputs.MutantToDiagnose.SuppliedInput!.Value;

            var monitoredProject = _mutationTestProcesses.FirstOrDefault(p =>
                p.Input.ProjectInfo.ProjectContents.Mutants.Any(m => m.Id == mutant));
            _logger.LogWarning("*** Mutant Diagnostic mode enabled ***");
            if (monitoredProject == null)
            {
                // we were given an invalid mutant ID
                _logger.LogError($"Unable to find mutant {mutant}. Please provide an existing mutant id.");
                return;
            }

            var results = monitoredProject.DiagnoseMutant(monitoredProject.Input.ProjectInfo.ProjectContents.Mutants, inputs.MutantToDiagnose.SuppliedInput.Value);
            _logger.LogInformation(GenerateDiagnoseReport(results));
            _logger.LogWarning("*** Mutant Diagnostic mode end ***");
        }

        public string GenerateDiagnoseReport(MutantDiagnostic results)
        {
            var report = new StringBuilder();
            var baseLine = results.RunResults[0].status;
            // diagnostic run
            if (baseLine == results.RunResults[1].status && baseLine == results.RunResults[2].status)
            {  
                report.AppendFormat("Mutant consistently appears as {0}. {1}.",
                    baseLine, baseLine switch
                    {
                        MutantStatus.NoCoverage or MutantStatus.Survived=> "You need to add some tests to fix that",
                        _ => "There is no visible issue"
                    });
            }
            else
            {
                report.AppendLine("Run results are not consistent!");
                if (baseLine == MutantStatus.Survived)
                {
                    // false positive
                    if (results.RunResults[1].status == baseLine)
                    {
                        _logger.LogInformation("Coverage analysis dit not properly capture coverage for this mutant.");
                        report.AppendLine("The coverage for this mutant was not properly determined. You can workaround this problem.");
                        var findDeclaringNodeText = results.DiagnosedMutant.Location;
                        report.AppendFormat("Add '// Stryker test full once' to {0}.", findDeclaringNodeText);
                        report.AppendLine();
                        report.Append("It was killed by these test(s): ");
                        report.AppendJoin(',', results.RunResults[2].killingTests.Except(results.CoveringTests));
                    }
                    else
                    {
                        _logger.LogInformation("There have been an unexpected interaction between two mutations.");
                        report.AppendLine("The tests for this mutant was corrupted by another mutant. As a work around, you should");
                        var findDeclaringNodeText = results.ConflictingMutant.Location;
                        report.AppendFormat("Add '// Stryker test apart once' before mutant {0} at {1}.",
                            results.ConflictingMutant.Id, results.ConflictingMutant.Location);
                        report.AppendLine();
                        report.AppendFormat("Diagnosed mutant {0} was killed by these test(s): ", results.DiagnosedMutant.Id);
                        report.AppendLine();
                        report.AppendJoin(',', results.RunResults[2].killingTests);
                    }
                }
                else if (results.RunResults[0].status == MutantStatus.NoCoverage)
                {
                    //TODO handle situation where a mutant is killed while not having coverage
                }
                else
                {
                    _logger.LogWarning("Stryker.NET is not able to produce a diagnostic for false negative.");
                }
            }

            return report.ToString();
        }
        private void TestMutants(IReporter reporters, IEnumerable<IReadOnlyMutant> mutantsNotRun, IReadOnlyProjectComponent readOnlyInputComponent)
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

        public void SetupLogging(ILoggerFactory loggerFactory)
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
