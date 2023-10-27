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
    public class IsPatternExpressionMutatorTests : TestBase
    {
        [Fact]
        public void ShouldMutateIsToIsNot()
        {
            var target = new IsPatternExpressionMutator();

            var expression = GenerateSimpleConstantPattern(false);

            var mutation = target.ApplyMutations(expression, null).First();

            mutation.OriginalNode.ShouldBeOfType<ConstantPatternSyntax>();
            mutation.ReplacementNode.ShouldBeOfType<UnaryPatternSyntax>();
            mutation.DisplayName.ShouldBe("Equality mutation");
        }

        [Fact]
        public void ShouldMutateIsNotToIs()
        {
            var target = new IsPatternExpressionMutator();

            var expression = GenerateSimpleConstantPattern(true);

            var mutation = target.ApplyMutations(expression, null).First();

            mutation.OriginalNode.ShouldBeOfType<UnaryPatternSyntax>();
            mutation.ReplacementNode.ShouldBeOfType<ConstantPatternSyntax>();
            mutation.DisplayName.ShouldBe("Equality mutation");
        }

        [Theory]
        [InlineData(">", new[] { SyntaxKind.LessThanToken, SyntaxKind.GreaterThanEqualsToken })]
        [InlineData("<", new[] { SyntaxKind.GreaterThanToken, SyntaxKind.LessThanEqualsToken })]
        [InlineData(">=", new[] { SyntaxKind.GreaterThanToken, SyntaxKind.LessThanToken })]
        [InlineData("<=", new[] { SyntaxKind.GreaterThanToken, SyntaxKind.LessThanToken })]
        public void ShouldMutateRelationalPattern(string @operator, SyntaxKind[] mutated)
        {
            var target = new IsPatternExpressionMutator();

            var expression = GenerateWithRelationalPattern(@operator);

            var result = target.ApplyMutations(expression, null).Skip(1).ToList();

            result.ForEach(mutation =>
            {
                mutation.OriginalNode.ShouldBeOfType<RelationalPatternSyntax>();
                mutation.ReplacementNode.ShouldBeOfType<RelationalPatternSyntax>();
                mutation.DisplayName.ShouldBe($"Equality mutation");
            });

            result
                .Select(mutation => (RelationalPatternSyntax)mutation.ReplacementNode)
                .Select(pattern => pattern.OperatorToken.Kind())
                .ShouldBe(mutated, true);
        }

        [Theory]
        [InlineData("and", new[] { SyntaxKind.OrPattern })]
        [InlineData("or", new[] { SyntaxKind.AndPattern })]
        public void ShouldMutateLogicalPattern(string @operator, SyntaxKind[] mutated)
        {
            var target = new IsPatternExpressionMutator();

            var expression = GenerateWithBinaryPattern(@operator);

            var result = target.ApplyMutations(expression, null).Skip(1).ToList();

            result.ForEach(mutation =>
            {
                mutation.OriginalNode.ShouldBeOfType<BinaryPatternSyntax>();
                mutation.ReplacementNode.ShouldBeOfType<BinaryPatternSyntax>();
                mutation.DisplayName.ShouldBe($"Logical mutation");
            });

            result
                .Select(mutation => (BinaryPatternSyntax)mutation.ReplacementNode)
                .Select(pattern => pattern.Kind())
                .ShouldBe(mutated, true);
        }

        [Theory]
        [MemberData(nameof(GenerateNotSupportedPatterns))]
        public void ShouldNotMutateNotSupportedPatterns(IsPatternExpressionSyntax expression)
        {
            var target = new IsPatternExpressionMutator();

            var result = target.ApplyMutations(expression, null).Skip(1).ToList();

            result.ShouldBeEmpty();
        }

        private IsPatternExpressionSyntax GenerateSimpleConstantPattern(bool isNotPattern)
        {
            SyntaxTree tree = CSharpSyntaxTree.ParseText($@"
using System;

namespace TestApplication
{{
    class Program
    {{
        static void Main(string[] args)
        {{
            var a = 1 is {(isNotPattern ? "not" : string.Empty)} 1;
        }}
    }}
}}");
            var isPatternExpression = tree.GetRoot()
                .DescendantNodes()
                .OfType<IsPatternExpressionSyntax>()
                .Single();

            return isPatternExpression;
        }

        private IsPatternExpressionSyntax GenerateWithRelationalPattern(string @operator)
        {
            SyntaxTree tree = CSharpSyntaxTree.ParseText($@"
using System;

namespace TestApplication
{{
    class Program
    {{
        static void Main(string[] args)
        {{
            var a = 1 is ({@operator} 1);
        }}
    }}
}}");
            var isPatternExpression = tree.GetRoot()
                .DescendantNodes()
                .OfType<IsPatternExpressionSyntax>()
                .Single();

            return isPatternExpression;
        }

        private IsPatternExpressionSyntax GenerateWithBinaryPattern(string pattern)
        {
            SyntaxTree tree = CSharpSyntaxTree.ParseText($@"
using System;

namespace TestApplication
{{
    class Program
    {{
        static void Main(string[] args)
        {{
            var a = 1 is (1 {pattern} 2);
        }}
    }}
}}");
            var isPatternExpression = tree.GetRoot()
                .DescendantNodes()
                .OfType<IsPatternExpressionSyntax>()
                .Single();

            return isPatternExpression;
        }

        public static IEnumerable<object[]> GenerateNotSupportedPatterns()
        {
            IsPatternExpressionSyntax GetExpressionFromTree(SyntaxTree tree)
            {
                return tree.GetRoot()
                    .DescendantNodes()
                    .OfType<IsPatternExpressionSyntax>()
                    .Single();
            }

            yield return new[]
            {
                GetExpressionFromTree(CSharpSyntaxTree.ParseText($@"
                    using System;

                    namespace TestApplication
                    {{
                        class Program
                        {{
                            static void Main(string[] args)
                            {{
                                var a = 1 is (1);
                            }}
                        }}
                    }}"
                ))
            };

            yield return new[]
            {
                GetExpressionFromTree(CSharpSyntaxTree.ParseText($@"
                    using System;

                    namespace TestApplication
                    {{
                        class Program
                        {{
                            static void Main(string[] args)
                            {{
                                var a = 1 is (int b);
                            }}
                        }}
                    }}"
                ))
            };

            yield return new[]
            {
                GetExpressionFromTree(CSharpSyntaxTree.ParseText($@"
                    using System;

                    namespace TestApplication
                    {{
                        class Program
                        {{
                            static void Main(string[] args)
                            {{
                                var a = 1 is (int);
                            }}
                        }}
                    }}"
                ))
            };

            yield return new[]
            {
                GetExpressionFromTree(CSharpSyntaxTree.ParseText($@"
                    using System;

                    namespace TestApplication
                    {{
                        class Program
                        {{
                            static void Main(string[] args)
                            {{
                                var a = ""test"" is ({{ Length: 1 }});
                            }}
                        }}
                    }}"
                ))
            };

            yield return new[]
            {
                GetExpressionFromTree(CSharpSyntaxTree.ParseText($@"
                    using System;

                    namespace TestApplication
                    {{
                        class Program
                        {{
                            static void Main(string[] args)
                            {{
                                var a = new[] {{ 1, 2 }} is ([1, _]);
                            }}
                        }}
                    }}"
                ))
            };
        }
    }
}
