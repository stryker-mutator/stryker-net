using Shouldly;
using Stryker.Core.Initialisation;
using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using Xunit;
using Xunit.Sdk;

namespace Stryker.Core.UnitTest.Initialisation
{
    public class ProjectInfoTests
    {
        [Fact]
        public void ShouldGenerateInjectionPath()
        {
            var target = new ProjectInfo()
            {
                TestProjectAnalyzerResults = new List<ProjectAnalyzerResult> {
                    new ProjectAnalyzerResult(null, null)
                    {
                        AssemblyPath = "/test/bin/Debug/TestApp.dll",
                    }
                },
                ProjectUnderTestAnalyzerResult = new ProjectAnalyzerResult(null, null)
                {
                    AssemblyPath = "/app/bin/Debug/AppToTest.dll",
                }
            };

            var expectedPath = FilePathUtils.NormalizePathSeparators("/test/bin/Debug/AppToTest.dll");
            target.GetInjectionPath(target.TestProjectAnalyzerResults.FirstOrDefault()).ShouldBe(expectedPath);
        }
        
        [Fact]
        public void ShouldGenerateProperDefaultCompilationOptions()
        {
            var target = new ProjectInfo()
            {
                TestProjectAnalyzerResults = new List<ProjectAnalyzerResult> {
                    new ProjectAnalyzerResult(null, null)
                    {
                        AssemblyPath = "/test/bin/Debug/TestApp.dll",
                    }
                },
                ProjectUnderTestAnalyzerResult = new ProjectAnalyzerResult(null, null)
                {
                    Properties = new Dictionary<string, string>()
                    {
                        { "AssemblyTitle", "TargetFileName"},
                        { "TargetFileName", "TargetFileName.dll"},
                    },
                    AssemblyPath = "/app/bin/Debug/AppToTest.dll",
                }
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
                TestProjectAnalyzerResults = new List<ProjectAnalyzerResult> {
                    new ProjectAnalyzerResult(null, null)
                    {
                        AssemblyPath = "/test/bin/Debug/TestApp.dll",
                    }
                },
                ProjectUnderTestAnalyzerResult = new ProjectAnalyzerResult(null, null)
                {
                    Properties = new Dictionary<string, string>
                    {
                        { "AssemblyTitle", "TargetFileName"},
                        { "TargetFileName", "TargetFileName.dll"},
                        { "OutputType", kindParam },
                        { "Nullable", "Annotations" }
                    },
                    AssemblyPath = "/app/bin/Debug/AppToTest.dll",
                }
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
                TestProjectAnalyzerResults = new List<ProjectAnalyzerResult> {
                    new ProjectAnalyzerResult(null, null)
                    {
                        AssemblyPath = "/test/bin/Debug/TestApp.UnitTest.dll",
                    }
                },
                ProjectUnderTestAnalyzerResult = new ProjectAnalyzerResult(null, null)
                {
                    AssemblyPath = "/app/bin/Debug/TestApp.dll",
                }
            };

            string expectedPath = FilePathUtils.NormalizePathSeparators("/test/bin/Debug/TestApp.dll");
            target.GetInjectionPath(target.TestProjectAnalyzerResults.FirstOrDefault()).ShouldBe(expectedPath);
        }
    }
}
