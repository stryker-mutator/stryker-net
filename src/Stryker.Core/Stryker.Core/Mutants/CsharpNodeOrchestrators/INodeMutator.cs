using Microsoft.CodeAnalysis;
using Stryker.Core.Helpers;
using static FSharp.Compiler.SyntaxTree;

namespace Stryker.Core.Mutants.CsharpNodeOrchestrators
{
    interface INodeMutator : ITypeHandler<SyntaxNode>
    {
        SyntaxNode Mutate(SyntaxNode node, MutationContext context);
    }
}
