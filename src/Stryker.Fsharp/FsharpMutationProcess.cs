using Stryker.Abstractions.Options;
using Stryker.Core.MutationTest;

namespace Stryker.Fsharp;

/// <summary>Stub for the F# mutation pipeline; implemented in later tasks.</summary>
public class FsharpMutationProcess : IMutationProcess
{
    public void Mutate(MutationTestInput input, IStrykerOptions options) =>
        throw new NotImplementedException("F# mutation is not implemented yet.");

    public void FilterMutants(MutationTestInput input) =>
        throw new NotImplementedException("F# mutant filtering is not implemented yet.");
}
