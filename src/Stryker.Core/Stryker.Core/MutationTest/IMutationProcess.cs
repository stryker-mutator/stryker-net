namespace Stryker.Core.MutationTest
{
    interface IMutationProcess
    {
        public void Mutate();

        public void FilterMutants();
    }
}
