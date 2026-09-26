using System.Text.Json;
using Shouldly;

namespace Stryker.TestRunner.MicrosoftTestPlatform.UnitTest;

[TestClass]
public class MtpCompatibilityTests
{
    [DataRow("1.9.1", false)]
    [DataRow("2.3.3", false)]
    [DataRow("2.4.0", true)]
    [DataRow("2.4.0-preview.1", true)]
    [DataRow("3.0.0", true)]
    [TestMethod]
    public void SupportsBlockingDataConsumerFromDepsFile_ShouldCheckDynamicExtensionVersion(
        string platformVersion,
        bool expected)
    {
        var dependenciesFile = Path.Combine(Path.GetTempPath(), $"{Guid.NewGuid():N}.deps.json");

        try
        {
            File.WriteAllText(
                dependenciesFile,
                JsonSerializer.Serialize(new
                {
                    libraries = new Dictionary<string, object>
                    {
                        [$"Microsoft.Testing.Platform/{platformVersion}"] = new { type = "package" }
                    }
                }));

            MtpCompatibility.SupportsBlockingDataConsumerFromDepsFile(dependenciesFile).ShouldBe(expected);
        }
        finally
        {
            File.Delete(dependenciesFile);
        }
    }

    [TestMethod]
    public void SupportsBlockingDataConsumerFromDepsFile_ShouldReturnFalseForInvalidFile()
    {
        var dependenciesFile = Path.Combine(Path.GetTempPath(), $"{Guid.NewGuid():N}.deps.json");

        try
        {
            File.WriteAllText(dependenciesFile, "{ invalid json");

            MtpCompatibility.SupportsBlockingDataConsumerFromDepsFile(dependenciesFile).ShouldBeFalse();
        }
        finally
        {
            File.Delete(dependenciesFile);
        }
    }
}
