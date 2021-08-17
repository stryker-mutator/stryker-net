using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Stryker.Core.Instrumentation
{
    /// <summary>
    /// Injects 'return default(...)' statement at the end of a method
    /// </summary>
    internal class EndingReturnEngineToLocalFunction: BaseEngine<LocalFunctionStatementSyntax>
    {
        public EndingReturnEngineToLocalFunction(string markerId) : base(markerId)
        {
        }

        public LocalFunctionStatementSyntax InjectReturn(LocalFunctionStatementSyntax method)
        {
            var newBody = EngineHelpers.InjectReturn(method.Body, method.ReturnType, method.Modifiers);

            return newBody == null ? method : method.WithBody(newBody).WithAdditionalAnnotations(Marker);
        }

        protected override SyntaxNode Revert(LocalFunctionStatementSyntax node) => node.WithBody(EngineHelpers.RemoveReturn(node.Body)).WithoutAnnotations(Marker);
    }
}
