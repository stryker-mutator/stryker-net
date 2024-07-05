using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Shouldly;
using Stryker.Core.Mutators;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Stryker.Core.UnitTest.Mutators
{
    [TestClass]
    public class StatementMutatorTests : TestBase
    {
        [TestMethod]
        public void ShouldBeMutationlevelStandard()
        {
            var target = new StatementMutator();
            target.MutationLevel.ShouldBe(MutationLevel.Standard);
        }

        [TestMethod]
        [DataRow("return;")]
        [DataRow("break;")]
        [DataRow("continue;")]
        [DataRow("goto test;")]
        [DataRow("throw null;")]
        [DataRow("yield break;")]
        [DataRow("yield return 0;")]
        [DataRow("await null;")]
        public void ShouldMutate(string statementString)
        {
            var source = $@"class Test {{
                void Method() {{
                    {statementString}
                }}
            }}";

            var tree = CSharpSyntaxTree.ParseText(source).GetRoot();

            var statement = tree.DescendantNodes().OfType<StatementSyntax>().Where(s => s is not BlockSyntax).First();

            var target = new StatementMutator();

            var result = target.ApplyMutations(statement, null).ToList();

            var mutation = result.ShouldHaveSingleItem();

            mutation.ReplacementNode.ShouldBeOfType(typeof(EmptyStatementSyntax));
            mutation.DisplayName.ShouldBe("Statement mutation");
        }

        [TestMethod]
        public void ShouldNotMutate()
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

            var result = statements.SelectMany(statement => target.ApplyMutations(statement, null)).ToList();

            result.ShouldBeEmpty();
        }
    }
}
