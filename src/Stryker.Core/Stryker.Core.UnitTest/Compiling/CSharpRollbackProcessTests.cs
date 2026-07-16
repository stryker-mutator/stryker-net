using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.IO.Pipes;
using System.Linq;
using System.Reflection;
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Emit;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Shouldly;
using Stryker.Abstractions;
using Stryker.Abstractions.Exceptions;
using Stryker.Configuration.Options;
using Stryker.Core.Compiling;
using Stryker.Core.InjectedHelpers;
using Stryker.Core.Mutants;
using Stryker.Core.MutationTest;
using Stryker.Core.ProjectComponents.SourceProjects;

namespace Stryker.Core.UnitTest.Compiling;

internal class CompilerWrapper(CSharpCompilation compilation) : ICompilationContent, IBaselineCompiler
{
    private CSharpCompilation _compilation = compilation;

    // Test seam: force the baseline outcome to exercise the mutation-induced branch, which real
    // (runtime-wrapped) mutators cannot currently produce in a mutant-free tree.
    public bool? FailsWithoutInstrumentationOverride { get; set; }

    public IEnumerable<SyntaxTree> SyntaxTrees => _compilation.SyntaxTrees;

    public void ReplaceSyntaxTree(SyntaxTree original, SyntaxTree updated) => _compilation = _compilation.ReplaceSyntaxTree(original, updated);

    public bool FailsWithoutInstrumentation(SyntaxTree erroringTree)
    {
        if (FailsWithoutInstrumentationOverride.HasValue)
        {
            return FailsWithoutInstrumentationOverride.Value;
        }

        SyntaxTree erroringBaseline = null;
        var unmutatedTrees = new List<SyntaxTree>();
        foreach (var sourceTree in _compilation.SyntaxTrees)
        {
            var cleaned = MutantPlacer.RemoveAllMutations(sourceTree);
            if (sourceTree == erroringTree)
            {
                erroringBaseline = cleaned;
            }

            unmutatedTrees.Add(cleaned);
        }

        var baseline = _compilation.RemoveAllSyntaxTrees().AddSyntaxTrees(unmutatedTrees);
        return erroringBaseline != null && MutantPlacer.HasCompileError(baseline, erroringBaseline);
    }

    public EmitResult Emit(Stream stream) => _compilation.Emit(stream);
}

[TestClass]
public class CSharpRollbackProcessTests : TestBase
{
    private readonly SyntaxAnnotation _ifEngineMarker = new("Injector", "IfInstrumentationEngine");
    private readonly SyntaxAnnotation _conditionalEngineMarker = new("Injector", "ConditionalInstrumentationEngine");

    private SyntaxAnnotation GetMutationIdMarker(int id) => new("MutationId", id.ToString());

    private SyntaxAnnotation GetMutationTypeMarker(Mutator type) => new("MutationType", type.ToString());

    [TestMethod]
    public void RollbackProcess_ShouldRollbackError_RollbackedCompilationShouldCompile()
    {
        var syntaxTree = CSharpSyntaxTree.ParseText(
            """
            using System;

            namespace ExampleProject
            {
                public class Calculator
                {
                    public int ActiveMutation = 1;

                    public string Subtract(string first, string second)
                    {
                        if(ActiveMutation == 1) {
                            return first - second; // this will not compile

                        } else {
                            return first + second;
                        }
                    }
                }
            }
            """);
        var ifStatement = syntaxTree
            .GetRoot()
            .DescendantNodes()
            .First(x => x is IfStatementSyntax);
        var annotatedSyntaxTree = syntaxTree.GetRoot()
            .ReplaceNode(
                ifStatement,
                ifStatement.WithAdditionalAnnotations(GetMutationIdMarker(1), _ifEngineMarker)
            ).SyntaxTree;

        var compiler = CSharpCompilation.Create("TestCompilation",
            syntaxTrees: new Collection<SyntaxTree>() { annotatedSyntaxTree },
            options: new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary),
            references: new List<PortableExecutableReference>() {
                MetadataReference.CreateFromFile(typeof(object).Assembly.Location),
                MetadataReference.CreateFromFile(typeof(Environment).Assembly.Location)
            });

        var target = new CSharpRollbackProcess();

        using var ms = new MemoryStream();
        var compileResult = compiler.Emit(ms);
        var compilerWrapper = new CompilerWrapper(compiler);
        target.RollbackMutationsInError(compilerWrapper, compileResult.Diagnostics, ICSharpRollbackProcess.Mode.Normal, false);

        var rolledBackResult = compilerWrapper.Emit(ms);

        rolledBackResult.Success.ShouldBeTrue();
    }

    [TestMethod]
    public void RollbackProcess_ShouldReportBaselineFailure_WhenErrorIsInUnmutatedFile()
    {
        // An unmutated file that does not compile on its own (e.g. a source generator
        // produced no output) must be reported as a baseline failure, not as an internal
        // Stryker rollback error: its diagnostics cannot have been caused by mutation.
        // Two distinct diagnostics so the message's joined code list (and its separator)
        // is exercised: NoSuchType -> CS0246, Missing.Symbol -> CS0103.
        var syntaxTree = CSharpSyntaxTree.ParseText(
            """
            namespace ExampleProject
            {
                public class Anchor
                {
                    public object Broken() { NoSuchType local; return Missing.Symbol; }
                }
            }
            """,
            path: "Anchor.cs");

        var compilerWrapper = BaselineFailureCompiler(syntaxTree);

        var target = new CSharpRollbackProcess();
        using var ms = new MemoryStream();
        var compileResult = compilerWrapper.Emit(ms);
        compileResult.Success.ShouldBeFalse();

        var exception = Should.Throw<CompilationException>(() =>
            target.RollbackMutationsInError(compilerWrapper, compileResult.Diagnostics,
                ICSharpRollbackProcess.Mode.Normal, false));

        exception.Message.ShouldContain("not caused by mutation");
        exception.Message.ShouldContain("Anchor.cs");
        exception.Message.ShouldContain("CS0103");
        exception.Message.ShouldContain("CS0246");
        exception.Message.ShouldContain(", "); // both codes joined with a separator
        exception.Message.ShouldNotContain("Internal error");
    }

    [TestMethod]
    public void RollbackProcess_ShouldReportBaselineFailure_WithGenericLabel_WhenFileHasNoPath()
    {
        // Same baseline failure, but the erroring tree has no FilePath (in-memory): the
        // message must use a generic label, not an empty quoted path. Exercises the
        // string.IsNullOrEmpty(FilePath) branch.
        var syntaxTree = CSharpSyntaxTree.ParseText(
            """
            namespace ExampleProject { public class Anchor { public object Broken() => Missing.Symbol; } }
            """);

        var compilerWrapper = BaselineFailureCompiler(syntaxTree);

        var target = new CSharpRollbackProcess();
        using var ms = new MemoryStream();
        var compileResult = compilerWrapper.Emit(ms);
        compileResult.Success.ShouldBeFalse();

        var exception = Should.Throw<CompilationException>(() =>
            target.RollbackMutationsInError(compilerWrapper, compileResult.Diagnostics,
                ICSharpRollbackProcess.Mode.Normal, false));

        exception.Message.ShouldContain("not caused by mutation");
        exception.Message.ShouldContain("an unmutated file"); // generic label, not a quoted path
        exception.Message.ShouldNotContain("''");
        exception.Message.ShouldNotContain("Internal error");
    }

    [TestMethod]
    public void RollbackProcess_ShouldReportInternalError_WhenErroringTreeStillHasMutants()
    {
        // A tree that DOES carry a mutant and fails to compile: in LastChance mode rollback
        // cannot recover, and because the erroring tree still has mutants this is a genuine
        // Stryker internal error (not a baseline failure) - the message must stay "Internal error".
        var syntaxTree = CSharpSyntaxTree.ParseText(
            """
            namespace ExampleProject
            {
                public class Calculator
                {
                    public int ActiveMutation = 1;
                    public string Ok(string a, string b)
                    {
                        if (ActiveMutation == 1) { return a + b; } else { return b + a; }
                    }
                    public object Broken() => Missing.Symbol;
                }
            }
            """,
            path: "Mixed.cs");
        var ifStatement = syntaxTree.GetRoot().DescendantNodes().First(x => x is IfStatementSyntax);
        var annotated = syntaxTree.GetRoot()
            .ReplaceNode(ifStatement, ifStatement.WithAdditionalAnnotations(GetMutationIdMarker(1), _ifEngineMarker))
            .SyntaxTree;

        var compilerWrapper = BaselineFailureCompiler(annotated);
        var target = new CSharpRollbackProcess();
        using var ms = new MemoryStream();
        var compileResult = compilerWrapper.Emit(ms);
        compileResult.Success.ShouldBeFalse();

        var exception = Should.Throw<CompilationException>(() =>
            target.RollbackMutationsInError(compilerWrapper, compileResult.Diagnostics,
                ICSharpRollbackProcess.Mode.LastChance, false));

        exception.Message.ShouldContain("Internal error");
        exception.Message.ShouldNotContain("not caused by mutation");
    }

    [TestMethod]
    public void RollbackProcess_ShouldReportInternalError_WhenErrorIsInStrykerInjectedHelper()
    {
        // A mutant-free tree can also be one of Stryker's OWN injected helpers (MutantControl
        // etc.), which are added with their resource name as the FilePath. If one of those fails
        // to compile it is purely an internal Stryker.NET problem, so it must NOT be reported as
        // a user-side baseline failure ("check your generator setup") - it keeps the internal
        // error path just as a mutant-carrying tree would.
        var helperName = CodeInjection.HelperFiles.First();
        var syntaxTree = CSharpSyntaxTree.ParseText(
            """
            namespace ExampleProject { public class Helper { public object Broken() => Missing.Symbol; } }
            """,
            path: helperName);

        var compilerWrapper = BaselineFailureCompiler(syntaxTree);

        var target = new CSharpRollbackProcess();
        using var ms = new MemoryStream();
        var compileResult = compilerWrapper.Emit(ms);
        compileResult.Success.ShouldBeFalse();

        var exception = Should.Throw<CompilationException>(() =>
            target.RollbackMutationsInError(compilerWrapper, compileResult.Diagnostics,
                ICSharpRollbackProcess.Mode.Normal, false));

        exception.Message.ShouldContain("Internal error");
        exception.Message.ShouldNotContain("not caused by mutation");
    }

    [TestMethod]
    public void RollbackProcess_ShouldNotReportBaselineFailure_WhenAnotherErroringTreeStillHasMutants()
    {
        // Cross-tree causality: a mutant in one tree can make Roslyn report the resulting compile
        // error in a DIFFERENT, mutant-free tree. While an erroring tree still carries a mutant
        // rollback can remove, a co-erroring mutant-free tree must NOT be reported as a baseline
        // failure - it is deferred so rolling back the mutant can clear it on the next compile pass.
        var mutatedTree = CSharpSyntaxTree.ParseText(
            """
            namespace ExampleProject
            {
                public class Calculator
                {
                    public int ActiveMutation = 1;
                    public string Subtract(string first, string second)
                    {
                        if (ActiveMutation == 1) { return first - second; } else { return first + second; }
                    }
                }
            }
            """,
            path: "Mutated.cs");
        var ifStatement = mutatedTree.GetRoot().DescendantNodes().First(x => x is IfStatementSyntax);
        var annotatedMutated = mutatedTree.GetRoot()
            .ReplaceNode(ifStatement, ifStatement.WithAdditionalAnnotations(GetMutationIdMarker(1), _ifEngineMarker))
            .SyntaxTree;

        // A separate mutant-free tree that also fails to compile.
        var unmutatedTree = CSharpSyntaxTree.ParseText(
            """
            namespace ExampleProject { public class Other { public object Broken() => Missing.Symbol; } }
            """,
            path: "Unmutated.cs");

        var compilerWrapper = BaselineFailureCompiler(annotatedMutated, unmutatedTree);
        var target = new CSharpRollbackProcess();
        using var ms = new MemoryStream();
        var compileResult = compilerWrapper.Emit(ms);
        compileResult.Success.ShouldBeFalse();

        // On a normal pass the mutant-free tree is deferred (not misreported as a baseline
        // failure); the mutant in the other tree is rolled back instead.
        var rolledBack = Should.NotThrow(() =>
            target.RollbackMutationsInError(compilerWrapper, compileResult.Diagnostics,
                ICSharpRollbackProcess.Mode.Normal, false));

        rolledBack.ShouldContain(1);
    }

    [TestMethod]
    public void FailsWithoutInstrumentation_IsFalse_WhenRemovingMutationsFixesTheBuild()
    {
        // A file that only fails to compile because of a mutation: stripping the mutation restores
        // a buildable baseline, so the tree does NOT fail without instrumentation (mutation-induced).
        var syntaxTree = CSharpSyntaxTree.ParseText(
            """
            namespace ExampleProject
            {
                public class Calculator
                {
                    public int ActiveMutation = 1;
                    public string Subtract(string first, string second)
                    {
                        if (ActiveMutation == 1) { return first - second; } else { return first + second; }
                    }
                }
            }
            """,
            path: "Mutated.cs");
        var ifStatement = syntaxTree.GetRoot().DescendantNodes().First(x => x is IfStatementSyntax);
        var annotated = syntaxTree.GetRoot()
            .ReplaceNode(ifStatement, ifStatement.WithAdditionalAnnotations(GetMutationIdMarker(1), _ifEngineMarker))
            .SyntaxTree;

        var compilerWrapper = BaselineFailureCompiler(annotated);
        using var ms = new MemoryStream();
        compilerWrapper.Emit(ms).Success.ShouldBeFalse();

        compilerWrapper.FailsWithoutInstrumentation(annotated).ShouldBeFalse();
    }

    [TestMethod]
    public void FailsWithoutInstrumentation_IsTrue_WhenTheFailureIsIndependentOfMutation()
    {
        // A file that fails to compile with no mutation involved (a missing symbol): stripping
        // mutations changes nothing, so the tree still fails on the baseline - not mutation-induced.
        var syntaxTree = CSharpSyntaxTree.ParseText(
            """
            namespace ExampleProject { public class Anchor { public object Broken() => Missing.Symbol; } }
            """,
            path: "Anchor.cs");
        var compilerWrapper = BaselineFailureCompiler(syntaxTree);
        using var ms = new MemoryStream();
        compilerWrapper.Emit(ms).Success.ShouldBeFalse();

        compilerWrapper.FailsWithoutInstrumentation(syntaxTree).ShouldBeTrue();
    }

    [TestMethod]
    public void FailsWithoutInstrumentation_IsCorrelatedToTheGivenTree_NotTheWholeCompilation()
    {
        // Two mutant-free trees: one compiles, one has a baseline error. The check must correlate to
        // the queried tree - the clean tree does NOT fail just because another tree fails on the
        // baseline (otherwise a mutation-induced failure could be misreported as baseline).
        var clean = CSharpSyntaxTree.ParseText(
            "namespace ExampleProject { public class Ok { public int Value => 1; } }", path: "Clean.cs");
        var broken = CSharpSyntaxTree.ParseText(
            "namespace ExampleProject { public class Bad { public object Broken() => Missing.Symbol; } }", path: "Broken.cs");
        var compilerWrapper = BaselineFailureCompiler(clean, broken);

        compilerWrapper.FailsWithoutInstrumentation(clean).ShouldBeFalse();
        compilerWrapper.FailsWithoutInstrumentation(broken).ShouldBeTrue();
    }

    [TestMethod]
    public void RollbackProcess_ShouldReportInternalError_WhenTheTreeCompilesOnTheBaseline()
    {
        // Future-proofing: if a mutant-free tree fails to compile yet its counterpart builds cleanly
        // on the fully-uninstrumented baseline, the failure was mutation-induced (rollback just could
        // not attribute it) - it must stay an internal error, not be blamed on the user's build.
        // Today's runtime-wrapped mutators cannot actually produce this, so the outcome is forced.
        var syntaxTree = CSharpSyntaxTree.ParseText(
            """
            namespace ExampleProject { public class Anchor { public object Broken() => Missing.Symbol; } }
            """,
            path: "Anchor.cs");
        var compilerWrapper = BaselineFailureCompiler(syntaxTree);
        compilerWrapper.FailsWithoutInstrumentationOverride = false;

        var target = new CSharpRollbackProcess();
        using var ms = new MemoryStream();
        var compileResult = compilerWrapper.Emit(ms);
        compileResult.Success.ShouldBeFalse();

        var exception = Should.Throw<CompilationException>(() =>
            target.RollbackMutationsInError(compilerWrapper, compileResult.Diagnostics,
                ICSharpRollbackProcess.Mode.Normal, false));

        exception.Message.ShouldContain("Internal error");
        exception.Message.ShouldNotContain("not caused by mutation");
    }

    [TestMethod]
    public void RemoveAllMutations_RestoresTheOriginalSource_IncludingStructuralRewrites()
    {
        // Stryker converts an expression-bodied member to a block body - a structural rewrite
        // tagged with an Injector annotation but no MutationId - to make room for statement-level
        // mutations. RemoveAllMutations must undo that conversion as well as the mutations, so the
        // mutation-free baseline it produces is the genuine original source (not a still-rewritten
        // one that a syntax-sensitive generator could react to).
        var syntaxTree = CSharpSyntaxTree.ParseText(
            """
            using System;

            namespace ExampleProject
            {
                public class Fake { public event Action ValueChanged; }

                public class Test
                {
                    private Fake AccountNumber = new Fake();
                    public void Subscribe() => AccountNumber.ValueChanged += RefreshAccountNumber;
                    private void RefreshAccountNumber() { }
                }
            }
            """,
            path: "Original.cs");
        var codeInjection = new CodeInjection();
        var mutator = new CsharpMutantOrchestrator(new MutantPlacer(codeInjection),
            options: new StrykerOptions { MutationLevel = MutationLevel.Complete });

        var mutated = mutator.Mutate(syntaxTree, null);
        MutantPlacer.GetAllMutations(mutated.GetRoot()).ShouldNotBeEmpty("the orchestrator should have instrumented the code");

        var restored = MutantPlacer.RemoveAllMutations(mutated);

        restored.GetRoot().GetAnnotatedNodes("Injector").ShouldBeEmpty("all instrumentation, including structural rewrites, must be reverted");
        restored.GetRoot().NormalizeWhitespace().ToFullString()
            .ShouldBe(syntaxTree.GetRoot().NormalizeWhitespace().ToFullString());
        // the baseline must be the genuine original project - its metadata is preserved
        restored.FilePath.ShouldBe("Original.cs");
        restored.Options.ShouldBe(mutated.Options);
    }

    private static CompilerWrapper BaselineFailureCompiler(params SyntaxTree[] syntaxTrees) =>
        new(CSharpCompilation.Create("TestCompilation",
            syntaxTrees: syntaxTrees,
            options: new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary),
            references: new List<PortableExecutableReference>() {
                MetadataReference.CreateFromFile(typeof(object).Assembly.Location),
                MetadataReference.CreateFromFile(typeof(Environment).Assembly.Location)
            }));

    [TestMethod]
    public void ShouldRollbackIssueInExpression()
    {
        var syntaxTree = CSharpSyntaxTree.ParseText(
            """

            using System;
            using System.Collections.Generic;
            using System.Linq;

            namespace ExampleProject
            {
                public class Test
                {
                    public void SomeLinq()
                    {
                        var list = new List<List<double>>();
                        int[] listProjected = list.Select(l => l.Count()).ToArray();
                    }
                }
            }
            """);
        var options = new StrykerOptions
        {
            MutationLevel = MutationLevel.Complete,
            DiagMode = true
        };
        var codeInjection = new CodeInjection();
        var placer = new MutantPlacer(codeInjection);
        var mutator = new CsharpMutantOrchestrator(placer, options: options);
        var helpers = new List<SyntaxTree>();
        foreach (var (name, code) in codeInjection.MutantHelpers)
        {
            helpers.Add(CSharpSyntaxTree.ParseText(code, path: name, encoding: Encoding.UTF32));
        }

        var mutant = mutator.Mutate(syntaxTree, null);
        helpers.Add(mutant);

        var references = new List<string> {
            typeof(object).Assembly.Location,
            typeof(List<string>).Assembly.Location,
            typeof(Enumerable).Assembly.Location,
            typeof(PipeStream).Assembly.Location,
            // MutantControl maps the mutant-id file via MemoryMappedFile (MTP runner). ReadInt32 comes from
            // UnmanagedMemoryAccessor, whose reference identity is System.Runtime.InteropServices (loaded by
            // name because the runtime type is forwarded to CoreLib).
            typeof(System.IO.MemoryMappedFiles.MemoryMappedFile).Assembly.Location,
            Assembly.Load("System.Runtime.InteropServices").Location,
        };
        Assembly.GetEntryAssembly().GetReferencedAssemblies().ToList().ForEach(a => references.Add(Assembly.Load(a).Location));

        var input = new MutationTestInput()
        {
            SourceProjectInfo = new SourceProjectInfo
            {
                AnalyzerResult = TestHelper.SetupProjectAnalyzerResult(
                    properties: new Dictionary<string, string>()
                    {
                        { "TargetDir", "" },
                        { "AssemblyName", "AssemblyName"},
                        { "TargetFileName", "TargetFileName.dll"},
                        { "SignAssembly", "true" },
                        { "AssemblyOriginatorKeyFile", Path.GetFullPath(Path.Combine("TestResources", "StrongNameKeyFile.snk")) }
                    },
                    projectFilePath: "TestResources",
                    // add a reference to system so the example code can compile
                    references: references.ToArray()
                ).Object
            }
        };

        var rollbackProcess = new CSharpRollbackProcess();

        var target = new CsharpCompilingProcess(input, rollbackProcess, options, syntaxTrees: helpers);

        using var ms = new MemoryStream();
        var result = target.Compile(ms, null);
        result.RolledbackIds.Count().ShouldBe(2); // should actually be 1 but thanks to issue #1745 rollback doesn't work
    }

    [TestMethod]
    public void ShouldRollbackAllMutationsInsideAExpressionBodyMethod()
    {
        var syntaxTree = CSharpSyntaxTree.ParseText(
            """
            using System;
            using System.Collections.Generic;
            using System.Linq;
            using System.Threading.Tasks;

            namespace ExampleProject
            {
                class Fake
                {
                    public string DisplayValue { get; set; }
                    public event Action ValueChanged;
                }

                public class Test
                {
                    private Fake AccountNumber = new Fake();
                    // this line triggers a compilation error on purpose
                    protected override void Random() =>
                        AccountNumber.ValueChanged += RefreshAccountNumber;

                    private void RefreshAccountNumber()
                    {
                    }
                }
            }
            """);
        var options = new StrykerOptions
        {
            MutationLevel = MutationLevel.Complete,
            DiagMode = true
        };
        var codeInjection = new CodeInjection();
        var placer = new MutantPlacer(codeInjection);
        var mutator = new CsharpMutantOrchestrator(placer, options: options);
        var helpers = new List<SyntaxTree>();
        foreach (var (name, code) in codeInjection.MutantHelpers)
        {
            helpers.Add(CSharpSyntaxTree.ParseText(code, path: name, encoding: Encoding.UTF32));
        }

        var mutant = mutator.Mutate(syntaxTree, null);

        helpers.Add(mutant);

        var references = new List<string> {
            typeof(object).Assembly.Location,
            typeof(List<string>).Assembly.Location,
            typeof(Enumerable).Assembly.Location,
            typeof(PipeStream).Assembly.Location,
            // MutantControl maps the mutant-id file via MemoryMappedFile (MTP runner). ReadInt32 comes from
            // UnmanagedMemoryAccessor, whose reference identity is System.Runtime.InteropServices (loaded by
            // name because the runtime type is forwarded to CoreLib).
            typeof(System.IO.MemoryMappedFiles.MemoryMappedFile).Assembly.Location,
            Assembly.Load("System.Runtime.InteropServices").Location,
        };
        Assembly.GetEntryAssembly().GetReferencedAssemblies().ToList().ForEach(a => references.Add(Assembly.Load(a).Location));

        var input = new MutationTestInput()
        {
            SourceProjectInfo = new SourceProjectInfo
            {
                AnalyzerResult = TestHelper.SetupProjectAnalyzerResult(
                    properties: new Dictionary<string, string>()
                    {
                        { "TargetDir", "" },
                        { "AssemblyName", "AssemblyName"},
                        { "TargetFileName", "TargetFileName.dll"},
                        { "SignAssembly", "true" },
                        { "AssemblyOriginatorKeyFile", Path.GetFullPath(Path.Combine("TestResources", "StrongNameKeyFile.snk")) }
                    },
                    projectFilePath: "TestResources",
                    // add a reference to system so the example code can compile
                    references: references.ToArray()
                ).Object
            }
        };

        var rollbackProcess = new CSharpRollbackProcess();

        var target = new CsharpCompilingProcess(input, rollbackProcess, options, helpers);

        using var ms = new MemoryStream();

        Action test = () => target.Compile(ms, null);
        test.ShouldThrow<CompilationException>();
    }

    [TestMethod]
    public void RollbackProcess_ShouldRollbackAllCompileErrors()
    {
        var syntaxTree = CSharpSyntaxTree.ParseText(
            """
            using System;
            namespace ExampleProject
            {
                public class Calculator
                {
                    public int ActiveMutation = 1;

                    public string Subtract(string first, string second)
                    {
                        if (ActiveMutation == 6) {
                            while (first.Length > 2)
                            {
                                return first - second;
                            }
                            while (first.Length < 2)
                            {
                                return second + first;
                            }
                            return null;
                        } else {
                            while (first.Length > 2)
                            {
                                return first + second;
                            }
                            while (first.Length < 2)
                            {
                                return (ActiveMutation == 7 ? second - first : second + first);
                            }
                            return null;
                        }
                    }
                }
            }
            """);
        var root = syntaxTree.GetRoot();

        var mutantIf = root.DescendantNodes().OfType<IfStatementSyntax>().First();
        root = root.ReplaceNode(
            mutantIf,
            mutantIf.WithAdditionalAnnotations(GetMutationIdMarker(6), _ifEngineMarker)
        );
        var y = root.DescendantNodes().OfType<ParenthesizedExpressionSyntax>();
        var mutantCondition = root.DescendantNodes().OfType<ParenthesizedExpressionSyntax>().First(x => x.Expression is ConditionalExpressionSyntax);
        root = root.ReplaceNode(
            mutantCondition,
            mutantCondition.WithAdditionalAnnotations(GetMutationIdMarker(7), _conditionalEngineMarker)
        );

        var annotatedSyntaxTree = root.SyntaxTree;

        var compiler = CSharpCompilation.Create("TestCompilation",
            syntaxTrees: new Collection<SyntaxTree>() { annotatedSyntaxTree },
            options: new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary),
            references: new List<PortableExecutableReference>() {
                MetadataReference.CreateFromFile(typeof(object).Assembly.Location),
                MetadataReference.CreateFromFile(typeof(Environment).Assembly.Location)
            });

        var target = new CSharpRollbackProcess();

        using var ms = new MemoryStream();
        var compileResult = compiler.Emit(ms);

        var compilerWrapper = new CompilerWrapper(compiler);

        var ids = target.RollbackMutationsInError(compilerWrapper, compileResult.Diagnostics, ICSharpRollbackProcess.Mode.Normal, false);

        var rollbackedResult = compilerWrapper.Emit(ms);

        rollbackedResult.Success.ShouldBeTrue();
        ids.ShouldBe(new Collection<int> { 6, 7 });
    }

    [TestMethod]
    public void RollbackProcess_ShouldRollbackErrorsAndKeepTheRest()
    {
        var syntaxTree = CSharpSyntaxTree.ParseText(
            """
            using System;

            namespace ExampleProject
            {
                public class StringMagic
                {
                    public int ActiveMutation = 1;

                    public string AddTwoStrings(string first, string second)
                    {
                        if(ActiveMutation == 8){
                            while (first.Length > 2)
                            {
                                return first - second;
                            }
                            while (first.Length < 2)
                            {
                                return second + first;
                            }
                            return null;
                        }else{if(ActiveMutation == 7){
                            while (first.Length > 2)
                            {
                                return first + second;
                            }
                            while (first.Length < 2)
                            {
                                return second - first;
                            }
                            return null;
                        }else{if(ActiveMutation == 6){
                            while (first.Length == 2)
                            {
                                return first + second;
                            }
                            while (first.Length < 2)
                            {
                                return second + first;
                            }
                            return null;
                        }else{
                            while (first.Length == 2)
                            {
                                return first + second;
                            }
                            while (first.Length < 2)
                            {
                                return second + first;
                            }
                            return null;
                        }}}
                    }
                }
            }
            """);
        var root = syntaxTree.GetRoot();

        var mutantIf1 = root.DescendantNodes().OfType<IfStatementSyntax>().First();
        root = root.ReplaceNode(
            mutantIf1,
            mutantIf1.WithAdditionalAnnotations(GetMutationIdMarker(8), _ifEngineMarker)
        );
        var mutantIf2 = root.DescendantNodes().OfType<IfStatementSyntax>().ToList()[1];
        root = root.ReplaceNode(
            mutantIf2,
            mutantIf2.WithAdditionalAnnotations(GetMutationIdMarker(7), _ifEngineMarker)
        );

        var annotatedSyntaxTree = root.SyntaxTree;

        var compiler = CSharpCompilation.Create("TestCompilation",
            syntaxTrees: new Collection<SyntaxTree>() { annotatedSyntaxTree },
            options: new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary),
            references: new List<PortableExecutableReference>() {
                MetadataReference.CreateFromFile(typeof(object).Assembly.Location),
                MetadataReference.CreateFromFile(typeof(Environment).Assembly.Location)
            });

        var target = new CSharpRollbackProcess();

        using var ms = new MemoryStream();
        var compileResult = compiler.Emit(ms);
        var compilerWrapper = new CompilerWrapper(compiler);

        var ids = target.RollbackMutationsInError(compilerWrapper, compileResult.Diagnostics, ICSharpRollbackProcess.Mode.Normal, false);

        var rollbackedResult = compilerWrapper.Emit(ms);

        rollbackedResult.Success.ShouldBeTrue();
        // validate that only mutation 8 and 7 were rollbacked
        ids.ShouldBe(new Collection<int> { 8, 7 });
    }

    [TestMethod]
    public void RollbackProcess_ShouldRollbackBlockMutationWhenLocalRollbackFails()
    {
        var syntaxTree = CSharpSyntaxTree.ParseText(
            """

            namespace ExampleProject
            {
                public class StringMagic
                {
                    public int ActiveMutation = 1;

                    public string this[string key]
                    {
                        get
                        {
                            if (ActiveMutation == 1) { ; } else {
                                if (ActiveMutation == 2) {
                                    return key; // some mutation
                                } else {
                                    return key + key;
                                }
                            }
                        }
                    }
                }
            }
            """);
        var root = syntaxTree.GetRoot();

        var mutantIf1 = root.DescendantNodes().OfType<IfStatementSyntax>().First();
        root = root.ReplaceNode(
            mutantIf1,
            mutantIf1.WithAdditionalAnnotations(GetMutationIdMarker(1), GetMutationTypeMarker(Mutator.Block), _ifEngineMarker)
        );
        var mutantIf2 = root.DescendantNodes().OfType<IfStatementSyntax>().ToList()[1];
        root = root.ReplaceNode(
            mutantIf2,
            mutantIf2.WithAdditionalAnnotations(GetMutationIdMarker(2), GetMutationTypeMarker(Mutator.String), _ifEngineMarker)
        );
        var annotatedSyntaxTree = root.SyntaxTree;

        var compiler = CSharpCompilation.Create("TestCompilation",
            syntaxTrees: new Collection<SyntaxTree>() { annotatedSyntaxTree },
            options: new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary),
            references: new List<PortableExecutableReference>() {
                MetadataReference.CreateFromFile(typeof(object).Assembly.Location),
                MetadataReference.CreateFromFile(typeof(Environment).Assembly.Location)
            });

        var target = new CSharpRollbackProcess();

        using var ms = new MemoryStream();
        var compileResult = compiler.Emit(ms);

        compileResult.Success.ShouldBeFalse();
        compileResult.Diagnostics.ShouldHaveSingleItem();
        var compilerWrapper = new CompilerWrapper(compiler);

        var ids = target.RollbackMutationsInError(compilerWrapper, compileResult.Diagnostics, ICSharpRollbackProcess.Mode.Normal, false);

        var rollbackedResult = compilerWrapper.Emit(ms);

        rollbackedResult.Success.ShouldBeTrue();
        // validate that only the block mutation was rolled back
        ids.ShouldBe(new Collection<int> { 1 });
    }

    [TestMethod]
    [Ignore("Not relevant, need to identify a new pattern that triggers safe mode.")]
    public void RollbackProcess_ShouldRollbackMethodWhenLocalRollbackFailsAndNoBlockMutationsFound()
    {
        var syntaxTree = CSharpSyntaxTree.ParseText(
            """
            using System;
            namespace ExampleProject
            {
                public class StringMagic
                {
                    public int ActiveMutation = 1;
                    public string AddTwoStrings(string first, string second, out string third)
                    {
                        var dummy = "";
                        if(ActiveMutation == 8){
                            while (first.Length > 2)
                            {
                                dummy = first + second;
                            }
                            while (first.Length < 2)
                            {
                                dummy =  second - first;
                            }
                        }else{if(ActiveMutation == 7){
                            while (first.Length > 2)
                            {
                                dummy =  first + second;
                            }
                            while (first.Length < 2)
                            {
                                dummy =  second - first;
                            }
                        }else{if(ActiveMutation == 6){
                            while (first.Length == 2)
                            {
                                dummy =  first + second;
                            }
                            while (first.Length < 2)
                            {
                                dummy =  second + first;
                            }
                        }else{
                            third = "good";
                            while (first.Length == 2)
                            {
                                dummy =  first + second;
                            }
                            while (first.Length < 2)
                            {
                                    dummy =  second + first;
                            }
                            return null;
                        }}}
                    }
                }
            }
            """);
        var root = syntaxTree.GetRoot();

        var mutantIf1 = root.DescendantNodes().OfType<IfStatementSyntax>().First();
        root = root.ReplaceNode(
            mutantIf1,
            mutantIf1.WithAdditionalAnnotations(GetMutationIdMarker(8), _ifEngineMarker)
        );
        var mutantIf2 = root.DescendantNodes().OfType<IfStatementSyntax>().ToList()[1];
        root = root.ReplaceNode(
            mutantIf2,
            mutantIf2.WithAdditionalAnnotations(GetMutationIdMarker(7), _ifEngineMarker)
        );
        var mutantIf3 = root.DescendantNodes().OfType<IfStatementSyntax>().ToList()[2];
        root = root.ReplaceNode(
            mutantIf3,
            mutantIf3.WithAdditionalAnnotations(GetMutationIdMarker(6), _ifEngineMarker)
        );
        var annotatedSyntaxTree = root.SyntaxTree;

        var compiler = CSharpCompilation.Create("TestCompilation",
            syntaxTrees: new Collection<SyntaxTree>() { annotatedSyntaxTree },
            options: new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary),
            references: new List<PortableExecutableReference>() {
                MetadataReference.CreateFromFile(typeof(object).Assembly.Location),
                MetadataReference.CreateFromFile(typeof(Environment).Assembly.Location)
            });

        var target = new CSharpRollbackProcess();

        using var ms = new MemoryStream();
        var compileResult = compiler.Emit(ms);
        var compilerWrapper = new CompilerWrapper(compiler);

        var ids = target.RollbackMutationsInError(compilerWrapper, compileResult.Diagnostics, ICSharpRollbackProcess.Mode.Normal, false);

        var result = compilerWrapper.Emit(ms);

        result.Success.ShouldBeFalse();
        result.Diagnostics.ShouldHaveSingleItem();
        ids = target.RollbackMutationsInError(compilerWrapper, result.Diagnostics, ICSharpRollbackProcess.Mode.Normal, false);

        result = compilerWrapper.Emit(ms);
        result.Success.ShouldBeTrue();
        // validate that all mutations are rolled back
        ids.ShouldBe(new Collection<int> { 8, 7, 6 });
    }

    [TestMethod]
    [DataRow("third = \"good\";")]
    [DataRow("(third, _) = (\"good\", 2);")]
    public void RollbackProcess_ShouldRollbackMutationsErasingAssignment(string assignment)
    {
        var syntaxTree = CSharpSyntaxTree.ParseText(
            $$"""
                using System;
                namespace ExampleProject
                {
                    public class StringMagic
                    {
                        public int ActiveMutation = 1;

                        public string AddTwoStrings
                        {
                            get
                            {
                                string first = string.Empty;
                                string second = string.Empty;
                                string third;
                                var dummy = "";
                                if(ActiveMutation == 8){
                                    while (first.Length > 2)
                                    {
                                        dummy = first + second;
                                    }
                                }else{if(ActiveMutation == 7){
                                while (first.Length > 2)
                                    {
                                        dummy =  first + second;
                                    }
                                }else{if(ActiveMutation == 6){
                                   {{assignment}}
                                while (first.Length == 2)
                                    {
                                        dummy =  first + second;
                                    }
                                    while (first.Length < 2)
                                    {
                                        dummy =  second + first;
                                    }
                                }else{
                                   {{assignment}}
                                    while (first.Length == 2)
                                    {
                                        dummy =  first + second;
                                    }
                                    while (first.Length < 2)
                                    {
                                        dummy =  second + first;
                                    }
                                }
                                }
                                }
                                return third;
                            }
                        }
                    }
                }
                """);
        var root = syntaxTree.GetRoot();

        var mutantIf1 = root.DescendantNodes().OfType<IfStatementSyntax>().First();
        root = root.ReplaceNode(
            mutantIf1,
            mutantIf1.WithAdditionalAnnotations(GetMutationIdMarker(8), _ifEngineMarker)
        );
        var mutantIf2 = root.DescendantNodes().OfType<IfStatementSyntax>().ToList()[1];
        root = root.ReplaceNode(
            mutantIf2,
            mutantIf2.WithAdditionalAnnotations(GetMutationIdMarker(7), _ifEngineMarker)
        );
        var mutantIf3 = root.DescendantNodes().OfType<IfStatementSyntax>().ToList()[2];
        root = root.ReplaceNode(
            mutantIf3,
            mutantIf3.WithAdditionalAnnotations(GetMutationIdMarker(6), _ifEngineMarker)
        );
        var annotatedSyntaxTree = root.SyntaxTree;

        var compiler = CSharpCompilation.Create("TestCompilation",
            syntaxTrees: new Collection<SyntaxTree> { annotatedSyntaxTree },
            options: new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary),
            references: new List<PortableExecutableReference> {
                MetadataReference.CreateFromFile(typeof(object).Assembly.Location),
                MetadataReference.CreateFromFile(typeof(Environment).Assembly.Location)
            });

        var target = new CSharpRollbackProcess();

        using var ms = new MemoryStream();
        var compileResult = compiler.Emit(ms);
        var compilerWrapper = new CompilerWrapper(compiler);

        var ids = target.RollbackMutationsInError(compilerWrapper, compileResult.Diagnostics, ICSharpRollbackProcess.Mode.Normal, false);
        var rollbackResult = compilerWrapper.Emit(ms);
        rollbackResult.Success.ShouldBeFalse();
        // rollback a single assignment erasing mutation
        ids.ShouldBe(new Collection<int> {8});

        ids = target.RollbackMutationsInError(compilerWrapper, rollbackResult.Diagnostics, ICSharpRollbackProcess.Mode.Normal, false);
        rollbackResult = compilerWrapper.Emit(ms);
        rollbackResult.Success.ShouldBeTrue();
        // validate that mutations 8 and 7 were rolled back
        ids.ShouldBe(new Collection<int> { 8, 7 });
    }

    [TestMethod]
    public void RollbackProcess_ShouldRollbackMutationsErasingAssignmentForOutVariables()
    {
        var syntaxTree = CSharpSyntaxTree.ParseText(
        """
        using System;
        namespace ExampleProject
        {
            public class StringMagic
            {
                public int ActiveMutation = 1;

                public string AddTwoStrings(out string third)
                {
                    string first = string.Empty;
                    string second = string.Empty;
                    var dummy = "";
                    if(ActiveMutation == 8){
                        while (first.Length > 2)
                        {
                            dummy = first + second;
                        }
                        while (first.Length < 2)
                        {
                            dummy =  second - first;
                        }
                    }else{if(ActiveMutation == 7){
                    while (first.Length > 2)
                        {
                            dummy =  first + second;
                        }
                        while (first.Length < 2)
                        {
                            dummy =  second + first;
                        }
                    }else{if(ActiveMutation == 6){
                        third = "good";
                        while (first.Length == 2)
                        {
                            dummy =  first + second;
                        }
                        while (first.Length < 2)
                        {
                            dummy =  second + first;
                        }
                    }else{
                        third = "good";
                        while (first.Length == 2)
                        {
                            dummy =  first + second;
                        }
                        while (first.Length < 2)
                        {
                            dummy =  second + first;
                        }
                    }}}
                    return dummy;
                }
            }
        }
        """);
        var root = syntaxTree.GetRoot();

        var mutantIf1 = root.DescendantNodes().OfType<IfStatementSyntax>().First();
        root = root.ReplaceNode(
            mutantIf1,
            mutantIf1.WithAdditionalAnnotations(GetMutationIdMarker(8), _ifEngineMarker)
        );
        var mutantIf2 = root.DescendantNodes().OfType<IfStatementSyntax>().ToList()[1];
        root = root.ReplaceNode(
            mutantIf2,
            mutantIf2.WithAdditionalAnnotations(GetMutationIdMarker(7), _ifEngineMarker)
        );
        var mutantIf3 = root.DescendantNodes().OfType<IfStatementSyntax>().ToList()[2];
        root = root.ReplaceNode(
            mutantIf3,
            mutantIf3.WithAdditionalAnnotations(GetMutationIdMarker(6), _ifEngineMarker)
        );
        var annotatedSyntaxTree = root.SyntaxTree;

        var compiler = CSharpCompilation.Create("TestCompilation",
            syntaxTrees: new Collection<SyntaxTree>() { annotatedSyntaxTree },
            options: new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary),
            references: new List<PortableExecutableReference>() {
                MetadataReference.CreateFromFile(typeof(object).Assembly.Location),
                MetadataReference.CreateFromFile(typeof(Environment).Assembly.Location)
            });

        var target = new CSharpRollbackProcess();

        using var ms = new MemoryStream();
        var compileResult = compiler.Emit(ms);
        var compilerWrapper = new CompilerWrapper(compiler);

        var ids = target.RollbackMutationsInError(compilerWrapper, compileResult.Diagnostics, ICSharpRollbackProcess.Mode.Normal, false);
        var rollbackResult = compilerWrapper.Emit(ms);

        rollbackResult.Success.ShouldBeTrue();
        // validate that mutations 8 and 7 were rolled back
        ids.ShouldBe(new Collection<int> { 8, 7 });
    }

    [TestMethod]
    public void RollbackProcess_ShouldRollbackMutationsErasingReturn()
    {
        var syntaxTree = CSharpSyntaxTree.ParseText(
        """
        using System;
        namespace ExampleProject
        {
            public class StringMagic
            {
                public int ActiveMutation = 1;

                public string AddTwoStrings()
                {
                    string first = string.Empty;
                    string second = string.Empty;
                    var dummy = "";
                    if(ActiveMutation == 8){
                        while (first.Length > 2)
                        {
                            dummy = first + second;
                        }
                        while (first.Length < 2)
                        {
                            dummy =  second - first;
                        }
                        return dummy;
                    }else{if(ActiveMutation == 7){
                    while (first.Length > 2)
                        {
                            dummy =  first + second;
                        }
                        while (first.Length < 2)
                        {
                            dummy =  second + first;
                        }
                    }else{if(ActiveMutation == 6){
                        while (first.Length == 2)
                        {
                            dummy =  first + second;
                        }
                        while (first.Length < 2)
                        {
                            dummy =  second + first;
                        }
                        return dummy;
                    }else{
                        while (first.Length == 2)
                        {
                            dummy =  first + second;
                        }
                        while (first.Length < 2)
                        {
                            dummy =  second + first;
                        }
                        return dummy;
                    }}}
                }
            }
        }
        """);
        var root = syntaxTree.GetRoot();

        var mutantIf1 = root.DescendantNodes().OfType<IfStatementSyntax>().First();
        root = root.ReplaceNode(
            mutantIf1,
            mutantIf1.WithAdditionalAnnotations(GetMutationIdMarker(8), _ifEngineMarker)
        );
        var mutantIf2 = root.DescendantNodes().OfType<IfStatementSyntax>().ToList()[1];
        root = root.ReplaceNode(
            mutantIf2,
            mutantIf2.WithAdditionalAnnotations(GetMutationIdMarker(7), _ifEngineMarker)
        );
        var mutantIf3 = root.DescendantNodes().OfType<IfStatementSyntax>().ToList()[2];
        root = root.ReplaceNode(
            mutantIf3,
            mutantIf3.WithAdditionalAnnotations(GetMutationIdMarker(6), _ifEngineMarker)
        );
        var annotatedSyntaxTree = root.SyntaxTree;

        var compiler = CSharpCompilation.Create("TestCompilation",
            syntaxTrees: new Collection<SyntaxTree>() { annotatedSyntaxTree },
            options: new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary),
            references: new List<PortableExecutableReference>() {
                MetadataReference.CreateFromFile(typeof(object).Assembly.Location),
                MetadataReference.CreateFromFile(typeof(Environment).Assembly.Location)
            });

        var target = new CSharpRollbackProcess();

        using var ms = new MemoryStream();
        var compileResult = compiler.Emit(ms);
        var compilerWrapper = new CompilerWrapper(compiler);

        var ids = target.RollbackMutationsInError(compilerWrapper, compileResult.Diagnostics, ICSharpRollbackProcess.Mode.Normal, false);
        var rollbackResult = compilerWrapper.Emit(ms);

        rollbackResult.Success.ShouldBeTrue();
        // validate that mutations 8 and 7 were rolled back
        ids.ShouldBe(new Collection<int> { 8, 7 });
    }

    [TestMethod]
    public void RollbackProcess_ShouldRollbackError_RolledBackCompilationShouldCompileWhenUriIsEmpty()
    {
        var syntaxTree = CSharpSyntaxTree.ParseText(
            """
            using System;

            namespace ExampleProject
            {
                public class Query
                {
                    public int ActiveMutation = 1;

                    public void Break()
                    {
                        if(ActiveMutation == 1)
                        {
                            string someQuery = "test";
                            new Uri(new Uri(string.Empty), "/API?" - someQuery);
                        }
                        else
                        {
                            string someQuery = "test";
                            new System.Uri(new System.Uri(string.Empty), "/API?" + someQuery);
                        }
                    }
                }
            }
            """);
        var ifStatement = syntaxTree
            .GetRoot()
            .DescendantNodes()
            .First(x => x is IfStatementSyntax);
        var annotatedSyntaxTree = syntaxTree.GetRoot()
            .ReplaceNode(
                ifStatement,
                ifStatement.WithAdditionalAnnotations(GetMutationIdMarker(1), _ifEngineMarker)
            ).SyntaxTree;

        var compiler = CSharpCompilation.Create("TestCompilation",
            syntaxTrees: new Collection<SyntaxTree>() { annotatedSyntaxTree },
            options: new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary),
            references: new List<PortableExecutableReference>() {
                MetadataReference.CreateFromFile(typeof(object).Assembly.Location),
                MetadataReference.CreateFromFile(typeof(Environment).Assembly.Location),
                MetadataReference.CreateFromFile(typeof(Uri).Assembly.Location),
            });

        var target = new CSharpRollbackProcess();
        var compilerWrapper = new CompilerWrapper(compiler);

        using var ms = new MemoryStream();
        var ids = target.RollbackMutationsInError(compilerWrapper, compiler.Emit(ms).Diagnostics, ICSharpRollbackProcess.Mode.Normal, false);
        compilerWrapper.Emit(ms).Success.ShouldBeTrue();

        // validate that only one of the compile errors marked the mutation as rolled back.
        ids.ShouldBe([1]);
    }

    [TestMethod]
    public void RollbackProcess_ShouldOnlyRaiseExceptionOnFinalAttempt()
    {

        var syntaxTree = CSharpSyntaxTree.ParseText(
            """
            using System;
            namespace ExampleProject
            {
                public class Query
                {
                    public int ActiveMutation = 1;

                    public void Break()
                    {
                        if(ActiveMutation == 1)
                        {
                            string someQuery = "test";
                            new Uri(new Uri(string.Empty), "/API?" - someQuery);
                        }
                        else
                        {
                            string someQuery = "test";
                            new System.Uri(new System.Uri(string.Empty), "/API?" + someQuery);
                        }
                        var error = "a"-"b":
                    }
                }
            }
            """);
        var ifStatement = syntaxTree
            .GetRoot()
            .DescendantNodes()
            .First(x => x is IfStatementSyntax);
        var annotatedSyntaxTree = syntaxTree.GetRoot()
            .ReplaceNode(
                ifStatement,
                ifStatement.WithAdditionalAnnotations(GetMutationIdMarker(1), _ifEngineMarker)
            ).SyntaxTree;

        var compiler = CSharpCompilation.Create("TestCompilation",
            syntaxTrees: new Collection<SyntaxTree>() { annotatedSyntaxTree },
            options: new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary),
            references: new List<PortableExecutableReference>() {
                MetadataReference.CreateFromFile(typeof(object).Assembly.Location),
                MetadataReference.CreateFromFile(typeof(Environment).Assembly.Location),
                MetadataReference.CreateFromFile(typeof(Uri).Assembly.Location),
            });

        var target = new CSharpRollbackProcess();

        using var ms = new MemoryStream();
        var compilerWrapper = new CompilerWrapper(compiler);

        // first compilation will roll back the mutation
        var fixedCompilation = target.RollbackMutationsInError(compilerWrapper, compiler.Emit(ms).Diagnostics, ICSharpRollbackProcess.Mode.Normal, false);

        // next attempt cannot roll back anything, so it assumes this is not fixable
        Should.Throw<CompilationException>(() => target.RollbackMutationsInError(compilerWrapper, compilerWrapper.Emit(ms).Diagnostics, ICSharpRollbackProcess.Mode.LastChance, false));
    }
}
