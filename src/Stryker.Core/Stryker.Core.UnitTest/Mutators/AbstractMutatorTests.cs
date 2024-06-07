using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Shouldly;
using Stryker.Core.Mutants;
using Stryker.Core.Mutators;
using Stryker.Core.Options;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Xunit;

[assembly: InternalsVisibleTo("DynamicProxyGenAssembly2")]
namespace Stryker.Core.UnitTest.Mutators
{
    public class AbstractMutatorTests : TestBase
    {
        // This class is needed for the tests in this file
        // Using Moq the ExampleMutator will be mocked to test the functionality in the abstract Mutator class
        internal class ExampleMutator : MutatorBase<BinaryExpressionSyntax>, IMutator
        {
            public override MutationLevel MutationLevel { get; } = MutationLevel.Complete;

            public ExampleMutator(MutationLevel mutationLevel)
            {
                MutationLevel = mutationLevel;
            }

            public override IEnumerable<Mutation> ApplyMutations(BinaryExpressionSyntax node, SemanticModel semanticModel)
            {
                // when this exception is thrown the test knows the method has been called by the BaseMutator
                throw new NotImplementedException();
            }
        }

        [Fact]
        public void Mutator_ShouldCallApplyMutations_OnExpectedType()
        {
            // the type BinaryExpressionSyntax should be mutated by the example mutator
            var originalNode = SyntaxFactory.BinaryExpression(SyntaxKind.AddExpression,
                SyntaxFactory.LiteralExpression(SyntaxKind.NumericLiteralExpression, SyntaxFactory.Literal(1)),
                SyntaxFactory.LiteralExpression(SyntaxKind.NumericLiteralExpression, SyntaxFactory.Literal(8)));

            var target = new ExampleMutator(MutationLevel.Basic);

            Should.Throw<NotImplementedException>(() => target.Mutate(originalNode, null, new StrykerOptions()));
        }

        [Fact]
        public void Mutator_ShouldNotCallApplyMutations_OnWrongType()
        {
            // the type ReturnStatementSyntax should NOT be mutated
            var originalNode = SyntaxFactory.ReturnStatement(SyntaxFactory.LiteralExpression(SyntaxKind.NumericLiteralExpression, SyntaxFactory.Literal(1)));

            var target = new ExampleMutator(MutationLevel.Basic);

            var result = target.Mutate(originalNode, null, new StrykerOptions());

            result.ShouldBeEmpty();
        }

        [Fact]
        public void Mutator_ShouldNotCallApplyMutations_OnWrongType2()
        {
            // the type AssignmentExpressionSyntax should NOT be mutated
            var originalNode = SyntaxFactory.AssignmentExpression(SyntaxKind.SimpleAssignmentExpression,
                SyntaxFactory.LiteralExpression(SyntaxKind.NumericLiteralExpression, SyntaxFactory.Literal(100)),
                SyntaxFactory.LiteralExpression(SyntaxKind.NumericLiteralExpression, SyntaxFactory.Literal(5)));

            var target = new ExampleMutator(MutationLevel.Basic);

            var result = target.Mutate(originalNode, null, new StrykerOptions());

            result.ShouldBeEmpty();
        }

        [Fact]
        public void ShouldNotMutateIfMutationLevelIsLow()
        {
            var originalNode = SyntaxFactory.BinaryExpression(SyntaxKind.AddExpression,
                SyntaxFactory.LiteralExpression(SyntaxKind.NumericLiteralExpression, SyntaxFactory.Literal(1)),
                SyntaxFactory.LiteralExpression(SyntaxKind.NumericLiteralExpression, SyntaxFactory.Literal(8)));

            // The mutator is of level Expert
            var target = new ExampleMutator(MutationLevel.Complete);

            // The options level is Beginner
            var options = new StrykerOptions
            {
                MutationLevel = MutationLevel.Standard
            };
            var result = target.Mutate(originalNode, null, options);

            // ApplyMutations should not have been called
            result.ShouldBeEmpty();
        }

        [Fact]
        public void ShouldMutateIfLevelIsEqual()
        {
            var originalNode = SyntaxFactory.BinaryExpression(SyntaxKind.AddExpression,
                SyntaxFactory.LiteralExpression(SyntaxKind.NumericLiteralExpression, SyntaxFactory.Literal(1)),
                SyntaxFactory.LiteralExpression(SyntaxKind.NumericLiteralExpression, SyntaxFactory.Literal(8)));
            var options = new StrykerOptions
            {
                MutationLevel = MutationLevel.Complete
            };
            var target = new ExampleMutator(MutationLevel.Complete);

            Should.Throw<NotImplementedException>(() => target.Mutate(originalNode, null, options));
        }
    }
}
