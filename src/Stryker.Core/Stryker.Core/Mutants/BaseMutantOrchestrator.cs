using System.Collections.Generic;
using System.Collections.ObjectModel;
using Stryker.Abstractions;
using Stryker.Abstractions.Mutants;
using Stryker.Abstractions.Options;
using Stryker.Configuration;

namespace Stryker.Core.Mutants;

public abstract class BaseMutantOrchestrator
{
    public readonly IStrykerOptions Options;
    private readonly IProvideId _idProvider;

    protected BaseMutantOrchestrator(IStrykerOptions options)
    {
        Options = options;
        _idProvider = Options.MutantIdProvider ?? new BasicIdProvider();
    }

    public bool MustInjectCoverageLogic =>
        Options != null && Options.OptimizationMode.HasFlag(OptimizationModes.CoverageBasedTest) &&
        !Options.OptimizationMode.HasFlag(OptimizationModes.CaptureCoveragePerTest);

        public ICollection<IMutant> Mutants { get; set; }

    protected int GetNextId() => _idProvider.NextId();

    /// <summary>
    /// Gets the stored mutants and resets the mutant list to an empty collection
    /// </summary>
    public virtual IReadOnlyCollection<IMutant> GetLatestMutantBatch()
    {
        var tempMutants = Mutants;
        Mutants = new Collection<IMutant>();
        return (IReadOnlyCollection<IMutant>)tempMutants;
    }
}

/// <summary>
/// Mutates abstract syntax trees using mutators and places all mutations inside the abstract syntax tree.
/// Orchestrator: to arrange or manipulate, especially by means of clever or thorough planning or maneuvering.
/// </summary>
/// <typeparam name="T">The type of syntax node to mutate</typeparam>
/// <typeparam name="TY">Associated semantic model if any</typeparam>
public abstract class BaseMutantOrchestrator<T, TY> : BaseMutantOrchestrator
{
    protected BaseMutantOrchestrator(IStrykerOptions input) : base(input)
    {}

    public abstract T Mutate(T input, TY semanticModel);
}
