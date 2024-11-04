using System.IO;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Serilog.Events;
using Stryker.Abstractions;
using Stryker.Abstractions.Mutators;
using Stryker.Abstractions.Options;
using Stryker.Core.InjectedHelpers;
using Stryker.Core.Mutants;

namespace Stryker.Core.UnitTest.Mutators;

[TestClass]
public class RegexStringSyntaxAttributeMutatorTests : TestBase
{
    private readonly CodeInjection            _injector = new();
    private readonly CsharpMutantOrchestrator _target;

    public RegexStringSyntaxAttributeMutatorTests() =>
        _target = new CsharpMutantOrchestrator(new MutantPlacer(_injector),
                                               options: new StrykerOptions
                                               {
                                                   MutationLevel = MutationLevel.Complete,
                                                   OptimizationMode = OptimizationModes.CoverageBasedTest,
                                                   ExcludedMutations = [Mutator.Block, Mutator.Statement],
                                                   LogOptions = new LogOptions { LogLevel = LogEventLevel.Verbose }
                                               });

    private async Task ShouldMutateCompiledSourceToExpectedAsync(string actual, string expected)
    {
        var cSharpParseOptions = CSharpParseOptions.Default.WithLanguageVersion(LanguageVersion.Preview);
        var syntaxTree = CSharpSyntaxTree.ParseText(actual, cSharpParseOptions);

        var basePath = Path.GetDirectoryName(typeof(object).Assembly.Location)!;
        var mscorlib = MetadataReference.CreateFromFile(typeof(object).Assembly.Location);
        var regex = MetadataReference.CreateFromFile(typeof(Regex).Assembly.Location);
        var attribute = MetadataReference.CreateFromFile(Path.Combine(basePath, "System.Runtime.dll"));

        Compilation compilation = CSharpCompilation.Create("MyCompilation",
                                                           [syntaxTree], [mscorlib, regex, attribute],
                                                           new CSharpCompilationOptions(OutputKind
                                                           .DynamicallyLinkedLibrary));

        var actualNode = _target.Mutate(syntaxTree, compilation.GetSemanticModel(syntaxTree));
        actual     = (await actualNode.GetRootAsync()).ToFullString();
        actual     = actual.Replace(_injector.HelperNamespace, "StrykerNamespace");
        actualNode = CSharpSyntaxTree.ParseText(actual, cSharpParseOptions);
        var expectedNode = CSharpSyntaxTree.ParseText(expected);
        actualNode.ShouldBeSemantically(expectedNode);
        actualNode.ShouldNotContainErrors();
    }


    [TestMethod]
    public Task ShouldMutateRegexStaticMethods()
    {
        var source = """
                     // Stryker disable block,statement
                     using System.Text.RegularExpressions;
                     namespace StrykerNet.UnitTest.Mutants.TestResources;
                     class RegexClass {
                         bool A(string input) {
                             return Regex.IsMatch(input, @"^abc");
                         }
                     }
                     """;

        var expected = """
                       // Stryker disable block,statement
                       using System.Text.RegularExpressions;
                       namespace StrykerNet.UnitTest.Mutants.TestResources;
                       class RegexClass {
                           bool A(string input) {
                               return Regex.IsMatch(input, (StrykerNamespace.MutantControl.IsActive(1)?"abc":@"^abc"));
                           }
                       }
                       """;
        return ShouldMutateCompiledSourceToExpectedAsync(source, expected);
    }

    [TestMethod]
    public Task ShouldMutateCustomRegexMethods()
    {
        var source = """
                     // Stryker disable block,statement
                     using System.Diagnostics.CodeAnalysis;
                     public class C {
                         public void M() {
                             Call("^abc");
                         }
                     
                         public static void Call([StringSyntax(StringSyntaxAttribute.Regex)]string s) {
                     
                         }
                     }
                     """;

        var expected =
            """
            // Stryker disable block,statement
            using System.Diagnostics.CodeAnalysis;
            public class C {
                public void M() {
                    Call((StrykerNamespace.MutantControl.IsActive(2)?"abc":"^abc"));
                }
                public static void Call([StringSyntax(StringSyntaxAttribute.Regex)]string s) {
            
                }
            }
            """;

        return ShouldMutateCompiledSourceToExpectedAsync(source, expected);
    }

    [TestMethod]
    public Task ShouldNotMutateCustomNonRegexMethods()
    {
        var source = """
                     // Stryker disable block,statement
                     using System.Diagnostics.CodeAnalysis;
                     public class C {
                         public void M() {
                             Call("^abc");
                         }
                     
                         public static void Call(string s) {
                     
                         }
                     }
                     """;

        var expected =
            """
            // Stryker disable block,statement
            using System.Diagnostics.CodeAnalysis;
            public class C {
                public void M() {
                    Call((StrykerNamespace.MutantControl.IsActive(2)?"":"^abc"));
                }
                public static void Call(string s) {}
            }
            """;

        return ShouldMutateCompiledSourceToExpectedAsync(source, expected);
    }

    [TestMethod]
    public Task ShouldNotMutateUnrelatedMethods()
    {
        var source = """
                     // Stryker disable block,statement
                     using System.Diagnostics.CodeAnalysis;
                     public class C {
                         public void M() {
                             Call(false);
                         }
                     
                         public static void Call(bool s) {
                     
                         }
                     }
                     """;

        var expected =
            """
            // Stryker disable block,statement
            using System.Diagnostics.CodeAnalysis;
            public class C {
                public void M() {
                    Call((StrykerNamespace.MutantControl.IsActive(2)?true:false));
                }
                public static void Call(bool s) {}
            }
            """;

        return ShouldMutateCompiledSourceToExpectedAsync(source, expected);
    }

    [TestMethod]
    public Task ShouldMutateRegexFields()
    {
        var source = """
                     using System.Diagnostics.CodeAnalysis;
                     public class C {
                         [StringSyntax(StringSyntaxAttribute.Regex)]
                         public string RegexPattern = "^abc";
                     }
                     """;

        var expected =
            """
            using System.Diagnostics.CodeAnalysis;
            public class C {
                [StringSyntax(StringSyntaxAttribute.Regex)]
                public string RegexPattern = (StrykerNamespace.MutantControl.IsActive(0)?"abc":"^abc");
            }
            """;

        return ShouldMutateCompiledSourceToExpectedAsync(source, expected);
    }

    [TestMethod]
    public Task ShouldMutateRegexProperties()
    {
        var source = """
                     using System.Diagnostics.CodeAnalysis;
                     public class C {
                         [StringSyntax(StringSyntaxAttribute.Regex)]
                         public string RegexPattern => "^abc";
                     }
                     """;

        var expected =
            """
            using System.Diagnostics.CodeAnalysis;
            public class C {
                [StringSyntax(StringSyntaxAttribute.Regex)]
                public string RegexPattern => (StrykerNamespace.MutantControl.IsActive(0)?"abc":"^abc");
            }
            """;

        return ShouldMutateCompiledSourceToExpectedAsync(source, expected);
    }

    [TestMethod]
    public Task ShouldNotApplyRegexMutationToNormalFields()
    {
        var source = """
                     using System.Diagnostics.CodeAnalysis;
                     public class C {
                         public string RegexPattern = "^abc";
                     }
                     """;

        var expected =
            """
            using System.Diagnostics.CodeAnalysis;
            public class C {
                public string RegexPattern = (StrykerNamespace.MutantControl.IsActive(0)?"":"^abc");
            }
            """;

        return ShouldMutateCompiledSourceToExpectedAsync(source, expected);
    }

    [TestMethod]
    public Task ShouldNotMutateOtherFields()
    {
        var source = """
                     using System.Diagnostics.CodeAnalysis;
                     public class C {
                         public bool RegexPattern = true;
                     }
                     """;

        var expected =
            """
            using System.Diagnostics.CodeAnalysis;
            public class C {
                public bool RegexPattern = (StrykerNamespace.MutantControl.IsActive(0)?false:true);
            }
            """;

        return ShouldMutateCompiledSourceToExpectedAsync(source, expected);
    }

    [TestMethod]
    public Task ShouldMutateImplicitRegexConstructor()
    {
        var source = """
                     using System.Diagnostics.CodeAnalysis;
                     public class C {
                         public static Regex RegexPattern = new("^abc");
                     }
                     """;

        var expected =
            """
            using System.Diagnostics.CodeAnalysis;
            public class C {
                public static Regex RegexPattern = StrykerNamespace.MutantContext.TrackValue(()=>new((StrykerNamespace.MutantControl.IsActive(0)?"abc":"^abc")));
            }
            """;

        return ShouldMutateCompiledSourceToExpectedAsync(source, expected);
    }

    [TestMethod]
    public Task ShouldMutateRegexFieldAssignment()
    {
        var source = """
                     // Stryker disable block,statement
                     using System.Diagnostics.CodeAnalysis;
                     public class C {
                         [StringSyntax(StringSyntaxAttribute.Regex)]
                         public string RegexPattern;
                         
                         public void M() {
                            RegexPattern = "^abc";
                         }
                     }
                     """;

        var expected =
            """
            // Stryker disable block,statement
            using System.Diagnostics.CodeAnalysis;
            public class C {
                [StringSyntax(StringSyntaxAttribute.Regex)]
                public string RegexPattern;
                
                public void M() {
                   RegexPattern = (StrykerNamespace.MutantControl.IsActive(1)?"abc":"^abc");
                }
            }
            """;

        return ShouldMutateCompiledSourceToExpectedAsync(source, expected);
    }

    [TestMethod]
    public Task ShouldMutateRegexPropertyAssignment()
    {
        var source = """
                     // Stryker disable block,statement
                     using System.Diagnostics.CodeAnalysis;
                     public class C {
                         [StringSyntax(StringSyntaxAttribute.Regex)]
                         public string RegexPattern { get; set; }
                         
                         public void M() {
                            RegexPattern = "^abc";
                         }
                     }
                     """;

        var expected =
            """
            // Stryker disable block,statement
            using System.Diagnostics.CodeAnalysis;
            public class C {
                [StringSyntax(StringSyntaxAttribute.Regex)]
                public string RegexPattern { get; set; }
                
                public void M() {
                   RegexPattern = (StrykerNamespace.MutantControl.IsActive(1)?"abc":"^abc");
                }
            }
            """;

        return ShouldMutateCompiledSourceToExpectedAsync(source, expected);
    }

    [TestMethod]
    public Task ShouldNotMutateNonRegexFieldAssignment()
    {
        var source = """
                     // Stryker disable block,statement
                     using System.Diagnostics.CodeAnalysis;
                     public class C {
                         public string RegexPattern;
                         
                         public void M() {
                            RegexPattern = "^abc";
                         }
                     }
                     """;

        var expected =
            """
            // Stryker disable block,statement
            using System.Diagnostics.CodeAnalysis;
            public class C {
                public string RegexPattern;
                
                public void M() {
                   RegexPattern = (StrykerNamespace.MutantControl.IsActive(1)?"":"^abc");
                }
            }
            """;

        return ShouldMutateCompiledSourceToExpectedAsync(source, expected);
    }

    [TestMethod]
    public Task ShouldNotMutateNonRegexPropertyAssignment()
    {
        var source = """
                     // Stryker disable block,statement
                     using System.Diagnostics.CodeAnalysis;
                     public class C {
                         public string RegexPattern { get; set; }
                         
                         public void M() {
                            RegexPattern = "^abc";
                         }
                     }
                     """;

        var expected =
            """
            // Stryker disable block,statement
            using System.Diagnostics.CodeAnalysis;
            public class C {
                public string RegexPattern { get; set; }
                
                public void M() {
                   RegexPattern = (StrykerNamespace.MutantControl.IsActive(1)?"":"^abc");
                }
            }
            """;

        return ShouldMutateCompiledSourceToExpectedAsync(source, expected);
    }

    [TestMethod]
    public Task ShouldMutateFullyQualifiedAttributeCustomRegexMethod()
    {
        var source = """
                     // Stryker disable block,statement
                     public class C {
                         public void M() {
                             Call("^abc");
                         }
                     
                         public static void Call([System.Diagnostics.CodeAnalysis.StringSyntax(System.Diagnostics.CodeAnalysis.StringSyntaxAttribute.Regex)]string s) {
                     
                         }
                     }
                     """;

        var expected =
            """
            // Stryker disable block,statement
            public class C {
                public void M() {
                    Call((StrykerNamespace.MutantControl.IsActive(2)?"abc":"^abc"));
                }
                public static void Call([System.Diagnostics.CodeAnalysis.StringSyntax(System.Diagnostics.CodeAnalysis.StringSyntaxAttribute.Regex)]string s) {
            
                }
            }
            """;

        return ShouldMutateCompiledSourceToExpectedAsync(source, expected);
    }

    [TestMethod]
    public Task ShouldMutateFullyQualifiedAttributeOnFieldAssignment()
    {
        var source = """
                     // Stryker disable block,statement
                     public class C {
                         [System.Diagnostics.CodeAnalysis.StringSyntax(System.Diagnostics.CodeAnalysis.StringSyntaxAttribute.Regex)]
                         public string RegexPattern;
                         
                         public void M() {
                            RegexPattern = "^abc";
                         }
                     }
                     """;

        var expected =
            """
            // Stryker disable block,statement
            public class C {
                [System.Diagnostics.CodeAnalysis.StringSyntax(System.Diagnostics.CodeAnalysis.StringSyntaxAttribute.Regex)]
                public string RegexPattern;
                
                public void M() {
                   RegexPattern = (StrykerNamespace.MutantControl.IsActive(1)?"abc":"^abc");
                }
            }
            """;

        return ShouldMutateCompiledSourceToExpectedAsync(source, expected);
    }

    [TestMethod]
    public Task ShouldMutateFullyQualifiedAttributeOnFieldInitialisation()
    {
        var source = """
                     public class C {
                         [System.Diagnostics.CodeAnalysis.StringSyntax(System.Diagnostics.CodeAnalysis.StringSyntaxAttribute.Regex)]
                         public string RegexPattern = "^abc";
                     }
                     """;

        var expected =
            """
            public class C {
                [System.Diagnostics.CodeAnalysis.StringSyntax(System.Diagnostics.CodeAnalysis.StringSyntaxAttribute.Regex)]
                public string RegexPattern = (StrykerNamespace.MutantControl.IsActive(0)?"abc":"^abc");
            }
            """;

        return ShouldMutateCompiledSourceToExpectedAsync(source, expected);
    }
}
