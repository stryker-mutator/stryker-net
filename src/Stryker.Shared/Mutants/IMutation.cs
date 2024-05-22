using Microsoft.CodeAnalysis;
using Stryker.Shared.Mutators;

namespace Stryker.Shared.Mutants;
public interface IMutation
{
    SyntaxNode OriginalNode { get; set; }
    SyntaxNode ReplacementNode { get; set; }
    string DisplayName { get; set; }
    Mutator Type { get; set; }
    string Description { get; set; }
}
