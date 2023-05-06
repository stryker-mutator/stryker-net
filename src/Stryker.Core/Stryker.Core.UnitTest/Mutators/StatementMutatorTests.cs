using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Shouldly;
using Stryker.Core.Mutators;
using Xunit;

namespace Stryker.Core.UnitTest.Mutators;

public class StatementMutatorTests : TestBase
{
    [Fact]
    public void ShouldBeMutationlevelStandard()
    {
        var target = new StatementMutator();
        target.MutationLevel.ShouldBe(MutationLevel.Standard);
    }

    [Theory]
    [InlineData("return;")]
    [InlineData("break;")]
    [InlineData("continue;")]
    [InlineData("goto test;")]
    [InlineData("throw null;")]
    [InlineData("yield break;")]
    [InlineData("yield return 0;")]
    [InlineData("await null;")]
    private void ShouldMutate(string statementString)
    {
        var source = $@"class Test {{
                void Method() {{
                    {statementString}
                }}
            }}";

        var tree = CSharpSyntaxTree.ParseText(source).GetRoot();

        var statement = tree.DescendantNodes().OfType<StatementSyntax>().Where(s => s is not BlockSyntax).First();

        var target = new StatementMutator();

        var result = target.ApplyMutations(statement).ToList();

        var mutation = result.ShouldHaveSingleItem();

        mutation.ReplacementNode.ShouldBeOfType(typeof(EmptyStatementSyntax));
        mutation.DisplayName.ShouldBe("Statement mutation");
    }

    [Fact]
    private void ShouldNotMutate()
    {
        SyntaxTree tree = CSharpSyntaxTree.ParseText($@"
namespace Test
{{
    class Program
    {{
        static int Method()
        {{
            int variable = 0;

            Method(out var x);

            if (x is Type v)
            {{
                return;
            }}

            switch (x)
            {{
                case 1:
                    return;
                    break;
                    continue;
                    goto X;
                    throw x;
            }}

            return 1;
        }}
    }}
}}");
        var statements = tree.GetRoot()
            .DescendantNodes()
            .OfType<StatementSyntax>();

        var target = new StatementMutator();

        var result = statements.SelectMany(target.ApplyMutations).ToList();

        result.ShouldBeEmpty();
    }
}
