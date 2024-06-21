using System.Collections.Generic;
using System.Reflection;
using Microsoft.Extensions.Logging;
using Moq;
using Shouldly;
using Stryker.Core.Initialisation.Buildalyzer;
using Xunit;

namespace Stryker.Core.UnitTest.ToolHelpers;

public class BuildalyzerHelperTests : TestBase
{
    [Fact]
    public void ShouldGetAssemblyAttributeFileName()
    {
        var projectAnalyzerResult = TestHelper.SetupProjectAnalyzerResult(
            properties: new Dictionary<string, string>() {
                { "GeneratedAssemblyInfoFile", "path.AssemblyInfo.cs" }
            }).Object;

        var result = projectAnalyzerResult.AssemblyAttributeFileName();

        result.ShouldBe("path.AssemblyInfo.cs");
    }

    [Fact]
    public void ShouldGetAssemblyAttributeFileNameDefault()
    {
        var projectAnalyzerResult = TestHelper.SetupProjectAnalyzerResult(
            properties: new(),
            projectFilePath: "path").Object;

        var result = projectAnalyzerResult.AssemblyAttributeFileName();

        result.ShouldBe("path.assemblyinfo.cs");
    }

    [Fact]
    public void ShouldLogAnalyzerLoadGeneralFailure()
    {
        var projectAnalyzerResult = TestHelper.SetupProjectAnalyzerResult(
            properties: new(),
            projectFilePath: "path", analyzers: ["1", "2"]).Object;

        var logger = new Mock<ILogger>(MockBehavior.Loose);

        projectAnalyzerResult.GetSourceGenerators(logger.Object).ShouldBeEmpty();
    }

    [Fact]
    public void ShouldLogAnalyzerLoadFailureWhenNoAnalyzer()
    {
        var projectAnalyzerResult = TestHelper.SetupProjectAnalyzerResult(
            properties: new(),
            projectFilePath: "path", analyzers: [Assembly.GetExecutingAssembly().Location]).Object;

        var logger = new Mock<ILogger>(MockBehavior.Loose);

        projectAnalyzerResult.GetSourceGenerators(logger.Object).ShouldBeEmpty();
    }
}
