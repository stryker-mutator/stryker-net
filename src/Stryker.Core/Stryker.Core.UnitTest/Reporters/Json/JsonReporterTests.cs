using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Abstractions;
using System.IO.Abstractions.TestingHelpers;
using System.Linq;
using System.Runtime.InteropServices;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Shouldly;
using Stryker.Abstractions;
using Stryker.Abstractions.Testing;
using Stryker.Core.ProjectComponents.Csharp;
using Stryker.Core.ProjectComponents.TestProjects;
using Stryker.Core.Reporters.Json;
using Stryker.Core.Reporters.Json.SourceFiles;

namespace Stryker.Core.UnitTest.Reporters.Json;

[TestClass]
public class JsonReporterTests : TestBase
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
    public JsonReporterTests() => _fileSystemMock.File.WriteAllText(_testFilePath, _testFileContents);

    [TestMethod]
    public void JsonMutantPositionLine_ThrowsArgumentExceptionWhenSetToLessThan1()
    {
        Should.Throw<ArgumentException>(() => new Position().Line = -1);
        Should.Throw<ArgumentException>(() => new Position().Line = 0);
    }

    [TestMethod]
    public void JsonMutantPositionColumn_ThrowsArgumentExceptionWhenSetToLessThan1()
    {
        Should.Throw<ArgumentException>(() => new Position().Column = -1);
        Should.Throw<ArgumentException>(() => new Position().Column = 0);
    }

    [TestMethod]
    public void JsonMutantLocation_FromValidFileLinePositionSpanShouldAdd1ToLineAndColumnNumbers()
    {
        var lineSpan = new FileLinePositionSpan(
            "",
            new LinePosition(2, 2),
            new LinePosition(4, 5));

        var jsonMutantLocation = new Core.Reporters.Json.Location(lineSpan);

        jsonMutantLocation.Start.Line.ShouldBe(3);
        jsonMutantLocation.Start.Column.ShouldBe(3);
        jsonMutantLocation.End.Line.ShouldBe(5);
        jsonMutantLocation.End.Column.ShouldBe(6);
    }

    [TestMethod]
    public void JsonReportFileComponent_ShouldHaveLanguageSet()
    {
        var folderComponent = ReportTestHelper.CreateProjectWith();
        var fileComponent = (CsharpFileLeaf)(folderComponent as CsharpFolderComposite).GetAllFiles().First();

        new SourceFile(fileComponent).Language.ShouldBe("cs");
    }

    [TestMethod]
    public void JsonReportFileComponent_ShouldContainOriginalSource()
    {
        var folderComponent = ReportTestHelper.CreateProjectWith();
        var fileComponent = (CsharpFileLeaf)(folderComponent as CsharpFolderComposite).GetAllFiles().First();

        new SourceFile(fileComponent).Source.ShouldBe(fileComponent.SourceCode);
    }

    [TestMethod]
    public void JsonReportFileComponents_ShouldContainMutants()
    {
        var folderComponent = ReportTestHelper.CreateProjectWith();
        foreach (var file in (folderComponent as CsharpFolderComposite).GetAllFiles())
        {
            var jsonReportComponent = new SourceFile((CsharpFileLeaf)file);
            foreach (var mutant in file.Mutants)
            {
                jsonReportComponent.Mutants.ShouldContain(m => m.Id == mutant.Id.ToString());
            }
        }
    }

    [TestMethod]
    public void JsonReportFileComponent_DoesNotContainDuplicateMutants()
    {
        var loggerMock = Mock.Of<ILogger>();
        var folderComponent = ReportTestHelper.CreateProjectWith(duplicateMutant: true);
        foreach (var file in (folderComponent as CsharpFolderComposite).GetAllFiles())
        {
            var jsonReportComponent = new SourceFile((CsharpFileLeaf)file, loggerMock);
            foreach (var mutant in file.Mutants)
            {
                jsonReportComponent.Mutants.ShouldContain(m => m.Id == mutant.Id.ToString());
            }
        }
    }

    [TestMethod]
    public void JsonReport_ThresholdsAreSet()
    {
        var folderComponent = ReportTestHelper.CreateProjectWith();

        var report = JsonReport.Build(new StrykerOptions(), folderComponent, It.IsAny<TestProjectsInfo>());

        report.ShouldSatisfyAllConditions(
            () => report.Thresholds.ShouldContainKey("high"),
            () => report.Thresholds.ShouldContainKey("low"));
    }

    [TestMethod]
    public void JsonReport_ShouldContainAtLeastOneFile()
    {
        var folderComponent = ReportTestHelper.CreateProjectWith();

        var report = JsonReport.Build(new StrykerOptions(), folderComponent, It.IsAny<TestProjectsInfo>());

        report.Files.Count.ShouldBeGreaterThan(0);
    }

    [TestMethod]
    public void JsonReport_ShouldContainTheProjectRoot()
    {
        var folderComponent = ReportTestHelper.CreateProjectWith();

        var report = JsonReport.Build(new StrykerOptions(), folderComponent, It.IsAny<TestProjectsInfo>());

        report.ProjectRoot.ShouldBe("/home/user/src/project/");
    }

    [TestMethod]
    public void JsonReport_ShouldContainFullPath()
    {
        var folderComponent = ReportTestHelper.CreateProjectWith(root: RuntimeInformation.IsOSPlatform(OSPlatform.Windows) ? "c://" : "/");

        var report = JsonReport.Build(new StrykerOptions(), folderComponent, It.IsAny<TestProjectsInfo>());
        var path = report.Files.Keys.First();

        Path.IsPathFullyQualified(path).ShouldBeTrue($"{path} should not be a relative path");
    }

    [TestMethod]
    public void JsonReporter_OnAllMutantsTestedShouldWriteJsonToFile()
    {
        // arrange
        var mockFileSystem = new MockFileSystem();
        var options = new StrykerOptions
        {
            Thresholds = new Thresholds { High = 80, Low = 60, Break = 0 },
            OutputPath = Directory.GetCurrentDirectory(),
            ReportFileName = "mutation-report"
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

        var reporter = new JsonReporter(options, mockFileSystem);

        // act
        reporter.OnAllMutantsTested(ReportTestHelper.CreateProjectWith(), testProjectsInfo);

        // assert
        var reportPath = Path.Combine(options.ReportPath, "mutation-report.json");
        mockFileSystem.FileExists(reportPath).ShouldBeTrue($"Path {reportPath} should exist but it does not.");
        var fileContents = mockFileSystem.File.OpenRead(reportPath);

        var report = JsonReportSerialization.DeserializeJsonReportAsync(fileContents).Result;

        report.ShouldNotBeNull();
        report.Thresholds.ShouldContainKeyAndValue("high", 80);
        report.Thresholds.ShouldContainKeyAndValue("low", 60);

        var testFile = report.TestFiles.ShouldHaveSingleItem();
        testFile.Key.ShouldBe(_testFilePath);
        testFile.Value.Language.ShouldBe("cs");
        testFile.Value.Source.ShouldBe(_testFileContents);

        var test = testFile.Value.Tests.ShouldHaveSingleItem();
        test.Name.ShouldBe("myUnitTestName");
        test.Id.ShouldBe(Guid.Empty.ToString());
        test.Location.Start.Line.ShouldBe(7);
        test.Location.End.Line.ShouldBe(11);
    }

    [TestMethod]
    public void JsonReporter_ShouldSupportDuplicateTestFiles()
    {
        // arrange
        var mockFileSystem = new MockFileSystem();
        var options = new StrykerOptions
        {
            Thresholds = new Thresholds { High = 80, Low = 60, Break = 0 },
            OutputPath = Directory.GetCurrentDirectory(),
            ReportFileName = "mutation-report"
        };

        var testProjectsInfo = new TestProjectsInfo(_fileSystemMock)
        {
            TestProjects = new List<TestProject>
            {
                new(_fileSystemMock, TestHelper.SetupProjectAnalyzerResult(
                    sourceFiles: new[] { _testFilePath }).Object),
                new(_fileSystemMock, TestHelper.SetupProjectAnalyzerResult(sourceFiles: new[] { _testFilePath }).Object)
            }
        };
        var node = CSharpSyntaxTree.ParseText(_testFileContents).GetRoot().DescendantNodes().OfType<MethodDeclarationSyntax>().Single();
        testProjectsInfo.TestProjects.First().TestFiles.First().AddTest("id", "myUnitTestName", node);
        testProjectsInfo.TestProjects.First().TestFiles.First().AddTest(Guid.NewGuid().ToString(), "myOtherTestName", node);
        testProjectsInfo.TestProjects.ElementAt(1).TestFiles.First().AddTest("id", "myUnitTestName", node);
        testProjectsInfo.TestProjects.ElementAt(1).TestFiles.First().AddTest(Guid.NewGuid().ToString(), "myLastTestName", node);

        var reporter = new JsonReporter(options, mockFileSystem);

        // act
        reporter.OnAllMutantsTested(ReportTestHelper.CreateProjectWith(), testProjectsInfo);

        // assert
        var reportPath = Path.Combine(options.ReportPath, "mutation-report.json");
        mockFileSystem.FileExists(reportPath).ShouldBeTrue($"Path {reportPath} should exist but it does not.");
        var fileContents = mockFileSystem.File.OpenRead(reportPath);

        var report = JsonReportSerialization.DeserializeJsonReportAsync(fileContents).Result;

        var testFile = report.TestFiles.ShouldHaveSingleItem();

        testFile.Value.Tests.Count().ShouldBe(2); // not three
    }
}
