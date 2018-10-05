using Microsoft.CodeAnalysis.CSharp.Syntax;
using Moq;
using Stryker.Core.Mutants.MutationHandlers;
using Xunit;

namespace Stryker.Core.UnitTest.Mutants.MutationHandlers
{
    public class MutationIfPlacerTests
    {
        [Fact]
        public void ShouldPlaceMutationInsideConditionalExpression()
        {
            var statement = MutationHandlerTestHelpers.GetNodeOfType<WhileStatementSyntax>("while(i > 10) {}");
            var mutation = MutationHandlerTestHelpers.GetNodeOfType<WhileStatementSyntax>("while(i < 10) {}");

            var handlerMock = new Mock<MutationHandler>(MockBehavior.Strict);
            var target = new MutationIfPlacer().SetSuccessor(handlerMock.Object);

            var result = target.HandleInsertMutation(statement, statement, 1);

            result.ToFullString().ShouldBeSemantically(@"
if(System.Environment.GetEnvironmentVariable(""ActiveMutation"") == ""1"") {
    while(i < 10) {}
} else {
    while(i > 10) {}
}");

        }
    }
}
