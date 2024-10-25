using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Shouldly;
using Stryker.Abstractions;
using Stryker.Abstractions.Mutants;
using Stryker.Core.MutantFilters;
using Stryker.Core.Mutants;
using Stryker.Core.Mutators;

namespace Stryker.Core.UnitTest.MutantFilters
{
    [TestClass]
    public class ExcludeLinqExpressionFilterTests : TestBase
    {

        [TestMethod]
        public void ShouldHaveDisplayName()
        {
            // Arrange
            var sut = new ExcludeLinqExpressionFilter();

            // Assert
            sut.DisplayName.ShouldBe("linq expression filter");
        }

        [TestMethod]
        [DataRow(LinqExpression.FirstOrDefault)]
        [DataRow(LinqExpression.First)]
        [DataRow(LinqExpression.SingleOrDefault)]
        [DataRow(LinqExpression.Single)]
        [DataRow(LinqExpression.Last)]
        [DataRow(LinqExpression.All)]
        [DataRow(LinqExpression.Any)]
        [DataRow(LinqExpression.Skip)]
        [DataRow(LinqExpression.Take)]
        [DataRow(LinqExpression.SkipWhile)]
        [DataRow(LinqExpression.TakeWhile)]
        [DataRow(LinqExpression.Min)]
        [DataRow(LinqExpression.Max)]
        [DataRow(LinqExpression.Sum)]
        [DataRow(LinqExpression.Average)]
        [DataRow(LinqExpression.OrderBy)]
        [DataRow(LinqExpression.OrderByDescending)]
        [DataRow(LinqExpression.ThenBy)]
        [DataRow(LinqExpression.ThenByDescending)]
        [DataRow(LinqExpression.Reverse)]
        [DataRow(LinqExpression.AsEnumerable)]
        [DataRow(LinqExpression.Union)]
        [DataRow(LinqExpression.Intersect)]
        [DataRow(LinqExpression.Concat)]
        [DataRow(LinqExpression.Except)]
        [DataRow(LinqExpression.IntersectBy)]
        [DataRow(LinqExpression.MaxBy)]
        [DataRow(LinqExpression.MinBy)]
        [DataRow(LinqExpression.Order)]
        [DataRow(LinqExpression.OrderDescending)]
        [DataRow(LinqExpression.SkipLast)]
        [DataRow(LinqExpression.TakeLast)]
        [DataRow(LinqExpression.UnionBy)]
        public void ShouldRemoveLinqExpressionWhenFilterIsCorrect(LinqExpression exp)
        {
            // Arrange
            var target = new LinqMutator();

            var expression = GenerateExpressions(exp.ToString());

            var result = target.ApplyMutations(expression, null).ToList();

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


        [TestMethod]
        [DataRow(LinqExpression.FirstOrDefault, LinqExpression.First)]
        [DataRow(LinqExpression.First, LinqExpression.FirstOrDefault)]
        [DataRow(LinqExpression.SingleOrDefault, LinqExpression.Single)]
        [DataRow(LinqExpression.Single, LinqExpression.SingleOrDefault)]
        [DataRow(LinqExpression.Last, LinqExpression.First)]
        [DataRow(LinqExpression.All, LinqExpression.Any)]
        [DataRow(LinqExpression.Any, LinqExpression.All)]
        [DataRow(LinqExpression.Skip, LinqExpression.Take)]
        [DataRow(LinqExpression.Take, LinqExpression.Skip)]
        [DataRow(LinqExpression.SkipWhile, LinqExpression.TakeWhile)]
        [DataRow(LinqExpression.TakeWhile, LinqExpression.SkipWhile)]
        [DataRow(LinqExpression.Min, LinqExpression.Max)]
        [DataRow(LinqExpression.Max, LinqExpression.Min)]
        [DataRow(LinqExpression.Sum, LinqExpression.Max)]
        [DataRow(LinqExpression.Average, LinqExpression.Min)]
        [DataRow(LinqExpression.OrderBy, LinqExpression.OrderByDescending)]
        [DataRow(LinqExpression.OrderByDescending, LinqExpression.OrderBy)]
        [DataRow(LinqExpression.ThenBy, LinqExpression.ThenByDescending)]
        [DataRow(LinqExpression.ThenByDescending, LinqExpression.ThenBy)]
        [DataRow(LinqExpression.Reverse, LinqExpression.AsEnumerable)]
        [DataRow(LinqExpression.AsEnumerable, LinqExpression.Reverse)]
        [DataRow(LinqExpression.Union, LinqExpression.Intersect)]
        [DataRow(LinqExpression.Intersect, LinqExpression.Union)]
        [DataRow(LinqExpression.Concat, LinqExpression.Except)]
        [DataRow(LinqExpression.Except, LinqExpression.Concat)]
        [DataRow(LinqExpression.MinBy, LinqExpression.MaxBy)]
        [DataRow(LinqExpression.MaxBy, LinqExpression.MinBy)]
        [DataRow(LinqExpression.SkipLast, LinqExpression.TakeLast)]
        [DataRow(LinqExpression.TakeLast, LinqExpression.SkipLast)]
        [DataRow(LinqExpression.Order, LinqExpression.OrderDescending)]
        [DataRow(LinqExpression.OrderDescending, LinqExpression.Order)]
        [DataRow(LinqExpression.UnionBy, LinqExpression.IntersectBy)]
        [DataRow(LinqExpression.IntersectBy, LinqExpression.UnionBy)]
        public void ShouldNotRemoveLinqExpressionWhenFilterIsDifferentName(LinqExpression exp, LinqExpression excludedExp)
        {
            // Arrange
            var target = new LinqMutator();

            var expression = GenerateExpressions(exp.ToString());

            var result = target.ApplyMutations(expression, null).ToList();

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

        [TestMethod]
        [DataRow(LinqExpression.FirstOrDefault)]
        [DataRow(LinqExpression.First)]
        [DataRow(LinqExpression.SingleOrDefault)]
        [DataRow(LinqExpression.Single)]
        [DataRow(LinqExpression.Last)]
        [DataRow(LinqExpression.All)]
        [DataRow(LinqExpression.Any)]
        [DataRow(LinqExpression.Skip)]
        [DataRow(LinqExpression.Take)]
        [DataRow(LinqExpression.SkipWhile)]
        [DataRow(LinqExpression.TakeWhile)]
        [DataRow(LinqExpression.Min)]
        [DataRow(LinqExpression.Max)]
        [DataRow(LinqExpression.Sum)]
        [DataRow(LinqExpression.Average)]
        [DataRow(LinqExpression.OrderBy)]
        [DataRow(LinqExpression.OrderByDescending)]
        [DataRow(LinqExpression.ThenBy)]
        [DataRow(LinqExpression.ThenByDescending)]
        [DataRow(LinqExpression.Reverse)]
        [DataRow(LinqExpression.AsEnumerable)]
        [DataRow(LinqExpression.Union)]
        [DataRow(LinqExpression.Intersect)]
        [DataRow(LinqExpression.Concat)]
        [DataRow(LinqExpression.Except)]
        [DataRow(LinqExpression.IntersectBy)]
        [DataRow(LinqExpression.MaxBy)]
        [DataRow(LinqExpression.MinBy)]
        [DataRow(LinqExpression.Order)]
        [DataRow(LinqExpression.OrderDescending)]
        [DataRow(LinqExpression.SkipLast)]
        [DataRow(LinqExpression.TakeLast)]
        [DataRow(LinqExpression.UnionBy)]
        public void ShouldNotFilterMutationsWhenFilterIsEmpty(LinqExpression exp)
        {
            // Arrange
            var target = new LinqMutator();

            var expression = GenerateExpressions(exp.ToString());

            var result = target.ApplyMutations(expression, null).ToList();

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


        private ExpressionSyntax GenerateExpressions(string expression)
        {
            var tree = CSharpSyntaxTree.ParseText($@"
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
                .OfType<MemberAccessExpressionSyntax>()
                .Single();

            return memberAccessExpression;
        }

    }
}
