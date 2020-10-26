namespace Stryker.Core.MutationTest
{
    interface IMutationProcess
    {
        public void Mutate();

        void FilterMutants();
    }
}
