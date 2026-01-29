using System.Linq;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Shouldly;
using Stryker.Core.Instrumentation;

namespace Stryker.Core.UnitTest.Instrumentation;

[TestClass]
public class RedirectMethodEngineShould
{
    [TestMethod]
    public void InjectSimpleMutatedMethod()
    {
        const string OriginalClass = """
                                     class Test
                                     {
                                         public void Basic(int x)
                                         {
                                             x++;
                                         }
                                     }
                                     """;
        const string MutatedMethod = @"public void Basic(int x) {x--;}";
        var parsedClass = SyntaxFactory.ParseSyntaxTree(OriginalClass).GetRoot().DescendantNodes().OfType<ClassDeclarationSyntax>().Single();
        var parsedMethod = (MethodDeclarationSyntax) SyntaxFactory.ParseMemberDeclaration(MutatedMethod);
        var originalMethod =  parsedClass.Members.OfType<MethodDeclarationSyntax>().Single();

        var engine = new RedirectMethodEngine();

        var injected = engine.InjectRedirect(parsedClass, SyntaxFactory.ParseExpression("ActiveMutation(2)"), originalMethod, parsedMethod);

        injected.Members.Count.ShouldBe(3);

        var expectedTree = SyntaxFactory.ParseSyntaxTree("""
                                                         class Test
                                                         {
                                                         public void Basic(int x)
                                                         {if(ActiveMutation(2)){Basic_1(x);}else{Basic_0(x);}}
                                                             public void Basic_0(int x)
                                                             {
                                                                 x++;
                                                             }
                                                         public void Basic_1(int x) {x--;}
                                                         }
                                                         """);
        var actualTree = SyntaxFactory.ParseSyntaxTree(injected.ToString());
        actualTree.ShouldBeSemantically(expectedTree);
    }

    [TestMethod]
    public void RollbackMutatedMethod()
    {
        const string OriginalClass = """
                                     class Test
                                     {
                                         public void Basic(int x)
                                         {
                                             x++;
                                         }
                                     }
                                     """;
        const string MutatedMethod = @"public void Basic(int x) {x--;}";
        var parsedClass = SyntaxFactory.ParseSyntaxTree(OriginalClass).GetRoot().DescendantNodes().OfType<ClassDeclarationSyntax>().Single();
        var parsedMethod = (MethodDeclarationSyntax) SyntaxFactory.ParseMemberDeclaration(MutatedMethod);
        var originalMethod =  parsedClass.Members.OfType<MethodDeclarationSyntax>().Single();

        var engine = new RedirectMethodEngine();
        var injected = engine.InjectRedirect(parsedClass, SyntaxFactory.ParseExpression("ActiveMutation(2)"), originalMethod, parsedMethod);

        // find the entry point
        var mutatedEntry =  injected.Members.OfType<MethodDeclarationSyntax>().First( p=> p.Identifier.ToString() == originalMethod.Identifier.ToString());
        var rolledBackClass = engine.RemoveInstrumentationFrom(injected ,mutatedEntry);

        rolledBackClass.ToString().ShouldBeSemantically(OriginalClass);
    }
}
