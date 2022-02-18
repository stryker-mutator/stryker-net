using System.Collections.Generic;
using Moq;
using Stryker.Core.Initialisation.ProjectAnalyzer;

namespace Stryker.Core.UnitTest
{
    public static class TestHelper
    {
        public static Mock<IAnalysisResult> SetupProjectAnalyzerResult(Dictionary<string, string> properties = null,
            string projectFilePath = null,
            string[] sourceFiles = null,
            IEnumerable<string> projectReferences = null,
            string targetFramework = null,
            string[] references = null)
        {
            var analyzerResultMock = new Mock<IAnalysisResult>();

            if (properties != null)
            {
                analyzerResultMock.Setup(x => x.Properties).Returns(properties);
            }
            if (projectFilePath != null)
            {
                analyzerResultMock.Setup(x => x.ProjectFilePath).Returns(projectFilePath);
            }
            if (sourceFiles != null)
            {
                analyzerResultMock.Setup(x => x.SourceFiles).Returns(sourceFiles);
            }
            if (projectReferences != null)
            {
                analyzerResultMock.Setup(x => x.ProjectReferences).Returns(projectReferences);
            }
            if (targetFramework != null)
            {
                analyzerResultMock.Setup(x => x.TargetFramework).Returns(targetFramework);
            }
            if (references != null)
            {
                analyzerResultMock.Setup(x => x.References).Returns(references);
            }

            return analyzerResultMock;
        }

        public static Mock<Buildalyzer.IAnalyzerResult> SetupProjectBuildalyzerResult(Dictionary<string, string> properties = null,
            string projectFilePath = null,
            string[] sourceFiles = null,
            IEnumerable<string> projectReferences = null,
            string targetFramework = null,
            string[] references = null)
        {
            var analyzerResultMock = new Mock<Buildalyzer.IAnalyzerResult>();

            if (properties != null)
            {
                analyzerResultMock.Setup(x => x.Properties).Returns(properties);
            }
            if (projectFilePath != null)
            {
                analyzerResultMock.Setup(x => x.ProjectFilePath).Returns(projectFilePath);
            }
            if (sourceFiles != null)
            {
                analyzerResultMock.Setup(x => x.SourceFiles).Returns(sourceFiles);
            }
            if (projectReferences != null)
            {
                analyzerResultMock.Setup(x => x.ProjectReferences).Returns(projectReferences);
            }
            if (targetFramework != null)
            {
                analyzerResultMock.Setup(x => x.TargetFramework).Returns(targetFramework);
            }
            if (references != null)
            {
                analyzerResultMock.Setup(x => x.References).Returns(references);
            }

            return analyzerResultMock;
        }

    }
}
