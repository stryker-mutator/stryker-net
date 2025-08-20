using Shouldly;

namespace Stryker.Solutions.Test;

[TestClass]
public sealed class SolutionFileShould
{
    [TestMethod]
    public void LoadStrykerSlnFile()
    {
        // Arrange
        var solution = SolutionFile.LoadSolution(@"..\..\..\..\Stryker.sln");
        // Act
        // Assert
        Assert.IsNotNull(solution);
    }

    [TestMethod]
    public void IdentifyStrykerBuildTypes()
    {
        // Arrange
        var solution = SolutionFile.LoadSolution(@"..\..\..\..\Stryker.sln");
        // Act
        // Assert
        solution.GetBuildTypes().ShouldBe(["Debug", "Release"]);
    }

    [TestMethod]
    public void IdentifyStrykerPlatform()
    {
        // Arrange
        var solution = SolutionFile.LoadSolution(@"..\..\..\..\Stryker.sln");
        // Act
        // Assert
        solution.GetPlatforms().ShouldBe(["Any CPU"]);
    }
}
