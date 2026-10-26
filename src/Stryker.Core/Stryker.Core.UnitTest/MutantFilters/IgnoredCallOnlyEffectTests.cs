using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Shouldly;
using Stryker.Abstractions;
using Stryker.Configuration.Options;
using Stryker.Configuration.Options.Inputs;
using Stryker.Core.MutantFilters;
using Stryker.Core.Mutants;
using Stryker.Core.Mutators;

namespace Stryker.Core.UnitTest.MutantFilters;

[TestClass]
public class IgnoredCallOnlyEffectTests : TestBase
{
    private static readonly IMutator[] Mutators =
    [
        new BinaryExpressionMutator(), new RelationalPatternMutator(), new BinaryPatternMutator(), new BlockMutator(),
        new BooleanMutator(), new ConditionalExpressionMutator(), new AssignmentExpressionMutator(),
        new PrefixUnaryMutator(), new PostfixUnaryMutator(), new NegateConditionMutator(), new InitializerMutator(),
        new ObjectCreationMutator(), new StatementMutator(), new NullCoalescingExpressionMutator(),
        new IsPatternExpressionMutator(),
    ];

    // body: statements of `int M(int count, bool flag, bool verbose, string error, List<int> list, List<int> other)`;
    // original: the mutated node's text; type: the mutator, or null for every mutant of that node
    [TestMethod]
    // a guard whose branches only log
    [DataRow("if (count > 0) _logger.LogWarning(\"{N}\", count); return 0;", "count > 0", null, true)]
    [DataRow("if (flag) _logger.LogInformation(\"on\"); else _logger.LogWarning(\"off\"); return 0;", "flag", null, true)]
    [DataRow("if (flag && verbose) { _logger.LogInformation(\"x\"); } return 0;", "flag && verbose", null, true)]
    [DataRow("if (Interlocked.CompareExchange(ref _state, 1, 0) == 0) _logger.LogWarning(\"x\"); return 0;", "Interlocked.CompareExchange(ref _state, 1, 0) == 0", "Equality", true)]
    [DataRow("if (_seen.Add(count) && verbose) _logger.LogWarning(\"x\"); return 0;", "_seen.Add(count) && verbose", "Logical", true)]
    [DataRow("if (other != null && other.Count > 0) _logger.LogWarning(\"x\"); return 0;", "other.Count > 0", "Equality", true)]
    [DataRow("if (flag) { if (verbose) { _logger.LogDebug(\"x\"); } } return 0;", "flag", null, true)]
    [DataRow("if (other != null && other.Count > 0) _logger.LogWarning(other.Count.ToString()); return 0;", "other.Count > 0", "Equality", true)]
    [DataRow("if (other != null && other.Count > 0) _logger.LogWarning(other.Count.ToString()); return 0;", "other != null", "Equality", false)]
    [DataRow("if (other != null || other.Count > 0) _logger.LogWarning(other.Count.ToString()); return 0;", "other.Count > 0", "Equality", false)]
    [DataRow("if (other != null && other.Count > 0) _logger.LogWarning(other.Count.ToString()); return 0;", "other != null && other.Count > 0", null, false)]
    // a local only logged: the counter, its guard, and a level picked for the log call
    [DataRow("var errors = 0; foreach (var i in list) { if (i < 0) { errors++; } } if (errors > 0) _logger.LogWarning(\"{N}\", errors); return 0;", "errors++", null, true)]
    [DataRow("var errors = 0; foreach (var i in list) { if (i < 0) { errors++; } } if (errors > 0) _logger.LogWarning(\"{N}\", errors); return 0;", "i < 0", null, true)]
    [DataRow("var errors = 0; foreach (var i in list) { if (i < 0) { errors++; } } if (errors > 0) _logger.LogWarning(\"{N}\", errors); return 0;", "{ errors++; }", null, true)]
    [DataRow("var total = 0; foreach (var i in list) { total += i; } _logger.LogInformation(\"{T}\", total); return 0;", "total += i", null, true)]
    [DataRow("var level = flag ? 1 : 2; _logger.Log(level, \"x\"); return 0;", "flag ? 1 : 2", null, true)]
    // a scope's state only reaches the logger
    [DataRow("using var scope = _logger.BeginScope(new Dictionary<string, object> { [\"a\"] = count }); return 0;", "new Dictionary<string, object> { [\"a\"] = count }", null, true)]
    // anything that also returns, writes state, or can throw stays mutated
    [DataRow("if (count > 0) { _logger.LogWarning(\"x\"); return 1; } return 0;", "count > 0", null, false)]
    [DataRow("if (count > 0) _logger.LogWarning(\"x\"); else Save(); return 0;", "count > 0", null, false)]
    [DataRow("var total = 0; if (count > 0) { _logger.LogWarning(\"x\"); total++; } return total;", "count > 0", null, false)]
    [DataRow("var total = 0; if (count > 0) { _logger.LogWarning(\"x\"); total++; } return total;", "total++", null, false)]
    [DataRow("var total = 0; total++; Save(total); _logger.LogWarning(\"{T}\", total); return 0;", "total++", null, false)]
    [DataRow("if (error is not null) _logger.LogWarning(error.Length.ToString()); return 0;", "error is not null", null, false)]
    [DataRow("if (string.IsNullOrEmpty(error)) { } else _logger.LogWarning(error.Trim()); return 0;", "string.IsNullOrEmpty(error)", null, false)]
    [DataRow("if (list.Count > 0) _logger.LogWarning(\"{F}\", list[0]); return 0;", "list.Count > 0", null, false)]
    [DataRow("if (other != null && other.Count > 0) _logger.LogWarning(\"x\"); return 0;", "other != null", null, false)]
    [DataRow("if (other != null && other.Count > 0) _logger.LogWarning(\"x\"); return 0;", "other != null && other.Count > 0", "Logical", false)]
    [DataRow("if (verbose && _seen.Add(count)) _logger.LogWarning(\"x\"); return 0;", "verbose && _seen.Add(count)", "Logical", false)]
    [DataRow("var rate = count == 0 ? 0d : 1 / (double)count; _logger.LogInformation(\"{R}\", rate); return 0;", "count == 0 ? 0d : 1 / (double)count", "Conditional", false)]
    [DataRow("var level = Check() ? 1 : 2; _logger.Log(level, \"x\"); return 0;", "Check() ? 1 : 2", "Conditional", false)]
    [DataRow("if (count > 0) _logger.LogWarning(\"x\"); if (count > 0) _state++; return 0;", "_state++", null, false)]
    [DataRow("var n = count + 1; if (n > 0) _logger.LogWarning(\"{N}\", n); return n;", "count + 1", null, false)]
    [DataRow("var n = count + 1; _logger.LogWarning(\"{N}\", n); return 0;", "count + 1", null, true)]
    public void ShouldOnlyFilterMutantsWhoseOnlyEffectIsAnIgnoredCall(string body, string original, string type, bool filtered)
    {
        var options = BuildOptions(ignoreMethodsEffects: true);
        var mutants = BuildMutants(body, original, type, options);

        var kept = new IgnoredMethodMutantFilter().FilterMutants(mutants, null, options).ToList();

        foreach (var mutant in mutants)
        {
            var label = $"{mutant.Mutation.DisplayName} on `{original}`";
            if (filtered)
            {
                kept.ShouldNotContain(mutant, $"{label} should have been filtered out.");
            }
            else
            {
                kept.ShouldContain(mutant, $"{label} should have been kept.");
            }
        }
    }

    [TestMethod]
    [DataRow("if (count > 0) _logger.LogWarning(\"{N}\", count); return 0;", "count > 0")]
    [DataRow("var errors = 0; foreach (var i in list) { if (i < 0) { errors++; } } if (errors > 0) _logger.LogWarning(\"{N}\", errors); return 0;", "errors++")]
    public void ShouldKeepMutantsOutsideIgnoredCallsWhenOptionIsOff(string body, string original)
    {
        var options = BuildOptions(ignoreMethodsEffects: false);
        var mutants = BuildMutants(body, original, null, options);

        var kept = new IgnoredMethodMutantFilter().FilterMutants(mutants, null, options).ToList();

        kept.ShouldBe(mutants);
    }

    private static StrykerOptions BuildOptions(bool ignoreMethodsEffects) => new()
    {
        MutationLevel = MutationLevel.Complete,
        IgnoredMethods = new IgnoreMethodsInput { SuppliedInput = ["Log*", "BeginScope"] }.Validate(),
        IgnoreMethodsEffects = ignoreMethodsEffects,
    };

    private static List<Mutant> BuildMutants(string body, string original, string type, StrykerOptions options)
    {
        var source = "using System.Collections.Generic; using System.Threading; class C { int _state; HashSet<int> _seen; ILogger _logger;"
            + " int M(int count, bool flag, bool verbose, string error, List<int> list, List<int> other) { " + body + " } void Save(int n = 0) { } bool Check() => true; }";
        var tree = CSharpSyntaxTree.ParseText(source);
        var model = CSharpCompilation.Create("t", [tree], [MetadataReference.CreateFromFile(typeof(object).Assembly.Location)])
            .GetSemanticModel(tree);
        var mutants = tree.GetRoot().DescendantNodes()
            .Where(n => n.ToString() == original)
            .SelectMany(n => Mutators.SelectMany(m => m.Mutate(n, model, options)))
            .Where(m => type is null || m.Type.ToString() == type)
            .Select(m => new Mutant { Mutation = m })
            .ToList();
        mutants.ShouldNotBeEmpty();
        return mutants;
    }
}
