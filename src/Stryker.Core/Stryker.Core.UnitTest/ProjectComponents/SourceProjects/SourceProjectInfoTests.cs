using System.Collections.Generic;
using System.IO.Abstractions.TestingHelpers;
using System.Linq;
using Buildalyzer;
using Microsoft.CodeAnalysis;
using Shouldly;
using Stryker.Core.Initialisation.Buildalyzer;
using Stryker.Core.ProjectComponents.SourceProjects;
using Xunit;

namespace Stryker.Core.UnitTest.ProjectComponents.SourceProjects
{
    public class SourceProjectInfoTests : TestBase
    {
        [Fact]
        public void ShouldGenerateProperDefaultCompilationOptions()
        {
            var target = new SourceProjectInfo()
            {
                AnalyzerResult = TestHelper.SetupProjectAnalyzerResult(
                    properties: new Dictionary<string, string>() {
                        { "TargetDir", "/test/bin/Debug/" },
                        { "TargetFileName", "TestName.dll" },
                        { "AssemblyName", "AssemblyName" }
                    }).Object
            };

            var options = target.AnalyzerResult.GetCompilationOptions();

            options.AllowUnsafe.ShouldBe(true);
            options.OutputKind.ShouldBe(OutputKind.DynamicallyLinkedLibrary);
            options.NullableContextOptions.ShouldBe(NullableContextOptions.Enable);
        }

        [Theory]
        [InlineData("Exe", OutputKind.ConsoleApplication)]
        [InlineData("WinExe", OutputKind.WindowsApplication)]
        [InlineData("AppContainerExe", OutputKind.WindowsRuntimeApplication)]
        public void ShouldGenerateProperCompilationOptions(string kindParam, OutputKind output)
        {
            var target = new SourceProjectInfo()
            {
                AnalyzerResult = TestHelper.SetupProjectAnalyzerResult(
                    properties: new Dictionary<string, string>() {
                        { "AssemblyTitle", "TargetFileName"},
                        { "TargetDir", "/test/bin/Debug/" },
                        { "TargetFileName", "TargetFileName.dll"},
                        { "OutputType", kindParam },
                        { "Nullable", "Annotations" },
                        { "AssemblyName", "AssemblyName" }
                    }).Object
            };

            var options = target.AnalyzerResult.GetCompilationOptions();

            options.AllowUnsafe.ShouldBe(true);
            options.OutputKind.ShouldBe(output);
            options.NullableContextOptions.ShouldBe(NullableContextOptions.Annotations);
        }
    }
}
