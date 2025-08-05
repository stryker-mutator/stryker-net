using System.Collections.Generic;
using Microsoft.CodeAnalysis;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Shouldly;
using Stryker.Core.ProjectComponents.SourceProjects;
using Stryker.Utilities.Buildalyzer;

namespace Stryker.Core.UnitTest.ProjectComponents.SourceProjects;

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

    [TestMethod]
    [DataRow(false, false)]
    [DataRow(false, true)]
    [DataRow(true, false)]
    [DataRow(true, true)]
    public void ShouldGenerateProperSigningCompilationOptions(bool signAssembly, bool delaySign)
    {
        var target = new SourceProjectInfo()
        {
            AnalyzerResult = TestHelper.SetupProjectAnalyzerResult(
                properties: new Dictionary<string, string>() {
                    { "TargetDir", "/test/bin/Debug/" },
                    { "TargetFileName", "TestName.dll" },
                    { "AssemblyName", "AssemblyName" },
                    { "SignAssembly", signAssembly.ToString() },
                    { "DelaySign", delaySign.ToString() },
                    { "AssemblyOriginatorKeyFile", "test/keyfile.snk" }
                }).Object
        };

        var options = target.AnalyzerResult.GetCompilationOptions();

        options.AllowUnsafe.ShouldBe(true);
        options.OutputKind.ShouldBe(OutputKind.DynamicallyLinkedLibrary);
        options.NullableContextOptions.ShouldBe(NullableContextOptions.Enable);
        if (signAssembly)
        {
            options.CryptoKeyFile.ShouldEndWith("test/keyfile.snk");
        }
        else
        {
            options.CryptoKeyFile.ShouldBeNull();
        }
        options.DelaySign.ShouldBe(signAssembly ? delaySign : null);
    }
}
