using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Shouldly;
using Stryker.Core.Mutators;

namespace Stryker.Core.UnitTest.Mutators;

[TestClass]
public class IsPatternExpressionMutatorTests : TestBase
{
    [TestMethod]
    public void ShouldMutateIsToIsNot()
    {
        var target = new IsPatternExpressionMutator();

        var expression = GenerateSimpleConstantPattern(false);

        var mutation = target.ApplyMutations(expression, null).First();

        mutation.OriginalNode.ToString().ShouldBe("1 is 1");
        mutation.ReplacementNode.ToString().ShouldBe("1 is not 1");
        mutation.DisplayName.ShouldBe("Equality mutation");
    }

    [TestMethod]
    public void ShouldMutateIsNotToIs()
    {
        var target = new IsPatternExpressionMutator();

        var expression = GenerateSimpleConstantPattern(true);

        var mutation = target.ApplyMutations(expression, null).First();

        mutation.OriginalNode.ToString().ShouldBe("1 is not 1");
        mutation.ReplacementNode.ToString().ShouldBe("1 is 1");
        mutation.DisplayName.ShouldBe("Equality mutation");
    }

    [TestMethod]
    [DynamicData(nameof(GenerateNotSupportedPatterns))]
    public void ShouldNotMutateNotSupportedPatterns(IsPatternExpressionSyntax expression)
    {
        var target = new IsPatternExpressionMutator();

        var result = target.ApplyMutations(expression, null).Skip(1).ToList();

        result.ShouldBeEmpty();
    }

    private IsPatternExpressionSyntax GenerateSimpleConstantPattern(bool isNotPattern)
    {
        var tree = CSharpSyntaxTree.ParseText($@"
using System;

namespace TestApplication
{{
    class Program
    {{
        static void Main(string[] args)
        {{
            var a = 1 is{(isNotPattern ? " not" : string.Empty)} 1;
        }}
    }}
}}");
        var isPatternExpression = tree.GetRoot()
            .DescendantNodes()
            .OfType<IsPatternExpressionSyntax>()
            .Single();

        return isPatternExpression;
    }

    public static IEnumerable<object[]> GenerateNotSupportedPatterns
    {
        get
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
