using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Shouldly;
using Stryker.Abstractions.ProjectComponents;
using Stryker.Abstractions.Reporting;
using Stryker.Core.Baseline.Utils;
using Stryker.Core.DiffProviders;
using Stryker.Core.Reporters.Json;
using Stryker.Core.Reporters.Json.TestFiles;
using System.Linq;

namespace Stryker.Core.UnitTest.Baseline.Utils;

[TestClass]
public class ContentTestMatcherTests : TestBase
{
    private const string Source = "class C { [Fact] void MyTest() { Assert.True(true); } }";

    private static ITestCase CreateCurrentTest(string source)
    {
        var tree = CSharpSyntaxTree.ParseText(source);
        var method = tree.GetRoot().DescendantNodes().OfType<MethodDeclarationSyntax>().First();

        var testCase = new Mock<ITestCase>();
        testCase.Setup(t => t.Node).Returns(method);
        return testCase.Object;
    }

    [TestMethod]
    public void IsTestUnchanged_ReturnsTrue_WhenSourceIsIdentical()
    {
        // Arrange
        var currentTest = CreateCurrentTest(Source);
        var baselineTest = new JsonTest("id")
        {
            Location = new Location(currentTest.Node.GetLocation().GetMappedLineSpan())
        };
        var diff = new DiffResult([new DiffChange { Operation = DiffOperation.Equal, Text = Source }], Source, Source);
        var target = new ContentTestMatcher();

        // Act
        var result = target.IsTestUnchanged(baselineTest, currentTest, diff);

        // Assert
        result.ShouldBeTrue();
    }

    [TestMethod]
    public void IsTestUnchanged_ReturnsFalse_WhenTestBodyChanged()
    {
        // Arrange
        var currentTest = CreateCurrentTest(Source);
        var baselineTest = new JsonTest("id")
        {
            Location = new Location(currentTest.Node.GetLocation().GetMappedLineSpan())
        };
        var newSource = "class C { [Fact] void MyTest() { Assert.False(false); } }";
        var diff = new DiffMatchPatchProvider().GetContentDiff(Source, newSource);
        var target = new ContentTestMatcher();

        // Act
        var result = target.IsTestUnchanged(baselineTest, currentTest, diff);

        // Assert
        result.ShouldBeFalse();
    }

    [TestMethod]
    public void IsTestUnchanged_ReturnsFalse_WhenLocationHasNoEnd()
    {
        // Arrange
        var currentTest = CreateCurrentTest(Source);
        var baselineTest = new JsonTest("id")
        {
            Location = new Location { Start = new Position { Line = 1, Column = 1 } }
        };
        var diff = new DiffResult([new DiffChange { Operation = DiffOperation.Equal, Text = Source }], Source, Source);
        var target = new ContentTestMatcher();

        // Act
        var result = target.IsTestUnchanged(baselineTest, currentTest, diff);

        // Assert
        result.ShouldBeFalse();
    }
}
