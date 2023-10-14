using System;
using System.Collections.Generic;
using Microsoft.Extensions.Logging;
using Moq;
using Shouldly;
using Stryker.Core.Initialisation.Buildalyzer;
using Xunit;

namespace Stryker.Core.UnitTest.ToolHelpers
{
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
                    properties: new Dictionary<string, string>(),
                    projectFilePath: "path").Object;

            var result = projectAnalyzerResult.AssemblyAttributeFileName();

            result.ShouldBe("path.assemblyinfo.cs");
        }

        [Fact]
        public void ShouldLogAnalyzerLoadFailure()
        {
            var projectAnalyzerResult = TestHelper.SetupProjectAnalyzerResult(
                properties: new Dictionary<string, string>(),
                projectFilePath: "path", analyzers: new []{"1", "2"}).Object;

            var logger = new Mock<ILogger>(MockBehavior.Loose);

            projectAnalyzerResult.GetSourceGenerators(logger.Object).ShouldBeEmpty();

        }

        
    }
}
