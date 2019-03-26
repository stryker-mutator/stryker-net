using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Moq;
using Shouldly;
using Stryker.Core.Compiling;
using Stryker.Core.Initialisation;
using Stryker.Core.MutationTest;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Collections.ObjectModel;
using System.IO;
using System.Reflection;
using Xunit;

namespace Stryker.Core.UnitTest.Compiling
{
    public class CompilingProcessTests
    {
        [Fact]
        public void CompilingProcessTests_ShouldCompile()
        {
            var syntaxTree = CSharpSyntaxTree.ParseText(@"using System;

namespace ExampleProject
{
    public class Calculator
    {
        public int Subtract(int first, int second)
        {
            return first - second;
        }
    }
}");
            var input = new MutationTestInput()
            {
                ProjectInfo = new ProjectInfo()
                {
                    ProjectUnderTestAnalyzerResult = new ProjectAnalyzerResult(null, null)
                    {
                        Properties = new Dictionary<string, string>()
                        {
                            { "AssemblyTitle", "AssemblyName"},
                        },
                        Resources = new List<ResourceDescription>()
                    },
                    TestProjectAnalyzerResult = new ProjectAnalyzerResult(null, null)
                    {
                        Properties = new Dictionary<string, string>()
                        {
                            { "AssemblyTitle", "AssemblyName"},
                        },
                        Resources = new List<ResourceDescription>()
                    }
                },
                AssemblyReferences = new List<PortableExecutableReference>() {
                    MetadataReference.CreateFromFile(typeof(object).Assembly.Location)
                }
            };
            var rollbackProcessMock = new Mock<IRollbackProcess>(MockBehavior.Strict);

            var target = new CompilingProcess(input, rollbackProcessMock.Object);

            using (var ms = new MemoryStream())
            {
                var result = target.Compile(new Collection<SyntaxTree>() { syntaxTree }, ms, false);
                result.Success.ShouldBe(true);
                ms.Length.ShouldBeGreaterThan(100, "No value was written to the MemoryStream by the compiler");
            }
        }

        [Fact]
        public void CompilingProcessTests_ShouldCallRollbackProcess_OnCompileError()
        {
            var syntaxTree = CSharpSyntaxTree.ParseText(@"using System;

namespace ExampleProject
{
    public class Calculator
    {
        public int Subtract(string first, string second)
        {
            return first - second;
        }
    }
}");
            var input = new MutationTestInput()
            {
                ProjectInfo = new ProjectInfo()
                {
                    ProjectUnderTestAnalyzerResult = new ProjectAnalyzerResult(null, null)
                    {
                        Properties = new Dictionary<string, string>()
                        {
                            { "AssemblyTitle", "AssemblyName"},
                        },
                        Resources = new List<ResourceDescription>()
                    },
                    TestProjectAnalyzerResult = new ProjectAnalyzerResult(null, null)
                    {
                        Properties = new Dictionary<string, string>()
                        {
                            { "AssemblyTitle", "AssemblyName"},
                        },
                        Resources = new List<ResourceDescription>()
                    }
                },
                AssemblyReferences = new List<PortableExecutableReference>() {
                    MetadataReference.CreateFromFile(typeof(object).Assembly.Location)
                }
            };
            var rollbackProcessMock = new Mock<IRollbackProcess>(MockBehavior.Strict);
            rollbackProcessMock.Setup(x => x.Start(It.IsAny<CSharpCompilation>(), It.IsAny<ImmutableArray<Diagnostic>>(), It.IsAny<bool>()))
                            .Returns((CSharpCompilation compilation, ImmutableArray<Diagnostic> diagnostics, bool devMode) =>
                            new RollbackProcessResult()
                            {
                                Compilation = compilation
                            });

            var target = new CompilingProcess(input, rollbackProcessMock.Object);

            using (var ms = new MemoryStream())
            {
                Should.Throw<ApplicationException>(() => target.Compile(new Collection<SyntaxTree>() { syntaxTree }, ms, false));
            }
            rollbackProcessMock.Verify(x => x.Start(It.IsAny<CSharpCompilation>(), It.IsAny<ImmutableArray<Diagnostic>>(), It.IsAny<bool>()),
                Times.Exactly(2));
        }

        [Fact]
        public void CompilingProcessTests_ShouldOnlyRollbackErrors()
        {
            var syntaxTree = CSharpSyntaxTree.ParseText(@"using System;

namespace ExampleProject
{
    public class Calculator
    {
        public int Subtract(int first, int second)
        {
            return first - second;
        }
    }
}");
            var input = new MutationTestInput()
            {
                ProjectInfo = new ProjectInfo()
                {
                    ProjectUnderTestAnalyzerResult = new ProjectAnalyzerResult(null, null)
                    {
                        Properties = new Dictionary<string, string>()
                        {
                            { "AssemblyTitle", "AssemblyName"},
                        },
                        Resources = new List<ResourceDescription>()
                    },
                    TestProjectAnalyzerResult = new ProjectAnalyzerResult(null, null)
                    {
                        Properties = new Dictionary<string, string>()
                        {
                            { "AssemblyTitle", "AssemblyName"},
                        },
                        Resources = new List<ResourceDescription>()
                    }
                },
                AssemblyReferences = new List<PortableExecutableReference>() {
                    MetadataReference.CreateFromFile(typeof(object).Assembly.Location)
                }
            };
            var rollbackProcessMock = new Mock<IRollbackProcess>(MockBehavior.Strict);

            var target = new CompilingProcess(input, rollbackProcessMock.Object);

            using (var ms = new MemoryStream())
            {
                target.Compile(new Collection<SyntaxTree>() { syntaxTree }, ms, false);

                ms.Length.ShouldBeGreaterThan(100, "No value was written to the MemoryStream by the compiler");
            }
        }




        [Fact]
        public void CompilingProcessTests_SignedAssembliesMustBeSigned()
        {
            var syntaxTree = CSharpSyntaxTree.ParseText(@"

namespace ExampleProject
{
    public class Calculator
    {
        public int Subtract(int first, int second)
        {
            return first - second;
        }
    }
}");
            var input = new MutationTestInput()
            {
                ProjectInfo = new ProjectInfo()
                {
                    ProjectUnderTestAnalyzerResult = new ProjectAnalyzerResult(null, null)
                    {
                        Properties = new Dictionary<string, string>()
                        {
                            { "AssemblyTitle", "AssemblyName"},
                        },
                        Resources = new List<ResourceDescription>(),
                        SignAssembly = true,
                        AssemblyOriginatorKeyFile = Path.GetFullPath(Path.Combine("TestResources", "StrongNameKeyFile.snk"))
                    },
                    TestProjectAnalyzerResult = new ProjectAnalyzerResult(null, null)
                    {
                        Properties = new Dictionary<string, string>()
                        {
                            { "AssemblyTitle", "AssemblyName"},
                        },
                        Resources = new List<ResourceDescription>()
                    }
                },
                AssemblyReferences = new List<PortableExecutableReference>() {
                    MetadataReference.CreateFromFile(typeof(object).Assembly.Location)
                },
                
            };
            var rollbackProcessMock = new Mock<IRollbackProcess>(MockBehavior.Strict);

            var target = new CompilingProcess(input, rollbackProcessMock.Object);

            using (var ms = new MemoryStream())
            {
                var result = target.Compile(new Collection<SyntaxTree>() { syntaxTree }, ms, false);
                result.Success.ShouldBe(true);

                var key = Assembly.Load(ms.ToArray()).GetName().GetPublicKey();
                key.Length.ShouldBe(160, "Assembly was not signed");
                ms.Length.ShouldBeGreaterThan(100, "No value was written to the MemoryStream by the compiler");
            }
        }
    }
}
