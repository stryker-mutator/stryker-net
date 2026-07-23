using Stryker.Abstractions.Options;

namespace Stryker.Core.MutationTest;

public interface IMutationProcess
{
    /// <summary>
    /// Mutates the syntax trees and stores the mutated trees in memory.
    /// This phase does not write to disk and can safely run in parallel with initial tests.
    /// </summary>
    void Mutate(MutationTestInput input, IStrykerOptions options);

    /// <summary>
    /// Compiles the mutated syntax trees and writes the mutated assembly to disk.
    /// This phase must run after initial tests complete to avoid file conflicts.
    /// </summary>
    void Compile(MutationTestInput input);

    void FilterMutants(MutationTestInput input);
}
