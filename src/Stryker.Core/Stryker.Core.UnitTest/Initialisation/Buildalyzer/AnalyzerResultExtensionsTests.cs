using System.Collections.Generic;
using Buildalyzer;
using Microsoft.CodeAnalysis;
using Moq;
using Shouldly;
using Stryker.Core.Initialisation.Buildalyzer;
using Xunit;

namespace Stryker.Core.UnitTest.Initialisation.Buildalyzer;
public class AnalyzerResultExtensionsTests
{
    [Fact]
    public void GetDiagnosticOptions_WithNoWarn()
    {
        // Arrange
        var analyzerResult = Mock.Of<IAnalyzerResult>();
        Mock.Get(analyzerResult)
            .SetupGet(g => g.Properties)
            .Returns(new Dictionary<string, string> { { "NoWarn", "EXTEXP0001;EXTEXP0002" } });

        // Act
        var diagOptions = IAnalyzerResultExtensions.GetDiagnosticOptions(analyzerResult);

        // Assert
        diagOptions.ShouldContain(new KeyValuePair<string, ReportDiagnostic>("EXTEXP0001", ReportDiagnostic.Suppress));
        diagOptions.ShouldContain(new KeyValuePair<string, ReportDiagnostic>("EXTEXP0002", ReportDiagnostic.Suppress));
    }

    [Fact]
    public void GetDiagnosticOptions_WithWarningsAsErrors()
    {
        // Arrange
        var analyzerResult = Mock.Of<IAnalyzerResult>();
        Mock.Get(analyzerResult)
            .SetupGet(g => g.Properties)
            .Returns(new Dictionary<string, string> { { "WarningsAsErrors", "EXTEXP0001;EXTEXP0002" } });

        // Act
        var diagOptions = IAnalyzerResultExtensions.GetDiagnosticOptions(analyzerResult);

        // Assert
        diagOptions.ShouldContain(new KeyValuePair<string, ReportDiagnostic>("EXTEXP0001", ReportDiagnostic.Error));
        diagOptions.ShouldContain(new KeyValuePair<string, ReportDiagnostic>("EXTEXP0002", ReportDiagnostic.Error));
    }

    [Fact]
    public void GetDiagnosticOptions_WithWarningsNotAsErrors()
    {
        // Arrange
        var analyzerResult = Mock.Of<IAnalyzerResult>();
        Mock.Get(analyzerResult)
            .SetupGet(g => g.Properties)
            .Returns(new Dictionary<string, string> { { "WarningsNotAsErrors", "EXTEXP0001;EXTEXP0002" } });

        // Act
        var diagOptions = IAnalyzerResultExtensions.GetDiagnosticOptions(analyzerResult);

        // Assert
        diagOptions.ShouldContain(new KeyValuePair<string, ReportDiagnostic>("EXTEXP0001", ReportDiagnostic.Warn));
        diagOptions.ShouldContain(new KeyValuePair<string, ReportDiagnostic>("EXTEXP0002", ReportDiagnostic.Warn));
    }

    [Fact]
    public void GetDiagnosticOptions_DealWithConflicts()
    {
        // Arrange
        var analyzerResult = Mock.Of<IAnalyzerResult>();
        Mock.Get(analyzerResult)
            .SetupGet(g => g.Properties)
            .Returns(new Dictionary<string, string> { { "WarningsNotAsErrors", "EXTEXP0001;EXTEXP0002" },{ "WarningsAsErrors", "EXTEXP0002" },
                { "NoWarn", "EXTEXP0001" }});

        // Act
        var diagOptions = IAnalyzerResultExtensions.GetDiagnosticOptions(analyzerResult);

        // Assert
        diagOptions.ShouldContain(new KeyValuePair<string, ReportDiagnostic>("EXTEXP0001", ReportDiagnostic.Suppress));
        diagOptions.ShouldContain(new KeyValuePair<string, ReportDiagnostic>("EXTEXP0002", ReportDiagnostic.Warn));
    }
}
