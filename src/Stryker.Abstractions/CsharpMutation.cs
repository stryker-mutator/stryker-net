using Microsoft.CodeAnalysis;

namespace Stryker.Abstractions;

/// <summary>
/// A mutation on C# code, carrying the Roslyn nodes involved. Location and texts are derived lazily from
/// the nodes on first access (they are only needed by filters and reporters), unless set explicitly.
/// </summary>
public class CsharpMutation : Mutation
{
    private MutationLocation _location;
    private string _originalText;
    private string _replacementText;

    public SyntaxNode OriginalNode { get; set; }
    public SyntaxNode ReplacementNode { get; set; }

    public override MutationLocation Location
    {
        get => _location ??= OriginalNode == null ? null : MutationLocation.FromRoslyn(OriginalNode);
        set => _location = value;
    }

    public override string OriginalText
    {
        get => _originalText ??= OriginalNode?.ToString();
        set => _originalText = value;
    }

    public override string ReplacementText
    {
        get => _replacementText ??= ReplacementNode?.ToString();
        set => _replacementText = value;
    }
}
