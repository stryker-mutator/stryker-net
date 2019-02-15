using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Shouldly;
using Stryker.Core.Mutators;
using Xunit;

namespace Stryker.Core.UnitTest.Mutators
{
    public class LinqMutatorTest
    {
        /// <summary>
        ///     Generator for different Linqexpressions
        /// </summary>
        /// <param name="expression"></param>
        /// <returns></returns>
        private MemberAccessExpressionSyntax GenerateExpressions(string expression)
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

            Test.{expression}();
        }}
    }}
}}");
            var memberAccessExpression = tree.GetRoot()
                .DescendantNodes()
                .OfType<MemberAccessExpressionSyntax>()
                .Single();

            return memberAccessExpression;
        }

        /// <summary>
        ///     Test method to check for correct mutation of different Linq Expression Mutations
        /// </summary>
        /// <param name="original"></param>
        /// <param name="expected"></param>
        [Theory]
        [InlineData(LinqExpression.FirstOrDefault, LinqExpression.SingleOrDefault)]
        [InlineData(LinqExpression.SingleOrDefault, LinqExpression.FirstOrDefault)]
        [InlineData(LinqExpression.First, LinqExpression.Last)]
        [InlineData(LinqExpression.Last, LinqExpression.First)]
        [InlineData(LinqExpression.All, LinqExpression.Any)]
        [InlineData(LinqExpression.Any, LinqExpression.All)]
        [InlineData(LinqExpression.Skip, LinqExpression.Take)]
        [InlineData(LinqExpression.Take, LinqExpression.Skip)]
        [InlineData(LinqExpression.SkipWhile, LinqExpression.TakeWhile)]
        [InlineData(LinqExpression.TakeWhile, LinqExpression.SkipWhile)]
        [InlineData(LinqExpression.Min, LinqExpression.Max)]
        [InlineData(LinqExpression.Max, LinqExpression.Min)]
        [InlineData(LinqExpression.Sum, LinqExpression.Count)]
        [InlineData(LinqExpression.Count, LinqExpression.Sum)]
        public void ShouldMutate(LinqExpression original, LinqExpression expected)
        {
            var target = new LinqMutator();

            var expression = GenerateExpressions(original.ToString());

            var result = target.ApplyMutations(expression).ToList();

            var mutation = result.ShouldHaveSingleItem();
            var replacement = mutation.ReplacementNode.ShouldBeOfType<MemberAccessExpressionSyntax>();
            replacement.Name.Identifier.ValueText.ShouldBe(expected.ToString());
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

            var result = target.ApplyMutations(GenerateExpressions(methodName));

            result.ShouldBeEmpty();
        }
    }
}