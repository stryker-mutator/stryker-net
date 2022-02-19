using System.Collections.Generic;
using System.Linq;
using Buildalyzer;
using Moq;
using Shouldly;
using Stryker.Core.Initialisation;
using Stryker.Core.Initialisation.ProjectAnalyzer;
using Xunit;
using IProjectAnalyzer = Buildalyzer.IProjectAnalyzer;

namespace Stryker.Core.UnitTest.Initialisation
{
    public class TargetFrameworkResolutionTests : TestBase
    {
        private IEnumerable<IAnalyzerResult> _analyzerResults = Enumerable.Empty<IAnalyzerResult>();
        private readonly BuildalyzerProjectAnalyzer _buildAlyzerProjectAnalyzer;

        public TargetFrameworkResolutionTests()
        {
            var projectAnalyzerMock = new Mock<IProjectAnalyzer>();
            var analyzerResultsMock = new Mock<IAnalyzerResults>();

            projectAnalyzerMock
                .Setup(m => m.Build())
                .Returns(analyzerResultsMock.Object);

            analyzerResultsMock
                .Setup(m => m.GetEnumerator())
                .Returns(() => _analyzerResults.GetEnumerator());

            _buildAlyzerProjectAnalyzer = new BuildalyzerProjectAnalyzer(projectAnalyzer: projectAnalyzerMock.Object);
        }

        [Fact]
        public void SelectsFirstFrameworkIfNoneSpecified()
        {
            var analyzerResultFrameworkXMock = new Mock<IAnalyzerResult>();
            var analyzerResultFrameworkYMock = new Mock<IAnalyzerResult>();

            analyzerResultFrameworkXMock.Setup(m => m.Succeeded).Returns(true);
            analyzerResultFrameworkYMock.Setup(m => m.Succeeded).Returns(true);
            analyzerResultFrameworkXMock.Setup(m => m.TargetFramework).Returns("X");
            analyzerResultFrameworkYMock.Setup(m => m.TargetFramework).Returns("Y");

            _analyzerResults = new[]
            {
                analyzerResultFrameworkXMock.Object,
                analyzerResultFrameworkYMock.Object
            };

            var result = _buildAlyzerProjectAnalyzer.Analyze(null);
            result.TargetFramework.ShouldBe("X");
        }

        [Fact]
        public void SelectsRespectiveFrameworkIfSpecifiedAndAvailable()
        {
            var analyzerResultFrameworkXMock = new Mock<IAnalyzerResult>();
            var analyzerResultFrameworkYMock = new Mock<IAnalyzerResult>();

            analyzerResultFrameworkXMock.Setup(m => m.Succeeded).Returns(true);
            analyzerResultFrameworkYMock.Setup(m => m.Succeeded).Returns(true);
            analyzerResultFrameworkXMock.Setup(m => m.TargetFramework).Returns("X");
            analyzerResultFrameworkYMock.Setup(m => m.TargetFramework).Returns("Y");

            _analyzerResults = new[]
            {
                analyzerResultFrameworkXMock.Object,
                analyzerResultFrameworkYMock.Object
            };

            var result = _buildAlyzerProjectAnalyzer.Analyze("Y");
            result.TargetFramework.ShouldBe("Y");
        }

        [Fact]
        public void SelectsFirstFrameworkIfSpecifiedButNotAvailable()
        {
            var analyzerResultFrameworkXMock = new Mock<IAnalyzerResult>();
            var analyzerResultFrameworkYMock = new Mock<IAnalyzerResult>();

            analyzerResultFrameworkXMock.Setup(m => m.Succeeded).Returns(true);
            analyzerResultFrameworkYMock.Setup(m => m.Succeeded).Returns(true);
            analyzerResultFrameworkXMock.Setup(m => m.TargetFramework).Returns("X");
            analyzerResultFrameworkYMock.Setup(m => m.TargetFramework).Returns("Y");

            _analyzerResults = new[]
            {
                analyzerResultFrameworkXMock.Object,
                analyzerResultFrameworkYMock.Object
            };

            var result = _buildAlyzerProjectAnalyzer.Analyze("Z");
            result.TargetFramework.ShouldBe("X");
        }
    }
}
