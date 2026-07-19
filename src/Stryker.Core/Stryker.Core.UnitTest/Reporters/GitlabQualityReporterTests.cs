using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Abstractions;
using System.IO.Abstractions.TestingHelpers;
using System.Linq;
using System.Text.Json;
using DotUtils.StreamUtils;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Shouldly;
using Stryker.Configuration.Options;
using Stryker.Core.ProjectComponents.TestProjects;
using Stryker.Core.Reporters;

namespace Stryker.Core.UnitTest.Reporters;

[TestClass]
public class GitlabQualityReporterTests : TestBase
{
    private readonly IFileSystem _fileSystemMock = new MockFileSystem();
    private readonly string _testFilePath = "c:\\mytestfile.cs";
    private readonly string _testFileContents = @"using Microsoft.VisualStudio.TestTools.UnitTesting;

    namespace ExtraProject.XUnit
    {
        public class UnitTest1
        {
            [TestMethod]
            public void Test1()
            {
                // example test
            }
        }
    }
    ";

    public GitlabQualityReporterTests() => _fileSystemMock.File.WriteAllText(_testFilePath, _testFileContents);

    [TestMethod]
    public void GitlabQualityReporter_OnAllMutantsTestedShouldWriteJsonToFile()
    {
        // arrange
        var reportName = "gitlab_quality.json";
        var mockFileSystem = new MockFileSystem();
        var options = new StrykerOptions
        {
            Thresholds = new Thresholds { High = 80, Low = 60, Break = 0 },
            OutputPath = Directory.GetCurrentDirectory(),
            ReportFileName = reportName
        };

        var testProjectsInfo = new TestProjectsInfo(_fileSystemMock)
        {
            TestProjects = new List<TestProject>
            {
                new(_fileSystemMock, TestHelper.SetupProjectAnalyzerResult(
                    sourceFiles: new[] { _testFilePath }).Object)
            }
        };
        var node = CSharpSyntaxTree.ParseText(_testFileContents).GetRoot().DescendantNodes().OfType<MethodDeclarationSyntax>().Single();
        testProjectsInfo.TestProjects.First().TestFiles.First().AddTest(Guid.Empty.ToString(), "myUnitTestName", node);

        var reporter = new GitlabQualityReporter(options, mockFileSystem);

        // act
        reporter.OnAllMutantsTested(ReportTestHelper.CreateProjectWith(), testProjectsInfo);

        // assert
        var reportPath = Path.Combine(options.ReportPath, reportName);
        mockFileSystem.FileExists(reportPath).ShouldBeTrue($"Path {reportPath} should exist but it does not.");
        var fileContents = mockFileSystem.File.OpenRead(reportPath);
        var content = fileContents.ReadToEnd();

        var report = JsonSerializer.Deserialize<dynamic[]>(content);

        report.ShouldNotBeNull();
        report.Length.ShouldBe(72);
    }
}
