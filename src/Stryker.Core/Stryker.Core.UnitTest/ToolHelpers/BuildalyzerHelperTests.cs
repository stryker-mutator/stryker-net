using System.Collections.Generic;
using System.Reflection;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Shouldly;
using Stryker.Configuration.Initialisation.Buildalyzer;

namespace Stryker.Configuration.UnitTest.ToolHelpers;

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
    public void ShouldLogAnalyzerLoadGeneralFailure()
    {
        var projectAnalyzerResult = TestHelper.SetupProjectAnalyzerResult(
            properties: new(),
            projectFilePath: "path", analyzers: ["1", "2"]).Object;

        var logger = new Mock<ILogger>(MockBehavior.Loose);

        projectAnalyzerResult.GetSourceGenerators(logger.Object).ShouldBeEmpty();
    }

    [TestMethod]
    public void ShouldLogAnalyzerLoadFailureWhenNoAnalyzer()
    {
        var projectAnalyzerResult = TestHelper.SetupProjectAnalyzerResult(
            properties: new(),
            projectFilePath: "path", analyzers: [Assembly.GetExecutingAssembly().Location]).Object;

        var logger = new Mock<ILogger>(MockBehavior.Loose);

        projectAnalyzerResult.GetSourceGenerators(logger.Object).ShouldBeEmpty();
    }
}
