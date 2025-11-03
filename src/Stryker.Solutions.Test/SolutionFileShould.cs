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

        var expectedProjects = new List<string>();
        expectedProjects.Add(Path.Combine("Stryker.CLI", "Stryker.CLI", "Stryker.CLI.csproj"));
        expectedProjects.Add(Path.Combine("Stryker.CLI", "Stryker.CLI.UnitTest", "Stryker.CLI.UnitTest.csproj"));
        expectedProjects.Add(Path.Combine("Stryker.Core", "Stryker.Core", "Stryker.Core.csproj"));
        expectedProjects.Add(Path.Combine("Stryker.Core", "Stryker.Core.UnitTest", "Stryker.Core.UnitTest.csproj"));
        expectedProjects.Add(Path.Combine("Stryker.DataCollector", "Stryker.DataCollector", "Stryker.DataCollector.csproj"));
        expectedProjects.Add(Path.Combine("Stryker.RegexMutators", "Stryker.RegexMutators", "Stryker.RegexMutators.csproj"));
        expectedProjects.Add(Path.Combine("Stryker.RegexMutators", "Stryker.RegexMutators.UnitTest", "Stryker.RegexMutators.UnitTest.csproj"));
        expectedProjects.Add(Path.Combine("Stryker.Abstractions", "Stryker.Abstractions.csproj"));
        expectedProjects.Add(Path.Combine("Stryker.Options", "Stryker.Configuration.csproj"));
        expectedProjects.Add(Path.Combine("Stryker.Utilities", "Stryker.Utilities.csproj"));
        expectedProjects.Add(Path.Combine("Stryker.TestRunner", "Stryker.TestRunner.csproj"));
        expectedProjects.Add(Path.Combine("Stryker.TestRunner.VsTest", "Stryker.TestRunner.VsTest.csproj"));
        expectedProjects.Add(Path.Combine("Stryker.TestRunner.VsTest.UnitTest", "Stryker.TestRunner.VsTest.UnitTest.csproj"));
        expectedProjects.Add(Path.Combine("Stryker.Solutions", "Stryker.Solutions.csproj"));
        expectedProjects.Add(Path.Combine("Stryker.Solutions.Test", "Stryker.Solutions.Test.csproj"));

        // Assert
        solution.ConfigurationExists("Debug", "Any CPU").ShouldBeTrue();
        // it should report all projects that are built in Stryker's Debug configuration
        solution.GetProjects("Debug").ShouldBe(expectedProjects);
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
        List<string> projectsList = [@"Stryker.CLI\Stryker.CLI\Stryker.CLI.csproj", @"Stryker.CLI\Stryker.CLI.UnitTest\Stryker.CLI.UnitTest.csproj",
            @"Stryker.Core\Stryker.Core\Stryker.Core.csproj", @"Stryker.Core\Stryker.Core.UnitTest\Stryker.Core.UnitTest.csproj",
            @"Stryker.DataCollector\Stryker.DataCollector\Stryker.DataCollector.csproj", @"Stryker.RegexMutators\Stryker.RegexMutators\Stryker.RegexMutators.csproj",
            @"Stryker.RegexMutators\Stryker.RegexMutators.UnitTest\Stryker.RegexMutators.UnitTest.csproj", "Stryker.Abstractions\\Stryker.Abstractions.csproj",
            "Stryker.Options\\Stryker.Configuration.csproj", "Stryker.Utilities\\Stryker.Utilities.csproj", "Stryker.TestRunner\\Stryker.TestRunner.csproj",
            "Stryker.TestRunner.VsTest\\Stryker.TestRunner.VsTest.csproj", "Stryker.TestRunner.VsTest.UnitTest\\Stryker.TestRunner.VsTest.UnitTest.csproj",
            "Stryker.Solutions\\Stryker.Solutions.csproj", "Stryker.Solutions.Test\\Stryker.Solutions.Test.csproj"];
        if (Path.DirectorySeparatorChar != '\\')
        {
            for (var i = 0; i < projectsList.Count; i++)
            {
                projectsList[i] = projectsList[i].Replace('\\', Path.DirectorySeparatorChar);
            }
        }
        solution.GetProjects("Debug").ShouldBe(projectsList);
    }
}
