using System;
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
            var testCaseA = new TestCase
            {
                Id = guid,
                Name = "1",
                Line = 1,
                Source = "1"
            };
            var testCaseB = new TestCase
            {
                Id = guid,
                Name = "1",
                Line = 1,
                Source = "1"
            };

            // Assert
            testCaseA.ShouldBe(testCaseB);
        }

        [Theory]
        [InlineData("fd4896a2-1bd9-4e83-9e81-308059525bc9", "2", 1, "1")]
        [InlineData("00000000-0000-0000-0000-000000000000", "1", 1, "1")]
        [InlineData("00000000-0000-0000-0000-000000000000", "2", 2, "1")]
        [InlineData("00000000-0000-0000-0000-000000000000", "2", 1, "2")]
        public void TestCaseNotEqualsWhenNotAllPropertiesEqual(string id, string name, int lineNumber, string source)
        {
            // Arrange
            var testCaseA = new TestCase
            {
                Id = new Guid(id),
                Name = name,
                Line = lineNumber,
                Source = source
            };
            var testCaseB = new TestCase
            {
                Id = Guid.Empty,
                Name = "2",
                Line = 1,
                Source = "1"
            };

            // Assert
            testCaseA.ShouldNotBe(testCaseB);
        }
    }
}
