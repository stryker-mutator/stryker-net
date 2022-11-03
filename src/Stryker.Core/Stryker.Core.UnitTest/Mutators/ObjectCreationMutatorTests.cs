using Microsoft.Build.Logging.StructuredLogger;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Shouldly;
using Stryker.Core.Mutators;
using Xunit;

namespace Stryker.Core.UnitTest.Mutators
{
    public class ObjectCreationMutatorTests : TestBase
    {
        [Fact]
        public void ShouldBeMutationLevelStandard()
        {
            var target = new ObjectCreationMutator();
            target.MutationLevel.ShouldBe(MutationLevel.Standard);
        }

        [Theory]
        [InlineData("new List<int> { 1, 3 }")]
        [InlineData("new Collection<int> { 1, 3 }")]
        [InlineData(@"new Dictionary<int, StudentName>()
        {
            { 111, new StudentName { FirstName='Foo', LastName='Bar', ID=211 } }
        };")]
        public void ShouldRemoveValuesFromCollectionInitializer(string initializer)
        {
            var objectCreationExpression = SyntaxFactory.ParseExpression(initializer) as ObjectCreationExpressionSyntax;

            var target = new ObjectCreationMutator();

            var result = target.ApplyMutations(objectCreationExpression);

            var mutation = result.ShouldHaveSingleItem();
            mutation.Type.ShouldBe(Mutator.Initializer);

            var replacement = mutation.ReplacementNode.ShouldBeOfType<ObjectCreationExpressionSyntax>();
            replacement.Initializer.Expressions.ShouldBeEmpty();
        }

        [Fact]
        public void ShouldRemoveValuesFromObjectInitializer()
        {
            var objectCreationExpression = SyntaxFactory.ParseExpression("new SomeClass { SomeProperty = SomeValue }") as ObjectCreationExpressionSyntax;

            var target = new ObjectCreationMutator();

            var result = target.ApplyMutations(objectCreationExpression);

            var mutation = result.ShouldHaveSingleItem();
            mutation.DisplayName.ShouldBe("Object initializer mutation");
            mutation.Type.ShouldBe(Mutator.Initializer);

            var replacement = mutation.ReplacementNode.ShouldBeOfType<ObjectCreationExpressionSyntax>();
            replacement.Initializer.Expressions.ShouldBeEmpty();
        }

        [Theory]
        [InlineData("new List<int> { }")]
        [InlineData("new SomeClass { }")]
        public void ShouldNotMutateEmptyInitializer(string initializer)
        {
            var objectCreationExpression = SyntaxFactory.ParseExpression(initializer) as ObjectCreationExpressionSyntax;

            var target = new ObjectCreationMutator();

            var result = target.ApplyMutations(objectCreationExpression);

            result.ShouldBeEmpty();
        }
    }
}
