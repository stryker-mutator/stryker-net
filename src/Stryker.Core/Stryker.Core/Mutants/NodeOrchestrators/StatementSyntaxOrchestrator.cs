using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Stryker.Core.Mutants.NodeOrchestrators
{
    internal class StatementSyntaxOrchestrator : StatementSpecificOrchestrator<StatementSyntax>
    {
        public StatementSyntaxOrchestrator(MutantOrchestrator mutantOrchestrator) : base(mutantOrchestrator)
        {
        }
    }
}
