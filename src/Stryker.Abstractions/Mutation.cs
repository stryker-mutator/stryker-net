using Microsoft.CodeAnalysis;

namespace Stryker.Abstractions;

/// <summary>
/// Represents a single mutation on code level
/// </summary>
public class Mutation
{
    public SyntaxNode OriginalNode { get; set; }
    public SyntaxNode ReplacementNode { get; set; }
    public string DisplayName { get; set; }
    public Mutator Type { get; set; }
    public string Description { get; set; }

    /// <summary>1-based source range of the original code.</summary>
    public MutationLocation Location { get; set; }
    public string OriginalText { get; set; }
    public string ReplacementText { get; set; }
}
