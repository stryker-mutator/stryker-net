using System.Collections.Generic;
using System.Collections.Immutable;
using System.IO;
using System.IO.Abstractions.TestingHelpers;
using System.Linq;
using System.Reflection;
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Shouldly;
using Stryker.Abstractions;
using Stryker.Abstractions.Exceptions;
using Stryker.Abstractions.Options;
using Stryker.Abstractions.Testing;
using Stryker.Configuration.Options;
using Stryker.Core.Compiling;
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
            SourceProjectInfo = new SourceProjectInfo(TestHelper.SetupProjectAnalyzerResult(
                    projectFilePath: "/c/project.csproj",
                    properties: new Dictionary<string, string>()
                    {
                        { "TargetDir", "" },
                        { "AssemblyName", "AssemblyName"},
                        { "TargetFileName", "TargetFileName.dll"},
                    },
                    // add a reference to system so the example code can compile
                    references: [typeof(object).Assembly.Location]
                ).Object, null)
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

        var analyzerResult = TestHelper.SetupProjectAnalyzerResult(
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
        ).Object;
        var input = new MutationTestInput()
        {
            SourceProjectInfo = new SourceProjectInfo(analyzerResult, null)
        };

        var rollbackProcessMock = new Mock<ICSharpRollbackProcess>(MockBehavior.Strict);

        var target = new CsharpCompilingProcess(input, rollbackProcessMock.Object, syntaxTrees: [syntaxTree]);

        using var ms = new MemoryStream();
        var result = target.Compile(ms, null);
        result.Success.ShouldBe(true);
        ms.Length.ShouldBeGreaterThan(100, "No value was written to the MemoryStream by the compiler");
    }

    [TestMethod]
    public void ShouldReportWhenProjectCantBeBuilt()
    {
        var syntaxTree = CSharpSyntaxTree.ParseText(GetExampleCode(false));
        var analyzerResult = TestHelper.SetupProjectAnalyzerResult(
            projectFilePath: "/c/project.csproj",
            properties: new Dictionary<string, string>()
            {
                { "TargetDir", "" },
                { "AssemblyName", "AssemblyName"},
                { "TargetFileName", "TargetFileName.dll"},
            },
            // add a reference to system so the example code can compile
            references: [typeof(object).Assembly.Location]
        ).Object;

        var input = new MutationTestInput()
        {
            SourceProjectInfo = new SourceProjectInfo(analyzerResult, null)
                {ProjectContents = new CsharpFileLeaf{SyntaxTree = syntaxTree, MutatedSyntaxTree = syntaxTree, SourceCode = GetExampleCode(false)}}
        };


        var rollbackProcessMock = new Mock<ICSharpRollbackProcess>(MockBehavior.Strict);

        var target = new CsharpCompilingProcess(input, rollbackProcessMock.Object, syntaxTrees: [syntaxTree]);
        rollbackProcessMock.Setup(x => x.RollbackMutationsInError(It.IsAny<ICompilationContent>(), It.IsAny<ImmutableArray<Diagnostic>>(), It.IsAny<ICSharpRollbackProcess.Mode>(), false))
                        .Returns((ICompilationContent _, ImmutableArray<Diagnostic> _, ICSharpRollbackProcess.Mode _, bool _) => []);

        using var ms = new MemoryStream();
        Should.Throw<CompilationException>(() => target.Compile(ms, null)).Message.ShouldBe("Failed to build mutated version.");;
        rollbackProcessMock.Verify(x => x.RollbackMutationsInError(It.IsAny<ICompilationContent>(), It.IsAny<ImmutableArray<Diagnostic>>(), ICSharpRollbackProcess.Mode.Normal, false),
            Times.AtLeast(2));
    }

    [TestMethod]
    public void ShouldReportWhenProjectCantBeBuiltWhenDiagMode()
    {
        var syntaxTree = CSharpSyntaxTree.ParseText(GetExampleCode(false));
        var input = new MutationTestInput()
        {
            SourceProjectInfo = new SourceProjectInfo(
            TestHelper.SetupProjectAnalyzerResult(
                    projectFilePath: "/c/project.csproj",
                    properties: new Dictionary<string, string>()
                    {
                        { "TargetDir", "" },
                        { "AssemblyName", "AssemblyName"},
                        { "TargetFileName", "TargetFileName.dll"},
                    },
                    // add a reference to system so the example code can compile
                    references: [typeof(object).Assembly.Location]
                ).Object, null)
                {ProjectContents = new CsharpFileLeaf{SyntaxTree = syntaxTree, MutatedSyntaxTree = syntaxTree, SourceCode = GetExampleCode(false)}
            }
        };
        var rollbackProcessMock = new Mock<ICSharpRollbackProcess>(MockBehavior.Strict);
        rollbackProcessMock.Setup(x => x.RollbackMutationsInError(It.IsAny<ICompilationContent>(), It.IsAny<ImmutableArray<Diagnostic>>(), It.IsAny<ICSharpRollbackProcess.Mode>(), true))
                        .Returns((ICompilationContent _, ImmutableArray<Diagnostic> _, ICSharpRollbackProcess.Mode _, bool _) =>
                            []);

        var target = new CsharpCompilingProcess(input, rollbackProcessMock.Object, new StrykerOptions{DiagMode = true}, [syntaxTree]);

        using (var ms = new MemoryStream())
        {
            Should.Throw<CompilationException>(() => target.Compile(ms, null)).Message.ShouldBe("Stryker is unable to build this project.");
        }
        rollbackProcessMock.Verify(x =>
                x.RollbackMutationsInError(It.IsAny<ICompilationContent>(),
                It.IsAny<ImmutableArray<Diagnostic>>(), ICSharpRollbackProcess.Mode.Normal, true),
            Times.AtLeast(2));
    }

    [TestMethod]
    public void ShouldReportWhenProjectCantBeBuiltWhenMutatedWhenDiagMode()
    {
        var mutated = CSharpSyntaxTree.ParseText(GetExampleCode(false));
        var original = CSharpSyntaxTree.ParseText(GetExampleCode(true));
        var input = new MutationTestInput()
        {
            SourceProjectInfo = new SourceProjectInfo(
                     TestHelper.SetupProjectAnalyzerResult(
                    projectFilePath: "/c/project.csproj",
                    properties: new Dictionary<string, string>()
                    {
                        { "TargetDir", "" },
                        { "AssemblyName", "AssemblyName"},
                        { "TargetFileName", "TargetFileName.dll"},
                    },
                    // add a reference to system so the example code can compile
                    references: [typeof(object).Assembly.Location]
                ).Object, null)
        {
            ProjectContents = new CsharpFileLeaf{ SyntaxTree = original, MutatedSyntaxTree = mutated, SourceCode = GetExampleCode(true) }
        }};

        var rollbackProcessMock = new Mock<ICSharpRollbackProcess>(MockBehavior.Strict);
        rollbackProcessMock.Setup(x => x.RollbackMutationsInError(It.IsAny<ICompilationContent>(), It.IsAny<ImmutableArray<Diagnostic>>(), It.IsAny<ICSharpRollbackProcess.Mode>(), true))
                        .Returns((ICompilationContent _, ImmutableArray<Diagnostic> _, ICSharpRollbackProcess.Mode _, bool _) =>
                            []);

        var target = new CsharpCompilingProcess(input, rollbackProcessMock.Object, new StrykerOptions{DiagMode = true}, [mutated]);

        using (var ms = new MemoryStream())
        {
            Should.Throw<CompilationException>(() => target.Compile(ms, null)).Message.ShouldBe("Failed to restore the project to a buildable state.");
        }
    }

    private static string GetExampleCode(bool isBuildable) =>
        $$"""
          using System;

          namespace ExampleProject
          {
          public class Calculator
          {
              public string Subtract(string first, string second)
              {
                  return first {{(isBuildable ? "+" : "-")}} second;
              }
          }
          }
          """;

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
        var analyzerResult = TestHelper.SetupProjectAnalyzerResult(
            projectFilePath: "/c/project.csproj",
            properties: new Dictionary<string, string>()
            {
                { "TargetDir", "" },
                { "AssemblyName", "AssemblyName"},
                { "TargetFileName", "TargetFileName.dll"},
            },
            // add a reference to system so the example code can compile
            references: [typeof(object).Assembly.Location]
        ).Object;

        var input = new MutationTestInput()
        {
            SourceProjectInfo = new SourceProjectInfo(analyzerResult, null)
        };

        var rollbackProcessMock = new Mock<ICSharpRollbackProcess>(MockBehavior.Strict);

        var target = new CsharpCompilingProcess(input, rollbackProcessMock.Object, syntaxTrees: [syntaxTree]);

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
        var analyzerResult = TestHelper.SetupProjectAnalyzerResult(
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
        ).Object;

        var input = new MutationTestInput()
        {
            SourceProjectInfo = new SourceProjectInfo(analyzerResult, null)
        };

        var rollbackProcessMock = new Mock<ICSharpRollbackProcess>(MockBehavior.Strict);

        var target = new CsharpCompilingProcess(input, rollbackProcessMock.Object, syntaxTrees: [syntaxTree]);

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
            SourceProjectInfo = new SourceProjectInfo(
                TestHelper.SetupProjectAnalyzerResult(
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
                ).Object, new TestProjectsInfo(new MockFileSystem(), TestLoggerFactory.CreateLogger<TestProjectsInfo>()))};

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
        var analyzerResult = TestHelper.SetupProjectAnalyzerResult(properties: new Dictionary<string, string>()
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
        ).Object;
        var input = new MutationTestInput()
        {
            SourceProjectInfo = new SourceProjectInfo(analyzerResult, null)
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
        var analyzerResult = TestHelper.SetupProjectAnalyzerResult(
            projectFilePath: "/c/project.csproj",
            properties: new Dictionary<string, string>()
            {
                { "TargetDir", "" },
                { "TargetFileName", "TargetFileName.dll" },
                { "AssemblyName", "AssemblyName"},
            },
            // add a reference to system so the example code can compile
            references: [typeof(object).Assembly.Location]
        ).Object;

        var input = new MutationTestInput()
        {
            SourceProjectInfo = new SourceProjectInfo(analyzerResult, null)

        };

        var rollbackProcessMock = new Mock<ICSharpRollbackProcess>(MockBehavior.Strict);

        var target = new CsharpCompilingProcess(input, rollbackProcessMock.Object, syntaxTrees: [syntaxTree]);

        using var ms = new MemoryStream();
        var result = target.Compile(ms, null);
        result.Success.ShouldBe(true);

        Assembly.Load(ms.ToArray()).GetName().Version.ToString().ShouldBe("0.0.0.0");
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

    [TestMethod]
    public void ShouldCompileAndRollbackErrorWhenMutationErasesTerminalThrow()
    {
        // issue #3783: erasing the terminal throw of a non-void method causes CS0161 at the method
        // declaration. Only the mutations erasing the throw should be rolled back; the others must survive.
        var sourceFile = """
            using System;
            using System.Collections.Generic;

            namespace ExampleProject;

            public static class MethodResolver
            {
                private static readonly List<string> _log = new();

                public static int FailForMissingKey(string key)
                {
                    _log.Add(key);
                    throw new KeyNotFoundException($"No handler matched {key}.");
                }
            }
            """;
        var projectContentsMutants = MutateAndCompileSource(sourceFile).ToList();
        // those results can change if mutators are added.
        // the block mutation on the method body and the statement mutation on the throw are genuine compile errors
        projectContentsMutants.Count(t => t.ResultStatus == MutantStatus.CompileError).ShouldBe(2);
        // the statement mutation on _log.Add and the string mutation must survive
        projectContentsMutants.Count(t => t.ResultStatus == MutantStatus.Pending).ShouldBe(2);
        projectContentsMutants.Where(t => t.Mutation.Type == Mutator.String)
            .ShouldAllBe(t => t.ResultStatus == MutantStatus.Pending);
    }

    [TestMethod]
    public void ShouldCompileAndRollbackErrorWhenMutationErasesThrowInCatchClause()
    {
        // issue #3783: erasing the throw of a catch clause lets the catch complete normally, breaking
        // definite assignment (CS0165) at a use site after the try/catch. Only the mutations erasing
        // the assignment or the throw should be rolled back; the others must survive.
        var sourceFile = """
            using System;
            using System.Collections.Generic;

            namespace ExampleProject;

            public static class Transformer
            {
                public static int Transform(string[] upperInputs)
                {
                    string[] transformed;
                    try
                    {
                        transformed = (string[])upperInputs.Clone();
                    }
                    catch (Exception e)
                    {
                        throw new InvalidOperationException("Failed to transform", e);
                    }
                    if (transformed.Length != upperInputs.Length)
                    {
                        return -1;
                    }
                    return transformed.Length;
                }
            }
            """;
        var projectContentsMutants = MutateAndCompileSource(sourceFile).ToList();
        // those results can change if mutators are added.
        // block mutations on the method body, try block and catch block plus the statement mutation
        // on the throw are genuine compile errors
        projectContentsMutants.Count(t => t.ResultStatus == MutantStatus.CompileError).ShouldBe(4);
        // the string, equality, unary and 'return -1' block mutations must survive
        projectContentsMutants.Count(t => t.ResultStatus == MutantStatus.Pending).ShouldBe(4);
        projectContentsMutants.Where(t => t.Mutation.Type == Mutator.String || t.Mutation.Type == Mutator.Equality)
            .ShouldAllBe(t => t.ResultStatus == MutantStatus.Pending);
    }

    [TestMethod]
    public void ShouldCompileAndRollbackErrorWhenCollectionExpressionAssignmentsAreErased()
    {
        // issue #3783: block mutations erasing collection expression assignments across if/else chains
        // break definite assignment (CS0165) at the use site. They must be attributed and rolled back
        // individually, without safe mode discarding the collection expression mutations.
        var sourceFile = """
            using System;

            namespace ExampleProject;

            public enum AccessArea { ViewAlpha, EditAlpha, ViewBeta, EditBeta }

            public static class AccessExtensions
            {
                public static (AccessArea[][], AccessArea[][]) ResolveRights(int kind, bool canEdit)
                {
                    AccessArea[][] primaryRights;
                    AccessArea[][] secondaryRights;
                    if (kind == 1)
                    {
                        if (canEdit)
                        {
                            primaryRights = [[AccessArea.ViewAlpha, AccessArea.EditAlpha]];
                            secondaryRights = [[AccessArea.ViewBeta, AccessArea.EditBeta]];
                        }
                        else
                        {
                            primaryRights = [[AccessArea.ViewAlpha]];
                            secondaryRights = [[AccessArea.ViewBeta]];
                        }
                    }
                    else
                    {
                        primaryRights = [[AccessArea.ViewAlpha]];
                        secondaryRights = [[AccessArea.ViewBeta]];
                    }
                    return (primaryRights, secondaryRights);
                }
            }
            """;
        var projectContentsMutants = MutateAndCompileSource(sourceFile).ToList();
        // those results can change if mutators are added.
        // only the block mutations erasing branch assignments are genuine compile errors
        projectContentsMutants.Where(t => t.ResultStatus == MutantStatus.CompileError)
            .ShouldAllBe(t => t.Mutation.Type == Mutator.Block);
        projectContentsMutants.Count(t => t.ResultStatus == MutantStatus.CompileError).ShouldBe(4);
        // collection expression mutations compile and must survive
        projectContentsMutants.Where(t => t.Mutation.Type == Mutator.CollectionExpression)
            .ShouldAllBe(t => t.ResultStatus == MutantStatus.Pending);
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
            (
                TestHelper.SetupProjectAnalyzerResult(
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
                new TestProjectsInfo(fileSystem)
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
            ),

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
