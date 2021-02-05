using Buildalyzer;
using Shouldly;
using Stryker.Core.Initialisation;
using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using Xunit;
using Xunit.Sdk;
using Stryker.Core.ToolHelpers;
using Stryker.Core.Initialisation.Buildalyzer;

namespace Stryker.Core.UnitTest.Initialisation
{
    public class ProjectInfoTests
    {
        [Fact]
        public void ShouldGenerateInjectionPath()
        {
            var target = new ProjectInfo()
            {
                TestProjectAnalyzerResults = new List<IAnalyzerResult> {
                    TestHelper.SetupProjectAnalyzerResult(
                    properties: new Dictionary<string, string>() {
                        { "TargetDir", "/test/bin/Debug/" },
                        { "TargetFileName", "TestName.dll" }
                    }).Object
                },
                ProjectUnderTestAnalyzerResult = TestHelper.SetupProjectAnalyzerResult(
                    properties: new Dictionary<string, string>() {
                        { "TargetDir", "/app/bin/Debug/" },
                        { "TargetFileName", "AppToTest.dll" }
                    }).Object
            };

            var expectedPath = FilePathUtils.NormalizePathSeparators("/test/bin/Debug/AppToTest.dll");
            target.GetInjectionFilePath(target.TestProjectAnalyzerResults.FirstOrDefault()).ShouldBe(expectedPath);
        }
        
        [Fact]
        public void ShouldGenerateProperDefaultCompilationOptions()
        {
            var target = new ProjectInfo()
            {
                TestProjectAnalyzerResults = new List<IAnalyzerResult> {
                    TestHelper.SetupProjectAnalyzerResult(
                    properties: new Dictionary<string, string>() {
                        { "TargetDir", "/test/bin/Debug/" },
                        { "TargetFileName", "TestName.dll" },
                        { "AssemblyName", "AssemblyName" }
                    }).Object
                },
                ProjectUnderTestAnalyzerResult = TestHelper.SetupProjectAnalyzerResult(
                    properties: new Dictionary<string, string>() {
                        { "TargetDir", "/test/bin/Debug/" },
                        { "TargetFileName", "TestName.dll" },
                        { "AssemblyName", "AssemblyName" }
                    }).Object
            };

            var options = target.ProjectUnderTestAnalyzerResult.GetCompilationOptions();

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
            var target = new ProjectInfo()
            {
                TestProjectAnalyzerResults = new List<IAnalyzerResult> {
                    TestHelper.SetupProjectAnalyzerResult(
                    properties: new Dictionary<string, string>() {
                        { "TargetDir", "/test/bin/Debug/" },
                        { "TargetFileName", "TestName.dll" },
                        { "AssemblyName", "AssemblyName" }
                    }).Object
                },
                ProjectUnderTestAnalyzerResult = TestHelper.SetupProjectAnalyzerResult(
                    properties: new Dictionary<string, string>() {
                        { "AssemblyTitle", "TargetFileName"},
                        { "TargetDir", "/test/bin/Debug/" },
                        { "TargetFileName", "TargetFileName.dll"},
                        { "OutputType", kindParam },
                        { "Nullable", "Annotations" },
                        { "AssemblyName", "AssemblyName" }
                    }).Object
            };

            var options = target.ProjectUnderTestAnalyzerResult.GetCompilationOptions();

            options.AllowUnsafe.ShouldBe(true);
            options.OutputKind.ShouldBe(output);
            options.NullableContextOptions.ShouldBe(NullableContextOptions.Annotations);
        }

        [Fact]
        public void ShouldGenerateTestBinariesPath()
        {
            var target = new ProjectInfo()
            {
                TestProjectAnalyzerResults = new List<IAnalyzerResult> {
                    TestHelper.SetupProjectAnalyzerResult(
                    properties: new Dictionary<string, string>() {
                        { "TargetDir", "/test/bin/Debug/" },
                        { "TargetFileName", "TestName.dll" }
                    }).Object
                },
                ProjectUnderTestAnalyzerResult = TestHelper.SetupProjectAnalyzerResult(
                    properties: new Dictionary<string, string>() {
                        { "TargetDir", "/app/bin/Debug/" },
                        { "TargetFileName", "AppToTest.dll" }
                    }).Object
            };

            string expectedPath = FilePathUtils.NormalizePathSeparators("/test/bin/Debug/TestName.dll");
            target.TestProjectAnalyzerResults.FirstOrDefault().GetAssemblyPath().ShouldBe(expectedPath);
        }
    }
}
