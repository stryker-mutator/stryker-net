using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO.Abstractions;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Stryker.Abstractions;
using Stryker.Abstractions.Exceptions;
using Stryker.Abstractions.Options;
using Stryker.Abstractions.ProjectComponents;
using Stryker.Abstractions.Reporting;
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
    Task<StrykerRunResult> RunMutationTestAsync(
        IStrykerInputs inputs,
        IReporter reporter,
        Func<IReadOnlyFileLeaf, IReadOnlyMutant, bool> mutantSelection);
    Task<StrykerRunResult> RunMutationTestAsync(
        IStrykerInputs inputs,
        IReporter reporter,
        Func<IReadOnlyFileLeaf, IReadOnlyMutant, bool> mutantSelection,
        CancellationToken cancellationToken);
    Task DiscoverMutantsAsync(
        IStrykerInputs inputs,
        IReporter reporter,
        Func<IReadOnlyFileLeaf, IReadOnlyMutant, bool> mutantSelection);
    Task DiscoverMutantsAsync(
        IStrykerInputs inputs,
        IReporter reporter,
        Func<IReadOnlyFileLeaf, IReadOnlyMutant, bool> mutantSelection,
        CancellationToken cancellationToken);
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
        => await RunMutationTestAsync(inputs, null, null);

    /// <summary>
    /// Starts a mutation test run with a custom reporter and optional mutant selection.
    /// </summary>
    /// <param name="inputs">User options.</param>
    /// <param name="reporter">Reporter that receives mutation test events.</param>
    /// <param name="mutantSelection">Optional request-specific mutant selection.</param>
    /// <returns>The mutation test result.</returns>
    public async Task<StrykerRunResult> RunMutationTestAsync(
        IStrykerInputs inputs,
        IReporter reporter,
        Func<IReadOnlyFileLeaf, IReadOnlyMutant, bool> mutantSelection)
        => await RunMutationTestAsync(inputs, reporter, mutantSelection, CancellationToken.None);

    public async Task<StrykerRunResult> RunMutationTestAsync(
        IStrykerInputs inputs,
        IReporter reporter,
        Func<IReadOnlyFileLeaf, IReadOnlyMutant, bool> mutantSelection,
        CancellationToken cancellationToken)
    {
        var stopwatch = new Stopwatch();
        stopwatch.Start();
        var restoredProjects = new HashSet<IMutationTestProcess>();

        cancellationToken.ThrowIfCancellationRequested();
        var options = inputs.ValidateAll();
        _logger.LogDebug("Stryker started with options: {@Options}", options);

        var reporters = reporter ?? _reporterFactory.Create(options);

        try
        {
            var preparedMutationTest = await PrepareMutationTestAsync(
                options,
                reporters,
                mutantSelection,
                analyzeCoverage: true,
                cancellationToken);
            var rootComponent = preparedMutationTest.RootComponent;
            var combinedTestProjectsInfo = preparedMutationTest.TestProjectsInfo;

            var allMutants = rootComponent.Mutants.ToList();
            var mutantsNotRun = rootComponent.NotRunMutants().ToList();

            if (!mutantsNotRun.Any())
            {
                if (allMutants.Any(x => x.ResultStatus == MutantStatus.Ignored))
                {
                    _logger.LogWarning(
                        "It looks like all mutants with tests were ignored. Try a re-run with less ignoring!");
                }

                if (allMutants.Any(x => x.ResultStatus == MutantStatus.NoCoverage))
                {
                    _logger.LogWarning(
                        "It looks like all non-ignored mutants are not covered by a test. Go add some tests!");
                }

                if (allMutants.Any(x => x.ResultStatus == MutantStatus.CompileError))
                {
                    _logger.LogWarning(
                        "It looks like all mutants resulted in compile errors. Mutants sure are strange!");
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
                cancellationToken.ThrowIfCancellationRequested();
                var mutants = project.Input.SourceProjectInfo.ProjectContents.Mutants
                    .Where(x => x.ResultStatus == MutantStatus.Pending)
                    .ToList();
                if (cancellationToken.CanBeCanceled)
                {
                    await project.TestAsync(mutants, cancellationToken).ConfigureAwait(false);
                }
                else
                {
                    await project.TestAsync(mutants).ConfigureAwait(false);
                }
            }

            // dispose and stop runners
            _projectOrchestrator.Dispose();

            // Restore assemblies
            RestoreProjects(restoredProjects);

            reporters.OnAllMutantsTested(rootComponent, combinedTestProjectsInfo);

            return new StrykerRunResult(options, rootComponent.GetMutationScore());
        }
#if !DEBUG
        catch (AggregateException ex) when  (ex.InnerException is CompilationException)
        {
            _logger.LogCritical("Compilation failed: {Message}.", ex.Message);
            if (!options.DiagMode)
            {
                _logger.LogCritical("Run with --diag to get more diagnotic information.");
            }
            return new StrykerRunResult(options, 0);
        }
        catch (Exception ex) when (ex is not InputException && ex is not CompilationException)
        // let the exception be caught by the debugger when in debug
        {
            _logger.LogError(ex, "An error occurred during the mutation test run ");
            throw;
        }

#endif
        finally
        {
            // Dispose runners to kill all spawned test processes, even on cancellation or error
            _projectOrchestrator.Dispose();
            RestoreProjects(restoredProjects);

            stopwatch.Stop();
            _logger.LogInformation("Time Elapsed {duration}", stopwatch.Elapsed);
        }
    }

    private void RestoreProjects(ISet<IMutationTestProcess> restoredProjects)
    {
        foreach (var project in _mutationTestProcesses.Where(restoredProjects.Add))
        {
            project.Restore();
        }
    }

    /// <summary>
    /// Discovers mutants without executing mutation tests.
    /// </summary>
    /// <param name="inputs">User options.</param>
    /// <param name="reporter">Reporter that receives the discovered mutants.</param>
    /// <param name="mutantSelection">Optional request-specific mutant selection.</param>
    public async Task DiscoverMutantsAsync(
        IStrykerInputs inputs,
        IReporter reporter,
        Func<IReadOnlyFileLeaf, IReadOnlyMutant, bool> mutantSelection)
        => await DiscoverMutantsAsync(inputs, reporter, mutantSelection, CancellationToken.None);

    public async Task DiscoverMutantsAsync(
        IStrykerInputs inputs,
        IReporter reporter,
        Func<IReadOnlyFileLeaf, IReadOnlyMutant, bool> mutantSelection,
        CancellationToken cancellationToken)
    {
        var stopwatch = Stopwatch.StartNew();
        cancellationToken.ThrowIfCancellationRequested();
        var options = inputs.ValidateAll();
        _logger.LogDebug("Stryker mutant discovery started with options: {@Options}", options);
        var reporters = reporter ?? _reporterFactory.Create(options);

        try
        {
            await PrepareMutationTestAsync(
                options,
                reporters,
                mutantSelection,
                analyzeCoverage: false,
                cancellationToken);
        }
        finally
        {
            _projectOrchestrator.Dispose();
            foreach (var project in _mutationTestProcesses)
            {
                project.Restore();
            }
            stopwatch.Stop();
            _logger.LogInformation("Time Elapsed {duration}", stopwatch.Elapsed);
        }
    }

    private async Task<PreparedMutationTest> PrepareMutationTestAsync(
        IStrykerOptions options,
        IReporter reporter,
        Func<IReadOnlyFileLeaf, IReadOnlyMutant, bool> mutantSelection,
        bool analyzeCoverage,
        CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        _mutationTestProcesses = (cancellationToken.CanBeCanceled
            ? await _projectOrchestrator.MutateProjectsAsync(
                options,
                reporter,
                null,
                cancellationToken)
            : await _projectOrchestrator.MutateProjectsAsync(options, reporter)).ToList();
        cancellationToken.ThrowIfCancellationRequested();

        var rootComponent = AddRootFolderIfMultiProject(
            _mutationTestProcesses.Select(x => x.Input.SourceProjectInfo.ProjectContents).ToList(), options);
        var combinedTestProjectsInfo = _mutationTestProcesses
            .Select(mtp => mtp.Input.SourceProjectInfo.TestProjectsInfo)
            .Aggregate(
                new TestProjectsInfo(new FileSystem(), _logger),
                (testProjects, current) => testProjects + current);

        _logger.LogInformation("{MutantsCount} mutants created", rootComponent.Mutants.Count());

        if (analyzeCoverage)
        {
            AnalyzeCoverage(options, cancellationToken);
        }

        foreach (var project in _mutationTestProcesses)
        {
            cancellationToken.ThrowIfCancellationRequested();
            project.FilterMutants();
        }

        ApplyMutantSelection(rootComponent, mutantSelection);
        reporter.OnMutantsCreated(rootComponent, combinedTestProjectsInfo);

        return new PreparedMutationTest(rootComponent, combinedTestProjectsInfo);
    }

    private static void ApplyMutantSelection(
        IReadOnlyProjectComponent rootComponent,
        Func<IReadOnlyFileLeaf, IReadOnlyMutant, bool> mutantSelection)
    {
        if (mutantSelection is null)
        {
            return;
        }

        foreach (var file in rootComponent.GetAllFiles())
        {
            foreach (var mutant in file.Mutants.Where(mutant =>
                         mutant.ResultStatus == MutantStatus.Pending && !mutantSelection(file, mutant)))
            {
                mutant.ResultStatus = MutantStatus.Ignored;
                mutant.ResultStatusReason = "Removed by mutation server request";
            }
        }
    }

    private void AnalyzeCoverage(IStrykerOptions options, CancellationToken cancellationToken)
    {
        if (!options.OptimizationMode.HasFlag(OptimizationModes.SkipUncoveredMutants) &&
            !options.OptimizationMode.HasFlag(OptimizationModes.CoverageBasedTest))
        {
            return;
        }

        _logger.LogInformation("Capture mutant coverage using '{OptimizationMode}' mode.", options.OptimizationMode);

        foreach (var project in _mutationTestProcesses)
        {
            cancellationToken.ThrowIfCancellationRequested();
            project.GetCoverage();
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

        return projectComponents.First();
    }

    private sealed class PreparedMutationTest
    {
        public PreparedMutationTest(
            IReadOnlyProjectComponent rootComponent,
            ITestProjectsInfo testProjectsInfo)
        {
            RootComponent = rootComponent;
            TestProjectsInfo = testProjectsInfo;
        }

        public IReadOnlyProjectComponent RootComponent { get; }
        public ITestProjectsInfo TestProjectsInfo { get; }
    }
}
