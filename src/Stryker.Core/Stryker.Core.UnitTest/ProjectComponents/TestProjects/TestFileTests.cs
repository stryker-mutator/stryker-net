using System;
using System.Collections.Generic;
using System.Linq;
using Shouldly;
using Stryker.Core.ProjectComponents.TestProjects;
using Xunit;

namespace Stryker.Core.UnitTest.ProjectComponents.TestProjects
{
    public class TestFileTests
    {
        [Fact]
        public void MergeTestFiles()
        {
            // Arrange
            var testCase1Id = Guid.NewGuid();
            var fileA = new TestFile
            {
                FilePath = "/c/",
                Source = "bla",
                Tests = new List<TestCase>
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
                Tests = new List<TestCase>
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

            // Act
            var fileAB = fileA + fileB;

            // Assert
            fileAB.Tests.Count().ShouldBe(2);
        }
    }
}
