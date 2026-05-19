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

        var fixedCompilation = target.Start(compiler, compileResult.Diagnostics, ICSharpRollbackProcess.Mode.Normal, false);

        var rolledBackResult = fixedCompilation.Compilation.Emit(ms);

        rolledBackResult.Success.ShouldBeTrue();
    }

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

        var target = new CsharpCompilingProcess(input, rollbackProcess, options);

        using var ms = new MemoryStream();
        var result = target.Compile(helpers, ms, null);
        result.RollbackedIds.Count().ShouldBe(2); // should actually be 1 but thanks to issue #1745 rollback doesn't work
    }

    [TestMethod]
    public void ShouldRollbackAllMutationsInsideAExpressionBodyMethod()
    {
        // Arrange: a class that has an inherently uncompilable `protected override void Random()` (no base class
        // declares `Random`). The Stryker orchestrator introduces mutations into the expression body.
        // With the new rollback behaviour: after rolling back all reachable mutations the broken tree still
        // cannot compile (CS0115 is unrelated to any mutation), so the rollback process removes the tree
        // from the compilation entirely rather than exhausting all 50 retries and then throwing.
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
        };
        Assembly.GetEntryAssembly()?.GetReferencedAssemblies().ToList().ForEach(a => references.Add(Assembly.Load(a).Location));

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

        var target = new CsharpCompilingProcess(input, rollbackProcess, options);

        // Act: the new rollback behaviour removes the broken source-generator-output-like tree instead of
        // exhausting 50 retries and throwing. Compilation should now succeed (with the broken tree removed)
        // and at least the expression-body mutations must have been rolled back.
        using var ms = new MemoryStream();
        var result = target.Compile(helpers, ms, null);

        // Assert: compilation succeeds; some mutations were rolled back before the tree was removed
        result.Success.ShouldBeTrue();
        result.RollbackedIds.ShouldNotBeEmpty();
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

        var fixedCompilation = target.Start(compiler, compileResult.Diagnostics, ICSharpRollbackProcess.Mode.Normal, false);

        var rollbackedResult = fixedCompilation.Compilation.Emit(ms);

        rollbackedResult.Success.ShouldBeTrue();
        fixedCompilation.RollbackedIds.ShouldBe(new Collection<int> { 6, 7 });
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

        var fixedCompilation = target.Start(compiler, compileResult.Diagnostics, ICSharpRollbackProcess.Mode.Normal, false);

        var rollbackedResult = fixedCompilation.Compilation.Emit(ms);

        rollbackedResult.Success.ShouldBeTrue();
        // validate that only mutation 8 and 7 were rollbacked
        fixedCompilation.RollbackedIds.ShouldBe(new Collection<int> { 8, 7 });
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

        var fixedCompilation = target.Start(compiler, compileResult.Diagnostics, ICSharpRollbackProcess.Mode.Normal, false);

        var rollbackedResult = fixedCompilation.Compilation.Emit(ms);

        rollbackedResult.Success.ShouldBeTrue();
        // validate that only the block mutation was rolled back
        fixedCompilation.RollbackedIds.ShouldBe(new Collection<int> { 1 });
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

        var fixedCompilation = target.Start(compiler, compileResult.Diagnostics, ICSharpRollbackProcess.Mode.Normal, false);

        var rollbackedResult = fixedCompilation.Compilation.Emit(ms);

        rollbackedResult.Success.ShouldBeFalse();
        rollbackedResult.Diagnostics.ShouldHaveSingleItem();
        fixedCompilation = target.Start(fixedCompilation.Compilation, rollbackedResult.Diagnostics, ICSharpRollbackProcess.Mode.Normal, false);

        rollbackedResult = fixedCompilation.Compilation.Emit(ms);
        rollbackedResult.Success.ShouldBeTrue();
        // validate that all mutations are rolled back
        fixedCompilation.RollbackedIds.ShouldBe(new Collection<int> { 8, 7, 6 });
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

        var fixedCompilation = target.Start(compiler, compileResult.Diagnostics, ICSharpRollbackProcess.Mode.Normal, false);
        var rollbackResult = fixedCompilation.Compilation.Emit(ms);
        rollbackResult.Success.ShouldBeFalse();
        // rollback a single assignment erasing mutation
        fixedCompilation.RollbackedIds.ShouldBe(new Collection<int> {8});

        fixedCompilation = target.Start(fixedCompilation.Compilation, rollbackResult.Diagnostics, ICSharpRollbackProcess.Mode.Normal, false);
        rollbackResult = fixedCompilation.Compilation.Emit(ms);
        rollbackResult.Success.ShouldBeTrue();
        // validate that mutations 8 and 7 were rolled back
        fixedCompilation.RollbackedIds.ShouldBe(new Collection<int> { 8, 7 });
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

        var fixedCompilation = target.Start(compiler, compileResult.Diagnostics, ICSharpRollbackProcess.Mode.Normal, false);
        var rollbackResult = fixedCompilation.Compilation.Emit(ms);

        rollbackResult.Success.ShouldBeTrue();
        // validate that mutations 8 and 7 were rolled back
        fixedCompilation.RollbackedIds.ShouldBe(new Collection<int> { 8, 7 });
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

        var fixedCompilation = target.Start(compiler, compileResult.Diagnostics, ICSharpRollbackProcess.Mode.Normal, false);
        var rollbackResult = fixedCompilation.Compilation.Emit(ms);

        rollbackResult.Success.ShouldBeTrue();
        // validate that mutations 8 and 7 were rolled back
        fixedCompilation.RollbackedIds.ShouldBe(new Collection<int> { 8, 7 });
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

        using var ms = new MemoryStream();
        var fixedCompilation = target.Start(compiler, compiler.Emit(ms).Diagnostics, ICSharpRollbackProcess.Mode.Normal, false);
        fixedCompilation.Compilation.Emit(ms).Success.ShouldBeTrue();

        // validate that only one of the compile errors marked the mutation as rolled back.
        fixedCompilation.RollbackedIds.ShouldBe([1]);
    }

    [TestMethod]
    public void RollbackProcess_ShouldRemoveTreeWithNoMutations_WhenCompileErrorOccurs_InNormalMode()
    {
        // Arrange: A tree with a compile error but NO Stryker mutation annotations (simulates a
        // source-generator output tree that references mutated user code).
        var brokenTree = CSharpSyntaxTree.ParseText(
            """
            using System;
            namespace ExampleProject
            {
                public class BrokenGeneratorOutput
                {
                    // The subtraction operator causes a CS0019 compile error - no mutation annotation
                    public string Value => "a" - "b";
                }
            }
            """);

        // A second tree that compiles fine independently
        var goodTree = CSharpSyntaxTree.ParseText(
            """
            using System;
            namespace ExampleProject
            {
                public class GoodClass
                {
                    public string Value => "hello";
                }
            }
            """);

        var compiler = CSharpCompilation.Create("TestCompilation",
            syntaxTrees: [brokenTree, goodTree],
            options: new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary),
            references: [
                MetadataReference.CreateFromFile(typeof(object).Assembly.Location),
                MetadataReference.CreateFromFile(typeof(Environment).Assembly.Location)
            ]);

        var target = new CSharpRollbackProcess();

        using var ms = new MemoryStream();
        var compileResult = compiler.Emit(ms);
        compileResult.Success.ShouldBeFalse();

        // Act: rollback in Normal mode – should remove the broken tree with no mutations
        var fixedCompilation = target.Start(compiler, compileResult.Diagnostics, ICSharpRollbackProcess.Mode.Normal, false);

        // Assert: no mutations were rolled back, but compilation should succeed after the broken tree is removed
        fixedCompilation.RollbackedIds.ShouldBeEmpty();
        fixedCompilation.Compilation.SyntaxTrees.ShouldNotContain(brokenTree);
        fixedCompilation.Compilation.Emit(ms).Success.ShouldBeTrue();
    }

    [TestMethod]
    public void RollbackProcess_ShouldThrowCompilationException_WhenNoMutationsInTree_AndLastChanceMode()
    {
        // Arrange: A tree with a compile error but NO Stryker mutation annotations
        var brokenTree = CSharpSyntaxTree.ParseText(
            """
            using System;
            namespace ExampleProject
            {
                public class BrokenGeneratorOutput
                {
                    public string Value => "a" - "b"; // no mutation annotation
                }
            }
            """);

        var compiler = CSharpCompilation.Create("TestCompilation",
            syntaxTrees: [brokenTree],
            options: new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary),
            references: [
                MetadataReference.CreateFromFile(typeof(object).Assembly.Location),
                MetadataReference.CreateFromFile(typeof(Environment).Assembly.Location)
            ]);

        var target = new CSharpRollbackProcess();

        using var ms = new MemoryStream();
        var compileResult = compiler.Emit(ms);

        // Act & Assert: LastChance mode should throw even for no-mutation trees
        Should.Throw<CompilationException>(() =>
            target.Start(compiler, compileResult.Diagnostics, ICSharpRollbackProcess.Mode.LastChance, false));
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
        // first compilation will roll back the mutation
        var fixedCompilation = target.Start(compiler, compiler.Emit(ms).Diagnostics, ICSharpRollbackProcess.Mode.Normal, false);

        // next attempt cannot roll back anything, so it assumes this is not fixable
        Should.Throw<CompilationException>(() => target.Start(fixedCompilation.Compilation, fixedCompilation.Compilation.Emit(ms).Diagnostics, ICSharpRollbackProcess.Mode.LastChance, false));
    }
}
