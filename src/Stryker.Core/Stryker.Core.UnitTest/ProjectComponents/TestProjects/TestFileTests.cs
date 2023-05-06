using System;
using Microsoft.CodeAnalysis.CSharp;
using Shouldly;
using Stryker.Core.ProjectComponents.TestProjects;
using Xunit;

namespace Stryker.Core.UnitTest.ProjectComponents.TestProjects;

public class TestFileTests
{
    [Fact]
    public void MergeTestFiles()
    {
        // Arrange
        var testCase1Id = Guid.NewGuid();
        var node = SyntaxFactory.Block();
        var fileA = new TestFile
        {
            FilePath = "/c/",
            Source = "bla"
        };
        fileA.AddTest(testCase1Id, "test1", node);
        var fileB = new TestFile
        {
            FilePath = "/c/",
            Source = "bla"
        };
        fileB.AddTest(testCase1Id, "test1", node);

        // Assert
        fileA.ShouldBe(fileB);
        fileA.GetHashCode().ShouldBe(fileB.GetHashCode());
    }
}
