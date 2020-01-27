using Shouldly;
using Stryker.Core.Initialisation;
using System.Collections.Generic;
using System.Linq;
using Xunit;

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

            string expectedPath = FilePathUtils.NormalizePathSeparators("/test/bin/Debug/AppToTest.dll");
            target.GetInjectionPath(target.TestProjectAnalyzerResults.FirstOrDefault()).ShouldBe(expectedPath);
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
