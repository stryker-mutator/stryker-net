namespace Stryker.Core.Mutants;
using Microsoft.CodeAnalysis;

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
