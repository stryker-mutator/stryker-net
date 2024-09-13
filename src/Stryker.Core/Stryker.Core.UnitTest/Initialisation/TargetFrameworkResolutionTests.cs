/*
using System.Collections.Generic;
using System.Linq;
using Buildalyzer;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Shouldly;
using Stryker.Core.Exceptions;
using Stryker.Core.Testing;

namespace Stryker.Core.UnitTest.Initialisation;

[TestClass]
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

        analyzerManagerMock.Setup( am => am.SetGlobalProperty(It.IsAny<string>(), It.IsAny<string>()));
        projectAnalyzerMock
            .Setup(m => m.Build(It.IsAny<string[]>()))
            .Returns(analyzerResultsMock.Object);

        analyzerResultsMock
            .Setup(m => m.GetEnumerator())
            .Returns(() => _analyzerResults.GetEnumerator());
        buildalyzerProviderMock.Setup(x => x.Provide(It.IsAny<string>(), It.IsAny<AnalyzerManagerOptions>())).Returns(analyzerManagerMock.Object);

        _projectFileReader = new ProjectFileReader(null, buildalyzerProviderMock.Object);
    }

    [TestMethod]
    public void ThrowsIfNoResultsWithFrameworks()
    {
        var analyzerResultFrameworkXMock = new Mock<IAnalyzerResult>();
        analyzerResultFrameworkXMock.Setup(m => m.Succeeded).Returns(true);
        analyzerResultFrameworkXMock.Setup(m => m.TargetFramework).Returns((string)null);
        _analyzerResults = new[]
        {
            analyzerResultFrameworkXMock.Object,
        };

        var analyzeProject = () => _projectFileReader.AnalyzeProject(null, null, null, null);
        analyzeProject.ShouldThrow<InputException>();
    }

    [TestMethod]
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

        var result = _projectFileReader.AnalyzeProject(null, null, null, null); 
        result.TargetFramework.ShouldBe("X");
    }

    [TestMethod]
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

        var result = _projectFileReader.AnalyzeProject(null, null, "Y", null);
        result.TargetFramework.ShouldBe("Y");
    }

    [TestMethod]
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

        var result = _projectFileReader.AnalyzeProject(null, null, "Z", null);
        result.TargetFramework.ShouldBe("X");
    }

    [TestMethod]
    public void SelectsSpecifiedConfiguration()
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

        var result = _projectFileReader.AnalyzeProject(null, null, null, "Release");
        result.TargetFramework.ShouldBe("X");
    }
}
*/
