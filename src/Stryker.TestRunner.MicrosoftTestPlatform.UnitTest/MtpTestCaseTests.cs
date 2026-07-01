using System.Text.Json;
using Shouldly;
using Stryker.TestRunner.MicrosoftTestPlatform.Models;

namespace Stryker.TestRunner.MicrosoftTestPlatform.UnitTest;

[TestClass]
public class MtpTestCaseTests
{
    [TestMethod]
    public void MtpTestCase_WithLocationProperties_PopulatesCodeFilePath()
    {
        var testNode = new TestNode("uid-1", "TestMethod1", "action", "discovered",
            LocationFile: "/path/to/TestFile.cs",
            LocationLineStart: 42);

        var testCase = new MtpTestCase(testNode);

        testCase.CodeFilePath.ShouldBe("/path/to/TestFile.cs");
        testCase.LineNumber.ShouldBe(42);
    }

    [TestMethod]
    public void MtpTestCase_WithLocationTypeAndMethod_BuildsFullyQualifiedName()
    {
        var testNode = new TestNode("uid-1", "TestMethod1", "action", "discovered",
            LocationType: "MyNamespace.MyTestClass",
            LocationMethod: "TestMethod1");

        var testCase = new MtpTestCase(testNode);

        testCase.FullyQualifiedName.ShouldBe("MyNamespace.MyTestClass.TestMethod1");
    }

    [TestMethod]
    public void MtpTestCase_WithoutLocationProperties_UsesDefaults()
    {
        var testNode = new TestNode("uid-1", "TestMethod1", "action", "discovered");

        var testCase = new MtpTestCase(testNode);

        testCase.CodeFilePath.ShouldBe(string.Empty);
        testCase.LineNumber.ShouldBe(0);
        testCase.FullyQualifiedName.ShouldBe("uid-1");
    }

    [TestMethod]
    public void MtpTestCase_WithoutLocationType_FallsBackToUid()
    {
        var testNode = new TestNode("uid-1", "TestMethod1", "action", "discovered",
            LocationFile: "/path/to/TestFile.cs",
            LocationLineStart: 10);

        var testCase = new MtpTestCase(testNode);

        testCase.FullyQualifiedName.ShouldBe("uid-1");
    }

    [TestMethod]
    public void MtpTestCase_PreservesBasicProperties()
    {
        var testNode = new TestNode("uid-1", "TestMethod1", "action", "discovered");

        var testCase = new MtpTestCase(testNode);

        testCase.Id.ShouldBe("uid-1");
        testCase.Name.ShouldBe("TestMethod1");
        testCase.Uri.ShouldBe(new Uri("executor://MicrosoftTestPlatform"));
    }

    [TestMethod]
    public void TestNode_DeserializesLocationPropertiesFromJson()
    {
        var json = """
            {
                "uid": "uid-1",
                "display-name": "TestMethod1",
                "node-type": "action",
                "execution-state": "discovered",
                "location.file": "/path/to/TestFile.cs",
                "location.line-start": 42,
                "location.line-end": 50,
                "location.type": "MyNamespace.MyTestClass",
                "location.method": "TestMethod1"
            }
            """;

        var testNode = JsonSerializer.Deserialize<TestNode>(json);

        testNode.ShouldNotBeNull();
        testNode.Uid.ShouldBe("uid-1");
        testNode.DisplayName.ShouldBe("TestMethod1");
        testNode.LocationFile.ShouldBe("/path/to/TestFile.cs");
        testNode.LocationLineStart.ShouldBe(42);
        testNode.LocationLineEnd.ShouldBe(50);
        testNode.LocationType.ShouldBe("MyNamespace.MyTestClass");
        testNode.LocationMethod.ShouldBe("TestMethod1");
    }

    [TestMethod]
    public void TestNode_DeserializesWithoutLocationPropertiesFromJson()
    {
        var json = """
            {
                "uid": "uid-1",
                "display-name": "TestMethod1",
                "node-type": "action",
                "execution-state": "discovered"
            }
            """;

        var testNode = JsonSerializer.Deserialize<TestNode>(json);

        testNode.ShouldNotBeNull();
        testNode.Uid.ShouldBe("uid-1");
        testNode.LocationFile.ShouldBeNull();
        testNode.LocationLineStart.ShouldBeNull();
        testNode.LocationType.ShouldBeNull();
        testNode.LocationMethod.ShouldBeNull();
    }
}
