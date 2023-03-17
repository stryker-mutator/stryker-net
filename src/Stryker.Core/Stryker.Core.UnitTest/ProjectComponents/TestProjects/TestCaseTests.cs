using System;
using Microsoft.CodeAnalysis.CSharp;
using Shouldly;
using Stryker.Core.ProjectComponents.TestProjects;
using Xunit;

namespace Stryker.Core.UnitTest.ProjectComponents.TestProjects
{
    public class TestCaseTests
    {
        [Fact]
        public void TestCaseEqualsWhenAllPropertiesEqual()
        {
            // Arrange
            var guid = Guid.NewGuid();
            var node = SyntaxFactory.Block();
            var testCaseA = new TestCase
            {
                Id = guid,
                Name = "1",
                Node = node
            };
            var testCaseB = new TestCase
            {
                Id = guid,
                Name = "1",
                Node = node
            };

            // Assert
            testCaseA.ShouldBe(testCaseB);
            testCaseA.GetHashCode().ShouldBe(testCaseB.GetHashCode());
        }

        [Theory]
        [InlineData("fd4896a2-1bd9-4e83-9e81-308059525bc9", "node2")]
        [InlineData("00000000-0000-0000-0000-000000000000", "node1")]
        public void TestCaseNotEqualsWhenNotAllPropertiesEqual(string id, string name)
        {
            // Arrange
            var node = SyntaxFactory.Block();
            var testCaseA = new TestCase
            {
                Id = new Guid(id),
                Name = name,
                Node = node
            };
            var testCaseB = new TestCase
            {
                Id = Guid.Empty,
                Name = "node2",
                Node = node
            };

            // Assert
            testCaseA.ShouldNotBe(testCaseB);
        }
    }
}
