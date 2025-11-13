using Shouldly;

namespace Stryker.Solutions.Test;

[TestClass]
public sealed class SolutionFileShould
{
    [TestMethod]
    public void LoadStrykerSlnFile()
    {
        // Arrange
        // Act
        var solution = SolutionFile.LoadSolution(Path.Combine("..","..","..","..","Stryker.sln"));
        // Assert
        Assert.IsNotNull(solution);
    }

    [TestMethod]
    public void IdentifyStrykerBuildTypes()
    {
        // Arrange
        // Act
        var solution = SolutionFile.LoadSolution(Path.Combine("..","..","..","..","Stryker.sln"));
        // Assert
        solution.GetBuildTypes().ShouldBe(["Debug", "Release"]);
    }

    [TestMethod]
    public void IdentifyStrykerPlatform()
    {
        // Arrange
        // Act
        var solution = SolutionFile.LoadSolution(Path.Combine("..","..","..","..","Stryker.sln"));

        // Assert
        solution.ConfigurationExists("Debug", "Any CPU").ShouldBeTrue();

    }

    [TestMethod]
    public void ProvideProjectListForGivenConfiguration()
    {
        // Arrange
        // Act
        var solution = SolutionFile.LoadSolution(Path.Combine("..","..","..","..","Stryker.sln"));
        // Assert
        solution.ConfigurationExists("Debug", "Any CPU").ShouldBeTrue();

        // it should report all projects that are built in Stryker's Debug configuration
        var expectedProjects = new List<string>
        {
            Path.Combine("Stryker.CLI", "Stryker.CLI", "Stryker.CLI.csproj"),
            Path.Combine("Stryker.CLI", "Stryker.CLI.UnitTest", "Stryker.CLI.UnitTest.csproj"),
            Path.Combine("Stryker.Core", "Stryker.Core", "Stryker.Core.csproj"),
            Path.Combine("Stryker.Core", "Stryker.Core.UnitTest", "Stryker.Core.UnitTest.csproj"),
            Path.Combine("Stryker.DataCollector", "Stryker.DataCollector", "Stryker.DataCollector.csproj"),
            Path.Combine("Stryker.RegexMutators", "Stryker.RegexMutators", "Stryker.RegexMutators.csproj"),
            Path.Combine("Stryker.RegexMutators", "Stryker.RegexMutators.UnitTest", "Stryker.RegexMutators.UnitTest.csproj"),
            Path.Combine("Stryker.Abstractions", "Stryker.Abstractions.csproj"),
            Path.Combine("Stryker.Options", "Stryker.Configuration.csproj"),
            Path.Combine("Stryker.Utilities", "Stryker.Utilities.csproj"),
            Path.Combine("Stryker.TestRunner", "Stryker.TestRunner.csproj"),
            Path.Combine("Stryker.TestRunner.VsTest", "Stryker.TestRunner.VsTest.csproj"),
            Path.Combine("Stryker.TestRunner.VsTest.UnitTest", "Stryker.TestRunner.VsTest.UnitTest.csproj"),
            Path.Combine("Stryker.Solutions", "Stryker.Solutions.csproj"),
            Path.Combine("Stryker.Solutions.Test", "Stryker.Solutions.Test.csproj")
        };
        solution.GetProjects("Debug").ShouldBe(expectedProjects);
    }
}
