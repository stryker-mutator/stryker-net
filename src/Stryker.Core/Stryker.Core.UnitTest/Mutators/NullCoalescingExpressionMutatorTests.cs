using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Shouldly;
using Stryker.Core.Mutators;
using System.Linq;
using Microsoft.CodeAnalysis;
using Xunit;

namespace Stryker.Core.UnitTest.Mutators
{
    public class NullCoalescingExpressionMutatorTests : TestBase
    {
        [Fact]
        public void ShouldBeMutationLevelBasic()
        {
            var target = new NullCoalescingExpressionMutator();
            target.MutationLevel.ShouldBe(MutationLevel.Basic);
        }

        [Fact]
        public void ShouldMutate()
        {
            // Arrange
            var target = new NullCoalescingExpressionMutator();
            var originalExpressionString = "a ?? b";
            var expectedExpressionStrings = new[] { "a", "b", "b ?? a" };
            var originalExpression = SyntaxFactory.ParseExpression(originalExpressionString);

            // Act
            var result = target.ApplyMutations(originalExpression as BinaryExpressionSyntax, null);

            // Assert
            result.Count().ShouldBe(3);

            foreach (var mutant in result)
            {
                expectedExpressionStrings.ShouldContain(mutant.ReplacementNode.ToString());
            }
        }

        [Fact]
        public void ShouldMutateThrowExpression()
        {
            // Arrange
            var target = new NullCoalescingExpressionMutator();
            var originalExpressionString = "a ?? throw new ArgumentException(nameof(a))";
            var originalExpression = SyntaxFactory.ParseExpression(originalExpressionString);

            // Act
            var result = target.ApplyMutations(originalExpression as BinaryExpressionSyntax, null);

            // Assert
            var mutant = result.ShouldHaveSingleItem();
            mutant.ReplacementNode.ToString().ShouldBe("a");
        }

        [Fact]
        public void ShouldNotMutateLeftToRightOrRemoveLeftIfNotNullable()
        {
            var syntaxTree = CSharpSyntaxTree.ParseText(
                """
                public TimeSpan? GetLocalDateTime(DateTimeOffset startTime, DateTimeOffset? endTime)
                {
                    return (endTime ?? startTime).LocalDateTime;
                }
                """);
            var compilation = CSharpCompilation.Create("TestAssembly")
                .WithOptions(new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary))
                .AddReferences(MetadataReference.CreateFromFile(typeof(object).Assembly.Location))
                .AddSyntaxTrees(syntaxTree);
            var semanticModel = compilation.GetSemanticModel(syntaxTree);
            var expression = syntaxTree.GetRoot().DescendantNodes().OfType<BinaryExpressionSyntax>().First();

            var target = new NullCoalescingExpressionMutator();
            var result = target.ApplyMutations(expression, semanticModel).ToList();

            result.ShouldHaveSingleItem();
            result.ShouldNotContain(x => x.Description == "Null coalescing mutation (left to right)");
            result.ShouldNotContain(x => x.Description == "Null coalescing mutation (remove left)");
        }


        [Fact]
        public void ShouldMutateIfBothSidesAreNullable()
        {
            var syntaxTree = CSharpSyntaxTree.ParseText(
                """
                public TimeSpan? GetLocalDateTime(DateTimeOffset? startTime, DateTimeOffset? endTime)
                {
                    return (endTime ?? startTime)?.LocalDateTime;
                }
                """);
            var compilation = CSharpCompilation.Create("TestAssembly")
                .WithOptions(new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary, nullableContextOptions: NullableContextOptions.Enable))
                .AddReferences(MetadataReference.CreateFromFile(typeof(object).Assembly.Location))
                .AddSyntaxTrees(syntaxTree);
            var semanticModel = compilation.GetSemanticModel(syntaxTree);
            var expression = syntaxTree.GetRoot().DescendantNodes().OfType<BinaryExpressionSyntax>().First();

            var target = new NullCoalescingExpressionMutator();
            var result = target.ApplyMutations(expression, semanticModel).ToList();

            result.Count.ShouldBe(3);
        }
    }
}
