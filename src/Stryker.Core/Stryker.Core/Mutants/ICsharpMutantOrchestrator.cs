using System.Collections.Generic;
using Microsoft.CodeAnalysis;
using Stryker.Core.Mutators;

namespace Stryker.Core.Mutants;

public interface ICsharpMutantOrchestrator
{
    IEnumerable<IMutator> Mutators { get; }
    MutantPlacer Placer { get; }
    bool MustInjectCoverageLogic { get; }
    ICollection<Mutant> Mutants { get; set; }
    int MutantCount { get; set; }

    /// <summary>
    /// Recursively mutates a single SyntaxNode
    /// </summary>
    /// <param name="input">The current root node</param>
    /// <returns>Mutated node</returns>
    SyntaxNode Mutate(SyntaxNode input, SemanticModel semanticModel);

    /// <summary>
    /// Returns true if the new mutant is a duplicate of a mutant already listed in Mutants.
    /// </summary>
    bool IsMutantDuplicate(Mutant newMutant, Mutation mutation);

    /// <summary>
    /// Gets the stored mutants and resets the mutant list to an empty collection
    /// </summary>
    IReadOnlyCollection<Mutant> GetLatestMutantBatch();
}
