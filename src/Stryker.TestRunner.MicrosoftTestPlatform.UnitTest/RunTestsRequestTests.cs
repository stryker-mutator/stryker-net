using System.Text.Json;
using Shouldly;
using Stryker.TestRunner.MicrosoftTestPlatform.Models;
using Stryker.TestRunner.MicrosoftTestPlatform.RPC;

namespace Stryker.TestRunner.MicrosoftTestPlatform.UnitTest;

[TestClass]
public class RunTestsRequestTests
{
    [TestMethod]
    public void RunTestsRequest_ShouldSerializeFilterUnderTheServersPropertyNames()
    {
        // Microsoft.Testing.Platform's server binds the run filter from the
        // "tests" property and each entry's "uid" and "display-name". A filter
        // serialized under any other property name is silently ignored and the
        // server runs the complete assembly, so mutation runs execute tests
        // that have no mutant assignment.

        // Arrange
        var runId = Guid.Parse("11111111-2222-3333-4444-555555555555");
        var request = new RunTestsRequest(
            runId,
            [new RunRequestTestNode("case-uid", "Example.SampleTests.Case")]);

        // Act
        var serialized = JsonSerializer.Serialize(request, RpcJsonSerializerOptions.Default);

        // Assert
        using var document = JsonDocument.Parse(serialized);
        var root = document.RootElement;
        root.GetProperty("runId").GetString().ShouldBe(runId.ToString());
        var tests = root.GetProperty("tests");
        tests.GetArrayLength().ShouldBe(1);
        var test = tests[0];
        test.GetProperty("uid").GetString().ShouldBe("case-uid");
        test.GetProperty("display-name").GetString().ShouldBe("Example.SampleTests.Case");
    }
}
