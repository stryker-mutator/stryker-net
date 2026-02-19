using System.Collections.Generic;
using Buildalyzer;
using Microsoft.CodeAnalysis;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Shouldly;
using Stryker.Utilities.Buildalyzer;

namespace Stryker.Core.UnitTest.Initialisation.Buildalyzer;

[TestClass]
public class AnalyzerResultExtensionsTests
{
    [TestMethod]
    public void GetDiagnosticOptions_WithNoWarn()
    {
        // Arrange
        var analyzerResult = Mock.Of<IAnalyzerResult>();
        Mock.Get(analyzerResult)
            .SetupGet(g => g.Properties)
            .Returns(new Dictionary<string, string> { { "NoWarn", "EXTEXP0001;EXTEXP0002" } });

        // Act
        var diagOptions = analyzerResult.GetDiagnosticOptions();

        // Assert
        diagOptions.ShouldContain(new KeyValuePair<string, ReportDiagnostic>("EXTEXP0001", ReportDiagnostic.Suppress));
        diagOptions.ShouldContain(new KeyValuePair<string, ReportDiagnostic>("EXTEXP0002", ReportDiagnostic.Suppress));
    }

    [TestMethod]
    public void GetDiagnosticOptions_WithWarningsAsErrors()
    {
        // Arrange
        var analyzerResult = Mock.Of<IAnalyzerResult>();
        Mock.Get(analyzerResult)
            .SetupGet(g => g.Properties)
            .Returns(new Dictionary<string, string> { { "WarningsAsErrors", "EXTEXP0001;EXTEXP0002" } });

        // Act
        var diagOptions = analyzerResult.GetDiagnosticOptions();

        // Assert
        diagOptions.ShouldContain(new KeyValuePair<string, ReportDiagnostic>("EXTEXP0001", ReportDiagnostic.Error));
        diagOptions.ShouldContain(new KeyValuePair<string, ReportDiagnostic>("EXTEXP0002", ReportDiagnostic.Error));
    }

    [TestMethod]
    public void GetDiagnosticOptions_WithWarningsNotAsErrors()
    {
        // Arrange
        var analyzerResult = Mock.Of<IAnalyzerResult>();
        Mock.Get(analyzerResult)
            .SetupGet(g => g.Properties)
            .Returns(new Dictionary<string, string> { { "WarningsNotAsErrors", "EXTEXP0001;\r\n    EXTEXP0002;EXTEXP0003  \r\n;  \r\n EXTEXP0004 \r\n  " } });

        // Act
        var diagOptions = analyzerResult.GetDiagnosticOptions();

        // Assert
        diagOptions.ShouldContain(new KeyValuePair<string, ReportDiagnostic>("EXTEXP0001", ReportDiagnostic.Warn));
        diagOptions.ShouldContain(new KeyValuePair<string, ReportDiagnostic>("EXTEXP0002", ReportDiagnostic.Warn));
        diagOptions.ShouldContain(new KeyValuePair<string, ReportDiagnostic>("EXTEXP0003", ReportDiagnostic.Warn));
        diagOptions.ShouldContain(new KeyValuePair<string, ReportDiagnostic>("EXTEXP0004", ReportDiagnostic.Warn));
    }

    [TestMethod]
    [DataRow("NoWarn", ReportDiagnostic.Suppress)]
    [DataRow("WarningsAsErrors", ReportDiagnostic.Error)]
    [DataRow("WarningsNotAsErrors", ReportDiagnostic.Warn)]
    public void GetDiagnosticOptions_DealWithDuplicate(string property, ReportDiagnostic reportDiagnostic)
    {
        // Arrange
        var analyzerResult = Mock.Of<IAnalyzerResult>();
        Mock.Get(analyzerResult)
            .SetupGet(g => g.Properties)
            .Returns(new Dictionary<string, string> { { property, "EXTEX0001;EXTEX0001" } });

        // Act
        var diagOptions = IAnalyzerResultExtensions.GetDiagnosticOptions(analyzerResult);

        // Assert
        diagOptions.ShouldHaveSingleItem();
        diagOptions.ShouldContain(new KeyValuePair<string, ReportDiagnostic>("EXTEX0001", reportDiagnostic));
    }

    [TestMethod]
    public void GetDiagnosticOptions_DealWithDuplicateConflicts()
    {
        // Arrange
        var analyzerResult = Mock.Of<IAnalyzerResult>();
        Mock.Get(analyzerResult)
            .SetupGet(g => g.Properties)
            .Returns(new Dictionary<string, string> { { "NoWarn", "EXTEX0001;EXTEX0001" },{ "WarningsAsErrors", "EXTEX0001;EXTEX0001" },
                { "WarningsNotAsErrors", "EXTEX0001;EXTEX0001" }});

        // Act
        var diagOptions = IAnalyzerResultExtensions.GetDiagnosticOptions(analyzerResult);

        // Assert
        diagOptions.ShouldHaveSingleItem();
        diagOptions.ShouldContain(new KeyValuePair<string, ReportDiagnostic>("EXTEX0001", ReportDiagnostic.Suppress));
    }


    [TestMethod]
    public void GetDiagnosticOptions_DealWithConflicts()
    {
        // Arrange
        var analyzerResult = Mock.Of<IAnalyzerResult>();
        Mock.Get(analyzerResult)
            .SetupGet(g => g.Properties)
            .Returns(new Dictionary<string, string> { { "WarningsNotAsErrors", "EXTEXP0001;EXTEXP0002" },{ "WarningsAsErrors", "EXTEXP0002" },
                { "NoWarn", "EXTEXP0001" }});

        // Act
        var diagOptions = analyzerResult.GetDiagnosticOptions();

        // Assert
        diagOptions.ShouldContain(new KeyValuePair<string, ReportDiagnostic>("EXTEXP0001", ReportDiagnostic.Suppress));
        diagOptions.ShouldContain(new KeyValuePair<string, ReportDiagnostic>("EXTEXP0002", ReportDiagnostic.Warn));
    }

    [TestMethod]
    [DataRow("NoWarn", ReportDiagnostic.Suppress)]
    [DataRow("WarningsAsErrors", ReportDiagnostic.Error)]
    [DataRow("WarningsNotAsErrors", ReportDiagnostic.Warn)]
    public void GetDiagnosticOptions_DealWithSpecialCharacters(string property, ReportDiagnostic reportDiagnostic)
    {
        // Arrange
        var analyzerResult = Mock.Of<IAnalyzerResult>();
        Mock.Get(analyzerResult)
            .SetupGet(g => g.Properties)
            .Returns(new Dictionary<string, string> { { property, "EXTEXP0001;\r\n    EXTEXP0002;EXTEXP0003  \r\n;  \r\n EXTEXP0004 \n  ; EXTEXP0004  " } });

        // Act
        var diagOptions = IAnalyzerResultExtensions.GetDiagnosticOptions(analyzerResult);

        // Assert
        diagOptions.Count.ShouldBe(4);
        diagOptions.ShouldContain(new KeyValuePair<string, ReportDiagnostic>("EXTEXP0001", reportDiagnostic));
        diagOptions.ShouldContain(new KeyValuePair<string, ReportDiagnostic>("EXTEXP0002", reportDiagnostic));
        diagOptions.ShouldContain(new KeyValuePair<string, ReportDiagnostic>("EXTEXP0003", reportDiagnostic));
        diagOptions.ShouldContain(new KeyValuePair<string, ReportDiagnostic>("EXTEXP0004", reportDiagnostic));
    }

    [TestMethod]
    [DataRow("IsTestProject", "true", true)]
    [DataRow("IsTestProject", "True", true)]
    [DataRow("IsTestProject", "false", false)]
    [DataRow("IsTestProject", "False", false)]
    [DataRow("IsTestProject", "NotABoolean", false)]
    [DataRow("IsTestProject", null, false)]
    [DataRow("IsTestingPlatformApplication", "true", true)]
    [DataRow("IsTestingPlatformApplication", "True", true)]
    [DataRow("IsTestingPlatformApplication", "false", false)]
    [DataRow("IsTestingPlatformApplication", "False", false)]
    [DataRow("IsTestingPlatformApplication", "NotABoolean", false)]
    [DataRow("IsTestingPlatformApplication", null, false)]
    public void IsTestProject(string property, string value, bool expected)
    {
        // Arrange
        var analyzerResult = Mock.Of<IAnalyzerResult>();
        Mock.Get(analyzerResult)
            .SetupGet(g => g.Properties)
            .Returns(value == null ? [] : new Dictionary<string, string> { { property, value } });
        Mock.Get(analyzerResult)
            .SetupGet(g => g.PackageReferences)
            .Returns(new Dictionary<string, IReadOnlyDictionary<string, string>>());

        // Act
        var isTestProject = IAnalyzerResultExtensions.IsTestProject([analyzerResult]);

        // Assert
        isTestProject.ShouldBe(expected);
    }
}
