using Microsoft.CodeAnalysis;

namespace Stryker.Core.Mutants;

/// <summary>
/// Used to keep track of mutant info. For example during the rollback process.
/// </summary>
public class MutantInfo
{
    public int? Id { get; set; }
    public string Engine { get; set; }
    public string Type { get; set; }
    public SyntaxNode Node { get; set; }
}
