using System;
using System.Collections.Generic;
using Buildalyzer;
using Moq;
using Shouldly;
using Stryker.Core.ProjectComponents.TestProjects;
using Xunit;

namespace Stryker.Core.UnitTest.ProjectComponents.TestProjects
{
    public class TestProjectTests
    {
        [Fact]
        public void TestProjectEquals()
        {
            // Arrange
            var testProjectAnalyzerResult = Mock.Of<IAnalyzerResult>();
            var testCase1Id = Guid.NewGuid();
            var fileA = new TestFile
            {
                FilePath = "/c/",
                Source = "bla",
                Tests = new HashSet<TestCase>
                {
                    new TestCase
                    {
                        Id = testCase1Id,
                        Line = 1,
                        Name = "test1",
                        Source = "bla"
                    },
                    new TestCase
                    {
                        Id = Guid.NewGuid(),
                        Line = 2,
                        Name = "test2",
                        Source = "bla"
                    }
                }
            };
            var fileB = new TestFile
            {
                FilePath = "/c/",
                Source = "bla",
                Tests = new HashSet<TestCase>
                {
                    new TestCase
                    {
                        Id = testCase1Id,
                        Line = 1,
                        Name = "test1",
                        Source = "bla"
                    }
                }
            };

            var testProjectA = new TestProject
            {
                TestProjectAnalyzerResult = testProjectAnalyzerResult,
                TestFiles = new HashSet<TestFile> { fileA }
            };

            var testProjectB = new TestProject
            {
                TestProjectAnalyzerResult = testProjectAnalyzerResult,
                TestFiles = new HashSet<TestFile> { fileB }
            };

            // Assert
            testProjectA.ShouldBe(testProjectB);
        }
    }
}
