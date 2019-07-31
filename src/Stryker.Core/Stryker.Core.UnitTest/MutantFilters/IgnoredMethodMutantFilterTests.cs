using System.IO;
using System.Reflection;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Text;
using Shouldly;
using Stryker.Core.MutantFilters;
using Stryker.Core.Mutants;
using Stryker.Core.Options;
using Xunit;

namespace Stryker.Core.UnitTest.MutantFilters
{
    public class IgnoredMethodMutantFilterTests
    {
        [Theory]
        [InlineData("Where", true)]
        [InlineData("ToList", false)]
        public void MutantFilter_NestedMethodsCalls(string ignoredMethodName, bool shouldSkipMutant)
        {
            // Arrange
            var source = @"
public class IgnoredMethodMutantFilter_NestedMethodCalls
{
    private void TestMethod()
    {
        var t = Enumerable.Range(0, 9).Where(x => x < 5).ToList();
    }
}";
            var baseSyntaxTree = CSharpSyntaxTree.ParseText(source).GetRoot();
            var originalNode = baseSyntaxTree.FindNode(new TextSpan(source.IndexOf('<'), 1));

            var mutant = new Mutant
            {
                Mutation = new Mutation
                {
                    OriginalNode = originalNode,
                }
            };

            var options = new StrykerOptions(ignoredMethods: new[] { ignoredMethodName });

            var sut = new IgnoredMethodMutantFilter();

            // Act
            var filteredMutants = sut.FilterMutants(new[] { mutant }, null, options);

            // Assert
            if (shouldSkipMutant)
                filteredMutants.ShouldNotContain(mutant);
            else
                filteredMutants.ShouldContain(mutant);
        }

        [Fact]
        public void MutantFilters_ShouldIgnoreSyntaxWithoutInvocations()
        {
            // Arrange
            var originalNode = SyntaxFactory.BinaryExpression(SyntaxKind.AddExpression,
                SyntaxFactory.LiteralExpression(SyntaxKind.NumericLiteralExpression, SyntaxFactory.Literal(1)),
                SyntaxFactory.LiteralExpression(SyntaxKind.NumericLiteralExpression, SyntaxFactory.Literal(8)));

            var mutant = new Mutant
            {
                Mutation = new Mutation
                {
                    OriginalNode = originalNode,
                }
            };

            var options = new StrykerOptions(ignoredMethods: new[] { "Fact" });

            var sut = new IgnoredMethodMutantFilter();

            // Act
            var filteredMutants = sut.FilterMutants(new[] { mutant }, null, options);

            // Assert
            filteredMutants.ShouldContain(mutant);
        }
    }
}