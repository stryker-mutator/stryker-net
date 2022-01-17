using System.Collections.Generic;
using System.Linq;
using Buildalyzer;
using Buildalyzer.Environment;
using Moq;
using Shouldly;
using Stryker.Core.Initialisation;
using Xunit;

namespace Stryker.Core.UnitTest.Initialisation
{
    public class TargetFrameworkResolutionTests : TestBase
    {
        private IEnumerable<IAnalyzerResult> _analyzerResults = Enumerable.Empty<IAnalyzerResult>();
        private readonly ProjectFileReader _projectFileReader;

        public TargetFrameworkResolutionTests()
        {
            var analyzerManagerMock = new Mock<IAnalyzerManager>();
            var projectAnalyzerMock = new Mock<IProjectAnalyzer>();
            var analyzerResultsMock = new Mock<IAnalyzerResults>();

            analyzerManagerMock
                .Setup(m => m.GetProject(It.IsAny<string>()))
                .Returns(projectAnalyzerMock.Object);

            projectAnalyzerMock
                .Setup(m => m.Build(It.IsAny<EnvironmentOptions>()))
                .Returns(analyzerResultsMock.Object);

            analyzerResultsMock
                .Setup(m => m.GetEnumerator())
                .Returns(() => _analyzerResults.GetEnumerator());

            _projectFileReader = new ProjectFileReader(manager: analyzerManagerMock.Object);
        }

        [Fact]
        public void SelectsFirstFrameworkIfNoneSpecified()
        {
            var analyzerResultFrameworkXMock = new Mock<IAnalyzerResult>();
            var analyzerResultFrameworkYMock = new Mock<IAnalyzerResult>();

            analyzerResultFrameworkXMock.Setup(m => m.TargetFramework).Returns("X");
            analyzerResultFrameworkYMock.Setup(m => m.TargetFramework).Returns("Y");

            _analyzerResults = new[]
            {
                analyzerResultFrameworkXMock.Object,
                analyzerResultFrameworkYMock.Object
            };

            var result = _projectFileReader.AnalyzeProject(null, null, null);
            result.TargetFramework.ShouldBe("X");
        }

        [Fact]
        public void SelectsRespectiveFrameworkIfSpecifiedAndAvailable()
        {
            var analyzerResultFrameworkXMock = new Mock<IAnalyzerResult>();
            var analyzerResultFrameworkYMock = new Mock<IAnalyzerResult>();

            analyzerResultFrameworkXMock.Setup(m => m.TargetFramework).Returns("X");
            analyzerResultFrameworkYMock.Setup(m => m.TargetFramework).Returns("Y");

            _analyzerResults = new[]
            {
                analyzerResultFrameworkXMock.Object,
                analyzerResultFrameworkYMock.Object
            };

            var result = _projectFileReader.AnalyzeProject(null, null, "Y");
            result.TargetFramework.ShouldBe("Y");
        }

        [Fact]
        public void SelectsFirstFrameworkIfSpecifiedButNotAvailable()
        {
            var analyzerResultFrameworkXMock = new Mock<IAnalyzerResult>();
            var analyzerResultFrameworkYMock = new Mock<IAnalyzerResult>();

            analyzerResultFrameworkXMock.Setup(m => m.TargetFramework).Returns("X");
            analyzerResultFrameworkYMock.Setup(m => m.TargetFramework).Returns("Y");

            _analyzerResults = new[]
            {
                analyzerResultFrameworkXMock.Object,
                analyzerResultFrameworkYMock.Object
            };

            var result = _projectFileReader.AnalyzeProject(null, null, "Z");
            result.TargetFramework.ShouldBe("X");
        }
    }
}
