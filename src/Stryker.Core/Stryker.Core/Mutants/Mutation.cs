using Microsoft.CodeAnalysis;
using Stryker.Shared.Mutants;
using Stryker.Shared.Mutators;

namespace Stryker.Core.Mutants;

/// <summary>
/// Represents a single mutation on code level
/// </summary>
public class Mutation : IMutation
{
    public SyntaxNode OriginalNode { get; set; }
    public SyntaxNode ReplacementNode { get; set; }
    public string DisplayName { get; set; }
    public Mutator Type { get; set; }
    public string Description { get; set; }
}
