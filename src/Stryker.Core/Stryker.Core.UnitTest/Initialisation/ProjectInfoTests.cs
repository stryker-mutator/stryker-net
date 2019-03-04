using Shouldly;
using Stryker.Core.Initialisation;
using System.Collections.Generic;
using Xunit;

namespace Stryker.Core.UnitTest.Initialisation
{
    public class ProjectInfoTests
    {
        [Fact]
        public void ShouldGenerateInjectionPath_NetCore()
        {
            var target = new ProjectInfo()
            {
                FullFramework = false,
                ProjectUnderTestAnalyzerResult = new ProjectAnalyzerResult(null, null)
                {
                    Properties = new Dictionary<string, string>()
                    {
                        { "TargetFileName", "TestApp.dll" }
                    }
                },
                TestProjectAnalyzerResult = new ProjectAnalyzerResult(null, null)
                {
                    Properties = new Dictionary<string, string>
                    {
                        { "OutputPath", "bin\\Debug\\netcoreapp2.1" },
                    }
                }
            };

            string expectedPath = FilePathUtils.ConvertPathSeparators("bin\\Debug\\netcoreapp2.1\\TestApp.dll");
            target.GetInjectionPath().ShouldBe(expectedPath);
        }

        [Fact]
        public void ShouldGenerateInjectionPath_FullFramework()
        {
            var target = new ProjectInfo()
            {
                FullFramework = true,
                ProjectUnderTestAnalyzerResult = new ProjectAnalyzerResult(null, null)
                {
                    Properties = new Dictionary<string, string>
                    {
                        { "TargetFileName", "TestApp.dll" }
                    }
                },
                TestProjectAnalyzerResult = new ProjectAnalyzerResult(null, null)
                {
                    Properties = new Dictionary<string, string>
                    {
                        { "MSBuildProjectDirectory", "C:\\ExampleProject" },
                        { "OutputPath", "bin\\Debug" }
                    }
                }
            };

            string expectedPath = FilePathUtils.ConvertPathSeparators("C:\\ExampleProject\\bin\\Debug\\TestApp.dll");
            target.GetInjectionPath().ShouldBe(expectedPath);
        }
    }
}
