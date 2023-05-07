using System.Collections.Generic;
using System.IO;
using Buildalyzer;
using Moq;

namespace Stryker.Core.UnitTest
{
    public static class TestHelper
    {
        public static Mock<IAnalyzerResult> SetupProjectAnalyzerResult(Dictionary<string, string> properties = null,
            string projectFilePath = null,
            string[] sourceFiles = null,
            IEnumerable<string> projectReferences = null,
            string targetFramework = null,
            IReadOnlyDictionary<string, IReadOnlyDictionary<string, string>> packageReferences = null,
            string[] references = null,
            string[] preprocessorSymbols = null
            )
        {
            var analyzerResultMock = new Mock<IAnalyzerResult>();

            if (properties != null)
            {
                analyzerResultMock.Setup(x => x.Properties).Returns(properties);
            }
            else
            {
                properties = new Dictionary<string, string>();
                analyzerResultMock.Setup(x => x.Properties).Returns(properties);
            }
            if (projectFilePath != null)
            {
                analyzerResultMock.Setup(x => x.ProjectFilePath).Returns(projectFilePath);
                if (!properties.ContainsKey("TargetDir"))
                {
                    properties["TargetDir"] = Path.Combine(Path.GetFullPath(projectFilePath), "bin", "Debug", targetFramework ?? "net");
                    properties["TargetFileName"] = Path.GetFileNameWithoutExtension(projectFilePath)+".dll";
                }
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
            if (preprocessorSymbols is not null)
            {
                analyzerResultMock.Setup(x => x.PreprocessorSymbols).Returns(preprocessorSymbols);
            }

            return analyzerResultMock;
        }
    }
}
