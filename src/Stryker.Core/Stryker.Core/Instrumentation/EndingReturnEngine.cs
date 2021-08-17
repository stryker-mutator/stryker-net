using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Stryker.Core.Helpers;

namespace Stryker.Core.Instrumentation
{
    /// <summary>
    /// Injects 'return default(...)' statement at the end of a method
    /// </summary>
    internal class EndingReturnEngine: BaseEngine<BaseMethodDeclarationSyntax>
    {
        public EndingReturnEngine(string markerId) : base(markerId)
        {
        }

        public BaseMethodDeclarationSyntax InjectReturn(BaseMethodDeclarationSyntax method)
        {
            var newBody = EngineHelpers.InjectReturn(method.Body, method.ReturnType(), method.Modifiers);

            return newBody == null ? method : method.WithBody(newBody).WithAdditionalAnnotations(Marker);
        }

        protected override SyntaxNode Revert(BaseMethodDeclarationSyntax node) => node.WithBody(EngineHelpers.RemoveReturn(node.Body)).WithoutAnnotations(Marker);
    }
}
