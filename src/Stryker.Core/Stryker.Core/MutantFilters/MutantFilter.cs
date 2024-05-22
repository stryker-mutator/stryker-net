namespace Stryker.Core.MutantFilters
{
    // Filters are executed in the order they appear in in this enum. If you change this order you change the order or filter execution
    public enum MutantFilter
    {
        Broadcast,

        ExcludeFromCodeCoverage,

        IgnoreMutation,

        IgnoreLinqMutation,

        IgnoreMethod,

        FilePattern,

        Since,

        Baseline,

        IgnoreBlockRemoval,
    }
}
