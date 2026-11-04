using Microsoft.CodeAnalysis;

namespace Stryker.Abstractions;

/// <summary>
/// A mutation on C# code, carrying the Roslyn nodes involved.
/// </summary>
public class CsharpMutation : Mutation
{
    public SyntaxNode OriginalNode { get; set; }
    public SyntaxNode ReplacementNode { get; set; }
}
