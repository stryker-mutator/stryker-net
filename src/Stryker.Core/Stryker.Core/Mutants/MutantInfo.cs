using Microsoft.CodeAnalysis;

namespace Stryker.Core.Mutants
{
    public class MutantInfo
    {
        public int? Id { get; set; }
        public string Engine { get; set; }
        public string Type { get; set; }
        public SyntaxNode Node { get; set; }
    }
}
