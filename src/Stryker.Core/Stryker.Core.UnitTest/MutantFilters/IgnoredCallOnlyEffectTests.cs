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
    [DataRow("if (other != null && count > 0) _logger.LogWarning(\"{N}\", other.Count); return 0;", "count > 0", "Equality", true)]
    [DataRow("if (other != null && other.Count > 0) _logger.LogWarning(\"{N}\", other.Count); return 0;", "other.Count > 0", "Equality", false)]
    [DataRow("if (other != null && other.Count > 0) _logger.LogWarning(\"{N}\", other.Count); return 0;", "other != null", "Equality", false)]
    [DataRow("if (other != null || other.Count > 0) _logger.LogWarning(\"{N}\", other.Count); return 0;", "other.Count > 0", "Equality", false)]
    [DataRow("if (other != null && other.Count > 0) _logger.LogWarning(\"{N}\", other.Count); return 0;", "other != null && other.Count > 0", null, false)]
    // a local only logged: the counter, its guard, and a level picked for the log call
    [DataRow("var errors = 0; foreach (var i in list) { if (i < 0) { errors++; } } if (errors > 0) _logger.LogWarning(\"{N}\", errors); return 0;", "errors++", null, true)]
    [DataRow("var errors = 0; foreach (var i in list) { if (i < 0) { errors++; } } if (errors > 0) _logger.LogWarning(\"{N}\", errors); return 0;", "i < 0", null, true)]
    [DataRow("var errors = 0; foreach (var i in list) { if (i < 0) { errors++; } } if (errors > 0) _logger.LogWarning(\"{N}\", errors); return 0;", "{ errors++; }", null, true)]
    [DataRow("var total = 0; foreach (var i in list) { total += i; } _logger.LogInformation(\"{T}\", total); return 0;", "total += i", null, true)]
    [DataRow("var level = flag ? 1 : 2; _logger.Log(level, \"x\"); return 0;", "flag ? 1 : 2", null, true)]
    // a scope's state only reaches the logger
    [DataRow("using var scope = _logger.BeginScope(new Dictionary<string, object> { [\"a\"] = count }); return 0;", "new Dictionary<string, object> { [\"a\"] = count }", null, true)]
    // an object passed to a call that is only followed by an ignored one is not an ignored argument
    [DataRow("Fetch(new List<int> { count }).IgnoredCheck(); return 0;", "new List<int> { count }", null, false)]
    [DataRow("Fetch(new Spec { A = count, B = verbose }).IgnoredCheck(); return 0;", "new Spec { A = count, B = verbose }", null, false)]
    // a guard that reads a bool copied from a validity check
    [DataRow("var ok = other.IsSuccess; if (ok) _logger.LogWarning(\"{V}\", other.Value); return 0;", "ok", null, false)]
    [DataRow("var found = _seen.TryGetValue(count, out var v); if (found) _logger.LogWarning(\"{N}\", v.Length); return 0;", "found", null, false)]
    [DataRow("var ok = other.IsSuccess; if (ok) _logger.LogWarning(\"x\"); return 0;", "ok", null, true)]
    [DataRow("var ok = IsValid(other); if (ok) _logger.LogWarning(\"{V}\", other.Value); return 0;", "ok", null, false)]
    [DataRow("var ok = IsValid(other); if (ok) _logger.LogWarning(\"x\"); return 0;", "ok", null, true)]
    [DataRow("if (IsValid(other)) _logger.LogWarning(\"{V}\", other.Value); return 0;", "IsValid(other)", null, false)]
    // a guard local built from a shape ValidatedBy does not list still carries what it reads
    [DataRow("var ok = flag ? IsValid(other) : false; if (ok) _logger.LogWarning(\"{V}\", other.Value); return 0;", "ok", null, false)]
    [DataRow("var ok = (bool)other.IsSuccess; if (ok) _logger.LogWarning(\"{V}\", other.Value); return 0;", "ok", null, false)]
    [DataRow("var ok = new Validator(other).IsSatisfied; if (ok) _logger.LogWarning(\"{V}\", other.Value); return 0;", "ok", null, false)]
    [DataRow("var found = _seen.TryGetValue(count, out var v); var ok = flag ? found : false; if (ok) _logger.LogWarning(\"{N}\", v.Length); return 0;", "ok", null, false)]
    // anything that also returns, writes state, or can throw stays mutated
    [DataRow("if (count > 0) { _logger.LogWarning(\"x\"); return 1; } return 0;", "count > 0", null, false)]
    [DataRow("if (count > 0) _logger.LogWarning(\"x\"); else Save(); return 0;", "count > 0", null, false)]
    [DataRow("var total = 0; if (count > 0) { _logger.LogWarning(\"x\"); total++; } return total;", "count > 0", null, false)]
    [DataRow("var total = 0; if (count > 0) { _logger.LogWarning(\"x\"); total++; } return total;", "total++", null, false)]
    [DataRow("var total = 0; total++; Save(total); _logger.LogWarning(\"{T}\", total); return 0;", "total++", null, false)]
    [DataRow("if (error is not null) _logger.LogWarning(\"{N}\", error.Length); return 0;", "error is not null", null, false)]
    [DataRow("if (string.IsNullOrEmpty(error)) { } else _logger.LogWarning(error.Trim()); return 0;", "string.IsNullOrEmpty(error)", null, false)]
    [DataRow("if (list.Count > 0) _logger.LogWarning(\"{F}\", list[0]); return 0;", "list.Count > 0", null, false)]
    [DataRow("if (other != null && other.Count > 0) _logger.LogWarning(\"x\"); return 0;", "other != null", null, false)]
    [DataRow("if (other != null && other.Count > 0) _logger.LogWarning(\"x\"); return 0;", "other != null && other.Count > 0", "Logical", false)]
    [DataRow("if (verbose && _seen.Add(count)) _logger.LogWarning(\"x\"); return 0;", "verbose && _seen.Add(count)", "Logical", false)]
    [DataRow("var rate = count == 0 ? 0d : 1 / (double)count; _logger.LogInformation(\"{R}\", rate); return 0;", "count == 0 ? 0d : 1 / (double)count", "Conditional", false)]
    [DataRow("var level = Check() ? 1 : 2; _logger.Log(level, \"x\"); return 0;", "Check() ? 1 : 2", "Conditional", false)]
    [DataRow("if (count > 0) _logger.LogWarning(\"x\"); if (count > 0) _state++; return 0;", "_state++", null, false)]
    [DataRow("var n = count + 1; if (n > 0) _logger.LogWarning(\"{N}\", n); return n;", "count + 1", null, false)]
    // overflow, division and remainder can throw
    [DataRow("var n = count + 1; _logger.LogWarning(\"{N}\", n); return 0;", "count + 1", null, false)]
    [DataRow("checked { var n = int.MaxValue; n--; _logger.LogWarning(\"{N}\", n); } return 0;", "n--", null, false)]
    [DataRow("decimal total = 0; foreach (var i in list) { total += i; } _logger.LogInformation(\"{T}\", total); return 0;", "total += i", null, false)]
    [DataRow("var n = 10 / count; _logger.LogWarning(\"{N}\", n); return 0;", "10 / count", null, false)]
    [DataRow("var n = 10 % count; _logger.LogWarning(\"{N}\", n); return 0;", "10 % count", null, false)]
    [DataRow("var total = 1; if (flag) { total /= count; } _logger.LogWarning(\"{N}\", total); return 0;", "flag", null, false)]
    [DataRow("var total = 1; if (flag) { total /= count; } _logger.LogWarning(\"{N}\", total); return 0;", "total /= count", null, false)]
    [DataRow("var n = 1; n++; _logger.LogWarning(\"{N}\", 1 / n); return 0;", "n++", null, false)]
    // a null test carried by a local
    [DataRow("var canLog = error != null; if (canLog) _logger.LogWarning(\"{N}\", error.Length); return 0;", "error != null", null, false)]
    // ignored calls whose receiver or arguments do something else
    [DataRow("if (flag) _logger.LogWarning(\"{N}\", ++_state); return 0;", "flag", null, false)]
    [DataRow("if (flag) _logger.LogWarning(count.ToString()); return 0;", "flag", null, false)]
    [DataRow("if (flag) _logger.LogWarning(\"n=\" + count); return 0;", "flag", null, true)]
    [DataRow("var n = 1; n++; _logger.LogWarning(\"{N}\", SaveAndReturn(n)); return 0;", "n++", null, false)]
    [DataRow("var n = 1; n++; _logger.LogWarning(\"{N}\", _state = n); return 0;", "n++", null, false)]
    [DataRow("if (flag && UpdateState().IgnoredCheck()) _logger.LogWarning(\"x\"); return 0;", "flag && UpdateState().IgnoredCheck()", "Logical", false)]
    // a field that shares its name with a local in a nested block
    [DataRow("{ var _state = 0; _logger.LogWarning(\"{N}\", _state); } _state++; return _state;", "_state++", null, false)]
    // an ignored call whose result is used
    [DataRow("var format = \"F0\"; if (flag) { format = \"F4\"; } return Describe(format).Length;", "flag", null, false)]
    [DataRow("var name = \"a\"; if (flag) { name = \"b\"; } using var scope = _logger.BeginScope(name); return 0;", "flag", null, true)]
    [DataRow("var name = \"a\"; if (flag) { name = \"b\"; } using (_logger.BeginScope(name)) { } return 0;", "flag", null, true)]
    // a guard that reads a null test carrier, and a write made under a null test
    [DataRow("var canLog = error != null; if (canLog) _logger.LogWarning(\"{N}\", error.Length); return 0;", "canLog", null, false)]
    [DataRow("var show = true; if (error == null) show = false; if (show) _logger.LogWarning(\"{N}\", error.Length); return 0;", "show", null, false)]
    [DataRow("if (error == null) verbose = false; if (verbose) _logger.LogWarning(\"{N}\", error.Length); return 0;", "verbose", null, false)]
    [DataRow("{ var canLog = error != null; if (canLog) _logger.LogWarning(\"{N}\", error.Length); } { var canLog = true; } return 0;", "canLog", null, false)]
    // a guard that makes a member read or an out variable valid
    [DataRow("if (_seen.TryGetValue(count, out var v)) _logger.LogWarning(\"{N}\", v.Length); return 0;", "_seen.TryGetValue(count, out var v)", null, false)]
    [DataRow("if (other.IsSuccess) _logger.LogWarning(\"{N}\", other.Value); return 0;", "other.IsSuccess", null, false)]
    [DataRow("var show = true; if (error == null) show = false; if (show) _logger.LogWarning(\"{N}\", error.Length); return 0;", "false", null, false)]
    // an awaited ignored call is not inert
    [DataRow("if (flag) await _logger.LogWarningAsync(\"x\"); return 0;", "flag", null, false)]
    // nameof is a constant
    [DataRow("if (flag) _logger.LogWarning(\"{P}\", nameof(count)); return 0;", "flag", null, true)]
    public void ShouldOnlyFilterMutantsWhoseOnlyEffectIsAnIgnoredCall(string body, string original, string type, bool filtered)
    {
        var options = BuildOptions(ignoreMethodsEffects: true);
        var mutants = BuildMutants(body, original, type, options);

        var kept = Filter(mutants, options);

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

        var kept = Filter(mutants, options);

        kept.ShouldBe(mutants);
    }

    [TestMethod]
    public void ShouldReportEffectSkipsUnderTheirOwnReason()
    {
        var options = BuildOptions(ignoreMethodsEffects: true);
        var guard = BuildMutants("if (count > 0) _logger.LogWarning(\"{N}\", count); return 0;", "count > 0", null, options);
        var argument = BuildMutants("_logger.LogWarning(\"{N}\", count > 0); return 0;", "count > 0", null, options);
        var filter = new BroadcastMutantFilter([new IgnoredMethodMutantFilter(), new IgnoreMethodsEffectsMutantFilter()]);

        filter.FilterMutants([.. guard, .. argument], null, options).ShouldBeEmpty();

        guard.ShouldAllBe(mutant => mutant.ResultStatusReason == "Removed by ignore-methods-effects filter");
        argument.ShouldAllBe(mutant => mutant.ResultStatusReason == "Removed by method filter");
    }

    private static List<Mutant> Filter(List<Mutant> mutants, StrykerOptions options) =>
        new IgnoreMethodsEffectsMutantFilter()
            .FilterMutants(new IgnoredMethodMutantFilter().FilterMutants(mutants, null, options), null, options)
            .Cast<Mutant>()
            .ToList();

    private static StrykerOptions BuildOptions(bool ignoreMethodsEffects) => new()
    {
        MutationLevel = MutationLevel.Complete,
        IgnoredMethods = new IgnoreMethodsInput { SuppliedInput = ["Log*", "BeginScope", "IgnoredCheck", "Describe"] }.Validate(),
        IgnoreMethodsEffects = ignoreMethodsEffects,
    };

    private static List<Mutant> BuildMutants(string body, string original, string type, StrykerOptions options)
    {
        var source = "using System.Collections.Generic; using System.Threading; class C { int _state; HashSet<int> _seen; ILogger _logger;"
            + " int M(int count, bool flag, bool verbose, string error, List<int> list, List<int> other) { " + body + " } void Save(int n = 0) { } bool Check() => true;"
            + " int SaveAndReturn(int n) => n; C UpdateState() => this; bool IgnoredCheck() => true; string Describe(string format) => format; }";
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
