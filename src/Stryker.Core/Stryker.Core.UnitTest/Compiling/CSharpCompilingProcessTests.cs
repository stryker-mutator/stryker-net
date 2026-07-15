using System.Collections.Generic;
using System.Collections.Immutable;
using System.IO;
using System.IO.Abstractions.TestingHelpers;
using System.Linq;
using System.Reflection;
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Shouldly;
using Stryker.Abstractions;
using Stryker.Abstractions.Exceptions;
using Stryker.Abstractions.Options;
using Stryker.Abstractions.Testing;
using Stryker.Configuration.Options;
using Stryker.Core.Compiling;
using Stryker.Core.InjectedHelpers;
using Stryker.Core.MutationTest;
using Stryker.Core.ProjectComponents;
using Stryker.Core.ProjectComponents.Csharp;
using Stryker.Core.ProjectComponents.SourceProjects;
using Stryker.Core.ProjectComponents.TestProjects;

namespace Stryker.Core.UnitTest.Compiling;

[TestClass]
public class CSharpCompilingProcessTests : TestBase
{
    [TestMethod]
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
            SourceProjectInfo = new SourceProjectInfo
            {
                AnalyzerResult = TestHelper.SetupProjectAnalyzerResult(
                    projectFilePath: "/c/project.csproj",
                    properties: new Dictionary<string, string>()
                    {
                        { "TargetDir", "" },
                        { "AssemblyName", "AssemblyName"},
                        { "TargetFileName", "TargetFileName.dll"},
                    },
                    // add a reference to system so the example code can compile
                    references: [typeof(object).Assembly.Location]
                ).Object
            }
        };
        var rollbackProcessMock = new Mock<ICSharpRollbackProcess>(MockBehavior.Strict);

        var target = new CsharpCompilingProcess(input, rollbackProcessMock.Object, syntaxTrees:[syntaxTree]);

        using var ms = new MemoryStream();
        using var symbol = new MemoryStream();
        var result = target.Compile(ms, symbol);
        result.Success.ShouldBe(true);
        ms.Length.ShouldBeGreaterThan(100, "No value was written to the MemoryStream by the compiler");
    }

    [TestMethod]
    public void CompilesWithoutMutations_IsTrue_WhenRemovingMutationsFixesTheBuild()
    {
        // A file that only fails to compile because of a mutation: with the mutation stripped and
        // the generators re-run the baseline builds, so the failure is mutation-induced.
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
            """);
        var ifStatement = syntaxTree.GetRoot().DescendantNodes().OfType<IfStatementSyntax>().First();
        var annotated = syntaxTree.GetRoot()
            .ReplaceNode(ifStatement, ifStatement.WithAdditionalAnnotations(
                new SyntaxAnnotation("MutationId", "1"), new SyntaxAnnotation("Injector", "IfInstrumentationEngine")))
            .SyntaxTree;
        var target = BuildCompilingProcess(annotated);
        target.GetSemanticModel(annotated); // initialise the compilation

        // The tree compiles on the uninstrumented baseline, so its failure was mutation-induced.
        ((IBaselineCompiler)target).FailsWithoutInstrumentation(annotated).ShouldBeFalse();
    }

    [TestMethod]
    public void CompilesWithoutMutations_IsFalse_WhenTheFailureIsIndependentOfMutation()
    {
        // A file that fails to compile with no mutation involved (a missing symbol): stripping
        // mutations and re-running generators changes nothing, so the baseline still fails.
        var syntaxTree = CSharpSyntaxTree.ParseText(
            """
            namespace ExampleProject { public class Anchor { public object Broken() => Missing.Symbol; } }
            """);
        var target = BuildCompilingProcess(syntaxTree);
        target.GetSemanticModel(syntaxTree); // initialise the compilation

        // The tree still fails on the uninstrumented baseline, so the failure is not mutation-caused.
        ((IBaselineCompiler)target).FailsWithoutInstrumentation(syntaxTree).ShouldBeTrue();
    }

    [TestMethod]
    public void CompilesWithoutMutations_ExcludesStrykerInjectedHelperTrees()
    {
        // A clean user file plus a (deliberately broken) Stryker injected helper tree. The baseline
        // is the user's source only - Stryker's own injected helpers must be dropped - so it
        // compiles cleanly; if the helper were kept, its error would wrongly make the baseline fail
        // and a mutation-induced failure would be misreported as baseline.
        var userTree = CSharpSyntaxTree.ParseText(
            "namespace ExampleProject { public class Ok { public int Value => 1; } }");
        var brokenHelper = CSharpSyntaxTree.ParseText(
            "namespace Stryker { public class Helper { public object X() => Missing.Symbol; } }",
            path: CodeInjection.HelperFiles.First());

        var target = BuildCompilingProcess(userTree, brokenHelper);
        target.GetSemanticModel(userTree); // initialise the compilation

        // The user tree compiles clean on the baseline (helper dropped), so its failure would be
        // mutation-induced - the broken helper does not turn it into a baseline failure.
        ((IBaselineCompiler)target).FailsWithoutInstrumentation(userTree).ShouldBeFalse();
    }

    private static CsharpCompilingProcess BuildCompilingProcess(params SyntaxTree[] syntaxTrees)
    {
        var input = new MutationTestInput
        {
            SourceProjectInfo = new SourceProjectInfo
            {
                AnalyzerResult = TestHelper.SetupProjectAnalyzerResult(
                    projectFilePath: "/c/project.csproj",
                    properties: new Dictionary<string, string>
                    {
                        { "TargetDir", "" }, { "AssemblyName", "AssemblyName" }, { "TargetFileName", "TargetFileName.dll" },
                    },
                    references: [typeof(object).Assembly.Location]).Object
            }
        };
        var rollbackProcessMock = new Mock<ICSharpRollbackProcess>(MockBehavior.Strict);
        return new CsharpCompilingProcess(input, rollbackProcessMock.Object, syntaxTrees: syntaxTrees);
    }

    [TestMethod]
    public void CompilingProcessTests_ShouldSupportPackageReferenceAliases()
    {
        var syntaxTree = CSharpSyntaxTree.ParseText(@"
extern alias TheAlias;
using TheAlias::System;

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
        var alias = new Dictionary<string, ImmutableArray<string>>();
        var immutableArray = ImmutableArray.Create("TheAlias");
        alias[typeof(object).Assembly.Location]=immutableArray;

        var input = new MutationTestInput
        {
            SourceProjectInfo = new SourceProjectInfo
            {
                AnalyzerResult = TestHelper.SetupProjectAnalyzerResult(
                    projectFilePath: "/c/project.csproj",
                    properties: new Dictionary<string, string>()
                    {
                        { "TargetDir", "" },
                        { "AssemblyName", "AssemblyName" },
                        { "TargetFileName", "TargetFileName.dll" },
                    },
                    // add a reference to system so the example code can compile
                    references: [typeof(object).Assembly.Location],
                    aliases: alias.ToImmutableDictionary()
                ).Object
            }
        };
        var rollbackProcessMock = new Mock<ICSharpRollbackProcess>(MockBehavior.Strict);


        var target = new CsharpCompilingProcess(input, rollbackProcessMock.Object, syntaxTrees:[syntaxTree]);

        using (var ms = new MemoryStream())
        {
            var result = target.Compile(ms, null);
            result.Success.ShouldBe(true);
            ms.Length.ShouldBeGreaterThan(100, "No value was written to the MemoryStream by the compiler");
        }
    }

    [TestMethod]
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
            SourceProjectInfo = new SourceProjectInfo
            {
                AnalyzerResult = TestHelper.SetupProjectAnalyzerResult(
                    projectFilePath: "/c/project.csproj",
                    properties: new Dictionary<string, string>()
                    {
                        { "TargetDir", "" },
                        { "AssemblyName", "AssemblyName"},
                        { "TargetFileName", "TargetFileName.dll"},
                    },
                    // add a reference to system so the example code can compile
                    references: [typeof(object).Assembly.Location]
                ).Object
            }
        };
        var rollbackProcessMock = new Mock<ICSharpRollbackProcess>(MockBehavior.Strict);
        rollbackProcessMock.Setup(x => x.RollbackMutationsInError(It.IsAny<ICompilationContent>(), It.IsAny<ImmutableArray<Diagnostic>>(), It.IsAny<ICSharpRollbackProcess.Mode>(), false))
                        .Returns((ICompilationContent _, ImmutableArray<Diagnostic> _, ICSharpRollbackProcess.Mode _, bool _) => null);

        var target = new CsharpCompilingProcess(input, rollbackProcessMock.Object, new StrykerOptions(), [syntaxTree]);

        using (var ms = new MemoryStream())
        {
            Should.Throw<CompilationException>(() => target.Compile(ms, null));
        }
        rollbackProcessMock.Verify(x => x.RollbackMutationsInError(It.IsAny<ICompilationContent>(), It.IsAny<ImmutableArray<Diagnostic>>(), ICSharpRollbackProcess.Mode.Normal, false),
            Times.AtLeast(2));
    }

    [TestMethod]
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
            SourceProjectInfo = new SourceProjectInfo
            {
                AnalyzerResult = TestHelper.SetupProjectAnalyzerResult(
                    projectFilePath: "/c/project.csproj",
                    properties: new Dictionary<string, string>()
                    {
                        { "TargetDir", "" },
                        { "AssemblyName", "AssemblyName"},
                        { "TargetFileName", "TargetFileName.dll"},
                    },
                    // add a reference to system so the example code can compile
                    references: [typeof(object).Assembly.Location]
                ).Object
            }
        };
        var rollbackProcessMock = new Mock<ICSharpRollbackProcess>(MockBehavior.Strict);

        var target = new CsharpCompilingProcess(input, rollbackProcessMock.Object, syntaxTrees:[syntaxTree]);

        using var ms = new MemoryStream();
        target.Compile(ms, null);

        ms.Length.ShouldBeGreaterThan(100, "No value was written to the MemoryStream by the compiler");
    }

    [TestMethod]
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
            SourceProjectInfo = new SourceProjectInfo
            {
                AnalyzerResult = TestHelper.SetupProjectAnalyzerResult(
                    properties: new Dictionary<string, string>()
                    {
                        { "TargetDir", "" },
                        { "AssemblyName", "AssemblyName" },
                        { "TargetFileName", "TargetFileName.dll" },
                        { "SignAssembly", "true" },
                        { "AssemblyOriginatorKeyFile", Path.GetFullPath(Path.Combine("TestResources", "StrongNameKeyFile.snk")) }
                    },
                    // add a reference to system so the example code can compile
                    references: [typeof(object).Assembly.Location],
                    projectFilePath: "TestResources"
                ).Object
            }
        };
        var rollbackProcessMock = new Mock<ICSharpRollbackProcess>(MockBehavior.Strict);
        var target = new CsharpCompilingProcess(input, rollbackProcessMock.Object, syntaxTrees:[syntaxTree]);

        using var ms = new MemoryStream();
        var result = target.Compile(ms, null);
        result.Success.ShouldBe(true);

        var key = Assembly.Load(ms.ToArray()).GetName().GetPublicKey();
        key.Length.ShouldBe(160, "Assembly was not signed");
        ms.Length.ShouldBeGreaterThan(100, "No value was written to the MemoryStream by the compiler");
    }

    [TestMethod]
    public void CompilingProcessTests_AssemblyShouldCompileUnsigned_WhenSignAssemblyTrue_ButKeyFileMissing()
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
        var input = new MutationTestInput
        {
            SourceProjectInfo = new SourceProjectInfo
            {
                AnalyzerResult = TestHelper.SetupProjectAnalyzerResult(
                    properties: new Dictionary<string, string>()
                    {
                        { "TargetDir", "" },
                        { "AssemblyName", "AssemblyName"},
                        { "TargetFileName", "TargetFileName.dll"},
                        { "SignAssembly", "true" }
                    },
                    // add a reference to system so the example code can compile
                    references: [typeof(object).Assembly.Location],
                    projectFilePath: "TestResources"
                ).Object
            }

        };
        var rollbackProcessMock = new Mock<ICSharpRollbackProcess>(MockBehavior.Strict);

        var target = new CsharpCompilingProcess(input, rollbackProcessMock.Object, syntaxTrees: [syntaxTree]);

        using var ms = new MemoryStream();
        var result = target.Compile(ms, null);
        result.Success.ShouldBe(true);

        var key = Assembly.Load(ms.ToArray()).GetName().GetPublicKey();
        key.Length.ShouldBe(0, "Assembly was signed");
        ms.Length.ShouldBeGreaterThan(100, "No value was written to the MemoryStream by the compiler");
    }

    [TestMethod]
    public void CompilingProcessTests_ProperlyFailsWhenSigningKeyIsNotFound()
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
            SourceProjectInfo = new SourceProjectInfo
            {
                AnalyzerResult = TestHelper.SetupProjectAnalyzerResult(properties: new Dictionary<string, string>()
                    {
                        { "TargetDir", "" },
                        { "TargetFileName", "TargetFileName.dll"},
                        { "AssemblyName", "AssemblyName"},
                        { "SignAssembly", "true" },
                        { "AssemblyOriginatorKeyFile", "DoesNotExist.snk" }
                    },
                    projectFilePath: "project.csproj",
                    // add a reference to system so the example code can compile
                    references: [typeof(object).Assembly.Location]
                ).Object
            }

        };
        var rollbackProcessMock = new Mock<ICSharpRollbackProcess>(MockBehavior.Strict);

        var target = new CsharpCompilingProcess(input, rollbackProcessMock.Object, syntaxTrees: [syntaxTree]);

        using var ms = new MemoryStream();
        Should.Throw<CompilationException>(() => target.Compile(ms, null));
    }

    [TestMethod]
    public void CompilingProcessTests_MustIncludeVersionInfo()
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
            SourceProjectInfo = new SourceProjectInfo
            {
                AnalyzerResult = TestHelper.SetupProjectAnalyzerResult(
                    projectFilePath: "/c/project.csproj",
                    properties: new Dictionary<string, string>()
                    {
                        { "TargetDir", "" },
                        { "TargetFileName", "TargetFileName.dll" },
                        { "AssemblyName", "AssemblyName"},
                    },
                    // add a reference to system so the example code can compile
                    references: [typeof(object).Assembly.Location]
                ).Object
            }
        };
        var rollbackProcessMock = new Mock<ICSharpRollbackProcess>(MockBehavior.Strict);

        var target = new CsharpCompilingProcess(input, rollbackProcessMock.Object, syntaxTrees:[syntaxTree]);

        using (var ms = new MemoryStream())
        {
            var result = target.Compile(ms, null);
            result.Success.ShouldBe(true);

            Assembly.Load(ms.ToArray()).GetName().Version.ToString().ShouldBe("0.0.0.0");
        }
    }

    [TestMethod]
    public void ShouldCompileAndRollbackErrorWhenUninitializedVariable()
    {
        var sourceFile = @"using System;
using System.Collections.Generic;

namespace ExampleProject
{
public class Calculator
{
    public int Dummy()
    {
        int z;
        int y;
        if (true)
        {
            if (true)
            {
                z = 1;
                y = 0;
            }
            else
            {
                z = 0;
                y = 1;
            }
        }
        return z + y;
    }
}
}";
        var projectContentsMutants = MutateAndCompileSource(sourceFile);
        // those results can change if mutators are added.
        projectContentsMutants.Count(t => t.ResultStatus == MutantStatus.CompileError).ShouldBe(9);
        projectContentsMutants.Count(t => t.ResultStatus == MutantStatus.Pending).ShouldBe(0);
    }

    [TestMethod]
    public void ShouldCompileAndRollbackErrorsForEventHandler()
    {
        var sourceFile = @"using System;

namespace ExampleProject
{
public class Calculator
{
    public int Subtract(int first, int second)
    {
        return first - second;
    }
    private event Action SendCompleted;

    void TestMethod(){
        Action<Action> unsubscribe = (handler) => SendCompleted -= handler;
    }
}
}";
        var projectContentsMutants = MutateAndCompileSource(sourceFile);
        // those results can change if mutators are added.
        projectContentsMutants.Count(t => t.ResultStatus == MutantStatus.CompileError).ShouldBe(1);
        projectContentsMutants.Count(t => t.ResultStatus == MutantStatus.Pending).ShouldBe(3);
    }

    private static IEnumerable<IMutant> MutateAndCompileSource(string sourceFile)
    {
        var filesystemRoot = Path.GetPathRoot(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location));

        var inputFile = new CsharpFileLeaf()
        {
            SourceCode = sourceFile,
            SyntaxTree = CSharpSyntaxTree.ParseText(sourceFile)
        };

        var fileSystem = new MockFileSystem(new Dictionary<string, MockFileData>
        {
            {
                Path.Combine(filesystemRoot, "ExampleProject", "Calculator.cs"),
                new MockFileData(sourceFile)
            },
            {
                Path.Combine(filesystemRoot, "ExampleProject.Test", "bin", "Debug", "netcoreapp2.0", "ExampleProject.dll"),
                new MockFileData("Bytecode")
            },
            {
                Path.Combine(filesystemRoot, "ExampleProject.Test", "obj", "Release", "netcoreapp2.0",
                    "ExampleProject.dll"),
                new MockFileData("Bytecode")
            }
        });

        var input = new MutationTestInput
        {
            SourceProjectInfo = new SourceProjectInfo
            {
                AnalyzerResult = TestHelper.SetupProjectAnalyzerResult(
                    projectFilePath: "/c/project.csproj",
                    properties: new Dictionary<string, string>
                    {
                        { "TargetDir", "Project" },
                        { "AssemblyName", "AssemblyName" },
                        { "TargetFileName", "TargetFileName.dll" },
                    },
                    // add a reference to system so the example code can compile, plus the assemblies the
                    // injected MutantControl needs for its MemoryMappedFile-based MTP mutant control. The
                    // MemoryMappedFiles assembly is compiled against the contract assemblies, so resolving it
                    // also requires System.Runtime (FileStream/FileMode/Object/Enum) and
                    // System.Runtime.InteropServices (UnmanagedMemoryAccessor, the base of the view accessor).
                    references:
                    [
                        typeof(object).Assembly.Location,
                        typeof(System.IO.MemoryMappedFiles.MemoryMappedFile).Assembly.Location,
                        Assembly.Load("System.Runtime").Location,
                        Assembly.Load("System.Runtime.InteropServices").Location
                    ]
                ).Object,
                TestProjectsInfo = new TestProjectsInfo(fileSystem)
                {
                    TestProjects = new List<TestProject> {
                        new TestProject(fileSystem, TestHelper.SetupProjectAnalyzerResult(
                            projectFilePath: "/c/test.csproj",
                            properties: new Dictionary<string, string>
                            {
                                { "TargetDir", "Project" },
                                { "AssemblyName", "AssemblyName" },
                                { "TargetFileName", "TargetFileName.dll" },
                            },
                            // add a reference to system so the example code can compile
                            references: [typeof(object).Assembly.Location]
                        ).Object),
                    }
                }
            },

            TestRunner = new Mock<ITestRunner>(MockBehavior.Default).Object
        };
         var folder = new FolderComposite();
        var injector = input.SourceProjectInfo.CodeInjector;
        folder.Add(inputFile);
        foreach (var (name, code) in injector.MutantHelpers)
        {
            folder.AddCompilationSyntaxTree(CSharpSyntaxTree.ParseText(code, path: name, encoding: Encoding.UTF32));
        }

        input.SourceProjectInfo.ProjectContents = folder;

        var options = new StrykerOptions
        {
            MutationLevel = MutationLevel.Complete,
            OptimizationMode = OptimizationModes.CoverageBasedTest,
        };
        var process = new CsharpMutationProcess(fileSystem, TestLoggerFactory.CreateLogger<CsharpMutationProcess>());
        process.Mutate(input, options);

        var projectContentsMutants = input.SourceProjectInfo.ProjectContents.Mutants;
        return projectContentsMutants;
    }
}
