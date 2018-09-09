using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;
using Shouldly;
using Stryker.Core.Compiling;
using Stryker.Core.UnitTest.MutationTest;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using Xunit;

namespace Stryker.Core.UnitTest.Compiling
{
    public class RollbackProcessTests
    {
        [Fact]
        public void RollbackProcess_ShouldRollbackError_RollbackedCompilationShouldCompile()
        {
            var syntaxTree = CSharpSyntaxTree.ParseText(@"using System;

namespace ExampleProject
{
    public class Calculator
    {
        public string Subtract(string first, string second)
        {
if(Environment.GetEnvironmentVariable(""ActiveMutation"") == ""1"") {
            return first - second; // this will not compile

} else {
            return first + second;
}
        }
    }
}");
            var ifStatement = syntaxTree.GetRoot()
                .DescendantNodes()
                .Where(x => x is IfStatementSyntax)
                .First();
            var annotatedSyntaxTree = syntaxTree.GetRoot()
                .ReplaceNode(
                    ifStatement, 
                    ifStatement.WithAdditionalAnnotations(new SyntaxAnnotation("MutationIf", "1"))
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

                var fixedCompilation = target.Start(compiler, compileResult.Diagnostics);
                
                var rollbackedResult = fixedCompilation.Compilation.Emit(ms);

                rollbackedResult.Success.ShouldBeTrue();
            }
        }

        [Fact]
        public void RollbackProcess_ShouldRollbackAllCompileErrors()
        {
            var syntaxTree = CSharpSyntaxTree.ParseText(@"using System;

namespace ExampleProject
{
    public class Calculator
    {
        public string Subtract(string first, string second)
        {
            if(Environment.GetEnvironmentVariable(""ActiveMutation"") == ""6"") {
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
                if(Environment.GetEnvironmentVariable(""ActiveMutation"") == ""7"") {
                    while (first.Length > 2)
                    {
                        return first + second;
                    }
                    while (first.Length < 2)
                    {
                        return second - first;
                    }
                    return null;
                } else {
                    while (first.Length > 2)
                    {
                        return first + second;
                    } 
                    while (first.Length < 2)
                    {
                        return second + first;
                    }
                    return null;
                }
            }
        }
    }
}");
            var root = syntaxTree.GetRoot();

            var mutantIf1 = root.DescendantNodes().OfType<IfStatementSyntax>().First();
            root = root.ReplaceNode(
                mutantIf1,
                mutantIf1.WithAdditionalAnnotations(new SyntaxAnnotation("MutationIf", "6"))
            );
            var mutantIf2 = root.DescendantNodes().OfType<IfStatementSyntax>().Last();
            root = root.ReplaceNode(
                mutantIf2,
                mutantIf2.WithAdditionalAnnotations(new SyntaxAnnotation("MutationIf", "7"))
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

            using (var ms = new MemoryStream())
            {
                var compileResult = compiler.Emit(ms);

                var fixedCompilation = target.Start(compiler, compileResult.Diagnostics);

                var rollbackedResult = fixedCompilation.Compilation.Emit(ms);

                rollbackedResult.Success.ShouldBeTrue();
                fixedCompilation.RollbackedIds.ShouldBe(new Collection<int> { 6, 7 });
            }
        }

        [Fact]
        public void RollbackProcess_ShouldRollbackErrorsAndKeepTheRest()
        {
            var syntaxTree = CSharpSyntaxTree.ParseText(@"using System;

namespace ExampleProject
{
    public class StringMagic
    {
        public string AddTwoStrings(string first, string second)
        {
            if(System.Environment.GetEnvironmentVariable(""ActiveMutation"")==""8""){            
                while (first.Length > 2)
                {
                    return first - second;
                } 
                while (first.Length < 2)
                {
                    return second + first;
                }
                return null;
            }else{if(System.Environment.GetEnvironmentVariable(""ActiveMutation"")==""7""){            
                while (first.Length > 2)
                {
                    return first + second;
                } 
                while (first.Length < 2)
                {
                    return second - first;
                }
                return null;
            }else{if(System.Environment.GetEnvironmentVariable(""ActiveMutation"")==""6""){            
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
                mutantIf1.WithAdditionalAnnotations(new SyntaxAnnotation("MutationIf", "8"))
            );
            var mutantIf2 = root.DescendantNodes().OfType<IfStatementSyntax>().ToList()[1];
            root = root.ReplaceNode(
                mutantIf2,
                mutantIf2.WithAdditionalAnnotations(new SyntaxAnnotation("MutationIf", "7"))
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

            using (var ms = new MemoryStream())
            {
                var compileResult = compiler.Emit(ms);

                var fixedCompilation = target.Start(compiler, compileResult.Diagnostics);

                var rollbackedResult = fixedCompilation.Compilation.Emit(ms);

                rollbackedResult.Success.ShouldBeTrue();
                // validate that only mutation 8 and 7 were rollbacked
                fixedCompilation.RollbackedIds.ShouldBe(new Collection<int> { 8, 7 });
            }
        }
    }
}
