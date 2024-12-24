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
using System.Text;

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
        var element = replacement.Elements.ShouldHaveSingleItem();
        element.ShouldBeOfType<ExpressionElementSyntax>().Expression.ShouldBeOfType<LiteralExpressionSyntax>().Token.IsKind(SyntaxKind.DefaultKeyword).ShouldBeTrue();
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
    [CollectionExpressionTest("Should mutate collection expression with spread elements",
                                 """
                                 using System;

                                 namespace ExampleProject;

                                 class ClassName {
                                     public void M() {
                                         int[] abc = [ 1, 5, 7 ];
                                         int[] bcd = [ 1, ..abc, 3 ];
                                     }
                                 }
                                 """, 2)]
    [CollectionExpressionTest("Should mutate collection expression with explicit cast",
                                 """
                                 using System;

                                 namespace ExampleProject;

                                 class ClassName {
                                     public void M() {
                                         int[] abc = [ 1, 5, 7 ];
                                         var bcd = (int[])[ 1, ..abc, 3 ];
                                     }
                                 }
                                 """, 2)]
    [CollectionExpressionTest("Should mutate nested collection expression",
                                 """
                                 using System;

                                 namespace ExampleProject;

                                 class ClassName {
                                     public void M() {
                                         int[][] abc = [ [ 1, 5 ], [ 7 ] ];
                                     }
                                 }
                                 """, 3)]
    [CollectionExpressionTest("Should mutate collection expression with inner array initialization",
                                 """
                                 using System;

                                 namespace ExampleProject;

                                 class ClassName {
                                     public void M() {
                                         int[][] abc = [ [ 1, 5 ], new [] { 7 } ];
                                     }
                                 }
                                 """, 2)]
    [CollectionExpressionTest("Should mutate collection expression with inner explicit spread collection expression",
                                 """
                                 using System;

                                 namespace ExampleProject;

                                 class ClassName {
                                     public void M() {
                                         int[] abc = [ ..(Span<int>)[ 1, 5 ], ..(Span<int>)[ 7 ] ];
                                     }
                                 }
                                 """, 3)]
    [CollectionExpressionTest("Should mutate empty collection expression",
                                 """
                                 using System;

                                 namespace ExampleProject;

                                 class ClassName {
                                     public void M() {
                                         int[] abc = [];
                                     }
                                 }
                                 """, 1)]
    [CollectionExpressionTest("Should mutated collection expression used as a generic parameter",
                                 """
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
    [CollectionExpressionTest("Empty collection expression mutation should not be ambiguous",
                                 """
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
    [CollectionExpressionTest("Filled collection expression mutation should not be ambiguous",
                                 """
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
    [CollectionExpressionTest("Empty collection expression mutation should not be ambiguous again",
                                 """
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
    [CollectionExpressionTest("Should mutate collection expression with varying sources",
                                 """
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
    [CollectionExpressionTest("Should mutate collection expression when nullable",
                                 """
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
    [CollectionExpressionTest("Should mutate heavily nested collection expression",
                                 """
                                 using System;
                                 using System.Collections;
                                 using System.Collections.Generic;
                                 using System.Collections.Immutable;
                                 using System.Linq;
                                 using System.IO;

                                 namespace ExampleProject;

                                 class ClassName {
                                     public static int[][][][][] Deep => [[[[[]]]]];
                                 }
                                 """, 5)]
    [CollectionExpressionTest("Should mutate empty collection expressions",
                                 """
                                 using System;
                                 using System.Collections;
                                 using System.Collections.Generic;
                                 using System.Collections.Immutable;
                                 using System.Linq;
                                 using System.IO;

                                 namespace ExampleProject;

                                 class ClassName {
                                     // https://learn.microsoft.com/en-us/dotnet/csharp/language-reference/proposals/csharp-12.0/collection-expressions#empty-collection-literal
                                     public static void Method() {
                                        int[] x = [];
                                        IEnumerable<int> y = [];
                                        List<int> z = [];
                                     }
                                 }
                                 """, 3)]
    [CollectionExpressionTest("Should support ref safety",
                                 """
                                 using System;
                                 using System.Collections;
                                 using System.Collections.Generic;
                                 using System.Collections.Immutable;
                                 using System.Linq;
                                 using System.IO;

                                 namespace ExampleProject;

                                 class ClassName {
                                     // https://learn.microsoft.com/en-us/dotnet/csharp/language-reference/proposals/csharp-12.0/collection-expressions#ref-safety
                                     static ReadOnlySpan<int> AsSpanConstants()
                                     {
                                         return [1, 2, 3]; // ok: span refers to assembly data section
                                     }
                                     
                                     static ReadOnlySpan<T> AsSpan3<T>(T x, T y, T z)
                                     {
                                         return (T[])[x, y, z]; // ok: span refers to T[] on heap
                                     }
                                 }
                                 """, 2)]
    [CollectionExpressionTest("Should support type inference",
                                 """
                                 using System;
                                 using System.Collections;
                                 using System.Collections.Generic;
                                 using System.Collections.Immutable;
                                 using System.Linq;
                                 using System.IO;

                                 namespace ExampleProject;

                                 class ClassName {
                                     // https://learn.microsoft.com/en-us/dotnet/csharp/language-reference/proposals/csharp-12.0/collection-expressions#type-inference
                                     public static void Method() {
                                        var a = AsArray([1, 2, 3]);          // AsArray<int>(int[])
                                        var b = AsListOfArray([[4, 5], []]); // AsListOfArray<int>(List<int[]>)
                                 
                                        static T[] AsArray<T>(T[] arg) => arg;
                                        static List<T[]> AsListOfArray<T>(List<T[]> arg) => arg;
                                     }
                                 }
                                 """, 4)]
    [CollectionExpressionTest("Should support overload resolution",
                                 """
                                 using System;
                                 using System.Collections;
                                 using System.Collections.Generic;
                                 using System.Collections.Immutable;
                                 using System.Linq;
                                 using System.IO;

                                 namespace ExampleProject;

                                 class ClassName {
                                     static void Generic<T>(Span<T> value) { }
                                     static void Generic<T>(T[] value) { }
                                     
                                     static void SpanDerived(Span<string> value) { }
                                     static void SpanDerived(object[] value) { }
                                     
                                     static void ArrayDerived(Span<object> value) { }
                                     static void ArrayDerived(string[] value) { }
                                     
                                     // https://learn.microsoft.com/en-us/dotnet/csharp/language-reference/proposals/csharp-12.0/collection-expressions#overload-resolution
                                     public static void Method() {
                                         // Array initializers
                                         Generic(new[] { "" });      // string[]
                                         ArrayDerived(new[] { "" }); // string[]
                                         
                                         // Collection expressions
                                         Generic([""]);              // Span<string>
                                         SpanDerived([""]);          // Span<string>
                                     }
                                 }
                                 """, 2)]
    [CollectionExpressionTest("Should not result in syntax ambiguities",
                                 """
                                 using System;
                                 using System.Collections;
                                 using System.Collections.Generic;
                                 using System.Collections.Immutable;
                                 using System.Linq;
                                 using System.IO;

                                 namespace ExampleProject;

                                 class ClassName {
                                     static void Generic<T>(Span<T> value) { }
                                     static void Generic<T>(T[] value) { }
                                     
                                     static void SpanDerived(Span<string> value) { }
                                     static void SpanDerived(object[] value) { }
                                     
                                     static void ArrayDerived(Span<object> value) { }
                                     static void ArrayDerived(string[] value) { }
                                     
                                     // https://learn.microsoft.com/en-us/dotnet/csharp/language-reference/proposals/csharp-12.0/collection-expressions#syntax-ambiguities
                                     public static void Method() {
                                         Range range1 = default;
                                         Range range2 = default;
                                         int e = 3;
                                         Range[] ranges = [range1, (..e), range2];
                                     }
                                 }
                                 """, 1)]
    [CollectionExpressionTest("Should support int to long implicit conversion",
                                 """
                                 using System;
                                 using System.Collections;
                                 using System.Collections.Generic;
                                 using System.Collections.Immutable;
                                 using System.Linq;
                                 using System.IO;

                                 namespace ExampleProject;

                                 class ClassName {
                                     static void Generic<T>(Span<T> value) { }
                                     static void Generic<T>(T[] value) { }
                                     
                                     static void SpanDerived(Span<string> value) { }
                                     static void SpanDerived(object[] value) { }
                                     
                                     static void ArrayDerived(Span<object> value) { }
                                     static void ArrayDerived(string[] value) { }
                                     
                                     // https://learn.microsoft.com/en-us/dotnet/csharp/language-reference/proposals/csharp-12.0/collection-expressions#resolved-questions
                                     public static void Method() {
                                         void DoWork(IEnumerable<long> values) { }
                                         // Needs to produce `longs` not `ints` for this to work.
                                         DoWork([1, 2, 3]);
                                     }
                                 }
                                 """, 1)]
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

        var injector = new CodeInjection();
        var orchestrator = new CsharpMutantOrchestrator(new MutantPlacer(injector),
                                                        options: new StrykerOptions
                                                        {
                                                            MutationLevel    = MutationLevel.Complete,
                                                            OptimizationMode = OptimizationModes.CoverageBasedTest,
                                                            ExcludedMutations = Enum.GetValues<Mutator>()
                                                               .Except([Mutator.CollectionExpression])
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
            var result =
                target.Compile([
                                   syntaxTree,
                                   ..injector.MutantHelpers.Select(a => CSharpSyntaxTree.ParseText(a.Value, path: a.Key,
                                                                    encoding: Encoding.UTF32))
                               ],
                               Stream.Null, Stream.Null);
            result.Success.ShouldBe(true);
            result.RollbackedIds.ShouldBeEmpty();
        }
        catch (CompilationException)
        {
            Assert.Fail($"Compilation failed with code: {syntaxTree}");
        }
    }
}

[AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
file class CollectionExpressionTestAttribute : DataRowAttribute
{
    /// <inheritdoc />
    public CollectionExpressionTestAttribute(string testName, string inputCode, int mutationCount) :
        base(inputCode, mutationCount) =>
        DisplayName = testName;
}
