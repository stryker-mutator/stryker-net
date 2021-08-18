using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Stryker.Core.Instrumentation
{
    /// <summary>
    /// Injects 'return default(...)' statement at the end of a method
    /// </summary>
    internal class EndingReturnEngine: BaseEngine<BlockSyntax>
    {
        public EndingReturnEngine(string markerId) : base(markerId)
        {
        }

        public BlockSyntax InjectReturn(BlockSyntax block, TypeSyntax type, SyntaxTokenList modifiers)
        {
            var newBody = EngineHelpers.InjectReturn(block, type, modifiers);

            return newBody == null ? block : newBody.WithAdditionalAnnotations(Marker);
        }

        protected override SyntaxNode Revert(BlockSyntax node) => EngineHelpers.RemoveReturn(node).WithoutAnnotations(Marker);
    }
}
