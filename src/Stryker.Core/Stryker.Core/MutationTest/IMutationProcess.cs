using Stryker.Abstractions.Options;

namespace Stryker.Core.MutationTest;

public interface IMutationProcess
{
    void Mutate(MutationTestInput input, IStrykerOptions options);

    void FilterMutants(MutationTestInput input);
}
