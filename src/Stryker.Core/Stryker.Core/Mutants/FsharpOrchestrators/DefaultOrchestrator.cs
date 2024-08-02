namespace Stryker.Configuration.Mutants.FsharpOrchestrators
{
    public class DefaultOrchestrator<T> : IFsharpTypeHandler<T>
    {
        public virtual T Mutate(T input, FsharpMutantOrchestrator iterator)
        {
            return input;
        }
    }
}
