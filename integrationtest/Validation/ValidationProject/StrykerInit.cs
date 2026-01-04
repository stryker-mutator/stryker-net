using System.IO;
using System.Linq;
using System.Text.Json;
using Shouldly;
using Stryker.CLI;
using Xunit;

namespace Validation;

public class StrykerInit
{

    [Fact]
    [Trait("Category", "InitCommand")]
    [Trait("Runtime", "netcore")]
    public void InitCommandDefaults()
    {
        var directory = new DirectoryInfo("../../../../../TargetProjects/InitCommand");
        var jsonFile = directory.GetFiles("*.json", SearchOption.AllDirectories).SingleOrDefault();
        jsonFile.ShouldNotBeNull("Json file missing");

        var configOutput = File.ReadAllText(jsonFile.FullName);
        var parsedConfigOuptut = JsonSerializer.Deserialize<FileBasedInputOuter>(configOutput);
        parsedConfigOuptut.ShouldNotBeNull();

        parsedConfigOuptut.Input.Project.ShouldBe("TestProject.csproj");
    }
}
