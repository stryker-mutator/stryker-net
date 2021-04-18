using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
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
        StrykerRunResult RunMutationTest(StrykerOptions options, IEnumerable<LogMessage> initialLogMessages = null);
    }

    public class StrykerRunner : IStrykerRunner
    {
        private readonly IProjectOrchestrator _projectOrchestrator;
        private IEnumerable<IMutationTestProcess> _mutationTestProcesses;
        private ILogger _logger;
        private readonly IReporterFactory _reporterFactory;

        public StrykerRunner(IProjectOrchestrator projectOrchestrator = null, IEnumerable<IMutationTestProcess> mutationTestProcesses = null, IReporterFactory reporterFactory = null)
        {
            _projectOrchestrator = projectOrchestrator ?? new ProjectOrchestrator();
            _mutationTestProcesses = mutationTestProcesses ?? new List<IMutationTestProcess>();
            _reporterFactory = reporterFactory ?? new ReporterFactory();
        }

        /// <summary>
        /// Starts a mutation test run
        /// </summary>
        /// <exception cref="StrykerInputException">For managed exceptions</exception>
        /// <param name="options">The user options</param>
        /// <param name="initialLogMessages">
        /// Allows to pass log messages that occured before the mutation test.
        /// The messages will be written to the logger after it was configured.
        /// </param>
        public StrykerRunResult RunMutationTest(StrykerOptions options, IEnumerable<LogMessage> initialLogMessages = null)
        {
            var stopwatch = new Stopwatch();
            stopwatch.Start();

            var reporters = _reporterFactory.Create(options);

            SetupLogging(options, initialLogMessages);

            try
            {
                // Mutate
                _mutationTestProcesses = _projectOrchestrator.MutateProjects(options, reporters).ToList();

                var rootComponent = AddRootFolderIfMultiProject(_mutationTestProcesses.Select(x => x.Input.ProjectInfo.ProjectContents).ToList(), options);

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
                    project.Test(project.Input.ProjectInfo.ProjectContents.Mutants.Where(x => x.ResultStatus == MutantStatus.NotRun).ToList());
                }

                reporters.OnAllMutantsTested(readOnlyInputComponent);

                return new StrykerRunResult(options, readOnlyInputComponent.GetMutationScore());
            }
            catch (Exception ex) when (!(ex is StrykerInputException))
            {
                _logger.LogError(ex, "An error occurred during the mutation test run ");
                throw;
            }
            finally
            {
                // log duration
                stopwatch.Stop();
                _logger.LogInformation("Time Elapsed {0}", stopwatch.Elapsed);
            }
        }

        private void SetupLogging(StrykerOptions options, IEnumerable<LogMessage> initialLogMessages = null)
        {
            // setup logging
            ApplicationLogging.ConfigureLogger(options.LogOptions, initialLogMessages);
            _logger = ApplicationLogging.LoggerFactory.CreateLogger<StrykerRunner>();

            _logger.LogDebug("Stryker started with options: {0}",
                JsonConvert.SerializeObject(options, new StringEnumConverter()));
        }

        private void AnalyseCoverage(StrykerOptions options)
        {
            if (options.Optimizations.HasFlag(OptimizationFlags.SkipUncoveredMutants) || options.Optimizations.HasFlag(OptimizationFlags.CoverageBasedTest))
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
                    FullPath = options.BasePath // in case of a solution run the basepath will be where the solution file is
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
