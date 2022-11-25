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
        ///     Generator for different Math expressions using class name
        /// </summary>
        /// <param name="expression"></param>
        /// <returns></returns>
        private InvocationExpressionSyntax GenerateClassCallExpression(string memberName, string expression)
        {
            SyntaxTree tree = CSharpSyntaxTree.ParseText($@"
using System;

namespace TestApplication
{{
    class Program
    {{
        static void Main(string[] args)
        {{
            {memberName}.{expression}(5.0);
        }}
    }}
}}");
            var memberAccessExpression = tree.GetRoot()
                .DescendantNodes()
                .OfType<InvocationExpressionSyntax>()
                .Single();

            return memberAccessExpression;
        }

        /// <summary>
        ///     Generator for different Math expressions with using static
        /// </summary>
        /// <param name="expression"></param>
        /// <returns></returns>
        private InvocationExpressionSyntax GenerateStaticCallExpression(string expression)
        {
            SyntaxTree tree = CSharpSyntaxTree.ParseText($@"
using System;
using static System.Math;

namespace TestApplication
{{
    class Program
    {{
        static void Main(string[] args)
        {{
            {expression}(5.0);
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
        ///     Test method to check for correct mutation of different Math Expression Mutations (in case of call with class name)
        /// </summary>
        [Theory]
        [MemberData(nameof(GetMethodSwapsTestData))]
        public void ShouldMutate(MathExpression original, MathExpression[] mutated)
        {
            var target = new MathMutator();

            var expression = GenerateClassCallExpression("Math", original.ToString());

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
        ///     Test method to check for correct mutation of different Math Expression Mutations (in case of call without class name)
        /// </summary>
        [Theory]
        [MemberData(nameof(GetMethodSwapsTestData))]
        public void ShouldMutateUsingStatic(MathExpression original, MathExpression[] mutated)
        {
            var target = new MathMutator();

            var expression = GenerateStaticCallExpression(original.ToString());

            var result = target.ApplyMutations(expression).ToList();

            result.ForEach(mutation =>
            {
                var replacement = mutation.ReplacementNode.ShouldBeOfType<InvocationExpressionSyntax>();
                var simpleMember = replacement.Expression.ShouldBeOfType<IdentifierNameSyntax>();

                mutation.DisplayName.ShouldStartWith($"Math method mutation ({original}() to");
            });

            result
                .Select(mutation => (InvocationExpressionSyntax)mutation.ReplacementNode)
                .Select(replacement => (IdentifierNameSyntax)replacement.Expression)
                .Select(name => Enum.Parse<MathExpression>(name.Identifier.ValueText))
                .ShouldBe(mutated, true);
        }

        /// <summary>
        ///     Test Method to check, if different expressions aren't mutated (in case of not supported methods)
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
        public void ShouldNotMutateOtherMethods(string methodName)
        {
            var target = new MathMutator();

            var result = target.ApplyMutations(GenerateClassCallExpression("Math", methodName));

            result.ShouldBeEmpty();
        }

        /// <summary>
        /// Test Method to check, if different expressions aren't mutated (in case of non-Math classes)
        /// </summary>
        [Theory]
        [InlineData("MyClass")]
        [InlineData("MyMath")]
        public void ShouldNotMutateOtherClasses(string className)
        {
            var original = GetMethodSwapsTestData().First()[0];
            var target = new MathMutator();

            var result = target.ApplyMutations(GenerateClassCallExpression(className, original.ToString()));

            result.ShouldBeEmpty();
        }

        public static IEnumerable<object[]> GetMethodSwapsTestData()
        {
            foreach (var trigonometricHyperbolicTestData in GetTrigonometricHyperbolicTestData())
            {
                yield return trigonometricHyperbolicTestData;
            }

            yield return new object[]
            {
                MathExpression.BitDecrement, new[] { MathExpression.BitIncrement }
            };

            yield return new object[]
            {
                MathExpression.BitIncrement, new[] { MathExpression.BitDecrement }
            };

            yield return new object[]
            {
                MathExpression.Ceiling, new[] { MathExpression.Floor }
            };

            yield return new object[]
            {
                MathExpression.Exp, new[] { MathExpression.Log }
            };

            yield return new object[]
            {
                MathExpression.Floor, new[] { MathExpression.Ceiling }
            };

            yield return new object[]
            {
                MathExpression.Log, new[] { MathExpression.Exp, MathExpression.Pow }
            };

            yield return new object[]
            {
                MathExpression.MaxMagnitude, new[] { MathExpression.MinMagnitude }
            };

            yield return new object[]
            {
                MathExpression.MinMagnitude, new[] { MathExpression.MaxMagnitude }
            };

            yield return new object[]
            {
                MathExpression.Pow, new[] { MathExpression.Log }
            };

            yield return new object[]
            {
                MathExpression.ReciprocalEstimate, new[] { MathExpression.ReciprocalSqrtEstimate }
            };

            yield return new object[]
            {
                MathExpression.ReciprocalSqrtEstimate, new[] { MathExpression.ReciprocalEstimate, MathExpression.Sqrt }
            };
        }

        private static IEnumerable<object[]> GetTrigonometricHyperbolicTestData()
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
