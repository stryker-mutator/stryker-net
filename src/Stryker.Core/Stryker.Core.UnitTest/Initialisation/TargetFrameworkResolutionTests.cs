using System;
using System.Collections.Generic;
using System.Linq;
using Buildalyzer;
using Moq;
using Shouldly;
using Stryker.Core.Exceptions;
using Stryker.Core.Initialisation;
using Stryker.Core.Testing;
using Xunit;

namespace Stryker.Core.UnitTest.Initialisation
{
    public class TargetFrameworkResolutionTests : TestBase
    {
        private IEnumerable<IAnalyzerResult> _analyzerResults = Enumerable.Empty<IAnalyzerResult>();
        private readonly ProjectFileReader _projectFileReader;

        public TargetFrameworkResolutionTests()
        {
            var analyzerManagerMock = new Mock<IAnalyzerManager>(MockBehavior.Strict);
            var projectAnalyzerMock = new Mock<IProjectAnalyzer>();
            var analyzerResultsMock = new Mock<IAnalyzerResults>();
            var buildalyzerProviderMock = new Mock<IBuildalyzerProvider>(MockBehavior.Strict);
            analyzerManagerMock
                .Setup(m => m.GetProject(It.IsAny<string>()))
                .Returns(projectAnalyzerMock.Object);

            projectAnalyzerMock
                .Setup(m => m.Build())
                .Returns(analyzerResultsMock.Object);

            analyzerResultsMock
                .Setup(m => m.GetEnumerator())
                .Returns(() => _analyzerResults.GetEnumerator());
            buildalyzerProviderMock.Setup(x => x.Provide(It.IsAny<string>(), It.IsAny<AnalyzerManagerOptions>())).Returns(analyzerManagerMock.Object);

            _projectFileReader = new ProjectFileReader(null, buildalyzerProviderMock.Object);
        }

        [Fact]
        public void ThrowsIfNoResultsWithFrameworks()
        {
            var analyzerResultFrameworkXMock = new Mock<IAnalyzerResult>();
            analyzerResultFrameworkXMock.Setup(m => m.Succeeded).Returns(true);
            analyzerResultFrameworkXMock.Setup(m => m.TargetFramework).Returns((string)null);
            _analyzerResults = new[]
            {
                analyzerResultFrameworkXMock.Object,
            };

            Func<IAnalyzerResult> analyzeProject = () => _projectFileReader.AnalyzeProject(null, null, null);
            analyzeProject.ShouldThrow<InputException>();
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

            var result = _projectFileReader.AnalyzeProject(null, null, null);
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

            var result = _projectFileReader.AnalyzeProject(null, null, "Y");
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

            var result = _projectFileReader.AnalyzeProject(null, null, "Z");
            result.TargetFramework.ShouldBe("X");
        }
    }
}
