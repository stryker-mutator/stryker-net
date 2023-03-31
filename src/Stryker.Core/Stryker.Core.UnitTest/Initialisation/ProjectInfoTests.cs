using System.Collections.Generic;
using System.IO;
using System.IO.Abstractions.TestingHelpers;
using System.Linq;
using System.Security.AccessControl;
using Buildalyzer;
using Microsoft.CodeAnalysis;
using Shouldly;
using Stryker.Core.Initialisation;
using Stryker.Core.Initialisation.Buildalyzer;
using Xunit;

namespace Stryker.Core.UnitTest.Initialisation
{
    public class ProjectInfoTests : TestBase
    {
        [Fact]
        public void ShouldGenerateInjectionPath()
        {
            var target = new ProjectInfo(new MockFileSystem())
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
            var target = new ProjectInfo(new MockFileSystem())
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
            var target = new ProjectInfo(new MockFileSystem())
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
            var target = new ProjectInfo(new MockFileSystem())
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

            var expectedPath = FilePathUtils.NormalizePathSeparators("/test/bin/Debug/TestName.dll");
            target.TestProjectAnalyzerResults.FirstOrDefault().GetAssemblyPath().ShouldBe(expectedPath);
        }

        [Fact]
        public void ShouldBackupBinaries()
        {
            var mockFileSystem = new MockFileSystem(new Dictionary<string, MockFileData>
            {
                { Path.Combine( "test", "bin", "Debug",  "AppToTest.dll"), new MockFileData("empty")}
            });
            
            var target = new ProjectInfo(mockFileSystem)
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

            target.BackupOriginalAssembly();
            mockFileSystem.ShouldContainFile(mockFileSystem.Path.Combine("test", "bin", "Debug",  "AppToTest.dll.stryker-unchanged"));
        }

        [Fact]
        public void ShouldRestoreBinaries()
        {
            var mockFileSystem = new MockFileSystem(new Dictionary<string, MockFileData>
            {
                { Path.Combine( "test", "bin", "Debug",  "AppToTest.dll"), new MockFileData("empty")},
                { Path.Combine( "test", "bin", "Debug",  "AppToTest.dll.stryker-unchanged"), new MockFileData("empty")}
            });
            
            var target = new ProjectInfo(mockFileSystem)
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

            target.RestoreOriginalAssembly();
            mockFileSystem.ShouldNotContainFile(mockFileSystem.Path.Combine("test", "bin", "Debug",  "AppToTest.dll.stryker-unchanged"));
        }

        [Fact]
        public void ShouldLogErrorWhenBackupNotFound()
        {
            var mockFileSystem = new MockFileSystem(new Dictionary<string, MockFileData>
            {
                { Path.Combine( "test", "bin", "Debug",  "AppToTest.dll"), new MockFileData("empty")},
            });
            
            var target = new ProjectInfo(mockFileSystem)
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

            target.RestoreOriginalAssembly();
            target.ProjectWarnings.Count.ShouldBe(1);
            target.ProjectWarnings[0].ShouldContain("Failed to restore original assembly");
        }

        [Fact]
        public void ShouldLogErrorWhenCantRestore()
        {
            var mockFileData = new MockFileData("empty")
            {
                Attributes = FileAttributes.ReadOnly
            };
            var mockFileSystem = new MockFileSystem(new Dictionary<string, MockFileData>
            {
                { Path.Combine( "test", "bin", "Debug",  "AppToTest.dll"), mockFileData},
                { Path.Combine( "test", "bin", "Debug",  "AppToTest.dll.stryker-unchanged"), mockFileData},
            });
            
            var target = new ProjectInfo(mockFileSystem)
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

            target.RestoreOriginalAssembly();
            target.ProjectWarnings.Count.ShouldBe(1);
            target.ProjectWarnings[0].ShouldContain("Failed to restore original assembly");
        }
    }
}
