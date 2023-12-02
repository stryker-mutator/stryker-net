using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Shouldly;
using Stryker.Core.Mutators;
using System.Linq;
using Xunit;

namespace Stryker.Core.UnitTest.Mutators
{
    public class LinqMutatorTest : TestBase
    {
        /// <summary>
        ///     Generator for different Linq expressions
        /// </summary>
        /// <param name="expression"></param>
        /// <returns></returns>
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

        [Fact]
        public void ShouldBeMutationLevelStandard()
        {
            var target = new LinqMutator();
            target.MutationLevel.ShouldBe(MutationLevel.Standard);
        }

        /// <summary>
        ///     Test method to check for correct mutation of different Linq Expression Mutations
        /// </summary>
        /// <param name="original"></param>
        /// <param name="expected"></param>
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
        public void ShouldMutate(LinqExpression original, LinqExpression expected)
        {
            var target = new LinqMutator();

            var expression = GenerateExpressions(original.ToString());

            var result = target.ApplyMutations(expression, null).ToList();

            var mutation = result.ShouldHaveSingleItem();
            var replacement = mutation.ReplacementNode.ShouldBeOfType<MemberAccessExpressionSyntax>();
           replacement.Name.Identifier.ValueText.ShouldBe(expected.ToString());

            mutation.DisplayName.ShouldBe($"Linq method mutation ({ original }() to { expected }())");
        }

        /// <summary>
        ///     Test Method to check, if different expressions aren't mutated
        /// </summary>
        /// <param name="methodName"></param>
        [Theory]
        [InlineData("AllData")]
        [InlineData("PriceFirstOrDefault")]
        [InlineData("TakeEntry")]
        [InlineData("ShouldNotMutate")]
        [InlineData("WriteLine")]
        public void ShouldNotMutate(string methodName)
        {
            var target = new LinqMutator();

            var result = target.ApplyMutations(GenerateExpressions(methodName), null);

            result.ShouldBeEmpty();
        }

        [Fact]
        public void ShouldMutateProperlyConditionalExpression()
        {
            var tree = CSharpSyntaxTree.ParseText(@"
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TestApplication
{
    class Program
    {
        static void Main(string[] args)
        {
            IEnumerable<string> Test = new[] {};

            Test?.First.Second.Third.All(_ => _!=null);
        }
    }
}");
            var memberAccessExpression = tree
                .GetRoot()
                .DescendantNodes()
                .OfType<MemberAccessExpressionSyntax>().Single(x => x.Name.ToString() == "All");
            var target = new LinqMutator();

            var result = target.ApplyMutations(memberAccessExpression, null);

            result.ShouldHaveSingleItem().ReplacementNode.ShouldBeOfType<MemberAccessExpressionSyntax>().Name.ToString().ShouldBe("Any");
        }
    }
}
