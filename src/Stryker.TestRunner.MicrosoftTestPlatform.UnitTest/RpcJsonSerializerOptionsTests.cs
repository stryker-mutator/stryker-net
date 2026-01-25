using System.Text.Json;
using Shouldly;
using Stryker.TestRunner.MicrosoftTestPlatform.RPC;

namespace Stryker.TestRunner.MicrosoftTestPlatform.UnitTest;

[TestClass]
public class RpcJsonSerializerOptionsTests
{
    [TestMethod]
    public void Default_ShouldReturnConfiguredOptions()
    {
        // Act
        var options = RpcJsonSerializerOptions.Default;

        // Assert
        options.ShouldNotBeNull();
        options.PropertyNamingPolicy.ShouldNotBeNull();
        options.PropertyNamingPolicy.ShouldBe(JsonNamingPolicy.CamelCase);
        options.PropertyNameCaseInsensitive.ShouldBeTrue();
    }

    [TestMethod]
    public void Default_ShouldReturnSameInstance()
    {
        // Act
        var options1 = RpcJsonSerializerOptions.Default;
        var options2 = RpcJsonSerializerOptions.Default;

        // Assert
        options1.ShouldBeSameAs(options2);
    }
}

