using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Shouldly;
using Stryker.Utilities.Buildalyzer;

namespace Stryker.Core.UnitTest.ToolHelpers;

[TestClass]
public class BuildalyzerHelperTests : TestBase
{
    [TestMethod]
    public void ShouldGetAssemblyAttributeFileName()
    {
        var projectAnalyzerResult = TestHelper.SetupProjectAnalyzerResult(
            properties: new Dictionary<string, string>() {
                { "GeneratedAssemblyInfoFile", "path.AssemblyInfo.cs" }
            }).Object;

        var result = projectAnalyzerResult.AssemblyAttributeFileName();

        result.ShouldBe("path.AssemblyInfo.cs");
    }

    [TestMethod]
    public void ShouldGetAssemblyAttributeFileNameDefault()
    {
        var projectAnalyzerResult = TestHelper.SetupProjectAnalyzerResult(
            properties: new(),
            projectFilePath: "path").Object;

        var result = projectAnalyzerResult.AssemblyAttributeFileName();

        result.ShouldBe("path.assemblyinfo.cs");
    }

    [TestMethod]
    public void ShouldHandleAdditionalFiles()
    {
        var path = Path.GetFullPath(Path.Combine("TestResources","ExampleSourceFile.cs"));
        var setupProjectAnalyzerResult = TestHelper.SetupProjectAnalyzerResult(
            properties: new Dictionary<string, string>(),
            projectFilePath: "path");
        setupProjectAnalyzerResult.Setup(x => x.AdditionalFiles).Returns([path]);
        var projectAnalyzerResult = setupProjectAnalyzerResult.Object;

        var result = projectAnalyzerResult.GetAdditionalTexts();

        // check we have a single additional text
        result.Where(x => x.Path == path).ShouldHaveSingleItem();
        var additionalText = result.Where(x => x.Path == path).Single();
        var fileContent = File.ReadAllText(path);
        // which content the provided text
        additionalText.GetText().ToString().ShouldBe(fileContent);
    }

    [TestMethod]
    public void ShouldLogAnalyzerLoadGeneralFailure()
    {
        var projectAnalyzerResult = TestHelper.SetupProjectAnalyzerResult(
            properties: new Dictionary<string, string>(),
            projectFilePath: "path", analyzers: ["1", "2"]).Object;

        var logger = new Mock<ILogger>(MockBehavior.Loose);

        projectAnalyzerResult.GetSourceGenerators(logger.Object).ShouldBeEmpty();
    }

    [TestMethod]
    public void ShouldLogAnalyzerLoadFailureWhenNoAnalyzer()
    {
        var projectAnalyzerResult = TestHelper.SetupProjectAnalyzerResult(
            properties: new Dictionary<string, string>(),
            projectFilePath: "path", analyzers: [Assembly.GetExecutingAssembly().Location]).Object;

        var logger = new Mock<ILogger>(MockBehavior.Loose);

        projectAnalyzerResult.GetSourceGenerators(logger.Object).ShouldBeEmpty();
    }
}
