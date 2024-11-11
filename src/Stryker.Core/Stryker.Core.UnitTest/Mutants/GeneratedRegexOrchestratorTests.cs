using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis;
using System.IO;
using System.Text.RegularExpressions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using VerifyMSTest;
using VerifyTests;
using System.Runtime.CompilerServices;
using Serilog.Events;
using Stryker.Abstractions;
using Stryker.Abstractions.Mutators;
using Stryker.Core.InjectedHelpers;
using Stryker.Core.Mutants;
using Stryker.Abstractions.Options;

namespace Stryker.Core.UnitTest.Mutants;

[TestClass]
[UsesVerify]
public partial class GeneratedRegexOrchestratorTests : TestBase
{
    private readonly CsharpMutantOrchestrator _target;
    private readonly CodeInjection            _injector = new();
    private readonly CSharpParseOptions       _previewOptions = CSharpParseOptions.Default.WithLanguageVersion(LanguageVersion.Preview);

    public GeneratedRegexOrchestratorTests() => _target = new CsharpMutantOrchestrator(new MutantPlacer(_injector),
                                                 options: new StrykerOptions
                                                 {
                                                     MutationLevel    = MutationLevel.Complete,
                                                     OptimizationMode = OptimizationModes.CoverageBasedTest,
                                                     LogOptions = new LogOptions
                                                     {
                                                         LogLevel = LogEventLevel.Verbose
                                                     }
                                                 });

    private async Task ShouldMutateSourceToExpectedAsync(string methodName, string actual)
    {
        var actualNode = _target.Mutate(CSharpSyntaxTree.ParseText(actual, _previewOptions), null);
        actual     = (await actualNode.GetRootAsync()).ToFullString();
        actual     = actual.Replace(_injector.HelperNamespace, "StrykerNamespace");
        actualNode = CSharpSyntaxTree.ParseText(actual, _previewOptions);
        actualNode.ShouldNotContainErrors();
        await Verifier.Verify(actual, "cs").UseMethodName(methodName).IgnoreParameters();
    }

    private async Task ShouldNotMutateSourceAsync(string actual)
    {
        var input = CSharpSyntaxTree.ParseText(actual, _previewOptions);
        var actualNode = _target.Mutate(input, null);
        actual     = (await actualNode.GetRootAsync()).ToFullString();
        actual     = actual.Replace(_injector.HelperNamespace, "StrykerNamespace");
        actualNode = CSharpSyntaxTree.ParseText(actual);
        actualNode.ShouldBeSemantically(input);
        actualNode.ShouldNotContainErrors();
    }

    private async Task ShouldMutateCompiledSourceToExpectedAsync(string methodName, string actual)
    {
        var cSharpParseOptions = _previewOptions.WithPreprocessorSymbols("GENERATED_REGEX");
        var syntaxTree = CSharpSyntaxTree.ParseText(actual, cSharpParseOptions);
        var basePath = Path.GetDirectoryName(typeof(object).Assembly.Location)!;

        var mscorlib = MetadataReference.CreateFromFile(typeof(object).Assembly.Location);
        var regex = MetadataReference.CreateFromFile(typeof(Regex).Assembly.Location);
        var attribute = MetadataReference.CreateFromFile(Path.Combine(basePath, "System.Runtime.dll"));
        
        Compilation compilation = CSharpCompilation.Create("MyCompilation",
                                                           [syntaxTree], [mscorlib, attribute, regex],
                                                           new CSharpCompilationOptions(OutputKind
                                                           .DynamicallyLinkedLibrary));

        var regexGeneratorDll = Path.Combine(basePath, "..", "..", "..", "packs", "Microsoft.NETCore.App.Ref",
                                             Path.GetFileName(basePath),
                                             "analyzers", "dotnet", "cs", "System.Text.RegularExpressions.Generator.dll");

        var sourceGenerator =
            Activator.CreateInstanceFrom(regexGeneratorDll, "System.Text.RegularExpressions.Generator.RegexGenerator")
                  ?.Unwrap() switch
            {
                IIncrementalGenerator ig => ig.AsSourceGenerator(),
                ISourceGenerator sg => sg,
                _ => null
            };

        if (sourceGenerator is not null)
        {
            GeneratorDriver driver = CSharpGeneratorDriver.Create([sourceGenerator], [], cSharpParseOptions);
            driver.RunGeneratorsAndUpdateCompilation(compilation, out compilation, out _);

            if (compilation.GetDiagnostics().Where(static a => a.Severity == DiagnosticSeverity.Error).Any())
            {
                Assert.Inconclusive("Initial compilation unsuccessful");
            }
        }

        var actualNode = _target.Mutate(syntaxTree, compilation.GetSemanticModel(syntaxTree));
        actual     = (await actualNode.GetRootAsync()).ToFullString();
        actual     = actual.Replace(_injector.HelperNamespace, "StrykerNamespace");
        actualNode = CSharpSyntaxTree.ParseText(actual, cSharpParseOptions);
        actualNode.ShouldNotContainErrors();
        await Verifier.Verify(actual, "cs").UseMethodName(methodName).IgnoreParameters();
    }

    public static IEnumerable<object[]> Tests =>
    [
        ["SimpleSingleRegexInDedicatedClass", """
                                              using System.Text.RegularExpressions;
                                              namespace StrykerNet.UnitTest.Mutants.TestResources;
                                              public partial class R {
                                                  [GeneratedRegex(@"^abc$", RegexOptions.IgnoreCase, "en-US")]
                                                  private static partial Regex AbcGeneratedRegex();
                                              }
                                              """],
        ["SimpleSingleRegexInDedicatedStruct", """
                                               using System.Text.RegularExpressions;
                                               namespace StrykerNet.UnitTest.Mutants.TestResources;
                                               public partial struct R {
                                                   [GeneratedRegex(@"^abc$", RegexOptions.IgnoreCase, "en-US")]
                                                   private static partial Regex AbcGeneratedRegex();
                                               }
                                               """],
        ["SimpleSingleRegexInDedicatedRecord", """
                                               using System.Text.RegularExpressions;
                                               namespace StrykerNet.UnitTest.Mutants.TestResources;
                                               public partial record R {
                                                   [GeneratedRegex(@"^abc$", RegexOptions.IgnoreCase, "en-US")]
                                                   private static partial Regex AbcGeneratedRegex();
                                               }
                                               """],
        ["SimpleSingleRegexInDedicatedRecordStruct", """
                                                     using System.Text.RegularExpressions;
                                                     namespace StrykerNet.UnitTest.Mutants.TestResources;
                                                     public partial record struct R {
                                                         [GeneratedRegex(@"^abc$", RegexOptions.IgnoreCase, "en-US")]
                                                         private static partial Regex AbcGeneratedRegex();
                                                     }
                                                     """],
        ["SingleRegexInDedicatedClassNamedParameter", """
                                              using System.Text.RegularExpressions;
                                              namespace StrykerNet.UnitTest.Mutants.TestResources;
                                              public partial class R {
                                                  [GeneratedRegex(RegexOptions.IgnoreCase, pattern: @"^abc$", "en-US")]
                                                  private static partial Regex AbcGeneratedRegex();
                                              }
                                              """],
        ["MultipleRegexInDedicatedClass", """
                                          using System.Text.RegularExpressions;
                                          namespace StrykerNet.UnitTest.Mutants.TestResources;
                                          public partial class R {
                                              [GeneratedRegex(@"^abc$", RegexOptions.IgnoreCase, "en-US")]
                                              private static partial Regex AbcGeneratedRegex();
                                              
                                              [GeneratedRegex(@"^[abc]\d?", RegexOptions.IgnoreCase, "en-US")]
                                              private static partial Regex AbcdGeneratedRegex();
                                          }
                                          """],
        ["MultipleRegexInSharedClass", """
                                       using System.Text.RegularExpressions;
                                       namespace StrykerNet.UnitTest.Mutants.TestResources;
                                       public partial class R {
                                           [GeneratedRegex(@"^abc$", RegexOptions.IgnoreCase, "en-US")]
                                           private static partial Regex AbcGeneratedRegex();
                                           
                                           static string Value => "test ".Substring(2);
                                           
                                           [GeneratedRegex(@"^[abc]\d?")]
                                           private static partial Regex AbcdGeneratedRegex();
                                       }
                                       """],
        ["MultipleRegexInMultipleClasses", """
                                           using System.Text.RegularExpressions;
                                           namespace StrykerNet.UnitTest.Mutants.TestResources;
                                           public partial class R1 {
                                               [GeneratedRegex(@"^abc$", RegexOptions.IgnoreCase, "en-US")]
                                               private static partial Regex Abc1GeneratedRegex();
                                               
                                               [GeneratedRegex(@"^[abc]\d?", RegexOptions.IgnoreCase, "en-US")]
                                               private static partial Regex Abcd1GeneratedRegex();
                                           }
                                           public partial class R2 {
                                               [GeneratedRegex(@"^abc\b$", RegexOptions.IgnoreCase, "en-US")]
                                               private static partial Regex Abc2GeneratedRegex();
                                               
                                               [GeneratedRegex(@"^\d[abc]\d?", RegexOptions.IgnoreCase, "en-US")]
                                               private static partial Regex Abcd2GeneratedRegex();
                                           }
                                           """],
        ["ComplexSingleRegexInDedicatedClass", """
                                               using System.Text.RegularExpressions;
                                               namespace StrykerNet.UnitTest.Mutants.TestResources;
                                               public partial class R {
                                                   [GeneratedRegex(@"^[abc]\d?", RegexOptions.IgnoreCase, "en-US")]
                                                   private static partial Regex AbcGeneratedRegex();
                                               }
                                               """],
        ["ComplexEmailRegexInDedicatedClass",
            """"
            using System.Text.RegularExpressions;
            namespace StrykerNet.UnitTest.Mutants.TestResources;
            public partial class R {
                [GeneratedRegex("""(?:[a-z0-9!#$%&'*+/=?^_`{|}~-]+(?:\.[a-z0-9!#$%&'*+/=?^_`{|}~-]+)*|"(?:[\x01-\x08\x0b\x0c\x0e-\x1f\x21\x23-\x5b\x5d-\x7f]|\\[\x01-\x09\x0b\x0c\x0e-\x7f])*")@(?:(?:[a-z0-9](?:[a-z0-9-]*[a-z0-9])?\.)+[a-z0-9](?:[a-z0-9-]*[a-z0-9])?|\[(?:(?:25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)\.){3}(?:25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?|[a-z0-9-]*[a-z0-9]:(?:[\x01-\x08\x0b\x0c\x0e-\x1f\x21-\x5a\x53-\x7f]|\\[\x01-\x09\x0b\x0c\x0e-\x7f])+)\])""", RegexOptions.IgnoreCase)]
                private static partial Regex EmailGeneratedRegex();
            }
            """"],
        ["SimpleSingleRegexInNestedClass", """
                                           using System.Text.RegularExpressions;
                                           namespace StrykerNet.UnitTest.Mutants.TestResources;
                                           public partial class R {
                                             public partial class R2 {
                                               [GeneratedRegex(@"^abc$", RegexOptions.IgnoreCase, "en-US")]
                                               private static partial Regex AbcGeneratedRegex();
                                             }
                                           }
                                           """],
        ["RealTest",
            """
            using System.Text.RegularExpressions;
            namespace StrykerNet.UnitTest.Mutants.TestResources;
            public partial class R {
                [GeneratedRegex(@"(?<doy>\w{3}), 0?(?<day>\d{1,2})-(?<month>\w{3})-(?<year>\d{2}) 0?(?<hour>\d{1,2}):0?(?<minute>\d{1,2}):0?(?<second>\d{1,2}) (?<tz>\w{3})")]
                private static partial Regex SpecificDateTimeRegex();
            }
            """],
        // https://developer.mozilla.org/en-US/docs/Web/JavaScript/Guide/Regular_expressions/Cheatsheet
        ["EveryRegexNodeType",
            """
            using System.Text.RegularExpressions;
            namespace StrykerNet.UnitTest.Mutants.TestResources;
            public partial class R {
                [GeneratedRegex(@"[xyz][a-c][^xyz][^a-c].\d\D\w\W\s\S\t\r\n\v\f[\b]\0\cM\cJ\x4A\u1234\p{Ll}\P{IsLatinExtended-A}+(?:\p{Sc}|\p{P})\*\\\.^$\b\Bx(?=y)x(?!y)(?<=y)(x)x(?<Name>x)(?:x)\1\k<Name>x*x+x?x{2}x{2,}x{4,5}x*?x+?x??x{2}?x{2,}?x{4,5}?")]
                private static partial Regex EveryGeneratedRegex();
            }
            """],
        ["GeneratedRegexProperty", """
                                   using System.Text.RegularExpressions;
                                   namespace StrykerNet.UnitTest.Mutants.TestResources;
                                   public partial class R {
                                       [GeneratedRegex(@"\b\w{5}\b")]
                                       private static partial Regex AbcGeneratedRegex { get; }
                                   }
                                   """]
    ];

    public static IEnumerable<object[]> RequiresCompilationTests =>
    [
        ["RegexAsConstField", """
                              using System.Text.RegularExpressions;
                              namespace StrykerNet.UnitTest.Mutants.TestResources;
                              public partial class R {
                                  public const string Regex = @"^abc$";
                                  
                                  [GeneratedRegex(Regex)]
                                  private static partial Regex AbcGeneratedRegex();
                              }
                              """],
        ["RegexAsInterpolatedConstField", """
                                          using System.Text.RegularExpressions;
                                          namespace StrykerNet.UnitTest.Mutants.TestResources;
                                          public partial class R {
                                              [GeneratedRegex(C.Regex)]
                                              private static partial Regex AbcdeGeneratedRegex();
                                          }
                                          public static class C {
                                              public const string Inner = @"abcde";
                                              public const string Regex = $@"^{Inner}$";
                                          }
                                          """],
#if NET9_0_OR_GREATER
        // https://learn.microsoft.com/en-us/dotnet/core/whats-new/dotnet-9/libraries#regular-expressions
        ["GeneratedRegexProperty", """
                                   using System.Text.RegularExpressions;
                                   namespace StrykerNet.UnitTest.Mutants.TestResources;
                                   public partial class R {
                                       [GeneratedRegex(@"\b\w{5}\b")]
                                       private static partial Regex AbcGeneratedRegex { get; }
                                   }
                                   """],
#endif
        ["RegexInIfDefBlock", """
                              using System.Text.RegularExpressions;
                              namespace StrykerNet.UnitTest.Mutants.TestResources;
                              public partial class R {
                              #if GENERATED_REGEX
                                  [GeneratedRegex(@"^abc$")]
                                  private static partial Regex AbcGeneratedRegex();
                              #else
                                  private static readonly Regex _abcGeneratedRegex = new Regex(@"^abc$");
                                  private static Regex AbcGeneratedRegex() => _abcGeneratedRegex;
                              #endif
                              }
                              """],
        ["RegexInIfDefBlockInverted", """
                              using System.Text.RegularExpressions;
                              namespace StrykerNet.UnitTest.Mutants.TestResources;
                              public partial class R {
                              #if !GENERATED_REGEX
                                  private static readonly Regex _abcGeneratedRegex = new Regex(@"^abc$");
                                  private static Regex AbcGeneratedRegex() => _abcGeneratedRegex;
                              #else
                                  [GeneratedRegex(@"^abc$")]
                                  private static partial Regex AbcGeneratedRegex();
                              #endif
                              }
                              """],
        ["OtherMutations", """
             using System.Text.RegularExpressions;
             namespace StrykerNet.UnitTest.Mutants.TestResources;
             public partial class R {
                 [GeneratedRegex(@"^abc$", RegexOptions.IgnoreCase, "en-US")]
                 private static partial Regex AbcGeneratedRegex();
                 
                 public void M() {
                    var toMutate = "a string is here";
                    var mutation2 = toMutate.PadLeft(5);
                 }
             }
             """]
    ];

    public static IEnumerable<object[]> NoMutationTests =>
    [
        [
            "NoMutationRegex", """
                               using System.Text.RegularExpressions;
                               namespace StrykerNet.UnitTest.Mutants.TestResources;
                               public partial class R {
                                   [GeneratedRegex(@"a", RegexOptions.IgnoreCase, "en-US")]
                                   private static partial Regex AbcGeneratedRegex();
                               }
                               """
        ],
        [
            "NoMutationInterface", """
                                   using System.Text.RegularExpressions;
                                   namespace StrykerNet.UnitTest.Mutants.TestResources;
                                   public partial interface R {
                                       [GeneratedRegex(@"a", RegexOptions.IgnoreCase, "en-US")]
                                       private static partial Regex AbcGeneratedRegex();
                                   }
                                   """
        ],
        [
            "EmptyPartialClass", """
                                 namespace StrykerNet.UnitTest.Mutants.TestResources;
                                 public partial class R {
                                 }
                                 """
        ]
    ];

    [System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE0060:Remove unused parameter", Justification = "Required for DynamicDataDisplayName")]
    public static string GetCustomDynamicDataDisplayName(MethodInfo methodInfo, object[] data) => $"{data[0]}";

    [TestMethod]
    [DynamicData(nameof(Tests), DynamicDataDisplayName = nameof(GetCustomDynamicDataDisplayName))]
    public Task ShouldMutateGeneratedRegex(string testName, string code) => ShouldMutateSourceToExpectedAsync(testName, code);

    [TestMethod]
    [DynamicData(nameof(RequiresCompilationTests), DynamicDataDisplayName = nameof(GetCustomDynamicDataDisplayName))]
    public Task ShouldMutateGeneratedRegexWithCompilation(string testName, string code) => ShouldMutateCompiledSourceToExpectedAsync(testName, code);

    [TestMethod]
    [DynamicData(nameof(NoMutationTests), DynamicDataDisplayName = nameof(GetCustomDynamicDataDisplayName))]
    public Task ShouldNotMutatedMutateGeneratedRegex(string testName, string code) => ShouldNotMutateSourceAsync(code);
}

public static class StaticSettingsUsage
{
    [ModuleInitializer]
    public static void Initialize()
    {
        VerifierSettings.RegisterStringComparer("cs", CompareSyntaxTrees);
        Verifier.UseSourceFileRelativeDirectory("Verified");
    }

    private static Task<CompareResult> CompareSyntaxTrees(string                              received, string verified,
                                                          IReadOnlyDictionary<string, object> context) =>
        Task.FromResult(CSharpSyntaxTree.ParseText(SourceText.From(received))
                                        .IsEquivalentTo(CSharpSyntaxTree.ParseText(SourceText.From(verified)))
                            ? CompareResult.Equal
                            : CompareResult.NotEqual());
}
