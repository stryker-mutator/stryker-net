using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Shouldly;
using Stryker.Abstractions.Mutators;
using Stryker.Core.Mutators;

namespace Stryker.Core.UnitTest.Mutators
{
    [TestClass]
    public class ObjectCreationMutatorTests : TestBase
    {
        [TestMethod]
        public void ShouldBeMutationLevelStandard()
        {
            var target = new ObjectCreationMutator();
            target.MutationLevel.ShouldBe(MutationLevel.Standard);
        }

        [TestMethod]
        [DataRow("new List<int> { 1, 3 }")]
        [DataRow("new Collection<int> { 1, 3 }")]
        [DataRow(@"new Dictionary<int, StudentName>()
        {
            { 111, new StudentName { FirstName='Foo', LastName='Bar', ID=211 } }
        };")]
        public void ShouldRemoveValuesFromCollectionInitializer(string initializer)
        {
            var objectCreationExpression = SyntaxFactory.ParseExpression(initializer) as ObjectCreationExpressionSyntax;

            var target = new ObjectCreationMutator();

            var result = target.ApplyMutations(objectCreationExpression, null);

            var mutation = result.ShouldHaveSingleItem();
            mutation.Type.ShouldBe(Mutator.Initializer);

            var replacement = mutation.ReplacementNode.ShouldBeOfType<ObjectCreationExpressionSyntax>();
            replacement.Initializer.Expressions.ShouldBeEmpty();
        }

        [TestMethod]
        public void ShouldRemoveValuesFromObjectInitializer()
        {
            var objectCreationExpression = SyntaxFactory.ParseExpression("new SomeClass { SomeProperty = SomeValue }") as ObjectCreationExpressionSyntax;

            var target = new ObjectCreationMutator();

            var result = target.ApplyMutations(objectCreationExpression, null);

            var mutation = result.ShouldHaveSingleItem();
            mutation.DisplayName.ShouldBe("Object initializer mutation");
            mutation.Type.ShouldBe(Mutator.Initializer);

            var replacement = mutation.ReplacementNode.ShouldBeOfType<ObjectCreationExpressionSyntax>();
            replacement.Initializer.Expressions.ShouldBeEmpty();
        }

        [TestMethod]
        [DataRow("new List<int> { }")]
        [DataRow("new SomeClass { }")]
        public void ShouldNotMutateEmptyInitializer(string initializer)
        {
            var objectCreationExpression = SyntaxFactory.ParseExpression(initializer) as ObjectCreationExpressionSyntax;

            var target = new ObjectCreationMutator();

            var result = target.ApplyMutations(objectCreationExpression, null);

            result.ShouldBeEmpty();
        }
    }
}
