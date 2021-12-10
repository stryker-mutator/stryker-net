using System;
using System.Collections.Generic;
using Shouldly;
using Stryker.Core.ProjectComponents.TestProjects;
using Xunit;

namespace Stryker.Core.UnitTest.ProjectComponents.TestProjects
{
    public class TestFileTests
    {
        [Fact]
        public void CombineTestFiles()
        {
            var testCase1Id = new Guid("eb0b2dfb-59f3-4031-ac2f-1c572715f2ce");
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

            var fileAB = fileA + fileB;

            fileAB.Tests.Count.ShouldBe(2);
        }
    }
}
