namespace Stryker.Abstractions.MutationTest
{
    public interface IMutationProcess
    {
        void Mutate(MutationTestInput input);

        void FilterMutants(MutationTestInput input);
    }
}
