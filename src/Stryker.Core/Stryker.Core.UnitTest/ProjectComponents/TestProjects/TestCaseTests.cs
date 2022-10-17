using System;
using Microsoft.CodeAnalysis;
using Moq;
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
            var node = Mock.Of<SyntaxNode>();
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
        [InlineData("fd4896a2-1bd9-4e83-9e81-308059525bc9", "2")]
        [InlineData("00000000-0000-0000-0000-000000000000", "1")]
        [InlineData("00000000-0000-0000-0000-000000000000", "2")]
        public void TestCaseNotEqualsWhenNotAllPropertiesEqual(string id, string name)
        {
            // Arrange
            var node = Mock.Of<SyntaxNode>();
            var testCaseA = new TestCase
            {
                Id = new Guid(id),
                Name = name,
                Node = node
            };
            var testCaseB = new TestCase
            {
                Id = Guid.Empty,
                Name = "2",
                Node = node
            };

            // Assert
            testCaseA.ShouldNotBe(testCaseB);
        }
    }
}
