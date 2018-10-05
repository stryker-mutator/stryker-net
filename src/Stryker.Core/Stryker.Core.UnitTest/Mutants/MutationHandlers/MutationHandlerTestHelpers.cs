using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Linq;

namespace Stryker.Core.UnitTest.Mutants.MutationHandlers
{
    public static class MutationHandlerTestHelpers
    {
        public static T GetNodeOfType<T>(string statementString) where T : StatementSyntax
        {
            var node = CSharpSyntaxTree.ParseText(@"class Test { 
    void Method() {"+
        statementString
    + @"}
}").GetRoot();

            return node.DescendantNodes().OfType<T>().First();
        }
    }
}
