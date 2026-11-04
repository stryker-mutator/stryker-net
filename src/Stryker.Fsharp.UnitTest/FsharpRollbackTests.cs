using System.IO.Abstractions;
using System.Reflection;
using System.Runtime.Loader;
using FSharp.Compiler.Diagnostics;
using FSharp.Compiler.Text;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.FSharp.Core;
using Shouldly;
using Stryker.Abstractions.Exceptions;
using Stryker.Core.InjectedHelpers;
using Stryker.Fsharp.Mutants;
using Stryker.Fsharp.ProjectComponents;
using Stryker.Fsharp.UnitTest.Fixtures;
using Stryker.Utilities.Buildalyzer;

namespace Stryker.Fsharp.UnitTest;

/// <summary>Compile-error rollback: real FCS compiles of hand-crafted splices, plus fake diagnostics where FCS cannot be steered.</summary>
[TestClass]
public class FsharpRollbackTests
{
    private static readonly CodeInjection Injector = new();
    private static string Ns => Injector.HelperNamespace;

    [TestMethod]
    public void Rollback_DiagnosticInsideSplice_MarksMutantBuildError()
    {
        const string source = "module Lib\nlet good () = 1\nlet bad () = 2\n";
        using var project = new TempFsharpProject(("Lib.fs", source));
        var leaf = LeafWithSplices(project, source, "Lib.fs", ("1", "10", 0), ("2", "\"oops\"", 1));
        var compiler = new Compiler(project);

        var result = FsharpRollback.Run([leaf], Ns, compiler.Compile);

        result.RolledBackIds.ShouldBe([1]);
        result.Compiled.Success.ShouldBeTrue();
        Call(result.Compiled.Assembly, project, "good", activeMutant: 0).ShouldBe(10, "the surviving mutant is compiled in");
        Call(result.Compiled.Assembly, project, "bad", activeMutant: 1).ShouldBe(2, "the rolled back mutant is gone");
    }

    [TestMethod]
    public void Rollback_UnmappedDiagnostic_DropsFileMutants()
    {
        const string source = "module Lib\nlet good () = 1\nlet other () = 2\n";
        using var project = new TempFsharpProject(("Lib.fs", source));
        var leaf = LeafWithSplices(project, source, "Lib.fs", ("1", "10", 0), ("2", "20", 1));
        var compiler = new Compiler(project);
        // while the file carries splices, report an error at the top of the file: outside every splice
        FsharpCompileResult ErrorOutsideSplices(IReadOnlyDictionary<string, string> overrides) =>
            overrides.ContainsKey(leaf.FullPath)
                ? new FsharpCompileResult(false, [], [Error(leaf.FullPath, 1, 0)])
                : compiler.Compile(overrides);

        var result = FsharpRollback.Run([leaf], Ns, ErrorOutsideSplices);

        result.RolledBackIds.ShouldBe([0, 1], ignoreOrder: true);
        result.Compiled.Success.ShouldBeTrue();
    }

    [TestMethod]
    public void Rollback_AlwaysFailingSplice_BoundedRetries()
    {
        var source = "module Lib\n" + string.Concat(Enumerable.Range(0, 60).Select(i => $"let v{i} = {i}\n"));
        var leaf = FsharpParse.Leaf(source, Path.GetFullPath("Lib.fs"));
        for (var i = 0; i < 60; i++)
        {
            var start = source.IndexOf($"let v{i} = {i}\n", StringComparison.Ordinal) + $"let v{i} = ".Length;
            leaf.Splices.Add(new TextSplice(i, FsharpLocations.ToLocation(RangeOf(source, start, i.ToString().Length, leaf.FullPath), source), i.ToString(), "x"));
        }

        var calls = 0;
        // never compiles: one error per attempt at the first splice still in the text, else at the top of the file
        FsharpCompileResult AlwaysFail(IReadOnlyDictionary<string, string> overrides)
        {
            calls++;
            var lines = overrides.GetValueOrDefault(leaf.FullPath, source).Split('\n');
            var index = Array.FindIndex(lines, l => l.Contains("MutantControl", StringComparison.Ordinal));
            return new FsharpCompileResult(false, [], [index < 0 ? Error(leaf.FullPath, 1, 0) : Error(leaf.FullPath, index + 1, lines[index].IndexOf('(', StringComparison.Ordinal))]);
        }

        var exception = Should.Throw<GeneralStrykerException>(() => FsharpRollback.Run([leaf], Ns, AlwaysFail));

        calls.ShouldBeGreaterThan(1);
        calls.ShouldBeLessThanOrEqualTo(FsharpRollback.MaxAttempts);
        exception.Message.ShouldContain("Lib.fs");
    }

    [TestMethod]
    public void Rollback_ErrorInNonMutatedFile_IsFatal()
    {
        const string sourceA = "module A\nlet one = 1\n";
        using var project = new TempFsharpProject(("A.fs", sourceA), ("B.fs", "module B\nlet two : int = 2\n"));
        var leafA = LeafWithSplices(project, sourceA, "A.fs", ("1", "2", 0));
        var compiler = new Compiler(project);

        var exception = Should.Throw<GeneralStrykerException>(() => FsharpRollback.Run([leafA], Ns, BreakB));

        exception.Message.ShouldContain("B.fs");
        // B.fs carries no splice: nothing to roll back, the run fails with the diagnostics
        FsharpCompileResult BreakB(IReadOnlyDictionary<string, string> overrides) =>
            compiler.Compile(new Dictionary<string, string>(overrides) { [Path.Combine(project.Directory, "B.fs")] = "module B\nlet two : int = \"no\"\n" });
    }

    [TestMethod]
    public void Orchestrator_ManyMutantsOneFile_SingleCompileSucceeds()
    {
        const string source = "module Lib\nlet calc () = 10 + 5\nlet flag () = if true then 1 else 2\nlet other () = if false then 3 else 4\n";
        using var project = new TempFsharpProject(("Lib.fs", source));
        var leaf = FsharpParse.Leaf(source, Path.Combine(project.Directory, "Lib.fs"));
        new FsharpMutantOrchestrator(Ns).Mutate(leaf).ShouldBe(2);
        // nested pair, hand-crafted: outer 10 + 5 -> 10 * 5, inner 5 -> 7
        leaf.Splices.Add(NestedSplice(leaf, source, "10 + 5", 100, "10 * 5"));
        leaf.Splices.Add(NestedSplice(leaf, source, "5", 101, "7"));
        var compiler = new Compiler(project);
        var compiles = 0;
        var text = FsharpMutantOrchestrator.ApplySplices(source, leaf.Splices, Ns);

        var result = FsharpRollback.Run([leaf], Ns, overrides => { compiles++; return compiler.Compile(overrides); });

        compiles.ShouldBe(1);
        result.RolledBackIds.ShouldBeEmpty();
        text.ShouldContain("IsActive 101");
        int Run(string fn, int id) => Call(result.Compiled.Assembly, project, fn, id);
        Run("calc", -1).ShouldBe(15, "neither: original");
        Run("calc", 100).ShouldBe(50, "outer only: outer mutation with inner original");
        Run("calc", 101).ShouldBe(17, "inner only: inner mutation");
        Run("calc", 0).ShouldBe(15);
        Run("flag", -1).ShouldBe(1);
        Run("flag", 0).ShouldBe(2);
        Run("other", -1).ShouldBe(4);
        Run("other", 1).ShouldBe(3);
    }

    [TestMethod]
    public void Rollback_ErrorInsideNestedSplice_MapsToInnermostSplice()
    {
        const string source = "module Lib\nlet calc () = 10 + 5\n";
        using var project = new TempFsharpProject(("Lib.fs", source));
        var leaf = FsharpParse.Leaf(source, Path.Combine(project.Directory, "Lib.fs"));
        leaf.Splices.Add(NestedSplice(leaf, source, "10 + 5", 0, "10 * 5"));
        leaf.Splices.Add(NestedSplice(leaf, source, "5", 1, "\"oops\""));
        var compiler = new Compiler(project);

        // report an error inside the inner guard of the outer mutated branch, which is also inside the outer splice
        FsharpCompileResult ErrorInsideInner(IReadOnlyDictionary<string, string> overrides)
        {
            var text = overrides.GetValueOrDefault(leaf.FullPath);
            return text != null && text.Contains("IsActive 1")
                ? new FsharpCompileResult(false, [], [Error(leaf.FullPath, 2, text.Split('\n')[1].IndexOf("IsActive 1", StringComparison.Ordinal))])
                : compiler.Compile(overrides);
        }

        var result = FsharpRollback.Run([leaf], Ns, ErrorInsideInner);

        result.RolledBackIds.Distinct().ShouldBe([1]);
        Call(result.Compiled.Assembly, project, "calc", activeMutant: 0).ShouldBe(50, "the outer mutant survives");
    }

    private static TextSplice NestedSplice(FsharpFileLeaf leaf, string source, string text, int id, string mutated)
    {
        var start = source.LastIndexOf(text, StringComparison.Ordinal);
        return new TextSplice(id, FsharpLocations.ToLocation(RangeOf(source, start, text.Length, leaf.FullPath), source), text, mutated);
    }

    private static FsharpFileLeaf LeafWithSplices(TempFsharpProject project, string source, string fileName, params (string Original, string Mutated, int Id)[] splices)
    {
        var leaf = FsharpParse.Leaf(source, Path.Combine(project.Directory, fileName));
        var searchFrom = 0;
        foreach (var (original, mutated, id) in splices)
        {
            // each splice replaces the next "= <original>" literal
            var start = source.IndexOf($"= {original}\n", searchFrom, StringComparison.Ordinal) + 2;
            searchFrom = start;
            leaf.Splices.Add(new TextSplice(id, FsharpLocations.ToLocation(RangeOf(source, start, original.Length, leaf.FullPath), source), original, mutated));
        }

        return leaf;
    }

    private static FSharp.Compiler.Text.Range RangeOf(string source, int start, int length, string file)
    {
        (int Line, int Column) PositionOf(int offset)
        {
            var before = source[..offset];
            return (before.Count(c => c == '\n') + 1, offset - (before.LastIndexOf('\n') + 1));
        }

        var (startLine, startColumn) = PositionOf(start);
        var (endLine, endColumn) = PositionOf(start + length);
        return RangeModule.mkRange(file, PositionModule.mkPos(startLine, startColumn), PositionModule.mkPos(endLine, endColumn));
    }

    private static FSharpDiagnostic Error(string file, int line, int column) =>
        FSharpDiagnostic.Create(FSharpDiagnosticSeverity.Error, "boom", 1, RangeModule.mkRange(file, PositionModule.mkPos(line, column), PositionModule.mkPos(line, column + 1)), FSharpOption<string>.None, FSharpOption<string>.None);

    /// <summary>Loads the compiled assembly with the helper beside it, activates a mutant and calls a unit -> int function of module Lib.</summary>
    private static int Call(byte[] assembly, TempFsharpProject project, string function, int activeMutant)
    {
        var directory = Path.Combine(project.Directory, "load-" + Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(directory);
        var assemblyPath = Path.Combine(directory, project.AnalyzerResult.GetAssemblyName() + ".dll");
        File.WriteAllBytes(assemblyPath, assembly);
        File.WriteAllBytes(Path.Combine(directory, $"{Ns}.StrykerHelpers.dll"), FsharpHelperAssembly.Compile(Injector, project.AnalyzerResult));
        var context = new AssemblyLoadContext(null, isCollectible: true);
        context.Resolving += (ctx, name) =>
        {
            var candidate = Path.Combine(directory, name.Name + ".dll");
            return File.Exists(candidate) ? ctx.LoadFromAssemblyPath(candidate) : null;
        };
        var lib = context.LoadFromAssemblyPath(assemblyPath);
        var helper = context.LoadFromAssemblyPath(Path.Combine(directory, $"{Ns}.StrykerHelpers.dll"));
        helper.GetType($"{Ns}.MutantControl")!.GetField("ActiveMutant", BindingFlags.Public | BindingFlags.Static)!.SetValue(null, activeMutant);
        return (int)lib.GetType("Lib")!.GetMethod(function)!.Invoke(null, null)!;
    }

    /// <summary>Compiles through the real <see cref="FsharpCompilingProcess"/> with the helper assembly referenced.</summary>
    private sealed class Compiler
    {
        private readonly FsharpCompilingProcess _process;
        private readonly string _helperPath;

        public Compiler(TempFsharpProject project)
        {
            _process = new FsharpCompilingProcess(project.AnalyzerResult, new FileSystem(), NullLogger.Instance);
            _helperPath = Path.Combine(project.Directory, $"{Ns}.StrykerHelpers.dll");
            File.WriteAllBytes(_helperPath, FsharpHelperAssembly.Compile(Injector, project.AnalyzerResult));
        }

        public FsharpCompileResult Compile(IReadOnlyDictionary<string, string> overrides) => _process.Compile(overrides, [_helperPath]);
    }
}
