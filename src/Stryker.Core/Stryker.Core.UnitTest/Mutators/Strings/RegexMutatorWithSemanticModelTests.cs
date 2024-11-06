using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Shouldly;
using Stryker.Abstractions;
using Stryker.Abstractions.Mutants;
using Stryker.Abstractions.Mutators;
using Stryker.Abstractions.Options;
using Stryker.Core.Mutators;

namespace Stryker.Core.UnitTest.Mutators.Strings;

[TestClass]
public class RegexMutatorWithSemanticModelTests : TestBase
{
    private static readonly IStrykerOptions _options = new StrykerOptions { MutationLevel = MutationLevel.Advanced };

    private static (SemanticModel semanticModel, LiteralExpressionSyntax expression)
        CreateSemanticModelFromExpression(string input)
    {
        // Parse the code into a syntax tree
        var syntaxTree = CSharpSyntaxTree.ParseText(input);

        // Create a compilation that contains the syntax tree
        var basePath = Path.GetDirectoryName(typeof(object).Assembly.Location)!;
        var mscorlib = MetadataReference.CreateFromFile(typeof(object).Assembly.Location);
        var regex = MetadataReference.CreateFromFile(typeof(Regex).Assembly.Location);
        var attribute = MetadataReference.CreateFromFile(Path.Combine(basePath, "System.Runtime.dll"));
        var compilation = CSharpCompilation.Create("TestAssembly")
                                           .WithOptions(new CSharpCompilationOptions(OutputKind
                                                           .DynamicallyLinkedLibrary))
                                           .AddReferences(mscorlib)
                                           .AddReferences(regex)
                                           .AddReferences(attribute)
                                           .AddSyntaxTrees(syntaxTree);

        // Get the semantic model from the compilation
        var semanticModel = compilation.GetSemanticModel(syntaxTree);

        var expression = syntaxTree.GetRoot().DescendantNodes().OfType<LiteralExpressionSyntax>()
                                   .First(a => a.IsKind(SyntaxKind.StringLiteralExpression));

        return (semanticModel, expression);
    }

    private static void ValidateMutation(IEnumerable<Mutation> result)
    {
        var mutation = result.ShouldHaveSingleItem();

        mutation.Type.ShouldBe(Mutator.Regex);
        mutation.DisplayName.ShouldBe("Regex anchor removal mutation");

        var replacement = mutation.ReplacementNode.ShouldBeOfType<LiteralExpressionSyntax>();
        replacement.Token.ValueText.ShouldBe("abc");
    }

    [TestMethod]
    public void ShouldMutateRegexStaticMethods()
    {
        var source = """
                     using System.Text.RegularExpressions;
                     namespace StrykerNet.UnitTest.Mutants.TestResources;
                     class RegexClass {
                         bool A(string input) {
                             return Regex.IsMatch(input, @"^abc");
                         }
                     }
                     """;

        var (semanticModel, expressionSyntax) = CreateSemanticModelFromExpression(source);
        var target = new StringMutator();
        var result = target.Mutate(expressionSyntax, semanticModel, _options);

        ValidateMutation(result);
    }

    [TestMethod]
    public void ShouldMutateCustomRegexMethods()
    {
        var source = """
                     using System.Diagnostics.CodeAnalysis;
                     public class C {
                         public void M() {
                             Call("^abc");
                         }
                     
                         public static void Call([StringSyntax(StringSyntaxAttribute.Regex)]string s) {
                     
                         }
                     }
                     """;

        var (semanticModel, expressionSyntax) = CreateSemanticModelFromExpression(source);
        var target = new StringMutator();
        var result = target.Mutate(expressionSyntax, semanticModel, _options);

        ValidateMutation(result);
    }

    [TestMethod]
    public void ShouldMutateCustomRegexMethodsWithManualSyntax()
    {
        var source = """
                     using System.Diagnostics.CodeAnalysis;
                     public class C {
                         public void M() {
                             Call("^abc");
                         }
                     
                         public static void Call([StringSyntax("Regex")]string s) {
                     
                         }
                     }
                     """;

        var (semanticModel, expressionSyntax) = CreateSemanticModelFromExpression(source);
        var target = new StringMutator();
        var result = target.Mutate(expressionSyntax, semanticModel, _options);

        ValidateMutation(result);
    }

    [TestMethod]
    public void ShouldNotMutateNonRegexParametersWithSimilarSyntax()
    {
        var source = """
                     using System.ComponentModel;
                     public class C {
                         public void M() {
                             Call("^abc");
                         }
                     
                         public static void Call([DefaultValue("Regex")]string s) {
                     
                         }
                     }
                     """;

        var (semanticModel, expressionSyntax) = CreateSemanticModelFromExpression(source);
        var target = new StringMutator();
        var result = target.Mutate(expressionSyntax, semanticModel, _options);

        result.Where(a => a.Type == Mutator.Regex).ShouldBeEmpty();
    }

    [TestMethod]
    public void ShouldNotMutateBadNonRegexMethod()
    {
        var source = """
                     using System.Diagnostics.CodeAnalysis;
                     public class C {
                         public void M() {
                             Call("^abc");
                         }
                     
                         public static void Call([StringSyntaxAttribute]string s) {
                     
                         }
                     }
                     """;

        var (semanticModel, expressionSyntax) = CreateSemanticModelFromExpression(source);
        var target = new StringMutator();
        var result = target.Mutate(expressionSyntax, semanticModel, _options);

        result.Where(a => a.Type == Mutator.Regex).ShouldBeEmpty();
    }

    [TestMethod]
    public void ShouldNotMutateNonRegexParameters()
    {
        var source = """
                     using System.Diagnostics.CodeAnalysis;
                     public class C {
                         public void M() {
                             Call("^abc");
                         }
                     
                         public static void Call([StringSyntaxAttribute(StringSyntaxAttribute.Json)]string s) {
                     
                         }
                     }
                     """;

        var (semanticModel, expressionSyntax) = CreateSemanticModelFromExpression(source);
        var target = new StringMutator();
        var result = target.Mutate(expressionSyntax, semanticModel, _options);

        result.Where(a => a.Type == Mutator.Regex).ShouldBeEmpty();
    }

    [TestMethod]
    public void ShouldNotMutateCustomNonRegexMethods()
    {
        var source = """
                     public class C {
                         public void M() {
                             Call("^abc");
                         }
                     
                         public static void Call(string s) {
                     
                         }
                     }
                     """;

        var (semanticModel, expressionSyntax) = CreateSemanticModelFromExpression(source);
        var target = new StringMutator();
        var result = target.Mutate(expressionSyntax, semanticModel, _options);

        result.Where(a => a.Type == Mutator.Regex).ShouldBeEmpty();
    }

    [TestMethod]
    public void ShouldMutateRegexFields()
    {
        var source = """
                     using System.Diagnostics.CodeAnalysis;
                     public class C {
                         [StringSyntax(StringSyntaxAttribute.Regex)]
                         public string RegexPattern = "^abc";
                     }
                     """;

        var (semanticModel, expressionSyntax) = CreateSemanticModelFromExpression(source);
        var target = new StringMutator();
        var result = target.Mutate(expressionSyntax, semanticModel, _options);

        ValidateMutation(result);
    }

    [TestMethod]
    public void ShouldMutateRegexProperties()
    {
        var source = """
                     using System.Diagnostics.CodeAnalysis;
                     public class C {
                         [StringSyntax(StringSyntaxAttribute.Regex)]
                         public string RegexPattern => "^abc";
                     }
                     """;

        var (semanticModel, expressionSyntax) = CreateSemanticModelFromExpression(source);
        var target = new StringMutator();
        var result = target.Mutate(expressionSyntax, semanticModel, _options);

        ValidateMutation(result);
    }

    [TestMethod]
    public void ShouldMutateRegexFieldsWithMultipleAttributes()
    {
        var source = """
                     using System.ComponentModel;
                     using System.Diagnostics.CodeAnalysis;
                     public class C {
                         [StringSyntax(StringSyntaxAttribute.Regex), DefaultValue(false)]
                         public string RegexPattern = "^abc";
                     }
                     """;

        var (semanticModel, expressionSyntax) = CreateSemanticModelFromExpression(source);
        var target = new StringMutator();
        var result = target.Mutate(expressionSyntax, semanticModel, _options);

        ValidateMutation(result);
    }

    [TestMethod]
    public void ShouldMutateRegexPropertiesWithMultipleAttributes()
    {
        var source = """
                     using System.ComponentModel;
                     using System.Diagnostics.CodeAnalysis;
                     public class C {
                         [StringSyntax(StringSyntaxAttribute.Regex), DefaultValue(false)]
                         public string RegexPattern => "^abc";
                     }
                     """;

        var (semanticModel, expressionSyntax) = CreateSemanticModelFromExpression(source);
        var target = new StringMutator();
        var result = target.Mutate(expressionSyntax, semanticModel, _options);

        ValidateMutation(result);
    }

    [TestMethod]
    public void ShouldNotApplyRegexMutationToNormalFields()
    {
        var source = """
                     using System.Diagnostics.CodeAnalysis;
                     public class C {
                         public string RegexPattern = "^abc";
                     }
                     """;

        var (semanticModel, expressionSyntax) = CreateSemanticModelFromExpression(source);
        var target = new StringMutator();
        var result = target.Mutate(expressionSyntax, semanticModel, _options);

        result.Where(a => a.Type == Mutator.Regex).ShouldBeEmpty();
    }

    [TestMethod]
    public void ShouldMutateImplicitRegexConstructor()
    {
        var source = """
                     using System.Diagnostics.CodeAnalysis;
                     public class C {
                         public static Regex RegexPattern = new("^abc");
                     }
                     """;

        var (semanticModel, expressionSyntax) = CreateSemanticModelFromExpression(source);
        var target = new StringMutator();
        var result = target.Mutate(expressionSyntax, semanticModel, _options);

        ValidateMutation(result);
    }

    [TestMethod]
    public void ShouldMutateRegexFieldAssignment()
    {
        var source = """
                     using System.Diagnostics.CodeAnalysis;
                     public class C {
                         [StringSyntax(StringSyntaxAttribute.Regex)]
                         public string RegexPattern;
                         
                         public void M() {
                            RegexPattern = "^abc";
                         }
                     }
                     """;

        var (semanticModel, expressionSyntax) = CreateSemanticModelFromExpression(source);
        var target = new StringMutator();
        var result = target.Mutate(expressionSyntax, semanticModel, _options);

        ValidateMutation(result);
    }

    [TestMethod]
    public void ShouldMutateRegexPropertyAssignment()
    {
        var source = """
                     using System.Diagnostics.CodeAnalysis;
                     public class C {
                         [StringSyntax(StringSyntaxAttribute.Regex)]
                         public string RegexPattern { get; set; }
                         
                         public void M() {
                            RegexPattern = "^abc";
                         }
                     }
                     """;

        var (semanticModel, expressionSyntax) = CreateSemanticModelFromExpression(source);
        var target = new StringMutator();
        var result = target.Mutate(expressionSyntax, semanticModel, _options);

        ValidateMutation(result);
    }

    [TestMethod]
    public void ShouldNotMutateNonRegexFieldAssignment()
    {
        var source = """
                     using System.Diagnostics.CodeAnalysis;
                     public class C {
                         public string RegexPattern;
                         
                         public void M() {
                            RegexPattern = "^abc";
                         }
                     }
                     """;

        var (semanticModel, expressionSyntax) = CreateSemanticModelFromExpression(source);
        var target = new StringMutator();
        var result = target.Mutate(expressionSyntax, semanticModel, _options);

        result.Where(a => a.Type == Mutator.Regex).ShouldBeEmpty();
    }

    [TestMethod]
    public void ShouldNotMutateNonRegexPropertyAssignment()
    {
        var source = """
                     using System.Diagnostics.CodeAnalysis;
                     public class C {
                         public string RegexPattern { get; set; }
                         
                         public void M() {
                            RegexPattern = "^abc";
                         }
                     }
                     """;

        var (semanticModel, expressionSyntax) = CreateSemanticModelFromExpression(source);
        var target = new StringMutator();
        var result = target.Mutate(expressionSyntax, semanticModel, _options);

        result.Where(a => a.Type == Mutator.Regex).ShouldBeEmpty();
    }

    [TestMethod]
    public void ShouldMutateFullyQualifiedAttributeCustomRegexMethod()
    {
        var source = """
                     public class C {
                         public void M() {
                             Call("^abc");
                         }
                     
                         public static void Call([System.Diagnostics.CodeAnalysis.StringSyntax(System.Diagnostics.CodeAnalysis.StringSyntaxAttribute.Regex)]string s) {
                     
                         }
                     }
                     """;

        var (semanticModel, expressionSyntax) = CreateSemanticModelFromExpression(source);
        var target = new StringMutator();
        var result = target.Mutate(expressionSyntax, semanticModel, _options);

        ValidateMutation(result);
    }

    [TestMethod]
    public void ShouldMutateFullyQualifiedAttributeOnFieldAssignment()
    {
        var source = """
                     public class C {
                         [System.Diagnostics.CodeAnalysis.StringSyntax(System.Diagnostics.CodeAnalysis.StringSyntaxAttribute.Regex)]
                         public string RegexPattern;
                         
                         public void M() {
                            RegexPattern = "^abc";
                         }
                     }
                     """;

        var (semanticModel, expressionSyntax) = CreateSemanticModelFromExpression(source);
        var target = new StringMutator();
        var result = target.Mutate(expressionSyntax, semanticModel, _options);

        ValidateMutation(result);
    }

    [TestMethod]
    public void ShouldMutateFullyQualifiedAttributeOnFieldInitialisation()
    {
        var source = """
                     public class C {
                         [System.Diagnostics.CodeAnalysis.StringSyntax(System.Diagnostics.CodeAnalysis.StringSyntaxAttribute.Regex)]
                         public string RegexPattern = "^abc";
                     }
                     """;

        var (semanticModel, expressionSyntax) = CreateSemanticModelFromExpression(source);
        var target = new StringMutator();
        var result = target.Mutate(expressionSyntax, semanticModel, _options);

        ValidateMutation(result);
    }
}
