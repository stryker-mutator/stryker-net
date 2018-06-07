namespace Stryker.Core.Mutants
{
    /// <summary>
    /// This interface should only contain readonly properties to ensure that others than the mutationtest proces cannot modify mutants.
    /// </summary>
    public interface IReadOnlyMutant
    {
        int Id { get; }
        Mutation Mutation { get; }
        MutantStatus ResultStatus { get; }
    }
}