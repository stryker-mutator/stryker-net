using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Shouldly;
using Stryker.Core.Mutators;
using Xunit;

namespace Stryker.Core.UnitTest.Mutators
{
    public class MathMutatorTest : TestBase
    {
        /// <summary>
        ///     Generator for different Math expressions
        /// </summary>
        /// <param name="expression"></param>
        /// <returns></returns>
        private InvocationExpressionSyntax GenerateExpression(string expression)
        {
            SyntaxTree tree = CSharpSyntaxTree.ParseText($@"
using System;

namespace TestApplication
{{
    class Program
    {{
        static void Main(string[] args)
        {{
            Math.{expression}(5.0);
        }}
    }}
}}");
            var memberAccessExpression = tree.GetRoot()
                .DescendantNodes()
                .OfType<InvocationExpressionSyntax>()
                .Single();

            return memberAccessExpression;
        }

        [Fact]
        public void ShouldBeMutationLevelAdvanced()
        {
            var target = new MathMutator();
            target.MutationLevel.ShouldBe(MutationLevel.Advanced);
        }

        /// <summary>
        ///     Test method to check for correct mutation of different Math Expression Mutations
        /// </summary>
        [Theory]
        [MemberData(nameof(GetTrigonometricHyperbolicTestData))]
        [InlineData(MathExpression.BitDecrement, new[] { MathExpression.BitIncrement })]
        [InlineData(MathExpression.BitIncrement, new[] { MathExpression.BitDecrement })]
        [InlineData(MathExpression.Ceiling, new[] { MathExpression.Floor })]
        [InlineData(MathExpression.Exp, new[] { MathExpression.Log })]
        [InlineData(MathExpression.Floor, new[] { MathExpression.Ceiling })]
        [InlineData(MathExpression.Log, new[] { MathExpression.Exp, MathExpression.Pow })]
        [InlineData(MathExpression.MaxMagnitude, new[] { MathExpression.MinMagnitude })]
        [InlineData(MathExpression.MinMagnitude, new[] { MathExpression.MaxMagnitude })]
        [InlineData(MathExpression.Pow, new[] { MathExpression.Log })]
        [InlineData(MathExpression.ReciprocalEstimate, new[] { MathExpression.ReciprocalSqrtEstimate })]
        [InlineData(MathExpression.ReciprocalSqrtEstimate, new[] { MathExpression.ReciprocalEstimate, MathExpression.Sqrt })]
        public void ShouldMutate(MathExpression original, MathExpression[] mutated)
        {
            var target = new MathMutator();

            var expression = GenerateExpression(original.ToString());

            var result = target.ApplyMutations(expression).ToList();

            result.ForEach(mutation =>
            {
                var replacement = mutation.ReplacementNode.ShouldBeOfType<InvocationExpressionSyntax>();
                var simpleMember = replacement.Expression.ShouldBeOfType<MemberAccessExpressionSyntax>();

                mutation.DisplayName.ShouldStartWith($"Math method mutation ({original}() to");
            });

            result
                .Select(mutation => (InvocationExpressionSyntax)mutation.ReplacementNode)
                .Select(replacement => (MemberAccessExpressionSyntax)replacement.Expression)
                .Select(simpleMember => simpleMember.Name.Identifier.ValueText)
                .Select(name => Enum.Parse<MathExpression>(name))
                .ShouldBe(mutated, true);
        }

        /// <summary>
        ///     Test Method to check, if different expressions aren't mutated
        /// </summary>
        [Theory]
        [InlineData("Abs")]
        [InlineData("Atan2")]
        [InlineData("Cbrt")]
        [InlineData("DivRem")]
        [InlineData("Log10")]
        [InlineData("Sqrt")]
        // there is no need to mutate Min/Max methods because they will duplicate Linq mutator
        [InlineData("Min")]
        [InlineData("Max")]
        public void ShouldNotMutate(string methodName)
        {
            var target = new MathMutator();

            var result = target.ApplyMutations(GenerateExpression(methodName));

            result.ShouldBeEmpty();
        }

        public static IEnumerable<object[]> GetTrigonometricHyperbolicTestData()
        {
            var functions = new[] { "cos", "sin", "tan" };

            foreach (var function in functions)
            {
                var otherFunctions = functions.Where(x => x != function).ToList();

                yield return new object[]
                {
                    Enum.Parse<MathExpression>(function, true),
                    otherFunctions.Select(x => Enum.Parse<MathExpression>(x, true)).Union(new [] { Enum.Parse<MathExpression>($"{function}h", true) }).ToArray()
                };

                yield return new object[]
                {
                    Enum.Parse<MathExpression>($"{function}h", true),
                    otherFunctions.Select(x => Enum.Parse<MathExpression>($"{x}h", true)).Union(new [] { Enum.Parse<MathExpression>(function, true) }).ToArray()
                };

                yield return new object[]
                {
                    Enum.Parse<MathExpression>($"a{function}", true),
                    otherFunctions.Select(x => Enum.Parse<MathExpression>($"a{x}", true)).Union(new [] { Enum.Parse<MathExpression>($"a{function}h", true) }).ToArray()
                };

                yield return new object[]
                {
                    Enum.Parse<MathExpression>($"a{function}h", true),
                    otherFunctions.Select(x => Enum.Parse<MathExpression>($"a{x}h", true)).Union(new [] { Enum.Parse<MathExpression>($"a{function}", true) }).ToArray()
                };
            }
        }
    }
}
