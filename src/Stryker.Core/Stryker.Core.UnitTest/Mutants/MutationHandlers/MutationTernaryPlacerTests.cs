using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Moq;
using Shouldly;
using System.Linq;
using Xunit;

namespace Stryker.Core.UnitTest.Mutants.MutationHandlers
{
    public class MutationTernaryPlacerTests
    {
        //[Fact]
        //public void OnWrongType_ShouldCallSuccessor()
        //{
        //    var statement = MutationHandlerTestHelpers.GetNodeOfType<LocalDeclarationStatementSyntax>("var i = 0;");
        //    var mutation = MutationHandlerTestHelpers.GetNodeOfType<LocalDeclarationStatementSyntax>("var i = 100;");

        //    var handlerMock = new Mock<MutationHandler>(MockBehavior.Strict);

        //    var target = new MutationTernaryPlacer().SetSuccessor(handlerMock.Object);

        //    target.HandleInsertMutation(statement, mutation, 2);

        //    // handlerMock will throw exception when called
        //}

        //[Fact]
        //public void ShouldPlaceMutationInsideConditionalExpression()
        //{
        //    var statement = MutationHandlerTestHelpers.GetNodeOfType<LocalDeclarationStatementSyntax>("var i = 0;");

        //    var handlerMock = new Mock<MutationHandler>(MockBehavior.Strict);
        //    var target = new MutationTernaryPlacer().SetSuccessor(handlerMock.Object);

        //    var result = target.HandleInsertMutation(statement, statement, 1);

        //    result.ToFullString().ShouldBeSemantically(@"var i = (System.Environment.GetEnvironmentVariable(""ActiveMutation"") == ""1"") ? 0 : 100;");
        //}
    }
}
