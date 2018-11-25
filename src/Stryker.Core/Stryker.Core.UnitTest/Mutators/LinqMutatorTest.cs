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
        private IEnumerable<MemberAccessExpressionSyntax> GenerateExpressions(string expression)
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
            IEnumerable<string> Test = new[] {{""1"", ""2"", ""3"", ""4"", ""5""}};

            Console.WriteLine(Test.{expression}());
        }}
    }}
}}");
            var memberAccessExpressions = tree.GetRoot()
                .DescendantNodes()
                .OfType<MemberAccessExpressionSyntax>()
                .Where(x => x.Name.Identifier.ValueText.Equals(expression));

            return memberAccessExpressions;
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
        [InlineData(LinqExpression.Distinct, LinqExpression.None)]
        [InlineData(LinqExpression.OrderBy, LinqExpression.None)]
        [InlineData(LinqExpression.OrderByDescending, LinqExpression.None)]
        [InlineData(LinqExpression.Reverse, LinqExpression.None)]
        public void ShouldMutate(LinqExpression original, LinqExpression expected)
        {
            var target = new LinqMutator();

            var expressions = GenerateExpressions(original.ToString());

            foreach (var expression in expressions)
            {
                var result = target.ApplyMutations(expression).ToList();

                result.ShouldHaveSingleItem();

                var first = result.First();

                first.ReplacementNode.ShouldBeOfType<MemberAccessExpressionSyntax>();

                var replacement = first.ReplacementNode as MemberAccessExpressionSyntax;
                var identifier = replacement.Name.Identifier;

                if (expected.Equals(LinqExpression.None))
                {
                    string.IsNullOrEmpty(identifier.ValueText).ShouldBeTrue();
                }
                else
                {
                    identifier.ValueText.ShouldBe(expected.ToString());
                }
            }
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

            var result = target.ApplyMutations(GenerateExpressions(methodName).First());

            result.ShouldBeEmpty();
        }
    }
}