using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.IO.MemoryMappedFiles;
using System.Linq;
using System.Reflection;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Shouldly;
using Stryker.Abstractions;
using Stryker.Configuration.Options;
using Stryker.Core.Compiling;
using Stryker.Core.InjectedHelpers;
using Stryker.Core.Mutants;
using Stryker.Core.UnitTest.Compiling;

namespace Stryker.Core.UnitTest;

[TestClass]
public class MutantAndCompileTests
{
    private readonly CodeInjection _injector = new();

    [TestMethod]
    public void RollbackShouldPreserveDirectives()
    {
        const string source = @"using System.Diagnostics.CodeAnalysis;
namespace Lib;

[Experimental(""MYEXP001"")]
public static class Experimental
{
    public static string? Find(string key) => key == ""alpha"" ? key : null;
}

public static class Consumer
{
    public static int Get(string key)
    {
#pragma warning disable MYEXP001
        return key switch {
            { } k when Experimental.Find(k) is { } found => found.Length,
            _ => 0
        };
#pragma warning restore MYEXP001
    }
}";

        var syntaxTree = CSharpSyntaxTree.ParseText(source);
        Type[] typeToLoad = [typeof(object), typeof(List<>), typeof(Enumerable), typeof(Nullable<>), typeof(MemoryMappedFile),
            typeof(ExperimentalAttribute), typeof(MemoryMappedViewAccessor)];
        var syntaxTrees = new List<SyntaxTree> { syntaxTree };
        syntaxTrees.AddRange(_injector.GetHelpersSyntaxTreesToInject(new CSharpParseOptions(LanguageVersion.CSharp14)));
        var mutator = new CsharpMutantOrchestrator(new MutantPlacer(_injector), options: new StrykerOptions
        {
            MutationLevel = MutationLevel.Complete,
        });

        List<MetadataReference> metadataReferences = [.. typeToLoad.Select(t => t.Assembly.Location).Distinct().
            Select(l => MetadataReference.CreateFromFile(l))];
        Assembly.GetAssembly(typeof(MemoryMappedViewAccessor)).GetReferencedAssemblies().Select(Assembly.Load).Select(a => a.Location).
            Distinct().Select(l => MetadataReference.CreateFromFile(l)).ToList().ForEach(metadataReferences.Add);
        var compilation = CSharpCompilation.Create("TestCompilation",
            syntaxTrees: syntaxTrees,
            options: new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary)
                .WithNullableContextOptions(NullableContextOptions.Enable),
            references: metadataReferences
        );
        var actualNode = mutator.Mutate(syntaxTree, compilation.GetSemanticModel(syntaxTree));

        compilation = compilation.ReplaceSyntaxTree(syntaxTree, actualNode);
        var target = new CSharpRollbackProcess();
        using var ms = new MemoryStream();
        var compileResult = compilation.Emit(ms);

        compileResult.Success.ShouldBeFalse();
        var compilerWrapper = new CompilerWrapper(compilation);

        target.RollbackMutationsInError(compilerWrapper, compileResult.Diagnostics, ICSharpRollbackProcess.Mode.Normal, false);

        var rollbackedResult = compilerWrapper.Emit(ms);

        rollbackedResult.Diagnostics.Where(d => d.Severity == DiagnosticSeverity.Error).ShouldBeEmpty();
    }
}
