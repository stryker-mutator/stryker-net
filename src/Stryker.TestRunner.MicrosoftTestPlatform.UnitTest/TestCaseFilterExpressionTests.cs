using Shouldly;
using Stryker.Abstractions.Exceptions;
using Stryker.TestRunner.MicrosoftTestPlatform.Models;

namespace Stryker.TestRunner.MicrosoftTestPlatform.UnitTest;

[TestClass]
public class TestCaseFilterExpressionTests
{
    private static readonly TestNode Test = new("uid-1", "Adds two numbers", "action", TestNodeStates.Discovered,
        LocationType: "MyNamespace.CalculatorTests", LocationMethod: "Add");

    [TestMethod]
    [DataRow("FullyQualifiedName=MyNamespace.CalculatorTests.Add", true)]
    [DataRow("FullyQualifiedName=mynamespace.calculatortests.add", true)]
    [DataRow("FullyQualifiedName=MyNamespace.CalculatorTests", false)]
    [DataRow("FullyQualifiedName!=MyNamespace.CalculatorTests.Add", false)]
    [DataRow("FullyQualifiedName~CalculatorTests", true)]
    [DataRow("FullyQualifiedName!~CalculatorTests", false)]
    [DataRow("CalculatorTests", true)]
    [DataRow("OtherTests", false)]
    [DataRow("Name=Adds two numbers", true)]
    [DataRow("DisplayName~two", true)]
    [DataRow("ClassName=MyNamespace.CalculatorTests", true)]
    [DataRow("ClassName=Other&Name~Adds", false)]
    [DataRow("ClassName=Other|Name~Adds", true)]
    [DataRow("(ClassName=Other|Name~Adds)&FullyQualifiedName!~Slow", true)]
    [DataRow("ClassName=Other|(Name~Adds&FullyQualifiedName~Slow)", false)]
    [DataRow(" FullyQualifiedName ~ Calculator | Name = x ", true)]
    [DataRow("OtherTests|CalculatorTests.Add", true)]
    public void Matches_EvaluatesTheVsTestFilterSyntax(string filter, bool expected)
    {
        TestCaseFilterExpression.Parse(filter).Matches(Test).ShouldBe(expected);
    }

    [TestMethod]
    public void Matches_UnescapesSpecialCharacters()
    {
        var test = new TestNode("uid-1", "Add(1, 2)", "action", TestNodeStates.Discovered);

        TestCaseFilterExpression.Parse(@"Name=Add\(1, 2\)").Matches(test).ShouldBeTrue();
        TestCaseFilterExpression.Parse(@"Name~a\|b").Matches(test).ShouldBeFalse();
    }

    [TestMethod]
    [DataRow("")]
    [DataRow("Name=")]
    [DataRow("=Value")]
    [DataRow("(Name=x")]
    [DataRow("Name=x)")]
    [DataRow("Name=x|")]
    [DataRow("Name=x&&Name=y")]
    public void Parse_RejectsInvalidFilters(string filter)
    {
        Should.Throw<InputException>(() => TestCaseFilterExpression.Parse(filter));
    }

    [TestMethod]
    public void Parse_RejectsPropertiesItCannotEvaluate()
    {
        var exception = Should.Throw<InputException>(() => TestCaseFilterExpression.Parse("TestCategory=Unit"));

        exception.Message.ShouldContain("TestCategory");
        exception.Details.ShouldContain("FullyQualifiedName");
    }
}
