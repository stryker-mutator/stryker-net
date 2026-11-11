
namespace Stryker.Abstractions;

/// <summary>
/// Represents a single mutation on code level
/// </summary>
public class Mutation
{
    public string DisplayName { get; set; }
    public Mutator Type { get; set; }
    public string Description { get; set; }

    /// <summary>1-based source range of the original code.</summary>
    public virtual MutationLocation Location { get; set; }
    public virtual string OriginalText { get; set; }
    public virtual string ReplacementText { get; set; }
}
