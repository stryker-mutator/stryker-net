﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Shouldly;
using Stryker.Core.Enumerations;
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
        private IEnumerable<InvocationExpressionSyntax> GenerateExpressions(LinqExpression expression)
        {

            SyntaxTree tree = CSharpSyntaxTree.ParseText(
$@"using System;
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

            Console.WriteLine(Test.{expression.ToString()}());
        }}
    }}
}}");
            CompilationUnitSyntax root = (CompilationUnitSyntax)tree.GetRoot();

            IEnumerable<InvocationExpressionSyntax> expressions =
                root.DescendantNodes()
                    .Where(d => d.Kind().Equals(SyntaxKind.InvocationExpression) &&
                                d.DescendantNodes()
                                    .Any(e => e.Kind().Equals(SyntaxKind.IdentifierName) &&
                                                ((IdentifierNameSyntax)e).Identifier
                                                .ValueText.Equals(
                                                    expression.ToString())))
                .Cast<InvocationExpressionSyntax>();

            return expressions;
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

            var expressions = GenerateExpressions(original);

            foreach (var expression in expressions)
            {
                var result = target.ApplyMutations(expression).ToList();

                result.ShouldHaveSingleItem();

                var first = result.First();

                first.ReplacementNode.IsKind(SyntaxKind.IdentifierName)
                    .ShouldBeTrue();

                ((IdentifierNameSyntax) first.ReplacementNode)
                    .Identifier.ValueText.Equals(
                        expected.ToString())
                    .ShouldBeTrue();

            }
        }

        /// <summary>
        ///     Test Method to check, if different expressions aren't mutated
        /// </summary>
        /// <param name="orginal"></param>
        [Theory]
        [InlineData(SyntaxKind.UncheckedExpression)]
        public void ShouldNotMutate(SyntaxKind orginal)
        {
            var target = new CheckedMutator();

            ExpressionSyntax es = SyntaxFactory.ParseExpression("4 + 2");
            var result = target.ApplyMutations(SyntaxFactory.CheckedExpression(orginal, es)).ToList();

            result.ShouldBeEmpty();
        }
    }
}
