using System;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Shouldly;
using Stryker.Core.ProjectComponents.TestProjects;

namespace Stryker.Core.UnitTest.ProjectComponents.TestProjects
{
    [TestClass]
    public class TestFileTests
    {
        [TestMethod]
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
}
