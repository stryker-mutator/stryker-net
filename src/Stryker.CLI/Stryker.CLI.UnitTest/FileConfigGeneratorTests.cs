using Microsoft.VisualStudio.TestTools.UnitTesting;
using Shouldly;
using Stryker.Configuration.Options;

namespace Stryker.CLI.UnitTest;

[TestClass]
public class FileConfigGeneratorTests
{
    [TestMethod]
    public void GeneratedConfig_ShouldNotSetLanguageVersion()
    {
        var config = FileConfigGenerator.GenerateConfigAsync(new StrykerInputs());

        config.ShouldNotContain("\"language-version\"");
    }
}
