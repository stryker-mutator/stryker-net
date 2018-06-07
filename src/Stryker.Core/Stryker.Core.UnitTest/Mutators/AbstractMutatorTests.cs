using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Moq;
using Shouldly;
using Stryker.Core.Mutants;
using Stryker.Core.Mutators;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using Xunit;

[assembly: InternalsVisibleTo("DynamicProxyGenAssembly2")]
namespace Stryker.Core.UnitTest.Mutators
{
    public class AbstractMutatorTests
    {
        // This class is needed for the tests in this file
        // Using Moq the ExampleMutator will be mocked to test the functionality in the abstract Mutator class
        internal class ExampleMutator : Mutator<BinaryExpressionSyntax>, IMutator
        {
            public override IEnumerable<Mutation> ApplyMutations(BinaryExpressionSyntax node)
            {
                throw new NotImplementedException();
            }
        }

        [Fact]
        public void Mutator_ShouldCallApplyMutations_OnExpectedType()
        {
            // the type BinaryExpressionSyntax should be mutated by the examplemutator
            var originalNode = SyntaxFactory.BinaryExpression(SyntaxKind.AddExpression,
                SyntaxFactory.LiteralExpression(SyntaxKind.NumericLiteralExpression, SyntaxFactory.Literal(1)),
                SyntaxFactory.LiteralExpression(SyntaxKind.NumericLiteralExpression, SyntaxFactory.Literal(8)));

            var mutatorMock = new Mock<ExampleMutator>(MockBehavior.Strict);
            mutatorMock.Setup(x => x.ApplyMutations(It.IsAny<BinaryExpressionSyntax>()))
                .Returns(new[] { new Mutation(), new Mutation() });

            var target = mutatorMock.Object;

            var result = target.Mutate(originalNode);

            result.Count().ShouldBe(2);
            mutatorMock.Verify(x => x.ApplyMutations(It.IsAny<BinaryExpressionSyntax>()), Times.Once);
        }

        [Fact]
        public void Mutator_ShouldNotCallApplyMutations_OnWrongType()
        {
            // the type ReturnStatementSyntax should NOT be mutated
            var originalNode = SyntaxFactory.ReturnStatement(SyntaxFactory.LiteralExpression(SyntaxKind.NumericLiteralExpression, SyntaxFactory.Literal(1)));

            var mutatorMock = new Mock<ExampleMutator>(MockBehavior.Strict);

            var target = mutatorMock.Object;

            var result = target.Mutate(originalNode);

            result.ShouldBeEmpty();
            mutatorMock.VerifyNoOtherCalls();
        }

        [Fact]
        public void Mutator_ShouldNotCallApplyMutations_OnWrongType2()
        {
            // the type AssignmentExpressionSyntax should NOT be mutated
            var originalNode = SyntaxFactory.AssignmentExpression(SyntaxKind.SimpleAssignmentExpression, 
                SyntaxFactory.LiteralExpression(SyntaxKind.NumericLiteralExpression, SyntaxFactory.Literal(100)),
                SyntaxFactory.LiteralExpression(SyntaxKind.NumericLiteralExpression, SyntaxFactory.Literal(5)));

            var mutatorMock = new Mock<ExampleMutator>(MockBehavior.Strict);

            var target = mutatorMock.Object;

            var result = target.Mutate(originalNode);

            result.ShouldBeEmpty();
            mutatorMock.VerifyNoOtherCalls();
        }
    }

    
}
