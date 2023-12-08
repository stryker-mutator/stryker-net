using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Shouldly;
using Stryker.Core.Mutants;
using Stryker.Core.Mutators;
using Xunit;

namespace Stryker.Core.UnitTest.Mutators
{
    public class BlockMutatorTests
    {
        [Fact]
        public void ShouldMutateNonEmptyConstructorOnClass()
        {
            var source = @"
class Program
{
    int doesNotNeedToBeInitialized;

    Program()
    {
        this.doesNotNeedToBeInitialized = 42;
    }
}";

            var mutation = GetMutations(source).ShouldHaveSingleItem();
            mutation.ReplacementNode.ChildNodes().ShouldBeEmpty();
        }

        [Fact]
        public void ShouldNotMutateStructConstructorAssignments()
        {
            var source = @"
struct Program
{
    int mustBeInitialized;
    bool alsoThisMustBeInitialized;

    Program(int value)
    {
        this.mustBeInitialized = value;

        if (value == 0)
        {
            this.alsoThisMustBeInitialized = true;
        }
        else
        {
            this.alsoThisMustBeInitialized = false;
        }
    }
}";

            GetMutations(source).ShouldBeEmpty();
        }

        [Fact]
        public void ShouldNotMutateSwitchCasesBlock()
        {
            var source = @"
struct Program
{
    int mustBeInitialized;
    bool alsoThisMustBeInitialized;

    Program(int value)
    {
        switch(value)
        {
            default:
            {
                value++;
            }
        }
    }
}";
            // there should be only a mutation for the whole method body
            GetMutations(source).ShouldHaveSingleItem();
        }

        [Fact]
        public void ShouldMutateLocalFunctionsInStructConstructors()
        {
            var source = @"
struct Program
{
    int mustBeInitialized;

    Program(int value)
    {
        int CalculateValue()
        {
            int value;
            value = 42; // Try to fool with this assignment
            return value;
        }

        this.mustBeInitialized = CalculateValue();
    }
}";

            GetMutations(source).Count().ShouldBe(
                1,
                "Should mutate the local function and only the local function");
        }

        [Fact]
        public void ShouldMutateStructConstructorNonAssignmentsAtRoot()
        {
            var source = @"
struct Program
{
    Program(int value)
    {
        throw new Exception();
    }
}";

            GetMutations(source).Count().ShouldBe(1);
        }

        [Fact]
        public void ShouldMutateStructConstructorNonAssignmentChild()
        {
            var source = @"
struct Program
{
    int mustBeInitialized;

    Program(int value)
    {
        if (value == 0)
        {
            throw new Exception();
        }

        this.mustBeInitialized = value;
    }
}";

            GetMutations(source).Count().ShouldBe(1);
        }

        [Fact]
        public void ShouldMutateVoidReturnsAsEmptyInMethod()
        {
            var source = @"
class Program
{
    void Method(bool input)
    {
        if (input)
        {
            return;
        }
    }
}";

            GetMutations(source).ShouldAllBe(mutation => !mutation.ReplacementNode.ChildNodes().Any());
        }

        [Fact]
        public void ShouldMutateVoidReturnsAsEmptyInLocalFunction()
        {
            var source = @"
class Program
{
    int Method(bool input)
    {
        void LocalFunction()
        {
            return;
        }

        return 42;
    }
}";

            var mutation = GetMutations(source)
                .Where(mutation => mutation.OriginalNode.Parent is LocalFunctionStatementSyntax
                {
                    Identifier: { Text: "LocalFunction" }
                })
                .ShouldHaveSingleItem();
            mutation.ReplacementNode.ChildNodes().ShouldBeEmpty();
        }

        [Fact]
        public void ShouldNotMutateAlreadyEmptyBlocks()
        {
            // E.g. empty constructors bodies and method overrides are totally valid
            // and produce false positives if mutated.

            var source = @"
class Program
{
    private Program()
    {
        // Nothing to do
    }

    void Method()
    {
        // Nothing to do
    }
}";
            GetMutations(source).ShouldBeEmpty();
        }

        [Fact]
        public void ShouldNotMutateInfiniteWhileLoops()
        {
            var source = @"
class Program
{
    void Method()
    {
        while (true)
        {
            // Should not be mutated or it will always time out
            break; // Just to make this more realistic...
        }
    }
}";
            GetMutations(source)
                .Where(mutation => mutation.OriginalNode.Parent is WhileStatementSyntax)
                .ShouldBeEmpty();
        }

        private static IEnumerable<Mutation> GetMutations(string source)
        {
            var statements = CSharpSyntaxTree
                .ParseText(source)
                .GetRoot()
                .DescendantNodes()
                .OfType<BlockSyntax>();

            var target = new BlockMutator();
            return statements.SelectMany(statement => target.ApplyMutations(statement, null));
        }

        private static void AssertDefaultReturn(Mutation mutation)
        {
            var block = mutation.ReplacementNode.ShouldBeOfType<BlockSyntax>();
            var returnStatement = block.Statements.OfType<ReturnStatementSyntax>().ShouldHaveSingleItem();
            returnStatement.Expression?.Kind().ShouldBe(SyntaxKind.DefaultLiteralExpression);
        }
    }
}
