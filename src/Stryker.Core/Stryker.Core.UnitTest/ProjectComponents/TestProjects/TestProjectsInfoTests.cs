using System;
using System.Collections.Generic;
using Buildalyzer;
using Moq;
using Shouldly;
using Stryker.Core.ProjectComponents.TestProjects;
using Xunit;

namespace Stryker.Core.UnitTest.ProjectComponents.TestProjects
{
    public class TestProjectsInfoTests
    {
        [Fact]
        public void MergeTestProjectsInfo()
        {
            // Arrange
            var testProjectAnalyzerResult = Mock.Of<IAnalyzerResult>();
            var testFiles = new HashSet<TestFile>
            {
                new TestFile
                {
                    FilePath = "/c/",
                    Source = "bla",
                    Tests = new HashSet<TestCase>
                    {
                        new TestCase
                        {
                            Id = Guid.NewGuid(),
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
                }
            };

            var testProjectsInfoA = new TestProjectsInfo
            {
                TestProjects = new HashSet<TestProject>
                {
                    new TestProject
                    {
                        TestProjectAnalyzerResult = testProjectAnalyzerResult,
                        TestFiles = testFiles
                    }
                }
            };
            var testProjectsInfoB = new TestProjectsInfo
            {
                TestProjects = new HashSet<TestProject>
                {
                    new TestProject
                    {
                        TestProjectAnalyzerResult = testProjectAnalyzerResult,
                        TestFiles = new HashSet<TestFile>(testFiles)
                        {
                            new TestFile
                            {
                                FilePath = "/d/",
                                Source = "blaaa",
                                Tests = new HashSet<TestCase>
                                {
                                    new TestCase
                                    {
                                        Id = Guid.NewGuid(),
                                        Line = 2,
                                        Name = "test2",
                                        Source = "blaaa"
                                    }
                                }
                            }
                        }
                    }
                }
            };

            // Act
            var testProjectsInfoAB = testProjectsInfoA + testProjectsInfoB;

            // Assert
            testProjectsInfoAB.TestFiles.Count.ShouldBe(2);
        }
    }
}
