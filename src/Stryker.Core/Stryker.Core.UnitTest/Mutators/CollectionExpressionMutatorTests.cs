using System.Collections.Generic;
using System.Collections.Immutable;
using System.IO;
using System;
using System.Linq;
using System.Reflection;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Shouldly;
using Stryker.Abstractions.Exceptions;
using Stryker.Abstractions;
using Stryker.Abstractions.Mutators;
using Stryker.Abstractions.Options;
using Stryker.Core.Compiling;
using Stryker.Core.InjectedHelpers;
using Stryker.Core.Mutants;
using Stryker.Core.MutationTest;
using Stryker.Core.Mutators;
using Stryker.Core.ProjectComponents.SourceProjects;

namespace Stryker.Core.UnitTest.Mutators;

[TestClass]
public class CollectionExpressionMutatorTests : TestBase
{
    [TestMethod]
    public void ShouldBeMutationLevelAdvanced()
    {
        var target = new CollectionExpressionMutator();
        target.MutationLevel.ShouldBe(MutationLevel.Advanced);
    }

    [TestMethod]
    [DataRow("[]")]
    [DataRow("[ ]")]
    [DataRow("[           ]")]
    [DataRow("[ /* Comment */ ]")]
    public void ShouldAddValueToEmptyCollectionExpression(string expression)
    {
        var expressionSyntax = SyntaxFactory.ParseExpression(expression) as CollectionExpressionSyntax;
        var target = new CollectionExpressionMutator();
        var result = target.ApplyMutations(expressionSyntax, null);
        
        var mutation = result.ShouldHaveSingleItem();
        mutation.DisplayName.ShouldBe("Collection expression mutation");
        var replacement = mutation.ReplacementNode.ShouldBeOfType<CollectionExpressionSyntax>();
        replacement.Elements.ShouldNotBeEmpty();
    }

    [TestMethod]
    [DataRow("[1, 2, 3]")]
    [DataRow("[-1, 3]")]
    [DataRow("[1, .. abc, 3]")]
    [DataRow("[..abc]")]
    public void ShouldRemoveValuesFromCollectionExpression(string expression)
    {
        var expressionSyntax = SyntaxFactory.ParseExpression(expression) as CollectionExpressionSyntax;
        var target = new CollectionExpressionMutator();
        var result = target.ApplyMutations(expressionSyntax, null);
        var mutation = result.ShouldHaveSingleItem();
        mutation.DisplayName.ShouldBe("Collection expression mutation");
        var replacement = mutation.ReplacementNode.ShouldBeOfType<CollectionExpressionSyntax>();
        replacement.Elements.ShouldBeEmpty();
    }

    [TestMethod]
    [DataRow("""
             using System;

             namespace ExampleProject;

             class ClassName {
                 public void M() {
                     int[] abc = [ 1, 5, 7 ];
                     int[] bcd = [ 1, ..abc, 3 ];
                 }
             }
             """, 2)]
    [DataRow("""
             using System;

             namespace ExampleProject;

             class ClassName {
                 public void M() {
                     int[] abc = [ 1, 5, 7 ];
                     var bcd = (int[])[ 1, ..abc, 3 ];
                 }
             }
             """, 2)]
    [DataRow("""
             using System;

             namespace ExampleProject;

             class ClassName {
                 public void M() {
                     int[][] abc = [ [ 1, 5 ], [ 7 ] ];
                 }
             }
             """, 3)]
    [DataRow("""
             using System;

             namespace ExampleProject;

             class ClassName {
                 public void M() {
                     int[][] abc = [ [ 1, 5 ], new [] { 7 } ];
                 }
             }
             """, 2)]
    [DataRow("""
             using System;

             namespace ExampleProject;

             class ClassName {
                 public void M() {
                     int[] abc = [ ..(Span<int>)[ 1, 5 ], ..(Span<int>)[ 7 ] ];
                 }
             }
             """, 3)]
    [DataRow("""
             using System;

             namespace ExampleProject;

             class ClassName {
                 public void M() {
                     int[] abc = [];
                 }
             }
             """, 1)]
    [DataRow("""
             using System;
             using System.Collections.Generic;

             namespace ExampleProject;

             class ClassName {
                 public IEnumerable<int> M() => Iter([ 1 ]);
             
                 public IEnumerable<T> Iter<T>(IList<T> list) {
                     foreach (var l in list) {
                         yield return l;
                     }
                 }
             }
             """, 1)]
    [DataRow("""
             using System;
             using System.Collections.Generic;

             namespace ExampleProject;

             class ClassName {
                 public IEnumerable<int> M() => Iter([ 1 ]);
             
                 public IEnumerable<T> Iter<T>(IList<T> list) {
                     foreach (var l in list) {
                         yield return l;
                     }
                 }
             
                 public IEnumerable<T> Iter<T>(IReadOnlyCollection<T> list) {
                     foreach (var l in list) {
                         yield return l;
                     }
                 }
             
                 public IEnumerable<T> Iter<T>(T[] list) {
                     foreach (var l in list) {
                         yield return l;
                     }
                 }
             
                 public IEnumerable<T> Iter<T>(ReadOnlyMemory<T> list) {
                     for (var i = 0; i < list.Length; i++) {
                         yield return list.Span[i];
                     }
                 }
             }
             """, 1)]
    [DataRow("""
             using System;
             using System.Collections.Generic;
             using System.Collections.Immutable;

             namespace ExampleProject;

             class ClassName {
                 public void M() {
                     ImmutableArray<int>? a = [];
                     var b = a ?? [];
                 }
             }
             """, 2)]
    [DataRow("""
             using System;
             using System.Collections.Generic;
             using System.Collections.Immutable;

             namespace ExampleProject;

             class ClassName {
                 public void M() {
                     List<int>? a = [];
                     var b = a ?? [];
                 }
             }
             """, 2)]
    [DataRow("""
             using System;
             using System.Collections.Generic;
             using System.Collections.Immutable;

             namespace ExampleProject;

             class ClassName {
                 public string[] M() {
                     string[] vowels = [ "a", "e", "i", "o", "u" ];
                     string[] consonants = [
                         "b", "c", "d", "f", "g", "h", "j", "k", "l", "m",
                         "n", "p", "q", "r", "s", "t", "v", "w", "x", "z"
                     ];
                     string[] alphabet = [..vowels, ..consonants, "y" ];
                     return alphabet;
                 }
             }
             """, 3)]
    [DataRow("""
             using System;
             using System.Collections;
             using System.Collections.Generic;
             using System.Collections.Immutable;
             using System.Linq;
             using System.IO;

             namespace ExampleProject;

             class ClassName {
                 public void GetLocalDateTime(Stream s) {
                     AddAll(Deserialize(s, Enumerable.Empty<string>()) ?? []);
                 }
                 public IEnumerable<int>? Deserialize(Stream s, IEnumerable<string> s2) {
                     return [];
                 }
                 public void AddAll(IEnumerable<int> list) { }
             }
             """, 2)]
    public void MutatedCollectionExpressionsShouldCompile(string inputText, int expectedMutants)
    {
        var syntaxTree = CSharpSyntaxTree.ParseText(inputText);

        var compilation = CSharpCompilation.Create("TestAssembly")
                                        .WithOptions(new
                                                         CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary,
                                                          nullableContextOptions: NullableContextOptions.Enable))
                                        .AddReferences(MetadataReference.CreateFromFile(typeof(object).Assembly
                                                       .Location))
                                        .AddSyntaxTrees(syntaxTree);
        var semanticModel = compilation.GetSemanticModel(syntaxTree);

        var orchestrator = new CsharpMutantOrchestrator(new MutantPlacer(new CodeInjection()),
                                                        options: new StrykerOptions
                                                        {
                                                            MutationLevel    = MutationLevel.Complete,
                                                            OptimizationMode = OptimizationModes.CoverageBasedTest
                                                        });
        syntaxTree = orchestrator.Mutate(syntaxTree, semanticModel);
        orchestrator.Mutants.Count(a => a.Mutation.Type == Mutator.CollectionExpression).ShouldBe(expectedMutants);

        List<string> references =
        [
            typeof(object).Assembly.Location,
            typeof(List<string>).Assembly.Location,
            typeof(Enumerable).Assembly.Location,
            typeof(ImmutableArray<>).Assembly.Location,
            typeof(ValueType).Assembly.Location,
            ..Assembly.GetEntryAssembly()?.GetReferencedAssemblies().Select(a => Assembly.Load(a).Location) ?? []
        ];

        var input = new MutationTestInput
        {
            SourceProjectInfo = new SourceProjectInfo
            {
                AnalyzerResult = TestHelper.SetupProjectAnalyzerResult(projectFilePath: "/c/project.csproj",
                                                                       properties: new Dictionary<string, string>
                                                                       {
                                                                           { "TargetDir", "" },
                                                                           { "AssemblyName", "AssemblyName" },
                                                                           {
                                                                               "TargetFileName",
                                                                               "TargetFileName.dll"
                                                                           }
                                                                       },
                                                                       references: references.ToArray()
                                                                      )
                                        .Object
            }
        };

        var target = new CsharpCompilingProcess(input);

        try
        {
            var result = target.Compile([syntaxTree], Stream.Null, Stream.Null);
            result.Success.ShouldBe(true);
        }
        catch (CompilationException)
        {
            Assert.Fail($"Compilation failed with code: {syntaxTree}");
        }
    }
}
