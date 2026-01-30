using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Stryker.Abstractions;
using Stryker.Abstractions.Exceptions;
using Stryker.Abstractions.Options;
using Stryker.Abstractions.ProjectComponents;
using Stryker.Configuration.Options;
using Stryker.Core.Initialisation;
using Stryker.Core.MutationTest;
using Stryker.Core.ProjectComponents;
using Stryker.Core.ProjectComponents.TestProjects;
using Stryker.Core.Reporters;

namespace Stryker.Core;

public interface IStrykerRunner
{
    Task<StrykerRunResult> RunMutationTestAsync(IStrykerInputs inputs);
}

public class StrykerRunner : IStrykerRunner
{
    private IEnumerable<IMutationTestProcess> _mutationTestProcesses;
    private readonly ILogger _logger;
    private readonly IReporterFactory _reporterFactory;
    private readonly IProjectOrchestrator _projectOrchestrator;

    public StrykerRunner(
        IReporterFactory reporterFactory,
        IProjectOrchestrator projectOrchestrator,
        ILogger<StrykerRunner> logger)
    {
        _reporterFactory = reporterFactory ?? throw new ArgumentNullException(nameof(reporterFactory));
        _projectOrchestrator = projectOrchestrator ?? throw new ArgumentNullException(nameof(projectOrchestrator));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _mutationTestProcesses = new List<IMutationTestProcess>();
    }

    /// <summary>
    /// Starts a mutation test run
    /// </summary>
    /// <param name="inputs">user options</param>
    /// <exception cref="InputException">For managed exceptions</exception>
    public async Task<StrykerRunResult> RunMutationTestAsync(IStrykerInputs inputs)
    {
        var stopwatch = new Stopwatch();
        stopwatch.Start();

        var options = inputs.ValidateAll();
        _logger.LogDebug("Stryker started with options: {@Options}", options);

        var reporters = _reporterFactory.Create(options);

        try
        {
            // Mutate
            _mutationTestProcesses = _projectOrchestrator.MutateProjects(options, reporters).ToList();

            var rootComponent = AddRootFolderIfMultiProject(_mutationTestProcesses.Select(x => x.Input.SourceProjectInfo.ProjectContents).ToList(), options);
            var combinedTestProjectsInfo = _mutationTestProcesses.Select(mtp => mtp.Input.TestProjectsInfo).Aggregate((a, b) => (TestProjectsInfo)a + (TestProjectsInfo)b);

            _logger.LogInformation("{MutantsCount} mutants created", rootComponent.Mutants.Count());

            AnalyzeCoverage(options);

            // Filter
            foreach (var project in _mutationTestProcesses)
            {
                project.FilterMutants();
            }

            // Report
            reporters.OnMutantsCreated(rootComponent, combinedTestProjectsInfo);

            var allMutants = rootComponent.Mutants.ToList();
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

                reporters.OnAllMutantsTested(rootComponent, combinedTestProjectsInfo);
                _projectOrchestrator.Dispose();
                return new StrykerRunResult(options, rootComponent.GetMutationScore());
            }

            // Report
            reporters.OnStartMutantTestRun(mutantsNotRun);

            // Test
            foreach (var project in _mutationTestProcesses)
            {
                await project.TestAsync(project.Input.SourceProjectInfo.ProjectContents.Mutants.Where(x => x.ResultStatus == MutantStatus.Pending).ToList()).ConfigureAwait(false);
            }
            // dispose and stop runners
            _projectOrchestrator.Dispose();

            // Restore assemblies
            foreach (var project in _mutationTestProcesses)
            {
                project.Restore();
            }

            reporters.OnAllMutantsTested(rootComponent, combinedTestProjectsInfo);

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
            _logger.LogInformation("Time Elapsed {duration}", stopwatch.Elapsed);
        }
    }

    private void AnalyzeCoverage(IStrykerOptions options)
    {
        if (options.OptimizationMode.HasFlag(OptimizationModes.SkipUncoveredMutants) || options.OptimizationMode.HasFlag(OptimizationModes.CoverageBasedTest))
        {
            _logger.LogInformation("Capture mutant coverage using '{OptimizationMode}' mode.", options.OptimizationMode);

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
    private IReadOnlyProjectComponent AddRootFolderIfMultiProject(IEnumerable<IReadOnlyProjectComponent> projectComponents, IStrykerOptions options)
    {
        if (!projectComponents.Any())
        {
            throw new NoTestProjectsException();
        }

        if (projectComponents.Count() > 1)
        {
            var rootComponent = new Solution
            {
                FullPath = options.ProjectPath // in case of a solution run the basePath will be where the solution file is
            };
            rootComponent.AddRange(projectComponents.Cast<IProjectComponent>());
            return rootComponent;
        }

        return projectComponents.FirstOrDefault();
    }
}
