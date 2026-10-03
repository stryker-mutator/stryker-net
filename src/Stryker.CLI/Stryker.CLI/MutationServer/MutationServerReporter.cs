using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Channels;
using System.Threading.Tasks;
using Stryker.Abstractions;
using Stryker.Abstractions.ProjectComponents;
using Stryker.Abstractions.Reporting;

namespace Stryker.CLI.MutationServer;

internal sealed class MutationServerReporter : IReporter
{
    internal const int ProgressBufferCapacity = 256;
    private readonly string _basePath;
    private readonly Func<MutationTestResult, Task> _reportProgress;
    private readonly Func<IReadOnlyFileLeaf, IReadOnlyMutant, bool> _mutantSelection;
    private readonly Func<IReadOnlyFileLeaf, bool> _fileSelection;
    private readonly CancellationToken _cancellationToken;
    private readonly ConcurrentDictionary<int, MutantContext> _mutantContexts = new();
    private readonly ConcurrentDictionary<int, byte> _reportedMutants = new();
    private readonly Channel<MutationTestResult> _progressChannel;
    private readonly Task _progressTask = Task.CompletedTask;

    public MutationServerReporter(
        string basePath,
        Func<MutationTestResult, Task> reportProgress = null,
        Func<IReadOnlyFileLeaf, IReadOnlyMutant, bool> mutantSelection = null,
        Func<IReadOnlyFileLeaf, bool> fileSelection = null,
        CancellationToken cancellationToken = default)
    {
        _basePath = basePath;
        _reportProgress = reportProgress;
        _mutantSelection = mutantSelection;
        _fileSelection = fileSelection;
        _cancellationToken = cancellationToken;

        if (_reportProgress is not null)
        {
            _progressChannel = Channel.CreateBounded<MutationTestResult>(
                new BoundedChannelOptions(ProgressBufferCapacity)
                {
                    FullMode = BoundedChannelFullMode.Wait,
                    SingleReader = true
                });
            _progressTask = SendProgressAsync();
        }
    }

    public DiscoverResult DiscoverResult { get; private set; } = new();

    public void OnMutantsCreated(IReadOnlyProjectComponent reportComponent, ITestProjectsInfo testProjectsInfo)
    {
        var files = new Dictionary<string, DiscoveredFile>();
        foreach (var file in reportComponent.GetAllFiles())
        {
            var fileName = MutationServerMutantIdentity.GetFileName(_basePath, file);
            var selectedMutants = file.Mutants
                .Where(mutant => _mutantSelection?.Invoke(file, mutant) != false)
                .ToArray();
            var mutants = selectedMutants
                .Where(mutant => mutant.ResultStatus != MutantStatus.Ignored)
                .Select(mutant => ToDiscoveredMutant(file, mutant))
                .ToArray();

            foreach (var mutant in selectedMutants)
            {
                _mutantContexts[mutant.Id] = new MutantContext(file, fileName);
            }

            if (mutants.Length > 0 || _fileSelection?.Invoke(file) == true)
            {
                files[fileName] = new DiscoveredFile { Mutants = mutants };
            }

            if (_reportProgress is not null)
            {
                foreach (var mutant in selectedMutants.Where(mutant => mutant.ResultStatus != MutantStatus.Pending))
                {
                    ReportMutant(mutant, new MutantContext(file, fileName));
                }
            }
        }

        DiscoverResult = new DiscoverResult { Files = files };
    }

    public void OnStartMutantTestRun(IEnumerable<IReadOnlyMutant> mutantsToBeTested)
    {
    }

    public void OnMutantTested(IReadOnlyMutant result)
    {
        if (_reportProgress is null || !_mutantContexts.TryGetValue(result.Id, out var context))
        {
            return;
        }

        ReportMutant(result, context);
    }

    private void ReportMutant(IReadOnlyMutant mutant, MutantContext context)
    {
        if (_cancellationToken.IsCancellationRequested || !_reportedMutants.TryAdd(mutant.Id, 0))
        {
            return;
        }

        var progress = new MutationTestResult
        {
            Files = new Dictionary<string, MutantResultFile>
            {
                [context.FileName] = new() { Mutants = [ToMutantResult(context.File, mutant)] }
            }
        };

        if (!_progressChannel.Writer.TryWrite(progress))
        {
            try
            {
                _progressChannel.Writer.WriteAsync(progress, _cancellationToken)
                    .AsTask()
                    .GetAwaiter()
                    .GetResult();
            }
            catch (OperationCanceledException) when (_cancellationToken.IsCancellationRequested)
            {
            }
        }
    }

    public void OnAllMutantsTested(IReadOnlyProjectComponent reportComponent, ITestProjectsInfo testProjectsInfo)
    {
        if (_reportProgress is null)
        {
            return;
        }

        foreach (var file in reportComponent.GetAllFiles())
        {
            var fileName = MutationServerMutantIdentity.GetFileName(_basePath, file);
            foreach (var mutant in file.Mutants.Where(mutant =>
                         _mutantSelection?.Invoke(file, mutant) != false))
            {
                ReportMutant(mutant, new MutantContext(file, fileName));
            }
        }
    }

    public async Task CompleteAsync()
    {
        if (_progressChannel is null)
        {
            return;
        }

        _progressChannel.Writer.TryComplete();
        await _progressTask.ConfigureAwait(false);
    }

    public Exception ProgressException { get; private set; }
    internal int QueuedProgressCount => _progressChannel?.Reader.Count ?? 0;

    private async Task SendProgressAsync()
    {
        await foreach (var progress in _progressChannel.Reader.ReadAllAsync())
        {
            if (_cancellationToken.IsCancellationRequested)
            {
                continue;
            }

            if (ProgressException is not null)
            {
                continue;
            }

            try
            {
                await _reportProgress(progress)
                    .WaitAsync(_cancellationToken)
                    .ConfigureAwait(false);
            }
            catch (OperationCanceledException) when (_cancellationToken.IsCancellationRequested)
            {
                return;
            }
            catch (Exception exception)
            {
                ProgressException = exception;
            }
        }
    }

    private DiscoveredMutant ToDiscoveredMutant(IReadOnlyFileLeaf file, IReadOnlyMutant mutant)
        => new()
        {
            Id = MutationServerMutantIdentity.GetId(_basePath, file, mutant),
            Location = MutationServerMutantIdentity.GetLocation(mutant),
            Description = mutant.Mutation.Description,
            MutatorName = mutant.Mutation.DisplayName,
            Replacement = mutant.Mutation.ReplacementNode?.ToString()
        };

    private MutantResult ToMutantResult(IReadOnlyFileLeaf file, IReadOnlyMutant mutant)
        => new()
        {
            Id = MutationServerMutantIdentity.GetId(_basePath, file, mutant),
            Location = MutationServerMutantIdentity.GetLocation(mutant),
            Description = mutant.Mutation.Description,
            MutatorName = mutant.Mutation.DisplayName,
            Replacement = mutant.Mutation.ReplacementNode?.ToString(),
            CoveredBy = GetTestIds(mutant.CoveringTests.GetIdentifiers()),
            KilledBy = GetTestIds(mutant.KillingTests.GetIdentifiers()),
            Static = mutant.IsStaticValue,
            Status = mutant.ResultStatus == MutantStatus.Pending
                ? nameof(MutantStatus.RuntimeError)
                : mutant.ResultStatus.ToString(),
            StatusReason = mutant.ResultStatus == MutantStatus.Pending
                ? mutant.ResultStatusReason ?? "Stryker did not complete this mutant test."
                : mutant.ResultStatusReason
        };

    private static IReadOnlyCollection<string> GetTestIds(IEnumerable<string> testIds)
    {
        var identifiers = testIds?.ToArray();
        return identifiers is { Length: > 0 } ? identifiers : null;
    }

    private sealed record MutantContext(IReadOnlyFileLeaf File, string FileName);
}
