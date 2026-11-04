using Stryker.Abstractions.Options;

namespace Stryker.Core.MutationTest;

public interface IMutationProcess
{
    void Mutate(MutationTestInput input, IStrykerOptions options);

    void FilterMutants(MutationTestInput input);

    /// <summary>Undoes what the process added to the test outputs besides the source assembly (restored by Core). No-op by default.</summary>
    void Restore(MutationTestInput input)
    {
    }
}
