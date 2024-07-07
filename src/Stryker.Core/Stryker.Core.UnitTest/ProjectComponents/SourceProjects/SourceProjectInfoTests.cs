using System.Collections.Generic;
using Microsoft.CodeAnalysis;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Shouldly;
using Stryker.Core.Initialisation.Buildalyzer;
using Stryker.Core.ProjectComponents.SourceProjects;

namespace Stryker.Core.UnitTest.ProjectComponents.SourceProjects
{
    [TestClass]
    public class SourceProjectInfoTests : TestBase
    {
        [TestMethod]
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

        [TestMethod]
        [DataRow("Exe", OutputKind.ConsoleApplication)]
        [DataRow("WinExe", OutputKind.WindowsApplication)]
        [DataRow("AppContainerExe", OutputKind.WindowsRuntimeApplication)]
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
