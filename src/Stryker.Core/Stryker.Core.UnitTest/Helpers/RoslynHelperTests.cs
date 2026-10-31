using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Shouldly;
using Stryker.Core.Helpers;

namespace Stryker.Core.UnitTest.Helpers;

[TestClass]
public class RoslynHelperTests : TestBase
{
    [TestMethod]
    [DataRow("s is { Length: > 0 } region")]
    [DataRow("s is var captured")]
    [DataRow("s is int n")]
    public void ContainsDeclarationsShouldDetectPatternDesignations(string expression)
    {
        SyntaxFactory.ParseExpression(expression).ContainsDeclarations().ShouldBeTrue();
    }

    [TestMethod]
    public void ContainsDeclarationsShouldDetectOutVarDeclaration()
    {
        SyntaxFactory.ParseExpression("int.TryParse(s, out var value)")
            .ContainsDeclarations().ShouldBeTrue();
    }

    [TestMethod]
    [DataRow("s.Length > 0")]
    [DataRow("s is [_, _]")]
    [DataRow("(a, b)")]
    // Recursive / var patterns without a SingleVariableDesignation introduce
    // no variable, so they must not trigger ContainsDeclarations.
    [DataRow("s is { Length: > 0 }")]
    [DataRow("s is var _")]
    public void ContainsDeclarationsShouldReturnFalseWithoutDeclarations(string expression)
    {
        SyntaxFactory.ParseExpression(expression).ContainsDeclarations().ShouldBeFalse();
    }
}
