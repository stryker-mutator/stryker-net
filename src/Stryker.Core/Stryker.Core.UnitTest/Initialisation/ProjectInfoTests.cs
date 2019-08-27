﻿using Shouldly;
using Stryker.Core.Initialisation;
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
                TestProjectAnalyzerResult = new ProjectAnalyzerResult(null, null)
                {
                    AssemblyPath = FilePathUtils.ConvertPathSeparators("\\test\\bin\\Debug\\TestApp.dll"),
                },
                ProjectUnderTestAnalyzerResult = new ProjectAnalyzerResult(null, null)
                {
                    AssemblyPath = FilePathUtils.ConvertPathSeparators("\\app\\bin\\Debug\\AppToTest.dll"),
                }
            };

            string expectedPath = FilePathUtils.ConvertPathSeparators("\\test\\bin\\Debug\\AppToTest.dll");
            target.GetInjectionPath().ShouldBe(expectedPath);
        }

        [Fact]
        public void ShouldGenerateTestBinariesPath()
        {
            var target = new ProjectInfo()
            {
                TestProjectAnalyzerResult = new ProjectAnalyzerResult(null, null)
                {
                    AssemblyPath = FilePathUtils.ConvertPathSeparators("\\test\\bin\\Debug\\TestApp.dll"),
                },
                ProjectUnderTestAnalyzerResult = new ProjectAnalyzerResult(null, null)
                {
                    AssemblyPath = FilePathUtils.ConvertPathSeparators("\\app\\bin\\Debug\\AppToTest.dll"),
                }
            };

            string expectedPath = FilePathUtils.ConvertPathSeparators("\\test\\bin\\Debug\\TestApp.dll");
            target.GetTestBinariesPath().ShouldBe(expectedPath);
        }
    }
}
