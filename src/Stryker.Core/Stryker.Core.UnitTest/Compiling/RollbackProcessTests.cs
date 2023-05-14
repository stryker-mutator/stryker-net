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
using Shouldly;
using Stryker.Core.Compiling;
using Stryker.Core.Exceptions;
using Stryker.Core.InjectedHelpers;
using Stryker.Core.Mutants;
using Stryker.Core.MutationTest;
using Stryker.Core.Mutators;
using Stryker.Core.Options;
using Stryker.Core.ProjectComponents.SourceProjects;
using Xunit;

namespace Stryker.Core.UnitTest.Compiling
{
    public class RollbackProcessTests : TestBase
    {
        private readonly SyntaxAnnotation _ifEngineMarker = new("Injector", "IfInstrumentationEngine");
        private readonly SyntaxAnnotation _conditionalEngineMarker = new("Injector", "ConditionalInstrumentationEngine");

        private SyntaxAnnotation GetMutationIdMarker(int id) => new("MutationId", id.ToString());

        private SyntaxAnnotation GetMutationTypeMarker(Mutator type) => new("MutationType", type.ToString());

        [Fact]
        public void RollbackProcess_ShouldRollbackError_RollbackedCompilationShouldCompile()
        {
            var syntaxTree = CSharpSyntaxTree.ParseText(@"using System;

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
}");
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

            var target = new RollbackProcess();

            using (var ms = new MemoryStream())
            {
                var compileResult = compiler.Emit(ms);

                var fixedCompilation = target.Start(compiler, compileResult.Diagnostics, false, false);

                var rollbackedResult = fixedCompilation.Compilation.Emit(ms);

                rollbackedResult.Success.ShouldBeTrue();
            }
        }

        [Fact]
        public void ShouldRollbackIssueInExpression()
        {
            var syntaxTree = CSharpSyntaxTree.ParseText(@"
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
}");
            var options = new StrykerOptions
            {
                MutationLevel = MutationLevel.Complete,
                DevMode = true
            };
            var codeInjection = new CodeInjection();
            var placer = new MutantPlacer(codeInjection);
            var mutator = new CsharpMutantOrchestrator( placer, options: options);
            var helpers = new List<SyntaxTree>();
            foreach (var (name, code) in codeInjection.MutantHelpers)
            {
                helpers.Add(CSharpSyntaxTree.ParseText(code, path: name, encoding: Encoding.UTF32));
            }

            var mutant = mutator.Mutate(syntaxTree.GetRoot());
            helpers.Add(mutant.SyntaxTree);

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

            var rollbackProcess = new RollbackProcess();

            var target = new CsharpCompilingProcess(input, rollbackProcess, options);

            using var ms = new MemoryStream();
            var result = target.Compile(helpers, ms, null);
            result.RollbackResult.RollbackedIds.Count().ShouldBe(2); // should actually be 1 but thanks to issue #1745 rollback doesn't work
        }

        [Fact]
        public void RollbackProcess_ShouldRollbackAllCompileErrors()
        {
            var syntaxTree = CSharpSyntaxTree.ParseText(@"using System;

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
}");
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

            var target = new RollbackProcess();

            using var ms = new MemoryStream();
            var compileResult = compiler.Emit(ms);

            var fixedCompilation = target.Start(compiler, compileResult.Diagnostics, false, false);

            var rollbackedResult = fixedCompilation.Compilation.Emit(ms);

            rollbackedResult.Success.ShouldBeTrue();
            fixedCompilation.RollbackedIds.ShouldBe(new Collection<int> { 6, 7 });
        }

        [Fact]
        public void RollbackProcess_ShouldRollbackErrorsAndKeepTheRest()
        {
            var syntaxTree = CSharpSyntaxTree.ParseText(@"using System;

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
}");
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

            var target = new RollbackProcess();

            using var ms = new MemoryStream();
            var compileResult = compiler.Emit(ms);

            var fixedCompilation = target.Start(compiler, compileResult.Diagnostics, false, false);

            var rollbackedResult = fixedCompilation.Compilation.Emit(ms);

            rollbackedResult.Success.ShouldBeTrue();
            // validate that only mutation 8 and 7 were rollbacked
            fixedCompilation.RollbackedIds.ShouldBe(new Collection<int> { 8, 7 });
        }

        [Fact]
        public void RollbackProcess_ShouldRollbackBlockMutationWhenLocalRollbackFails()
        {
            var syntaxTree = CSharpSyntaxTree.ParseText(@"
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
}");
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

            var target = new RollbackProcess();

            using var ms = new MemoryStream();
            var compileResult = compiler.Emit(ms);

            compileResult.Success.ShouldBeFalse();
            compileResult.Diagnostics.ShouldHaveSingleItem();

            var fixedCompilation = target.Start(compiler, compileResult.Diagnostics, false, false);

            var rollbackedResult = fixedCompilation.Compilation.Emit(ms);

            rollbackedResult.Success.ShouldBeTrue();
            // validate that only the block mutation was rollbacked
            fixedCompilation.RollbackedIds.ShouldBe(new Collection<int> { 1 });
        }

        [Fact]
        public void RollbackProcess_ShouldRollbackMethodWhenLocalRollbackFailsAndNoBlockMutationsFound()
        {
            var syntaxTree = CSharpSyntaxTree.ParseText(@"using System;

namespace ExampleProject
{
    public class StringMagic
    {
        public int ActiveMutation = 1;
        public string AddTwoStrings(string first, string second, out string third)
        {
            var dummy = """";
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
                third = ""good"";
                while (first.Length == 2)
                {
                    dummy =  first + second;
                }
                while (first.Length < 2)
                {
                    dummy =  second + first;
                }
            }}}
                return null;
        }
    }
}");
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

            var target = new RollbackProcess();

            using var ms = new MemoryStream();
            var compileResult = compiler.Emit(ms);

            var fixedCompilation = target.Start(compiler, compileResult.Diagnostics, false, false);

            var rollbackedResult = fixedCompilation.Compilation.Emit(ms);

            rollbackedResult.Success.ShouldBeFalse();
            rollbackedResult.Diagnostics.ShouldHaveSingleItem();

            fixedCompilation = target.Start(fixedCompilation.Compilation, rollbackedResult.Diagnostics, false, false);
            rollbackedResult = fixedCompilation.Compilation.Emit(ms);
            rollbackedResult.Success.ShouldBeTrue();
            // validate that all mutations are rolled back
            fixedCompilation.RollbackedIds.ShouldBe(new Collection<int> { 8, 7, 6 });
        }

        [Fact]
        public void RollbackProcess_ShouldRollbacConstructorWhenLocalRollbackFailsAndNoBlockMutationsFound()
        {
            var syntaxTree = CSharpSyntaxTree.ParseText(@"using System;

namespace ExampleProject
{
    public class StringMagic
    {
        public int ActiveMutation = 1;
        public StringMagic(string first, string second, out string third)
        {
            var dummy = """";
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
                third = ""good"";
                while (first.Length == 2)
                {
                    dummy =  first + second;
                }
                while (first.Length < 2)
                {
                    dummy =  second + first;
                }
            }}}
        }
    }
}");
            var root = syntaxTree.GetRoot();
            // we manually inject mutation markers
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

            var target = new RollbackProcess();

            using var ms = new MemoryStream();
            var compileResult = compiler.Emit(ms);

            var fixedCompilation = target.Start(compiler, compileResult.Diagnostics, false, false);

            var rollbackedResult = fixedCompilation.Compilation.Emit(ms);

            rollbackedResult.Success.ShouldBeFalse();
            rollbackedResult.Diagnostics.ShouldHaveSingleItem();

            fixedCompilation = target.Start(fixedCompilation.Compilation, rollbackedResult.Diagnostics, false, false);
            rollbackedResult = fixedCompilation.Compilation.Emit(ms);
            rollbackedResult.Success.ShouldBeTrue();
            // validate that all mutations are rolled back
            fixedCompilation.RollbackedIds.ShouldBe(new Collection<int> { 8, 7, 6 });
        }

        [Fact]
        public void RollbackProcess_ShouldRollbackAccessorWhenLocalRollbackFails()
        {
            var syntaxTree = CSharpSyntaxTree.ParseText(@"using System;

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
                var dummy = """";
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
                    third = ""good"";
                    while (first.Length == 2)
                    {
                        dummy =  first + second;
                    }
                    while (first.Length < 2)
                    {
                        dummy =  second + first;
                    }
                }}}
                return third;
            }
        }
    }
}");
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

            var target = new RollbackProcess();

            using var ms = new MemoryStream();
            var compileResult = compiler.Emit(ms);

            var fixedCompilation = target.Start(compiler, compileResult.Diagnostics, false, false);

            var rollbackedResult = fixedCompilation.Compilation.Emit(ms);

            rollbackedResult.Success.ShouldBeFalse();
            rollbackedResult.Diagnostics.ShouldHaveSingleItem();

            fixedCompilation = target.Start(fixedCompilation.Compilation, rollbackedResult.Diagnostics, false, false);
            rollbackedResult = fixedCompilation.Compilation.Emit(ms);

            rollbackedResult.Success.ShouldBeTrue();
            // validate that only mutation 8 and 7 were rolled back
            fixedCompilation.RollbackedIds.ShouldBe(new Collection<int> { 8, 7, 6 });
        }

        [Fact]
        public void RollbackProcess_ShouldFailWhenLocalRollbackFailsAndInDevMode()
        {
            var syntaxTree = CSharpSyntaxTree.ParseText(@"using System;

namespace ExampleProject
{
    public class StringMagic
    {
        public int ActiveMutation = 1;

        public string AddTwoStrings(string first, string second, out string third)
        {
            var dummy = """";
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
                third = ""good"";
                while (first.Length == 2)
                {
                    dummy =  first + second;
                }
                while (first.Length < 2)
                {
                    dummy =  second + first;
                }
            }}}
                return null;
        }
    }
}");
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
                mutantIf3.WithAdditionalAnnotations(GetMutationIdMarker(7), _ifEngineMarker)
            );
            var annotatedSyntaxTree = root.SyntaxTree;

            var compiler = CSharpCompilation.Create("TestCompilation",
                syntaxTrees: new Collection<SyntaxTree>() { annotatedSyntaxTree },
                options: new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary),
                references: new List<PortableExecutableReference>() {
                    MetadataReference.CreateFromFile(typeof(object).Assembly.Location),
                    MetadataReference.CreateFromFile(typeof(Environment).Assembly.Location)
                });

            var target = new RollbackProcess();

            using var ms = new MemoryStream();
            var compileResult = compiler.Emit(ms);
            var fixedCompilation = target.Start(compiler, compileResult.Diagnostics, false, false);

            var rollbackedResult = fixedCompilation.Compilation.Emit(ms);

            rollbackedResult.Success.ShouldBeFalse();
            rollbackedResult.Diagnostics.ShouldHaveSingleItem();
            Should.Throw<CompilationException>(() => target.Start(fixedCompilation.Compilation, rollbackedResult.Diagnostics, false, true));
        }

        [Fact]
        public void RollbackProcess_ShouldRollbackError_RollbackedCompilationShouldCompileWhenUriIsEmpty()
        {
            var syntaxTree = CSharpSyntaxTree.ParseText(@"
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
                string someQuery = ""test"";
                new Uri(new Uri(string.Empty), ""/API?"" - someQuery);
            }
            else
            {
                string someQuery = ""test"";
                new System.Uri(new System.Uri(string.Empty), ""/API?"" + someQuery);
            }
        }
    }
}");
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

            var target = new RollbackProcess();

            using var ms = new MemoryStream();
            var fixedCompilation = target.Start(compiler, compiler.Emit(ms).Diagnostics, false, false);
            fixedCompilation.Compilation.Emit(ms).Success.ShouldBeTrue();

            // validate that only one of the compile errors marked the mutation as rollbacked.
            fixedCompilation.RollbackedIds.ShouldBe(new Collection<int> { 1 });
        }

         [Fact]
        public void RollbackProcess_ShouldOnlyRaiseExceptionOnFinalAttempt()
        {
            var syntaxTree = CSharpSyntaxTree.ParseText(@"
using System;

namespace ExampleProject
{
    public class MyException: Exception
    {
        public MyException(""a""-""b"", new Exception())
        {
        }
    }
}");

            var compiler = CSharpCompilation.Create("TestCompilation",
                syntaxTrees: new Collection<SyntaxTree>() { syntaxTree },
                options: new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary),
                references: new List<PortableExecutableReference>() {
                    MetadataReference.CreateFromFile(typeof(object).Assembly.Location),
                    MetadataReference.CreateFromFile(typeof(Environment).Assembly.Location),
                    MetadataReference.CreateFromFile(typeof(Uri).Assembly.Location),
                });

            var target = new RollbackProcess();

            using var ms = new MemoryStream();
            var compileResult = compiler.Emit(ms);

            Should.NotThrow(() => target.Start(compiler, compileResult.Diagnostics, false, false));
            Should.Throw<CompilationException>(() => target.Start(compiler, compileResult.Diagnostics, true, false));
        }
    }
}
