using System;
using Microsoft.CodeAnalysis;
using Stryker.Abstractions.Mutators;

namespace Stryker.Abstractions.Mutants;

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

    public string? ReplacementText { get; set; }

    public FileLinePositionSpan? OriginalLocation { get; set; }
}
