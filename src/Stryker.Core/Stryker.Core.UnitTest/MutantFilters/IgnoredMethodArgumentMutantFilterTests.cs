using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Shouldly;
using Stryker.Abstractions;
using Stryker.Configuration.Options;
using Stryker.Configuration.Options.Inputs;
using Stryker.Core.MutantFilters;
using Stryker.Core.Mutants;

namespace Stryker.Core.UnitTest.MutantFilters;

[TestClass]
public class IgnoredMethodArgumentMutantFilterTests : TestBase
{
    [TestMethod]
    public void ShouldHaveName()
    {
        var target = new IgnoredMethodArgumentMutantFilter() as IMutantFilter;
        target.DisplayName.ShouldBe("method argument filter");
    }

    [TestMethod]
    public void ShouldHaveType()
    {
        var target = new IgnoredMethodArgumentMutantFilter() as IMutantFilter;
        target.Type.ShouldBe(MutantFilter.IgnoreMethodArgument);
    }

    // The mutant is the smallest expression that covers the anchor.
    [TestMethod]
    [DataRow("IgnoredMethod(true);", "true")]
    [DataRow("x.IgnoredMethod(true);", "true")]
    [DataRow("x?.IgnoredMethod(true);", "true")]
    [DataRow("IgnoredMethod<int>(true);", "true")]
    [DataRow("IgnoredMethod(x == 1 ? true : false);", "==")]
    [DataRow("IgnoredMethod(y => y > 1);", "y > 1")]
    [DataRow("Other(IgnoredMethod(true));", "true")]
    [DataRow("await Task.Delay(1).IgnoredMethod(false);", "false")]
    public void ShouldFilterMutantsInsideTheArgumentsOfAnIgnoredCall(string statement, string anchor)
    {
        var mutant = BuildMutant<ExpressionSyntax>(statement, anchor);

        var filteredMutants = new IgnoredMethodArgumentMutantFilter().FilterMutants(new[] { mutant }, null, OptionsIgnoring("IgnoredMethod"));

        filteredMutants.ShouldNotContain(mutant);
    }

    // None of these mutants sits inside the argument list of the ignored call. ignore-methods filters the first three.
    [TestMethod]
    [DataRow("SomeMethod(true).IgnoredMethod(false);", "true")]
    [DataRow("await store.ListAsync(o => o.Total > limit).IgnoredMethod(false);", "o.Total > limit")]
    [DataRow("x = IgnoredMethod(true);", "x = IgnoredMethod(true)")]
    [DataRow("var b = IgnoredMethod(true) == y;", "==")]
    [DataRow("Other(true).Then(IgnoredMethod(false));", "true")]
    [DataRow("NotIgnored(true);", "true")]
    public void ShouldKeepExpressionMutantsOutsideTheArgumentsOfAnIgnoredCall(string statement, string anchor)
    {
        var mutant = BuildMutant<ExpressionSyntax>(statement, anchor);

        var filteredMutants = new IgnoredMethodArgumentMutantFilter().FilterMutants(new[] { mutant }, null, OptionsIgnoring("IgnoredMethod"));

        filteredMutants.ShouldContain(mutant);
    }

    [TestMethod]
    [DataRow("IgnoredMethod(true);")]
    [DataRow("await store.SaveAsync(id, true).IgnoredMethod(false);")]
    public void ShouldKeepTheStatementThatHoldsAnIgnoredCall(string statement)
    {
        var mutant = BuildMutant<StatementSyntax>(statement, statement);

        var filteredMutants = new IgnoredMethodArgumentMutantFilter().FilterMutants(new[] { mutant }, null, OptionsIgnoring("IgnoredMethod"));

        filteredMutants.ShouldContain(mutant);
    }

    [TestMethod]
    public void ShouldKeepTheBlockThatOnlyHoldsIgnoredCalls()
    {
        var mutant = BuildMutant<BlockSyntax>("{ IgnoredMethod(true); }", "IgnoredMethod(true);");

        var filteredMutants = new IgnoredMethodArgumentMutantFilter().FilterMutants(new[] { mutant }, null, OptionsIgnoring("IgnoredMethod"));

        filteredMutants.ShouldContain(mutant);
    }

    [TestMethod]
    public void ShouldKeepTheAwaitAroundAnIgnoredCall()
    {
        // The negate-expression mutant of a while condition.
        var mutant = BuildMutant<AwaitExpressionSyntax>("while (await cursor.MoveNextAsync(ct).IgnoredMethod(false)) { }", "await");

        var filteredMutants = new IgnoredMethodArgumentMutantFilter().FilterMutants(new[] { mutant }, null, OptionsIgnoring("IgnoredMethod"));

        filteredMutants.ShouldContain(mutant);
    }

    [TestMethod]
    public void ShouldKeepMutantsInTheBodyOfAMethodDeclarationWithAnIgnoredName()
    {
        var source = "class Test { void IgnoredMethod(bool b) { Foo(true); } }";
        var mutant = BuildMutantFromSource<ExpressionSyntax>(source, "true");

        var filteredMutants = new IgnoredMethodArgumentMutantFilter().FilterMutants(new[] { mutant }, null, OptionsIgnoring("IgnoredMethod"));

        filteredMutants.ShouldContain(mutant);
    }

    [TestMethod]
    [DataRow("ConfigureAwait", true)]
    [DataRow("Configure*", true)]
    [DataRow("*Await", true)]
    [DataRow("configureawait", true)]
    [DataRow("Configure", false)]
    [DataRow("ListAsync", false)]
    [DataRow("", false)]
    public void ShouldMatchCallsWithTheIgnoreMethodsPatternSyntax(string pattern, bool shouldSkipMutant)
    {
        var mutant = BuildMutant<ExpressionSyntax>("await store.ListAsync(ct).ConfigureAwait(false);", "false");

        var filteredMutants = new IgnoredMethodArgumentMutantFilter().FilterMutants(new[] { mutant }, null, OptionsIgnoring(pattern));

        if (shouldSkipMutant)
        {
            filteredMutants.ShouldNotContain(mutant);
        }
        else
        {
            filteredMutants.ShouldContain(mutant);
        }
    }

    [TestMethod]
    public void ShouldKeepEveryMutantWhenNoMethodIsIgnored()
    {
        var mutant = BuildMutant<ExpressionSyntax>("await store.ListAsync(ct).ConfigureAwait(false);", "false");

        var filteredMutants = new IgnoredMethodArgumentMutantFilter().FilterMutants(new[] { mutant }, null, new StrykerOptions());

        filteredMutants.ShouldContain(mutant);
    }

    // The scenario that ignore-methods: ["ConfigureAwait"] gets wrong: only the ConfigureAwait arguments may be ignored.
    [TestMethod]
    public void ShouldOnlyFilterTheConfigureAwaitArgumentsOfAnAwaitedChain()
    {
        var source = """
            public sealed class OrderService
            {
                public async Task<int> ReviewLargeOrdersAsync(decimal limit, CancellationToken ct)
                {
                    var large = await store.ListAsync(o => o.Total > limit, ct).ConfigureAwait(false);
                    foreach (var order in large)
                    {
                        await store.SaveAsync(order.Id, order.Total > 2 * limit, ct).ConfigureAwait(false);
                        await notifier.NotifyAsync(order.Id, ct).ConfigureAwait(false);
                    }

                    return large.Count;
                }

                public static async Task<int> CountAsync(ICursor cursor, CancellationToken ct)
                {
                    var count = 0;
                    while (await cursor.MoveNextAsync(ct).ConfigureAwait(false))
                    {
                        count++;
                    }

                    return count;
                }
            }
            """;
        var root = CSharpSyntaxTree.ParseText(source).GetRoot();
        var configureAwaitArguments = root.DescendantNodes().OfType<LiteralExpressionSyntax>()
            .Where(l => l.IsKind(SyntaxKind.FalseLiteralExpression))
            .Select(AsMutant)
            .ToList();
        var otherMutants = new IMutant[]
        {
            AsMutant(Find<BinaryExpressionSyntax>(root, "o.Total > limit")),
            AsMutant(Find<BinaryExpressionSyntax>(root, "order.Total > 2 * limit")),
            AsMutant(Find<BinaryExpressionSyntax>(root, "2 * limit")),
            AsMutant(Find<ExpressionStatementSyntax>(root, "await store.SaveAsync")),
            AsMutant(Find<ExpressionStatementSyntax>(root, "await notifier.NotifyAsync")),
            AsMutant(Find<AwaitExpressionSyntax>(root, "await cursor.MoveNextAsync")),
            AsMutant(Find<ForEachStatementSyntax>(root, "foreach").Statement),
        };

        var filteredMutants = new IgnoredMethodArgumentMutantFilter()
            .FilterMutants(configureAwaitArguments.Concat(otherMutants).ToList(), null, OptionsIgnoring("ConfigureAwait"))
            .ToList();

        configureAwaitArguments.Count.ShouldBe(4);
        filteredMutants.ShouldBe(otherMutants);
    }

    private static StrykerOptions OptionsIgnoring(string pattern) => new()
    {
        IgnoredMethodArguments = new IgnoreMethodArgumentsInput { SuppliedInput = new[] { pattern } }.Validate()
    };

    private static IMutant BuildMutant<T>(string statement, string anchor) where T : SyntaxNode =>
        BuildMutantFromSource<T>($"class Test {{ async Task StubMethod() {{ {statement} }} }}", anchor);

    private static IMutant BuildMutantFromSource<T>(string source, string anchor) where T : SyntaxNode =>
        AsMutant(Find<T>(CSharpSyntaxTree.ParseText(source).GetRoot(), anchor));

    // The smallest node of type T that covers the first occurrence of the anchor.
    private static T Find<T>(SyntaxNode root, string anchor) where T : SyntaxNode =>
        root.FindNode(new TextSpan(root.ToFullString().IndexOf(anchor, System.StringComparison.Ordinal), anchor.Length), getInnermostNodeForTie: true)
            .FirstAncestorOrSelf<T>()
            .ShouldNotBeNull($"no {typeof(T).Name} covers '{anchor}'");

    private static IMutant AsMutant(SyntaxNode node) => new Mutant { Mutation = new Mutation { OriginalNode = node } };
}
