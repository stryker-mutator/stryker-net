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
        var solution = SolutionFile.GetSolution(Path.Combine("..","..","..","..","Stryker.sln"));
        // Assert
        Assert.IsNotNull(solution);
    }

    [TestMethod]
    public void IdentifyStrykerBuildTypes()
    {
        // Arrange
        // Act
        var solution =SolutionFile.GetSolution(Path.Combine("..","..","..","..","Stryker.sln"));
        // Assert
        solution.GetBuildTypes().ShouldBe(["Debug", "Release"]);
    }

    [TestMethod]
    public void IdentifyStrykerPlatform()
    {
        // Arrange
        // Act
        var solution = SolutionFile.GetSolution(Path.Combine("..","..","..","..","Stryker.sln"));

        // Assert
        solution.ConfigurationExists("Debug", "Any CPU").ShouldBeTrue();
    }

    [TestMethod]
    [DataRow("Any CPU")]
    [DataRow("AnyCPU")]
    [DataRow("Z80")]
    public void DetectPlatformIfNotSpecified(string platform)
    {
        // Arrange
        List<string> projects = ["Project.csproj", "Test.csproj"];
        // Act
        var solution = SolutionFile.BuildFromProjectList( projects, [platform]);

        // Assert
        solution.GetProjectsWithDetails("Debug").ShouldBe(projects.Select(p => (p, "Debug", platform)));
    }

    [TestMethod]
    public void DefaultPlatformToAnyCpuIfNotSpecified()
    {
        // Arrange
        List<string> projects = ["Project.csproj", "Test.csproj"];
        // Act
        var solution = SolutionFile.BuildFromProjectList( projects, ["Z80", "Any CPU"]);

        // Assert
        solution.GetProjectsWithDetails("Debug").ShouldBe(projects.Select(p => (p, "Debug", "Any CPU")));
    }

    [TestMethod]
    public void DefaultPlatformToFirstIfAnyCpuNotProvided()
    {
        // Arrange
        List<string> projects = ["Project.csproj", "Test.csproj"];
        // Act
        var solution = SolutionFile.BuildFromProjectList( projects, ["Z80", "6502"]);

        // Assert
        solution.GetProjectsWithDetails("Debug").ShouldBe(projects.Select(p => (p, "Debug", "Z80")));
    }

    [TestMethod]
    public void ProvideProjectListForGivenConfiguration()
    {
        // Arrange
        // Act
        var solution = SolutionFile.GetSolution(Path.Combine("..","..","..","..","Stryker.sln"));
        // Assert
        solution.ConfigurationExists("Debug", "Any CPU").ShouldBeTrue();

        // it should report all projects that are built in Stryker's Debug configuration
        var expectedProjects = new List<string>
        {
            Path.Combine("Stryker.CLI", "Stryker.CLI", "Stryker.CLI.csproj"),
            Path.Combine("Stryker.CLI", "Stryker.CLI.UnitTest", "Stryker.CLI.UnitTest.csproj"),
            Path.Combine("Stryker.Core", "Stryker.Core", "Stryker.Core.csproj"),
            Path.Combine("Stryker.Core", "Stryker.Core.UnitTest", "Stryker.Core.UnitTest.csproj"),
            Path.Combine("Stryker.DataCollector", "Stryker.DataCollector.csproj"),
            Path.Combine("Stryker.RegexMutators", "Stryker.RegexMutators", "Stryker.RegexMutators.csproj"),
            Path.Combine("Stryker.RegexMutators", "Stryker.RegexMutators.UnitTest", "Stryker.RegexMutators.UnitTest.csproj"),
            Path.Combine("Stryker.Abstractions", "Stryker.Abstractions.csproj"),
            Path.Combine("Stryker.Configuration", "Stryker.Configuration.csproj"),
            Path.Combine("Stryker.Utilities", "Stryker.Utilities.csproj"),
            Path.Combine("Stryker.TestRunner", "Stryker.TestRunner.csproj"),
            Path.Combine("Stryker.TestRunner.VsTest", "Stryker.TestRunner.VsTest.csproj"),
            Path.Combine("Stryker.TestRunner.VsTest.UnitTest", "Stryker.TestRunner.VsTest.UnitTest.csproj"),
            Path.Combine("Stryker.Solutions", "Stryker.Solutions.csproj"),
            Path.Combine("Stryker.Solutions.Test", "Stryker.Solutions.Test.csproj"),
            Path.Combine("Stryker.TestRunner.MicrosoftTestPlatform", "Stryker.TestRunner.MicrosoftTestPlatform.csproj"),
            Path.Combine("Stryker.TestRunner.MicrosoftTestPlatform.UnitTest", "Stryker.TestRunner.MicrosoftTestPlatform.UnitTest.csproj"),
        };
        solution.GetProjects("Debug").ShouldBe(expectedProjects, ignoreOrder: true);
    }

    [TestMethod]
    [DataRow("ExampleLibrary.sln", "Any CPU")]
    [DataRow("ExampleLibrary.slnx", "AnyCPU")]
    public void ProvideProjectListForGivenConfigurationOnSolutionWithMultiplePlatforms(string solutionFile, string expectedPlatform)
    {
        // Arrange
        // Act
        var solution = SolutionFile.GetSolution(Path.Combine("..","..","..","..","..","fixtures","ExampleLibrary",solutionFile));

        // Assert
        var expectedProjectDetails = new List<(string file, string buildType, string platform)>
        {
            (Path.Combine("src", "ExampleLibrary.csproj"), "Debug", expectedPlatform),
            (Path.Combine("tests", "ExampleLibrary.Tests.csproj"), "Debug", expectedPlatform),
        };
        solution.GetProjects("Debug").ShouldBe(expectedProjectDetails.Select(x => x.file));
        solution.GetProjectsWithDetails("Debug").ShouldBe(expectedProjectDetails);
    }

    [TestMethod]
    public void PickExactMatch()
    {
        // Arrange
        var solution = SolutionFile.BuildFromProjectList(["Project.csproj", "Test.csproj"], ["x86", "x64"]);

        var match = solution.GetMatching("Debug", "x64");
        // Assert
        match.ShouldBe(("Debug", "x64"));
    }

    [TestMethod]
    public void FallBackOnDebug()
    {
        // Arrange
        var solution = SolutionFile.BuildFromProjectList(["Project.csproj", "Test.csproj"], ["x86", "x64"]);

        var match = solution.GetMatching("Stryker", "x64");
        // Assert
        match.ShouldBe(("Debug", "x64"));
    }

    [TestMethod]
    public void FallBackOnAnyCPU()
    {
        // Arrange
        var solution = SolutionFile.BuildFromProjectList(["Project.csproj", "Test.csproj"], ["AnyCPU"]);

        var match = solution.GetMatching("Debug", "x64");
        // Assert
        match.ShouldBe(("Debug", "AnyCPU"));
    }

    [TestMethod]
    public void PickFirstIfNoMatch()
    {
        // Arrange
        var solution = SolutionFile.BuildFromProjectList(["Project.csproj", "Test.csproj"], ["AnyCPU"]);

        var match = solution.GetMatching("Stryker", "x64");
        // Assert
        match.ShouldBe(("Debug", "AnyCPU"));
    }
}
