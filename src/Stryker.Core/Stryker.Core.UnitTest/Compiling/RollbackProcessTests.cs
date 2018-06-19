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
                    ifStatement.WithAdditionalAnnotations(new SyntaxAnnotation("MutantIf", "1"))
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
            var code = @"using System;

namespace ExampleProject
{
    public class Calculator
    {
        public string Subtract(string first, string second)
        {
            if(Environment.GetEnvironmentVariable(""ActiveMutation"") == ""6"") {
                if(first.Length > 2)
                {
                    return first - second;
                } else
                {
                    return second + first;
                }
            } else {
                if(Environment.GetEnvironmentVariable(""ActiveMutation"") == ""7"") {
                    if(first.Length > 2)
                    {
                        return first + second;
                    } else
                    {
                        return second - first;
                    }
                } else {
                    if(first.Length > 2)
                    {
                        return first + second;
                    } else
                    {
                        return second + first;
                    }
                }
            }
        }
    }
}";
            var syntaxTree = CSharpSyntaxTree.ParseText(code);
            var root = syntaxTree.GetRoot();
            Console.WriteLine(code);
            Console.WriteLine("Fullspan should be 1094 but was " + root.FullSpan);
            var mutantIf1 = root.FindNode(new TextSpan(154, 921));
            root = root.ReplaceNode(
                mutantIf1,
                mutantIf1.WithAdditionalAnnotations(new SyntaxAnnotation("MutantIf", "6"))
            );
            var mutantIf2 = root.FindNode(new TextSpan(462, 598));
            root = root.ReplaceNode(
                mutantIf2,
                mutantIf2.WithAdditionalAnnotations(new SyntaxAnnotation("MutantIf", "7"))
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
            if(System.Environment.GetEnvironmentVariable(""ActiveMutation"")==""8""){            if(first.Length > 2)
            {
                return first + second;
            } else
            {
                return second - first;
            }
        }else{if(System.Environment.GetEnvironmentVariable(""ActiveMutation"")==""7""){            if(first.Length > 2)
            {
                return first - second;
            } else
            {
                return second + first;
            }
}else{if(System.Environment.GetEnvironmentVariable(""ActiveMutation"")==""6""){            if(first.Length >= 2)
            {
                return first + second;
            } else
            {
                return second + first;
            }
}else{if(System.Environment.GetEnvironmentVariable(""ActiveMutation"")==""5""){            if(first.Length< 2)
            {
                return first + second;
            } else
            {
                return second + first;
            }
}else{            if(first.Length > 2)
            {
                return first + second;
            } else
            {
                return second + first;
            }
}}}}        }
    }
}");
            var root = syntaxTree.GetRoot();
            var ifStatements = root.DescendantNodes().Where(x => x is IfStatementSyntax).ToList();
            var mutantIf1 = root.FindNode(new TextSpan(172, 1207));
            root = root.ReplaceNode(
                mutantIf1,
                mutantIf1.WithAdditionalAnnotations(new SyntaxAnnotation("MutantIf", "8"))
            );
            var mutantIf2 = root.FindNode(new TextSpan(434, 944));
            root = root.ReplaceNode(
                mutantIf2,
                mutantIf2.WithAdditionalAnnotations(new SyntaxAnnotation("MutantIf", "7"))
            );
            var mutantIf3 = root.FindNode(new TextSpan(688, 689));
            root = root.ReplaceNode(
                mutantIf3,
                mutantIf3.WithAdditionalAnnotations(new SyntaxAnnotation("MutantIf", "6"))
            );
            var mutantIf4 = root.FindNode(new TextSpan(943, 433));
            root = root.ReplaceNode(
                mutantIf4,
                mutantIf4.WithAdditionalAnnotations(new SyntaxAnnotation("MutantIf", "5"))
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
                fixedCompilation.RollbackedIds.ShouldBe(new Collection<int> { 8, 7 });
            }
        }
    }
}
