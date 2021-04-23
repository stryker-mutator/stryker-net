using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Shouldly;
using Stryker.Core.Compiling;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using Microsoft.CodeAnalysis.Emit;
using Stryker.Core.Exceptions;
using Xunit;

namespace Stryker.Core.UnitTest.Compiling
{
    public class RollbackProcessTests
    {
        private readonly SyntaxAnnotation _ifEngineMarker = new SyntaxAnnotation("Injector", "IfInstrumentationEngine");
        private readonly SyntaxAnnotation _conditionalEngineMarker = new SyntaxAnnotation("Injector", "ConditionalInstrumentationEngine");

        private SyntaxAnnotation GetMutationMarker(int id)
        {
            return new SyntaxAnnotation("Mutation", id.ToString());
        }

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
            var ifStatement = syntaxTree
                .GetRoot()
                .DescendantNodes()
                .First(x => x is IfStatementSyntax);
            var annotatedSyntaxTree = syntaxTree.GetRoot()
                .ReplaceNode(
                    ifStatement, 
                    ifStatement.WithAdditionalAnnotations(GetMutationMarker(1), _ifEngineMarker)
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
                while (first.Length > 2)
                {
                    return first + second;
                } 
                while (first.Length < 2)
                {
                    return (System.Environment.GetEnvironmentVariable(""ActiveMutation"") == ""7"" ? second - first : second + first);
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
                mutantIf.WithAdditionalAnnotations(GetMutationMarker(6), _ifEngineMarker)
            );
            var mutantCondition = root.DescendantNodes().First(x => x is ParenthesizedExpressionSyntax parenthesized && parenthesized.Expression is ConditionalExpressionSyntax);
            root = root.ReplaceNode(
                mutantCondition,
                mutantCondition.WithAdditionalAnnotations(GetMutationMarker(7), _conditionalEngineMarker)
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

                var fixedCompilation = target.Start(compiler, compileResult.Diagnostics,false, false);

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
                mutantIf1.WithAdditionalAnnotations(GetMutationMarker(8), _ifEngineMarker)
            );
            var mutantIf2 = root.DescendantNodes().OfType<IfStatementSyntax>().ToList()[1];
            root = root.ReplaceNode(
                mutantIf2,
                mutantIf2.WithAdditionalAnnotations(GetMutationMarker(7), _ifEngineMarker)
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

                var fixedCompilation = target.Start(compiler, compileResult.Diagnostics, false,false);

                var rollbackedResult = fixedCompilation.Compilation.Emit(ms);

                rollbackedResult.Success.ShouldBeTrue();
                // validate that only mutation 8 and 7 were rollbacked
                fixedCompilation.RollbackedIds.ShouldBe(new Collection<int> { 8, 7 });
            }
        }


        [Fact]
        public void RollbackProcess_ShouldRollbackMethodWhenLocalRollbackFails()
        {
            var syntaxTree = CSharpSyntaxTree.ParseText(@"using System;

namespace ExampleProject
{
    public class StringMagic
    {
        public string AddTwoStrings(string first, string second, out string third)
        {
            var dummy = """";
            if(System.Environment.GetEnvironmentVariable(""ActiveMutation"")==""8""){            
                while (first.Length > 2)
                {
                    dummy = first + second;
                } 
                while (first.Length < 2)
                {
                    dummy =  second - first;
                }
            }else{if(System.Environment.GetEnvironmentVariable(""ActiveMutation"")==""7""){            
                while (first.Length > 2)
                {
                    dummy =  first + second;
                } 
                while (first.Length < 2)
                {
                    dummy =  second - first;
                }
            }else{if(System.Environment.GetEnvironmentVariable(""ActiveMutation"")==""6""){            
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
                mutantIf1.WithAdditionalAnnotations(GetMutationMarker(8), _ifEngineMarker)
            );
            var mutantIf2 = root.DescendantNodes().OfType<IfStatementSyntax>().ToList()[1];
            root = root.ReplaceNode(
                mutantIf2,
                mutantIf2.WithAdditionalAnnotations(GetMutationMarker(7), _ifEngineMarker)
            );
            var mutantIf3 = root.DescendantNodes().OfType<IfStatementSyntax>().ToList()[2];
            root = root.ReplaceNode(
                mutantIf3,
                mutantIf3.WithAdditionalAnnotations(GetMutationMarker(6), _ifEngineMarker)
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

                var fixedCompilation = target.Start(compiler, compileResult.Diagnostics, false,false);

                var rollbackedResult = fixedCompilation.Compilation.Emit(ms);

                rollbackedResult.Success.ShouldBeFalse();
                rollbackedResult.Diagnostics.ShouldHaveSingleItem();

                fixedCompilation = target.Start(fixedCompilation.Compilation, rollbackedResult.Diagnostics, false,false);
                rollbackedResult = fixedCompilation.Compilation.Emit(ms);
                rollbackedResult.Success.ShouldBeTrue();
                // validate that only mutation 8 and 7 were rollbacked
                fixedCompilation.RollbackedIds.ShouldBe(new Collection<int> { 8, 7 ,6});
            }
        }

        [Fact]
        public void RollbackProcess_ShouldRollbackAcessorWhenLocalRollbackFails()
        {
            var syntaxTree = CSharpSyntaxTree.ParseText(@"using System;

namespace ExampleProject
{
    public class StringMagic
    {
        public string AddTwoStrings
        {
            get
            {
                string first = string.Empty;
                string second = string.Empty;
                string third;
                var dummy = """";
                if(System.Environment.GetEnvironmentVariable(""ActiveMutation"")==""8""){            
                    while (first.Length > 2)
                    {
                        dummy = first + second;
                    } 
                    while (first.Length < 2)
                    {
                        dummy =  second - first;
                    }
                }else{if(System.Environment.GetEnvironmentVariable(""ActiveMutation"")==""7""){            
                    while (first.Length > 2)
                    {
                        dummy =  first + second;
                    } 
                    while (first.Length < 2)
                    {
                        dummy =  second - first;
                    }
                }else{if(System.Environment.GetEnvironmentVariable(""ActiveMutation"")==""6""){            
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
                mutantIf1.WithAdditionalAnnotations(GetMutationMarker(8), _ifEngineMarker)
            );
            var mutantIf2 = root.DescendantNodes().OfType<IfStatementSyntax>().ToList()[1];
            root = root.ReplaceNode(
                mutantIf2,
                mutantIf2.WithAdditionalAnnotations(GetMutationMarker(7), _ifEngineMarker)
            );
            var mutantIf3 = root.DescendantNodes().OfType<IfStatementSyntax>().ToList()[2];
            root = root.ReplaceNode(
                mutantIf3,
                mutantIf3.WithAdditionalAnnotations(GetMutationMarker(6), _ifEngineMarker)
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

                var fixedCompilation = target.Start(compiler, compileResult.Diagnostics, false,false);

                var rollbackedResult = fixedCompilation.Compilation.Emit(ms);
                
                rollbackedResult.Success.ShouldBeFalse();
                rollbackedResult.Diagnostics.ShouldHaveSingleItem();

                fixedCompilation = target.Start(fixedCompilation.Compilation, rollbackedResult.Diagnostics, false,false);
                rollbackedResult = fixedCompilation.Compilation.Emit(ms);

                rollbackedResult.Success.ShouldBeTrue();
                // validate that only mutation 8 and 7 were rollbacked
                fixedCompilation.RollbackedIds.ShouldBe(new Collection<int> { 8, 7 ,6});
            }
        }

        [Fact]
        public void RollbackProcess_ShouldFailWhenLocalRollbackFailsAndInDevMode()
        {
            var syntaxTree = CSharpSyntaxTree.ParseText(@"using System;

namespace ExampleProject
{
    public class StringMagic
    {
        public string AddTwoStrings(string first, string second, out string third)
        {
            var dummy = """";
            if(System.Environment.GetEnvironmentVariable(""ActiveMutation"")==""8""){            
                while (first.Length > 2)
                {
                    dummy = first + second;
                } 
                while (first.Length < 2)
                {
                    dummy =  second - first;
                }
            }else{if(System.Environment.GetEnvironmentVariable(""ActiveMutation"")==""7""){            
                while (first.Length > 2)
                {
                    dummy =  first + second;
                } 
                while (first.Length < 2)
                {
                    dummy =  second - first;
                }
            }else{if(System.Environment.GetEnvironmentVariable(""ActiveMutation"")==""6""){            
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
                mutantIf1.WithAdditionalAnnotations(GetMutationMarker(8), _ifEngineMarker)
            );
            var mutantIf2 = root.DescendantNodes().OfType<IfStatementSyntax>().ToList()[1];
            root = root.ReplaceNode(
                mutantIf2,
                mutantIf2.WithAdditionalAnnotations(GetMutationMarker(7), _ifEngineMarker)
            );
            var mutantIf3 = root.DescendantNodes().OfType<IfStatementSyntax>().ToList()[2];
            root = root.ReplaceNode(
                mutantIf3,
                mutantIf3.WithAdditionalAnnotations(GetMutationMarker(7), _ifEngineMarker)
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
                var fixedCompilation = target.Start(compiler, compileResult.Diagnostics, false,false);

                var rollbackedResult = fixedCompilation.Compilation.Emit(ms);
                
                rollbackedResult.Success.ShouldBeFalse();
                rollbackedResult.Diagnostics.ShouldHaveSingleItem();
                Should.Throw<StrykerCompilationException>(() => {target.Start(fixedCompilation.Compilation, rollbackedResult.Diagnostics, false,true);});
            }
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
        public void Break()
        {
            if(System.Environment.GetEnvironmentVariable(""ActiveMutation"")==""1"")
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
                    ifStatement.WithAdditionalAnnotations(GetMutationMarker(1), _ifEngineMarker)
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

            using (var ms = new MemoryStream())
            {
                var fixedCompilation = target.Start(compiler, compiler.Emit(ms).Diagnostics, false,false);

                fixedCompilation.Compilation.Emit(ms).Success.ShouldBeTrue();
                
                // validate that only one of the compile errors marked the mutation as rollbacked.
                fixedCompilation.RollbackedIds.ShouldBe(new Collection<int> { 1 });
            }
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

            using (var ms = new MemoryStream())
            {
                var compileResult = compiler.Emit(ms);

                Should.NotThrow(() => target.Start(compiler, compileResult.Diagnostics, false, false));
                Should.Throw<StrykerCompilationException>(() => target.Start(compiler, compileResult.Diagnostics, true, false));
            }
        }
    }
}
