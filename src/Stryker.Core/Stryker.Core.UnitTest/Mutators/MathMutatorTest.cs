using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Shouldly;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Stryker.Core.Mutators;
using Stryker.Core.UnitTest;
using Stryker.Abstractions;

namespace Stryker.Core.UnitTest.Mutators;

[TestClass]
public class MathMutatorTest : TestBase
{
    /// <summary>
    ///     Generator for different Math expressions using class name
    /// </summary>
    /// <param name="expression"></param>
    /// <returns></returns>
    private InvocationExpressionSyntax GenerateClassCallExpression(string memberName, string expression)
    {
        var tree = CSharpSyntaxTree.ParseText($@"
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
        var tree = CSharpSyntaxTree.ParseText($@"
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

    [TestMethod]
    public void ShouldBeMutationLevelAdvanced()
    {
        var target = new MathMutator();
        target.MutationLevel.ShouldBe(MutationLevel.Advanced);
    }

    /// <summary>
    ///     Test method to check for correct mutation of different Math Expression Mutations (in case of call with class name)
    /// </summary>
    [TestMethod]
    [DynamicData(nameof(MethodSwapsTestData))]
    public void ShouldMutate(MathExpression original, MathExpression[] mutated)
    {
        var target = new MathMutator();

        var expression = GenerateClassCallExpression("Math", original.ToString());

        var result = target.ApplyMutations(expression, null).ToList();

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
    [TestMethod]
    [DynamicData(nameof(MethodSwapsTestData))]
    public void ShouldMutateUsingStatic(MathExpression original, MathExpression[] mutated)
    {
        var target = new MathMutator();

        var expression = GenerateStaticCallExpression(original.ToString());

        var result = target.ApplyMutations(expression, null).ToList();

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
    [TestMethod]
    [DataRow("Abs")]
    [DataRow("Atan2")]
    [DataRow("Cbrt")]
    [DataRow("DivRem")]
    [DataRow("Log10")]
    [DataRow("Sqrt")]
    // there is no need to mutate Min/Max methods because they will duplicate Linq mutator
    [DataRow("Min")]
    [DataRow("Max")]
    public void ShouldNotMutateOtherMethods(string methodName)
    {
        var target = new MathMutator();

        var result = target.ApplyMutations(GenerateClassCallExpression("Math", methodName), null);

        result.ShouldBeEmpty();
    }

    /// <summary>
    ///     Test Method to check, if different expressions aren't mutated (in case of non-Math classes)
    /// </summary>
    [TestMethod]
    [DataRow("MyClass")]
    [DataRow("MyMath")]
    public void ShouldNotMutateOtherClasses(string className)
    {
        var original = MethodSwapsTestData.First()[0];
        var target = new MathMutator();

        var result = target.ApplyMutations(GenerateClassCallExpression(className, original.ToString()), null);

        result.ShouldBeEmpty();
    }

    /// <summary>
    ///     Explicit test: verifies that using static import, Floor(5.0) is mutated to Ceiling(5.0).
    /// </summary>
    [TestMethod]
    public void ShouldMutateStaticFloorToCeiling()
    {
        var sourceCode = @"
using System;
using static System.Math;
namespace TestApplication
{
    class Program
    {
        static void Main(string[] args)
        {
            Floor(5.0);
        }
    }
}";
        var tree = CSharpSyntaxTree.ParseText(sourceCode);
        var compilation = CSharpCompilation.Create("TestCompilation")
            .AddReferences(
                MetadataReference.CreateFromFile(typeof(object).Assembly.Location),
                MetadataReference.CreateFromFile(typeof(Math).Assembly.Location))
            .AddSyntaxTrees(tree);
        var semanticModel = compilation.GetSemanticModel(tree);
        var expression = tree.GetRoot().DescendantNodes().OfType<InvocationExpressionSyntax>().Single();
        var target = new MathMutator();
        var result = target.ApplyMutations(expression, semanticModel).ToList();

        result.Count.ShouldBe(1);
        var mutatedMethodName = ((IdentifierNameSyntax)((InvocationExpressionSyntax)result[0].ReplacementNode).Expression).Identifier.ValueText;
        Enum.Parse<MathExpression>(mutatedMethodName).ShouldBe(MathExpression.Ceiling);
    }

    /// <summary>
    ///     Explicit test: verifies that using static import, Exp(5.0) is mutated to Log(5.0).
    /// </summary>
    [TestMethod]
    public void ShouldMutateStaticExpToLog()
    {
        var sourceCode = @"
using System;
using static System.Math;
namespace TestApplication
{
    class Program
    {
        static void Main(string[] args)
        {
            Exp(5.0);
        }
    }
}";
        var tree = CSharpSyntaxTree.ParseText(sourceCode);
        var compilation = CSharpCompilation.Create("TestCompilation")
            .AddReferences(
                MetadataReference.CreateFromFile(typeof(object).Assembly.Location),
                MetadataReference.CreateFromFile(typeof(Math).Assembly.Location))
            .AddSyntaxTrees(tree);
        var semanticModel = compilation.GetSemanticModel(tree);
        var expression = tree.GetRoot().DescendantNodes().OfType<InvocationExpressionSyntax>().Single();
        var target = new MathMutator();
        var result = target.ApplyMutations(expression, semanticModel).ToList();

        result.Count.ShouldBe(1);
        var mutatedMethodName = ((IdentifierNameSyntax)((InvocationExpressionSyntax)result[0].ReplacementNode).Expression).Identifier.ValueText;
        Enum.Parse<MathExpression>(mutatedMethodName).ShouldBe(MathExpression.Log);
    }


    public static IEnumerable<object[]> MethodSwapsTestData
    {
        get
        {
            foreach (var trigonometricHyperbolicTestData in TrigonometricHyperbolicTestData)
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
    }


    private static IEnumerable<object[]> TrigonometricHyperbolicTestData
    {
        get
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
