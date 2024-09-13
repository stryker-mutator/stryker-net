using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Shouldly;
using Stryker.Core.Mutators;

namespace Stryker.Core.UnitTest.Mutators
{
    [TestClass]
    public class SwitchExpressionMutatorTests : TestBase
    {
        [TestMethod]
        [DataRow(">", new[] { SyntaxKind.LessThanToken, SyntaxKind.GreaterThanEqualsToken })]
        [DataRow("<", new[] { SyntaxKind.GreaterThanToken, SyntaxKind.LessThanEqualsToken })]
        [DataRow(">=", new[] { SyntaxKind.GreaterThanToken, SyntaxKind.LessThanToken })]
        [DataRow("<=", new[] { SyntaxKind.GreaterThanToken, SyntaxKind.LessThanToken })]
        public void ShouldMutateRelationalPattern(string @operator, SyntaxKind[] mutated)
        {
            var target = new SwitchExpressionMutator();

            var expression = GenerateWithRelationalPattern(@operator);

            var result = target.ApplyMutations(expression, null).ToList();

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

        [TestMethod]
        [DataRow("and", new[] { SyntaxKind.OrPattern })]
        [DataRow("or", new[] { SyntaxKind.AndPattern })]
        public void ShouldMutateLogicalPattern(string @operator, SyntaxKind[] mutated)
        {
            var target = new SwitchExpressionMutator();

            var expression = GenerateWithBinaryPattern(@operator);

            var result = target.ApplyMutations(expression, null).ToList();

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

        [TestMethod]
        [DynamicData(nameof(GenerateNotSupportedPatterns))]
        public void ShouldNotMutateNotSupportedPatterns(SwitchExpressionSyntax expression)
        {
            var target = new SwitchExpressionMutator();

            var result = target.ApplyMutations(expression, null).ToList();

            result.ShouldBeEmpty();
        }

        private SwitchExpressionSyntax GenerateWithRelationalPattern(string @operator)
        {
            var tree = CSharpSyntaxTree.ParseText($@"
using System;

namespace TestApplication
{{
    class Program
    {{
        static void Main(string[] args)
        {{
            var a = 1 switch
            {{
                {@operator} 0 => 1
            }};
        }}
    }}
}}");
            var switchExpression = tree.GetRoot()
                .DescendantNodes()
                .OfType<SwitchExpressionSyntax>()
                .Single();

            return switchExpression;
        }

        private SwitchExpressionSyntax GenerateWithBinaryPattern(string pattern)
        {
            var tree = CSharpSyntaxTree.ParseText($@"
using System;

namespace TestApplication
{{
    class Program
    {{
        static void Main(string[] args)
        {{
            var a = 1 switch
            {{
                1 {pattern} 2 => 1
            }};
        }}
    }}
}}");
            var switchExpression = tree.GetRoot()
                .DescendantNodes()
                .OfType<SwitchExpressionSyntax>()
                .Single();

            return switchExpression;
        }

        public static IEnumerable<object[]> GenerateNotSupportedPatterns
        {
            get
            {
                SwitchExpressionSyntax GetExpressionFromTree(SyntaxTree tree)
                {
                    return tree.GetRoot()
                        .DescendantNodes()
                        .OfType<SwitchExpressionSyntax>()
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
                                    var a = 1 switch
                                    {{
                                        1 => 2
                                    }};
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
                                    var a = 1 switch
                                    {{
                                        _ => 2
                                    }};
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
                                    var a = 1 switch
                                    {{
                                        int b => 1
                                    }};
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
                                    var a = 1 switch
                                    {{
                                        int => 1
                                    }};
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
                                    var a = ""test"" switch
                                    {{
                                        {{ Length: 1 }} => 1
                                    }};
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
                                    var a = new[] {{ 1, 2 }} switch
                                    {{
                                        [1, _] => 1
                                    }};
                                }}
                            }}
                        }}"
                    ))
                };
            }
        }
    }
}
