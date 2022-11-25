using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;
using Shouldly;
using Stryker.Core.MutantFilters;
using Stryker.Core.Mutants;
using Stryker.Core.Mutators;
using Stryker.Core.Options;
using Xunit;

namespace Stryker.Core.UnitTest.MutantFilters
{
    public class ExcludeLinqExpressionFilterTests : TestBase
    {

        [Fact]
        public void ShouldHaveDisplayName()
        {
            // Arrange
            var sut = new ExcludeLinqExpressionFilter();

            // Assert
            sut.DisplayName.ShouldBe("linq expression filter");
        }

        [Theory]
        [InlineData(LinqExpression.FirstOrDefault)]
        [InlineData(LinqExpression.First)]
        [InlineData(LinqExpression.SingleOrDefault)]
        [InlineData(LinqExpression.Single)]
        [InlineData(LinqExpression.Last)]
        [InlineData(LinqExpression.All)]
        [InlineData(LinqExpression.Any)]
        [InlineData(LinqExpression.Skip)]
        [InlineData(LinqExpression.Take)]
        [InlineData(LinqExpression.SkipWhile)]
        [InlineData(LinqExpression.TakeWhile)]
        [InlineData(LinqExpression.Min)]
        [InlineData(LinqExpression.Max)]
        [InlineData(LinqExpression.Sum)]
        [InlineData(LinqExpression.Average)]
        [InlineData(LinqExpression.OrderBy)]
        [InlineData(LinqExpression.OrderByDescending)]
        [InlineData(LinqExpression.ThenBy)]
        [InlineData(LinqExpression.ThenByDescending)]
        [InlineData(LinqExpression.Reverse)]
        [InlineData(LinqExpression.AsEnumerable)]
        [InlineData(LinqExpression.Union)]
        [InlineData(LinqExpression.Intersect)]
        [InlineData(LinqExpression.Concat)]
        [InlineData(LinqExpression.Except)]
        [InlineData(LinqExpression.IntersectBy)]
        [InlineData(LinqExpression.MaxBy)]
        [InlineData(LinqExpression.MinBy)]
        [InlineData(LinqExpression.Order)]
        [InlineData(LinqExpression.OrderDescending)]
        [InlineData(LinqExpression.SkipLast)]
        [InlineData(LinqExpression.TakeLast)]
        [InlineData(LinqExpression.UnionBy)]
        public void ShouldRemoveLinqExpressionWhenFilterIsCorrect(LinqExpression exp)
        {
            // Arrange
            var target = new LinqMutator();

            var expression = GenerateExpressions(exp.ToString());

            var result = target.ApplyMutations(expression).ToList();

            var mutants = result.Select(s => new Mutant() { ResultStatus = MutantStatus.Survived, Mutation = s });

            var sut = new ExcludeLinqExpressionFilter();

            // Act
            var mutations = sut.FilterMutants(mutants, null, new StrykerOptions()
            {
                ExcludedLinqExpressions = new List<LinqExpression>()
                {
                    exp
                }
            });

            // Assert
            mutations.ShouldBeEmpty();
        }


        [Theory]
        [InlineData(LinqExpression.FirstOrDefault, LinqExpression.First)]
        [InlineData(LinqExpression.First, LinqExpression.FirstOrDefault)]
        [InlineData(LinqExpression.SingleOrDefault, LinqExpression.Single)]
        [InlineData(LinqExpression.Single, LinqExpression.SingleOrDefault)]
        [InlineData(LinqExpression.Last, LinqExpression.First)]
        [InlineData(LinqExpression.All, LinqExpression.Any)]
        [InlineData(LinqExpression.Any, LinqExpression.All)]
        [InlineData(LinqExpression.Skip, LinqExpression.Take)]
        [InlineData(LinqExpression.Take, LinqExpression.Skip)]
        [InlineData(LinqExpression.SkipWhile, LinqExpression.TakeWhile)]
        [InlineData(LinqExpression.TakeWhile, LinqExpression.SkipWhile)]
        [InlineData(LinqExpression.Min, LinqExpression.Max)]
        [InlineData(LinqExpression.Max, LinqExpression.Min)]
        [InlineData(LinqExpression.Sum, LinqExpression.Max)]
        [InlineData(LinqExpression.Average, LinqExpression.Min)]
        [InlineData(LinqExpression.OrderBy, LinqExpression.OrderByDescending)]
        [InlineData(LinqExpression.OrderByDescending, LinqExpression.OrderBy)]
        [InlineData(LinqExpression.ThenBy, LinqExpression.ThenByDescending)]
        [InlineData(LinqExpression.ThenByDescending, LinqExpression.ThenBy)]
        [InlineData(LinqExpression.Reverse, LinqExpression.AsEnumerable)]
        [InlineData(LinqExpression.AsEnumerable, LinqExpression.Reverse)]
        [InlineData(LinqExpression.Union, LinqExpression.Intersect)]
        [InlineData(LinqExpression.Intersect, LinqExpression.Union)]
        [InlineData(LinqExpression.Concat, LinqExpression.Except)]
        [InlineData(LinqExpression.Except, LinqExpression.Concat)]
        [InlineData(LinqExpression.MinBy, LinqExpression.MaxBy)]
        [InlineData(LinqExpression.MaxBy, LinqExpression.MinBy)]
        [InlineData(LinqExpression.SkipLast, LinqExpression.TakeLast)]
        [InlineData(LinqExpression.TakeLast, LinqExpression.SkipLast)]
        [InlineData(LinqExpression.Order, LinqExpression.OrderDescending)]
        [InlineData(LinqExpression.OrderDescending, LinqExpression.Order)]
        [InlineData(LinqExpression.UnionBy, LinqExpression.IntersectBy)]
        [InlineData(LinqExpression.IntersectBy, LinqExpression.UnionBy)]
        public void ShouldNotRemoveLinqExpressionWhenFilterIsDifferentName(LinqExpression exp, LinqExpression excludedExp)
        {
            // Arrange
            var target = new LinqMutator();

            var expression = GenerateExpressions(exp.ToString());

            var result = target.ApplyMutations(expression).ToList();

            var mutants = result.Select(s => new Mutant() { ResultStatus = MutantStatus.Survived, Mutation = s });

            var sut = new ExcludeLinqExpressionFilter();

            // Act
            var mutations = sut.FilterMutants(mutants, null, new StrykerOptions()
            {
                ExcludedLinqExpressions = new List<LinqExpression>()
                {
                    excludedExp
                }
            });

            // Assert
            mutations.ShouldNotBeEmpty();
        }

        [Theory]
        [InlineData(LinqExpression.FirstOrDefault)]
        [InlineData(LinqExpression.First)]
        [InlineData(LinqExpression.SingleOrDefault)]
        [InlineData(LinqExpression.Single)]
        [InlineData(LinqExpression.Last)]
        [InlineData(LinqExpression.All)]
        [InlineData(LinqExpression.Any)]
        [InlineData(LinqExpression.Skip)]
        [InlineData(LinqExpression.Take)]
        [InlineData(LinqExpression.SkipWhile)]
        [InlineData(LinqExpression.TakeWhile)]
        [InlineData(LinqExpression.Min)]
        [InlineData(LinqExpression.Max)]
        [InlineData(LinqExpression.Sum)]
        [InlineData(LinqExpression.Average)]
        [InlineData(LinqExpression.OrderBy)]
        [InlineData(LinqExpression.OrderByDescending)]
        [InlineData(LinqExpression.ThenBy)]
        [InlineData(LinqExpression.ThenByDescending)]
        [InlineData(LinqExpression.Reverse)]
        [InlineData(LinqExpression.AsEnumerable)]
        [InlineData(LinqExpression.Union)]
        [InlineData(LinqExpression.Intersect)]
        [InlineData(LinqExpression.Concat)]
        [InlineData(LinqExpression.Except)]
        [InlineData(LinqExpression.IntersectBy)]
        [InlineData(LinqExpression.MaxBy)]
        [InlineData(LinqExpression.MinBy)]
        [InlineData(LinqExpression.Order)]
        [InlineData(LinqExpression.OrderDescending)]
        [InlineData(LinqExpression.SkipLast)]
        [InlineData(LinqExpression.TakeLast)]
        [InlineData(LinqExpression.UnionBy)]
        public void ShouldNotFilterMutationsWhenFilterIsEmpty(LinqExpression exp)
        {
            // Arrange
            var target = new LinqMutator();

            var expression = GenerateExpressions(exp.ToString());

            var result = target.ApplyMutations(expression).ToList();

            var mutants = result.Select(s => new Mutant() { ResultStatus = MutantStatus.Survived, Mutation = s });

            var sut = new ExcludeLinqExpressionFilter();

            // Act
            var mutations = sut.FilterMutants(mutants, null, new StrykerOptions()
            {
                ExcludedLinqExpressions = Enumerable.Empty<LinqExpression>()
            });

            // Assert
            mutations.ShouldNotBeEmpty();
        }


        private InvocationExpressionSyntax GenerateExpressions(string expression)
        {
            SyntaxTree tree = CSharpSyntaxTree.ParseText($@"
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TestApplication
{{
    class Program
    {{
        static void Main(string[] args)
        {{
            IEnumerable<string> Test = new[] {{}};

            Test.{expression}(_ => _!=null);
        }}
    }}
}}");
            var memberAccessExpression = tree.GetRoot()
                .DescendantNodes()
                .OfType<InvocationExpressionSyntax>()
                .Single();

            return memberAccessExpression;
        }

    }
}
