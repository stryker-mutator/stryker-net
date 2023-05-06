using System.Collections.Generic;
using Buildalyzer;
using Moq;

namespace Stryker.Core.UnitTest;

public static class TestHelper
{
    public static Mock<IAnalyzerResult> SetupProjectAnalyzerResult(Dictionary<string, string> properties = null,
        string projectFilePath = null,
        string[] sourceFiles = null,
        IEnumerable<string> projectReferences = null,
        string targetFramework = null,
        IReadOnlyDictionary<string, IReadOnlyDictionary<string, string>> packageReferences = null,
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
        if (packageReferences != null)
        {
            analyzerResultMock.Setup(x => x.PackageReferences).Returns(packageReferences);
        }
        if (references != null)
        {
            analyzerResultMock.Setup(x => x.References).Returns(references);
        }

        return analyzerResultMock;
    }
}
