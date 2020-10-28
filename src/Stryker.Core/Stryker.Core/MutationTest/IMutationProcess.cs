namespace Stryker.Core.MutationTest
{
    interface IMutationProcess
    {
        void Mutate();

        void FilterMutants();
    }
}
