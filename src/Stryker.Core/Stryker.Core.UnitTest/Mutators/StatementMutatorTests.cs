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
    public class StatementMutatorTests
    {
        [Fact]
        public void ShouldBeMutationlevelStandard()
        {
            var target = new StatementMutator();
            target.MutationLevel.ShouldBe(MutationLevel.Standard);
        }

        public static IEnumerable<object[]> MutableStatements => new List<object[]>
        {
            new object[] { SyntaxFactory.BreakStatement() },
            new object[] { SyntaxFactory.ContinueStatement() },
            new object[] { SyntaxFactory.YieldStatement(SyntaxKind.YieldBreakStatement) },
            new object[]
            {
                SyntaxFactory.YieldStatement(
                    SyntaxKind.YieldReturnStatement,
                    SyntaxFactory.LiteralExpression(SyntaxKind.NumericLiteralExpression, SyntaxFactory.Literal(1))
                )
            },
            new object[] { SyntaxFactory.ReturnStatement() },
            new object[] {
                SyntaxFactory.IfStatement(
                    SyntaxFactory.LiteralExpression(SyntaxKind.TrueLiteralExpression),
                    SyntaxFactory.EmptyStatement()
                )
            },
        };

        [Theory]
        [MemberData(nameof(MutableStatements))]
        private void ShouldMutate(StatementSyntax statement)
        {
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
        delegate void TestDelegate();

        static int Method()
        {{
            int variable = 0;
            TestDelegate lambda = () => {{ }};
            TestDelegate anonymousMethod = delegate () {{ }};
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
}
