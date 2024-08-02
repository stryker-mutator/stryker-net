namespace Stryker.Configuration.MutationTest
{
    public interface IMutationProcess
    {
        void Mutate(MutationTestInput input);

        void FilterMutants(MutationTestInput input);
    }
}
