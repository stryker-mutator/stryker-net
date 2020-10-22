namespace Stryker.Core.MutationTest
{
    interface IMutationTestProcessMethod
    {
        public void Mutate();

        public void FilterMutants();
    }
}
