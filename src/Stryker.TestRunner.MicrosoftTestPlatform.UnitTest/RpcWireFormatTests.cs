using System.Text.Json;
using Shouldly;
using Stryker.TestRunner.MicrosoftTestPlatform.Models;
using Stryker.TestRunner.MicrosoftTestPlatform.RPC;

namespace Stryker.TestRunner.MicrosoftTestPlatform.UnitTest;

/// <summary>
/// Asserts the on-the-wire shape of the payloads Stryker sends to a Microsoft Testing Platform
/// server, against the names the platform actually reads them under.
/// </summary>
/// <remarks>
/// These assertions deliberately inspect the raw JSON instead of round-tripping through Stryker's
/// own records. A round-trip test uses the same record on both ends, so it passes for any property
/// name and cannot detect a mismatch with the platform - which is how the run request came to send
/// its test selection as <c>testCases</c> while the platform reads <c>tests</c>.
/// </remarks>
[TestClass]
public class RpcWireFormatTests
{
    private static readonly TestNode TestNodeWithoutLocation =
        new("some-uid", "SomeTest", "action", "discovered");

    // The JsonDocument owns the pooled memory backing its elements, so it is disposed here and the
    // element is cloned: a cloned JsonElement is detached from the document and stays valid after it.
    private static JsonElement Serialize<T>(T value)
    {
        using var document = JsonDocument.Parse(JsonSerializer.Serialize(value, RpcJsonSerializerOptions.Default));
        return document.RootElement.Clone();
    }

    [TestMethod]
    public void RunTestsRequest_SerializesSelectionAsTests()
    {
        var request = new RunTestsRequest(Guid.NewGuid(), [TestNodeWithoutLocation]);

        var json = Serialize(request);

        json.TryGetProperty("tests", out var tests).ShouldBeTrue(
            "Microsoft.Testing.Platform reads the run request test selection from 'tests' (JsonRpcStrings.Tests).");
        tests.GetArrayLength().ShouldBe(1);
    }

    [TestMethod]
    public void RunTestsRequest_DoesNotSerializeSelectionAsTestCases()
    {
        var request = new RunTestsRequest(Guid.NewGuid(), [TestNodeWithoutLocation]);

        var json = Serialize(request);

        json.TryGetProperty("testCases", out _).ShouldBeFalse(
            "'testCases' is not read by the platform, so a selection sent under that name is silently dropped and every test runs.");
    }

    [TestMethod]
    public void RunTestsRequest_SerializesRunId()
    {
        var runId = Guid.NewGuid();

        var json = Serialize(new RunTestsRequest(runId, [TestNodeWithoutLocation]));

        json.GetProperty("runId").GetGuid().ShouldBe(runId);
    }

    [TestMethod]
    public void RunTestsRequest_OmitsSelection_WhenRunningEveryTest()
    {
        var json = Serialize(new RunTestsRequest(Guid.NewGuid()));

        json.TryGetProperty("tests", out _).ShouldBeFalse(
            "An absent selection must be omitted so the platform runs every test.");
    }

    [TestMethod]
    [DataRow("location.file")]
    [DataRow("location.line-start")]
    [DataRow("location.line-end")]
    [DataRow("location.type")]
    [DataRow("location.method")]
    public void TestNode_OmitsLocationProperty_WhenAbsent(string propertyName)
    {
        var json = Serialize(TestNodeWithoutLocation);

        json.TryGetProperty(propertyName, out _).ShouldBeFalse(
            $"The platform probes '{propertyName}' with TryGetValue and then asserts it is not null, so an explicit null fails the request.");
    }

    [TestMethod]
    public void TestNode_SerializesRequiredProperties()
    {
        var json = Serialize(TestNodeWithoutLocation);

        json.GetProperty("uid").GetString().ShouldBe("some-uid");
        json.GetProperty("display-name").GetString().ShouldBe("SomeTest");
        json.GetProperty("node-type").GetString().ShouldBe("action");
        json.GetProperty("execution-state").GetString().ShouldBe("discovered");
    }

    [TestMethod]
    public void TestNode_SerializesLocationProperties_WhenPresent()
    {
        var node = new TestNode("some-uid", "SomeTest", "action", "discovered",
            LocationFile: "/src/SomeTests.cs",
            LocationLineStart: 10,
            LocationLineEnd: 20,
            LocationType: "SomeTests",
            LocationMethod: "SomeTest");

        var json = Serialize(node);

        json.GetProperty("location.file").GetString().ShouldBe("/src/SomeTests.cs");
        json.GetProperty("location.line-start").GetInt32().ShouldBe(10);
        json.GetProperty("location.line-end").GetInt32().ShouldBe(20);
        json.GetProperty("location.type").GetString().ShouldBe("SomeTests");
        json.GetProperty("location.method").GetString().ShouldBe("SomeTest");
    }

    [TestMethod]
    public void TestNode_DiscoveredWithoutLocation_RoundTripsWithoutNullLocation()
    {
        // A node the server reported without location info must survive being echoed back as part of
        // a run request selection, which is the direction that makes the null-vs-absent distinction matter.
        const string DiscoveredNode = """
            {"uid":"some-uid","display-name":"SomeTest","node-type":"action","execution-state":"discovered"}
            """;

        var node = JsonSerializer.Deserialize<TestNode>(DiscoveredNode, RpcJsonSerializerOptions.Default);

        node.ShouldNotBeNull();
        node.LocationFile.ShouldBeNull();

        var json = Serialize(new RunTestsRequest(Guid.NewGuid(), [node]));

        var serializedNode = json.GetProperty("tests")[0];
        serializedNode.TryGetProperty("location.file", out _).ShouldBeFalse();
        serializedNode.GetProperty("uid").GetString().ShouldBe("some-uid");
    }

    [TestMethod]
    public void DiscoveryRequest_SerializesRunId()
    {
        var runId = Guid.NewGuid();

        var json = Serialize(new DiscoveryRequest(runId));

        json.GetProperty("runId").GetGuid().ShouldBe(runId);
    }
}
