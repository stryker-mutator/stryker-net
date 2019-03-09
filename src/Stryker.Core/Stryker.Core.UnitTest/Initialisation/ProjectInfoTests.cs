using Shouldly;
using Stryker.Core.Initialisation;
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
                ProjectUnderTestAnalyzerResult = new ProjectAnalyzerResult(null, null)
                {
                    AssemblyPath = "/bin/Debug/netcoreapp2.1/TestApp.dll",
                }
            };

            string expectedPath = "/bin/Debug/netcoreapp2.1/TestApp.dll";
            target.GetInjectionPath().ShouldBe(expectedPath);
        }

        [Fact]
        public void ShouldGenerateInjectionPath_FullFramework()
        {
            var target = new ProjectInfo()
            {
                ProjectUnderTestAnalyzerResult = new ProjectAnalyzerResult(null, null)
                {
                    AssemblyPath = "/bin/Debug/TestApp.dll",
                }
            };

            string expectedPath = "/bin/Debug/TestApp.dll";
            target.GetInjectionPath().ShouldBe(expectedPath);
        }
    }
}
