using System.Collections.Generic;
using Moq;
using Stryker.Core.Initialisation.SolutionAnalyzer;

namespace Stryker.Core.UnitTest
{
    public static class TestHelper
    {
        public static Mock<IAnalyzerResult> SetupProjectAnalyzerResult(Dictionary<string, string> properties = null,
            string projectFilePath = null,
            string[] sourceFiles = null,
            IEnumerable<string> projectReferences = null,
            string targetFramework = null,
            string[] references = null)
        {
            var analyzerResultMock = new Mock<IAnalyzerResult>();

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
