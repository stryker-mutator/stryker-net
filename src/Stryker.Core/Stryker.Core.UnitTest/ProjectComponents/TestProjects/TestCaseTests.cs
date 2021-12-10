using System;
using Shouldly;
using Stryker.Core.ProjectComponents.TestProjects;
using Xunit;

namespace Stryker.Core.UnitTest.ProjectComponents.TestProjects
{
    public class TestCaseTests
    {
        [Fact]
        public void TestCaseEquals()
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
    }
}
