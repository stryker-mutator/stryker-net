using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Moq;
using Stryker.Core.Mutants.MutationHandlers;
using System.Linq;
using Xunit;

namespace Stryker.Core.UnitTest.Mutants.MutationHandlers
{
    public class MutationTernaryPlacerTests
    {
        [Fact]
        public void OnWrongType_ShouldCallMutationIfPlacer()
        {
            var node = CSharpSyntaxTree.ParseText(@"class Test { 
void Method() {
var i = 0;
}
}").GetRoot();

            var statements = node.DescendantNodes().OfType<LocalDeclarationStatementSyntax>().First();
            var mutation = CSharpSyntaxTree.ParseText(@"var i = 100;").GetRoot().DescendantNodes().OfType<StatementSyntax>().First();

            var handlerMock = new Mock<MutationHandler>(MockBehavior.Strict);

            var target = new MutationTernaryPlacer().SetSuccessor(handlerMock.Object);

            target.HandleInsertMutation((StatementSyntax)node, (StatementSyntax)mutation, 2);
            
        }
    }
}
