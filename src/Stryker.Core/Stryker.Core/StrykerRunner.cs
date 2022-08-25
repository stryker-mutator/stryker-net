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
            var diagnoseMode = inputs.MutantToDiagnose.SuppliedInput.HasValue; 
            var options = inputs.ValidateAll();
            _logger.LogDebug("Stryker started with options: {@Options}", options);

            if (diagnoseMode)
            {
                _logger.LogInformation("Diagnose mode: Stryker will analyze mutant {mutantId} and suggest remediation action(s) if possible.", inputs.MutantToDiagnose.SuppliedInput);
            }
            // get reporters (none if diagnose mode is enabled)
            var reporters = diagnoseMode ? new BroadcastReporter(Enumerable.Empty<IReporter>()) : _reporterFactory.Create(options);

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
                    reporters.OnStartMutantTestRun(Enumerable.Empty<IReadOnlyMutant>());
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

                if (!diagnoseMode)
                {
                    // normal test run
                    TestMutants(reporters, mutantsNotRun, rootComponent);
                }
                else
                {
                    DiagnoseMutant(inputs);
                }

                return new StrykerRunResult(options, rootComponent.GetMutationScore());
            }
#if !DEBUG
            catch (System.Exception ex) when (ex is not InputException)
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
                _logger.LogInformation("Time Elapsed {duration}", stopwatch.Elapsed);
            }
        }

        private void DiagnoseMutant(IStrykerInputs inputs)
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
                // all tests sessions gave the same result: no problem related to coverage analysis
                report.AppendFormat("Mutant consistently appears as {0}. {1}.", baseLine, baseLine switch
                    {
                        MutantStatus.NoCoverage => "You need to add some tests to fix that",
                        MutantStatus.Survived => $"Modifying the following tests may help you kill this one: {string.Join(", ", results.CoveringTests.Take(20))}",
                        MutantStatus.Ignored => $"Reason seems to be: {results.DiagnosedMutant.ResultStatusReason}",
                        MutantStatus.NotRun => "This should not happen. You can check on Github to see if there is an open issue about this and open one if you want help", 
                        _ => "There is no visible issue"
                    });
            }
            else
            {
                report.AppendLine("Run results are not consistent!");
                switch (baseLine)
                {
                    // false positive
                    case MutantStatus.Survived when results.RunResults[1].status == baseLine:
                    case MutantStatus.NoCoverage:
                    {
                        _logger.LogInformation("Coverage analysis dit not properly capture coverage for this mutant.");
                        var declaringNodeLocation = results.DiagnosedMutant.GetRelativeLocation(string.Empty);
                        report.
                            AppendLine("The coverage for this mutant was not properly determined. You can workaround this problem.").
                            AppendFormat("Add '// Stryker test full once' to {0}.", declaringNodeLocation).
                            AppendLine().Append("It was killed by these test(s): ").
                            AppendJoin(',', results.RunResults[2].killingTests.Except(results.CoveringTests));
                        break;
                    }
                    case MutantStatus.Survived:
                    {
                        var findDeclaringNodeText = results.ConflictingMutant.Location;
                        _logger.LogInformation("There have been an unexpected interaction between two mutations.");
                        report.AppendLine("The test results for this mutant were corrupted by another mutant. As a workaround, you can").
                            AppendFormat("Add '// Stryker test apart once' before mutant {0} at {1}.",
                                results.ConflictingMutant.Id, findDeclaringNodeText).
                            AppendLine().
                            AppendFormat("Diagnosed mutant {0} was killed by these test(s): ", results.DiagnosedMutant.Id).AppendLine().
                            AppendJoin(',', results.RunResults[2].killingTests).AppendLine().
                            AppendFormat("Alternatively, you may add this comment to mutant {0} at {1}.", results.DiagnosedMutant.Id, results.DiagnosedMutant.Location) ;
                        break;
                    }
                    default:
                        _logger.LogWarning("Stryker.NET is not able to produce a diagnostic for false negative.");
                        break;
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
            if (!options.OptimizationMode.HasFlag(OptimizationModes.SkipUncoveredMutants) &&
                !options.OptimizationMode.HasFlag(OptimizationModes.CoverageBasedTest)) return;
            _logger.LogInformation($"Capturing mutant coverage using '{options.OptimizationMode}' mode.");

            foreach (var project in _mutationTestProcesses)
            {
                project.GetCoverage();
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
                    FullPath = options.ProjectPath // in case of a solution run the basePath will be where the solution file is
                };
                rootComponent.AddRange(projectComponents);
                return rootComponent;
            }

            return projectComponents.FirstOrDefault();
        }
    }
}
