using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Abstractions.TestingHelpers;
using Moq;
using Shouldly;
using Spectre.Console.Testing;
using Stryker.Core.Mutants;
using Stryker.Core.Options;
using Stryker.Core.Options.Inputs;
using Stryker.Core.ProjectComponents.TestProjects;
using Stryker.Core.Reporters.Html;
using Stryker.Core.Reporters.Html.RealTime;
using Stryker.Core.Reporters.WebBrowserOpener;
using Xunit;

namespace Stryker.Core.UnitTest.Reporters.Html
{
    public class HtmlReporterTests : TestBase
    {
        private readonly Mock<IRealTimeMutantHandler> _handlerMock = new();

        [Fact]
        public void ShouldWriteJsonToFile()
        {
            var mockProcess = new Mock<IWebbrowserOpener>();
            var mockFileSystem = new MockFileSystem();
            var options = new StrykerOptions
            {
                Thresholds = new Thresholds { High = 80, Low = 60, Break = 0 },
                OutputPath = Directory.GetCurrentDirectory(),
                ReportFileName = "mutation-report"
            };
            var reporter = new HtmlReporter(options, mockFileSystem, browser: mockProcess.Object, mutantHandler: _handlerMock.Object);

            reporter.OnAllMutantsTested(ReportTestHelper.CreateProjectWith(), It.IsAny<TestProjectsInfo>());
            var reportPath = Path.Combine(options.ReportPath, "mutation-report.html");
            mockFileSystem.FileExists(reportPath).ShouldBeTrue($"Path {reportPath} should exist but it does not.");
        }

        [Fact]
        public void ShouldReplacePlaceholdersInHtmlFile()
        {
            var mockProcess = new Mock<IWebbrowserOpener>();
            var mockFileSystem = new MockFileSystem();
            var options = new StrykerOptions
            {
                Thresholds = new Thresholds { High = 80, Low = 60, Break = 0 },
                OutputPath = Directory.GetCurrentDirectory(),
                ReportFileName = "mutation-report"
            };
            var reporter = new HtmlReporter(options, mockFileSystem, browser: mockProcess.Object, mutantHandler: _handlerMock.Object);

            reporter.OnAllMutantsTested(ReportTestHelper.CreateProjectWith(), It.IsAny<TestProjectsInfo>());
            var reportPath = Path.Combine(options.ReportPath, "mutation-report.html");

            var fileContents = mockFileSystem.GetFile(reportPath).TextContents;

            fileContents.ShouldNotContain("##REPORT_JS##");
            fileContents.ShouldNotContain("##REPORT_TITLE##");
            fileContents.ShouldNotContain("##REPORT_JSON##");
        }

        [Fact]
        public void ShouldSupportSpacesInPath()
        {
            var mockProcess = new Mock<IWebbrowserOpener>();
            var mockFileSystem = new MockFileSystem();
            var options = new StrykerOptions
            {
                Thresholds = new Thresholds { High = 80, Low = 60, Break = 0 },
                OutputPath = " folder \\ next level",
                ReportFileName = "mutation-report"
            };
            var reporter = new HtmlReporter(options, mockFileSystem, browser: mockProcess.Object, mutantHandler: _handlerMock.Object);

            reporter.OnAllMutantsTested(ReportTestHelper.CreateProjectWith(), null);
            var reportPath = Path.Combine(options.ReportPath, "mutation-report.html");

            var fileContents = mockFileSystem.GetFile(reportPath).TextContents;

            fileContents.ShouldNotContain("##REPORT_JS##");
            fileContents.ShouldNotContain("##REPORT_TITLE##");
            fileContents.ShouldNotContain("##REPORT_JSON##");
        }

        [Fact]
        public void ShouldSupportSpacesInConsole()
        {
            var mockFileSystem = new MockFileSystem();
            var mockAnsiConsole = new TestConsole().EmitAnsiSequences();
            var options = new StrykerOptions
            {
                Thresholds = new Thresholds { High = 80, Low = 60, Break = 0 },
                OutputPath = " folder \\ next level",
                ReportFileName = "mutation-report"
            };
            var reporter = new HtmlReporter(options, mockFileSystem, mockAnsiConsole, Mock.Of<IWebbrowserOpener>(), mutantHandler: _handlerMock.Object);
            var mutationTree = ReportTestHelper.CreateProjectWith();

            var testProjectInfo = new TestProjectsInfo(mockFileSystem)
            {
                TestProjects = new List<TestProject>()
                {

                }
            };

            reporter.OnAllMutantsTested(mutationTree, testProjectInfo);

            var reportUri = "file://%20folder%20/%20next%20level/reports/mutation-report.html";
            mockAnsiConsole.Output.ShouldContain(reportUri);
        }

        [Fact]
        public void ShouldContainJsonInHtmlReportFile()
        {
            var mockProcess = new Mock<IWebbrowserOpener>();
            var mockFileSystem = new MockFileSystem();
            var options = new StrykerOptions
            {
                Thresholds = new Thresholds { High = 80, Low = 60, Break = 0 },
                OutputPath = Directory.GetCurrentDirectory(),
                ReportFileName = "mutation-report"
            };
            var reporter = new HtmlReporter(options, mockFileSystem, browser: mockProcess.Object, mutantHandler: _handlerMock.Object);
            var mutationTree = ReportTestHelper.CreateProjectWith();

            reporter.OnAllMutantsTested(mutationTree, It.IsAny<TestProjectsInfo>());
            var reportPath = Path.Combine(options.ReportPath, "mutation-report.html");

            var fileContents = mockFileSystem.GetFile(reportPath).TextContents;

            fileContents.ShouldContain(@"""thresholds"":{");
            fileContents.ShouldContain(@"""high"":80");
            fileContents.ShouldContain(@"""low"":60");
        }

        [Fact]
        public void ShouldOpenHtmlReportImmediatelyIfOptionIsProvided()
        {
            var mockProcess = new Mock<IWebbrowserOpener>();
            var mockFileSystem = new MockFileSystem();
            var options = new StrykerOptions
            {
                ReportTypeToOpen = ReportType.Html,
                OutputPath = Directory.GetCurrentDirectory(),
                ReportFileName = "mutation-report"
            };
            var testProjectInfo = new TestProjectsInfo(mockFileSystem)
            {
                TestProjects = Array.Empty<TestProject>()
            };
            var reporter = new HtmlReporter(options, mockFileSystem, browser: mockProcess.Object, mutantHandler: _handlerMock.Object);
            var mutationTree = ReportTestHelper.CreateProjectWith();

            reporter.OnMutantsCreated(mutationTree, testProjectInfo);

            var reportUri = Path.Combine(options.ReportPath, $"{options.ReportFileName}.html");

            // Check if browser open action is invoked
            mockProcess.Verify(m => m.Open(reportUri));
        }

        [Fact]
        public void ShouldOpenHtmlReportImmediatelyIfOptionIsProvidedAndSpacesInPath()
        {
            var mockProcess = new Mock<IWebbrowserOpener>();
            var mockFileSystem = new MockFileSystem();
            var options = new StrykerOptions
            {
                ReportTypeToOpen = ReportType.Html,
                OutputPath = " folder \\ next level",
                ReportFileName = "mutation-report"
            };

            var reporter = new HtmlReporter(options, mockFileSystem, browser: mockProcess.Object, mutantHandler: _handlerMock.Object);
            var mutationTree = ReportTestHelper.CreateProjectWith();

            var testProjectInfo = new TestProjectsInfo(mockFileSystem)
            {
                TestProjects = Array.Empty<TestProject>()
            };

            reporter.OnMutantsCreated(mutationTree, testProjectInfo);

            var reportUri = Path.Combine(options.ReportPath, $"{options.ReportFileName}.html");

            // Check if browser open action is invoked
            mockProcess.Verify(m => m.Open(reportUri));
            mockProcess.VerifyNoOtherCalls();
        }

        [Fact]
        public void ShouldCloseSseEndpointAfterReportingAllMutantsTested()
        {
            var mockProcess = new Mock<IWebbrowserOpener>();
            var mockFileSystem = new MockFileSystem();
            var options = new StrykerOptions
            {
                ReportTypeToOpen = ReportType.Html,
                Thresholds = new Thresholds { High = 80, Low = 60, Break = 0 },
                OutputPath = Directory.GetCurrentDirectory(),
                ReportFileName = "mutation-report"
            };
            var reporter = new HtmlReporter(options, mockFileSystem, browser: mockProcess.Object, mutantHandler: _handlerMock.Object);
            var mutationTree = ReportTestHelper.CreateProjectWith();

            reporter.OnAllMutantsTested(mutationTree, It.IsAny<TestProjectsInfo>());

            _handlerMock.Verify(s => s.CloseSseEndpoint());
        }

        [Fact]
        public void ShouldSendMutantEventIfOpenReportOptionIsProvided()
        {
            var mockProcess = new Mock<IWebbrowserOpener>();
            var mockFileSystem = new MockFileSystem();
            var options = new StrykerOptions
            {
                ReportTypeToOpen = ReportType.Html,
                Thresholds = new Thresholds { High = 80, Low = 60, Break = 0 },
                OutputPath = Directory.GetCurrentDirectory(),
                ReportFileName = "mutation-report"
            };
            var reporter = new HtmlReporter(options, mockFileSystem, browser: mockProcess.Object, mutantHandler: _handlerMock.Object);

            reporter.OnMutantTested(new Mutant());

            _handlerMock.Verify(h => h.SendMutantTestedEvent(It.IsAny<Mutant>()));
        }

        [Fact]
        public void ShouldNotSendMutantEventIfOpenReportOptionIsProvided()
        {
            var mockProcess = new Mock<IWebbrowserOpener>();
            var mockFileSystem = new MockFileSystem();
            var options = new StrykerOptions
            {
                Thresholds = new Thresholds { High = 80, Low = 60, Break = 0 },
                OutputPath = Directory.GetCurrentDirectory(),
                ReportFileName = "mutation-report"
            };
            var reporter = new HtmlReporter(options, mockFileSystem, browser: mockProcess.Object, mutantHandler: _handlerMock.Object);

            reporter.OnMutantTested(new Mutant());

            _handlerMock.VerifyNoOtherCalls();
        }

        [Theory]
        [InlineData(ReportType.Dashboard)]
        [InlineData(null)]
        public void ShouldNotOpenHtmlReportIfHtmlOptionIsNotProvided(ReportType? reportType)
        {
            var mockProcess = new Mock<IWebbrowserOpener>();
            var mockFileSystem = new MockFileSystem();
            var options = new StrykerOptions
            {
                ReportTypeToOpen = reportType,
                OutputPath = Directory.GetCurrentDirectory()
            };

            var reporter = new HtmlReporter(options, mockFileSystem, browser: mockProcess.Object, mutantHandler: _handlerMock.Object);
            var mutationTree = ReportTestHelper.CreateProjectWith();

            reporter.OnAllMutantsTested(mutationTree, It.IsAny<TestProjectsInfo>());

            // Check if browser open action is invoked
            mockProcess.VerifyNoOtherCalls();
        }
    }
}
