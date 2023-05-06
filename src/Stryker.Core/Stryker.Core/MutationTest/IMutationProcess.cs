namespace Stryker.Core.MutationTest;

public interface IMutationProcess
{
    void Mutate();

    void FilterMutants();
}
