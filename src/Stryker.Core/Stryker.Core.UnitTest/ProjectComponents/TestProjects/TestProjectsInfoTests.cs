using System;
using System.Collections.Generic;
using System.Linq;
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
            var testFiles = new List<TestFile>
            {
                new TestFile
                {
                    FilePath = "/c/",
                    Source = "bla",
                    Tests = new List<TestCase>
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
                TestProjects = new List<TestProject>
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
                TestProjects = new List<TestProject>
                {
                    new TestProject
                    {
                        TestProjectAnalyzerResult = testProjectAnalyzerResult,
                        TestFiles = testFiles
                    }
                }
            };
            var testProjectsInfoC = new TestProjectsInfo
            {
                TestProjects = new List<TestProject>
                {
                    new TestProject
                    {
                        TestProjectAnalyzerResult = Mock.Of<IAnalyzerResult>(),
                        TestFiles = new List<TestFile>
                        {
                            new TestFile
                            {
                                FilePath = "/c/",
                                Source = "bla",
                                Tests = new List<TestCase>
                                {
                                    new TestCase
                                    {
                                        Id = Guid.NewGuid(),
                                        Line = 1,
                                        Name = "test1",
                                        Source = "bla"
                                    }
                                }
                            }
                        }
                    }
                }
            };

            // Act
            var testProjectsInfoABC = testProjectsInfoA + testProjectsInfoB + testProjectsInfoC;

            // Assert
            testProjectsInfoABC.TestFiles.Count().ShouldBe(2);
            testProjectsInfoABC.TestFiles.First().Tests.Count().ShouldBe(2);
            testProjectsInfoABC.TestFiles.ElementAt(1).Tests.Count().ShouldBe(1);
        }
    }
}
