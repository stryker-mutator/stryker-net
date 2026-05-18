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
        var solution = SolutionFile.GetSolution(Path.Combine("..","..","..","..","Stryker.slnx"));
        // Assert
        Assert.IsNotNull(solution);
    }

    [TestMethod]
    public void IdentifyStrykerBuildTypes()
    {
        // Arrange
        // Act
        var solution =SolutionFile.GetSolution(Path.Combine("..","..","..","..","Stryker.slnx"));
        // Assert
        solution.GetBuildTypes().ShouldBe(["Debug", "Release"]);
    }

    [TestMethod]
    public void IdentifyStrykerPlatform()
    {
        // Arrange
        // Act
        var solution = SolutionFile.GetSolution(Path.Combine("..","..","..","..","Stryker.slnx"));

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
        var solution = SolutionFile.BuildFromProjectList("Solution.sln", projects, [platform], [platform]);

        // Assert
        solution.GetProjectsWithDetails("Debug").ShouldBe(projects.Select(p => (p, "Debug", platform)));
    }

    [TestMethod]
    public void DefaultPlatformToAnyCpuIfNotSpecified()
    {
        // Arrange
        List<string> projects = ["Project.csproj", "Test.csproj"];
        // Act
        var solution = SolutionFile.BuildFromProjectList("Solution.sln", projects, ["Z80", "Any CPU"]);

        // Assert
        solution.GetProjectsWithDetails("Debug").ShouldBe(projects.Select(p => (p, "Debug", "Any CPU")));
    }

    [TestMethod]
    public void DefaultPlatformToFirstIfAnyCpuNotProvided()
    {
        // Arrange
        List<string> projects = ["Project.csproj", "Test.csproj"];
        // Act
        var solution = SolutionFile.BuildFromProjectList("Solution.sln",  projects, ["Z80", "6502"]);

        // Assert
        solution.GetProjectsWithDetails("Debug").ShouldBe(projects.Select(p => (p, "Debug", "Z80")));
    }

    [TestMethod]
    public void ProvideProjectListForGivenConfiguration()
    {
        // Arrange
        // Act
        var solution = SolutionFile.GetSolution(Path.GetFullPath(Path.Combine("..","..","..","..","Stryker.slnx")));
        // Assert
        solution.ConfigurationExists("Debug", "Any CPU").ShouldBeTrue();
        var solutionPath = Path.GetDirectoryName(solution.FileName);
        // it should report all projects that are built in Stryker's Debug configuration
        var expectedProjects = new List<string>
        {
            Path.Combine(solutionPath, "Stryker.CLI", "Stryker.CLI", "Stryker.CLI.csproj"),
            Path.Combine(solutionPath, "Stryker.CLI", "Stryker.CLI.UnitTest", "Stryker.CLI.UnitTest.csproj"),
            Path.Combine(solutionPath, "Stryker.Core", "Stryker.Core", "Stryker.Core.csproj"),
            Path.Combine(solutionPath, "Stryker.Core", "Stryker.Core.UnitTest", "Stryker.Core.UnitTest.csproj"),
            Path.Combine(solutionPath, "Stryker.DataCollector", "Stryker.DataCollector.csproj"),
            Path.Combine(solutionPath, "Stryker.RegexMutators", "Stryker.RegexMutators", "Stryker.RegexMutators.csproj"),
            Path.Combine(solutionPath, "Stryker.RegexMutators", "Stryker.RegexMutators.UnitTest", "Stryker.RegexMutators.UnitTest.csproj"),
            Path.Combine(solutionPath, "Stryker.Abstractions", "Stryker.Abstractions.csproj"),
            Path.Combine(solutionPath, "Stryker.Configuration", "Stryker.Configuration.csproj"),
            Path.Combine(solutionPath, "Stryker.Utilities", "Stryker.Utilities.csproj"),
            Path.Combine(solutionPath, "Stryker.TestRunner", "Stryker.TestRunner.csproj"),
            Path.Combine(solutionPath, "Stryker.TestRunner.VsTest", "Stryker.TestRunner.VsTest.csproj"),
            Path.Combine(solutionPath, "Stryker.TestRunner.VsTest.UnitTest", "Stryker.TestRunner.VsTest.UnitTest.csproj"),
            Path.Combine(solutionPath, "Stryker.Solutions", "Stryker.Solutions.csproj"),
            Path.Combine(solutionPath, "Stryker.Solutions.Test", "Stryker.Solutions.Test.csproj"),
            Path.Combine(solutionPath, "Stryker.TestRunner.MicrosoftTestPlatform", "Stryker.TestRunner.MicrosoftTestPlatform.csproj"),
            Path.Combine(solutionPath, "Stryker.TestRunner.MicrosoftTestPlatform.UnitTest", "Stryker.TestRunner.MicrosoftTestPlatform.UnitTest.csproj"),
        };
        solution.GetProjects("Debug").ShouldBe(expectedProjects, ignoreOrder: true);
    }

    [TestMethod]
    [DataRow("MicrosoftTestPlatform.sln")]
    [DataRow("MicrosoftTestPlatform.slnx")]
    public void ProvideProjectListForGivenConfigurationOnSolutionWithMultiplePlatforms(string solutionFile)
    {
        // Arrange
        // Act
        var solution = SolutionFile.GetSolution( Path.GetFullPath(Path.Combine("..","..","..","..","..","integrationtest","TargetProjects",solutionFile)));
        var solutionPath = Path.GetDirectoryName(solution.FileName);

        // Assert
        var expectedProjects = new List<string>
        {
            Path.Combine(solutionPath, "NetCore", "TargetProject", "TargetProject.csproj"),
            Path.Combine(solutionPath, "NetCore", "Library", "Library.csproj"),
            Path.Combine(solutionPath, "MicrosoftTestPlatform", "UnitTests.MSTest", "UnitTests.MSTest.csproj"),
            Path.Combine(solutionPath, "MicrosoftTestPlatform", "UnitTests.XUnit", "UnitTests.XUnit.csproj"),
            Path.Combine(solutionPath, "MicrosoftTestPlatform", "UnitTests.NUnit", "UnitTests.NUnit.csproj"),
            Path.Combine(solutionPath, "MicrosoftTestPlatform", "UnitTests.TUnit", "UnitTests.TUnit.csproj"),
        };
        solution.GetProjects("Debug").ShouldBe(expectedProjects, ignoreOrder: true);

        var projectsWithDetails = solution.GetProjectsWithDetails("Debug").ToList();
        projectsWithDetails.Select(x => x.file).ShouldBe(expectedProjects, ignoreOrder: true);
        projectsWithDetails.All(x => x.buildType == "Debug").ShouldBeTrue();
        projectsWithDetails.All(x => x.platform == "Any CPU" || x.platform == "AnyCPU").ShouldBeTrue();
    }

    [TestMethod]
    public void PickExactMatch()
    {
        // Arrange
        var solution = SolutionFile.BuildFromProjectList("Solution.sln", ["Project.csproj", "Test.csproj"], ["x86", "x64"]);

        var match = solution.GetMatching("Debug", "x64");
        // Assert
        match.ShouldBe(("Debug", "x64"));
    }

    [TestMethod]
    public void FallBackOnDebug()
    {
        // Arrange
        var solution = SolutionFile.BuildFromProjectList("Solution.sln", ["Project.csproj", "Test.csproj"], ["x86", "x64"]);

        var match = solution.GetMatching("Stryker", "x64");
        // Assert
        match.ShouldBe(("Debug", "x64"));
    }

    [TestMethod]
    public void FallBackOnAnyCPU()
    {
        // Arrange
        var solution = SolutionFile.BuildFromProjectList("Solution.sln", ["Project.csproj", "Test.csproj"], ["AnyCPU"]);

        var match = solution.GetMatching("Debug", "x64");
        // Assert
        match.ShouldBe(("Debug", "Any CPU"));
    }

    [TestMethod]
    public void PickFirstIfNoMatch()
    {
        // Arrange
        var solution = SolutionFile.BuildFromProjectList("Solution.sln", ["Project.csproj", "Test.csproj"], ["AnyCPU"]);

        var match = solution.GetMatching("Stryker", "x64");
        // Assert
        match.ShouldBe(("Debug", "Any CPU"));
    }
}
