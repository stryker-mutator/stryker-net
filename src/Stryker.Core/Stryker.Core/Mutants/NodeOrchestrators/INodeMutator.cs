using Microsoft.CodeAnalysis;
using Stryker.Core.Helpers;
using static FSharp.Compiler.SyntaxTree;

namespace Stryker.Core.Mutants.NodeOrchestrators
{
    interface INodeMutator : ITypeHandler<SyntaxNode>
    {
        SyntaxNode Mutate(SyntaxNode node, MutationContext context);
    }

    interface IFsharpNodeMutator : ITypeHandler<SynExpr>
    {
        SynExpr Mutate(SynExpr node, MutationContext context);
    }
}
