using System.Collections.Generic;
using System.Linq;
using Buildalyzer;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Shouldly;
using Stryker.Configuration.Options;
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

    [TestMethod]
    public void GetParseOptions_ShouldReturnBasicOptions_WhenNoFeaturesAreUsed()
    {
        // Arrange
        var properties = new Dictionary<string, string>();
        var preprocessorSymbols = new[] { "DEBUG" };
        var analyzerResult = CreateAnalyzerResultWithProperties(properties, preprocessorSymbols);
        var options = CreateStrykerOptions(LanguageVersion.CSharp12);

        // Act
        var parseOptions = analyzerResult.GetParseOptions(options);

        // Assert
        parseOptions.LanguageVersion.ShouldBe(LanguageVersion.CSharp12);
        parseOptions.PreprocessorSymbolNames.ShouldContain("DEBUG");
        parseOptions.Features.ShouldBeEmpty();
    }

    [TestMethod]
    [DataRow("InterceptorsNamespaces", "Microsoft.Extensions.DependencyInjection", "InterceptorsPreview", null)]
    [DataRow("Features", "InterceptorsPreview", "InterceptorsPreview", null)]
    [DataRow("Features", "InterceptorsPreview;FunctionPointers", "InterceptorsPreview", "FunctionPointers")]
    public void GetParseOptions_ShouldEnableFeatures_WhenPropertyIsSet(string propertyName, string propertyValue, string expectedFeature1, string expectedFeature2)
    {
        // Arrange
        var properties = new Dictionary<string, string> { { propertyName, propertyValue } };
        var analyzerResult = CreateAnalyzerResultWithProperties(properties);

        // Act
        var parseOptions = analyzerResult.GetParseOptions(CreateStrykerOptions());

        // Assert
        parseOptions.Features.ShouldContain(f => f.Key == expectedFeature1 && f.Value == "true");
        if (expectedFeature2 != null)
        {
            parseOptions.Features.ShouldContain(f => f.Key == expectedFeature2 && f.Value == "true");
        }
    }

    [TestMethod]
    public void GetParseOptions_ShouldIncludeInterceptorsNamespaces_WhenInterceptorsNamespacesIsSet()
    {
        // Arrange
        var properties = new Dictionary<string, string>
        {
            { "InterceptorsNamespaces", "Microsoft.Extensions.DependencyInjection" }
        };
        var analyzerResult = CreateAnalyzerResultWithProperties(properties);

        // Act
        var parseOptions = analyzerResult.GetParseOptions(CreateStrykerOptions());

        // Assert
        parseOptions.Features.ShouldContain(f => f.Key == "InterceptorsNamespaces" && f.Value == "Microsoft.Extensions.DependencyInjection");
    }

    [TestMethod]
    public void GetParseOptions_ShouldEnableMultipleFeatures_WhenFeaturesPropertyHasMultipleFeaturesWithSemicolon()
    {
        // Arrange
        var properties = new Dictionary<string, string>
        {
            { "Features", "InterceptorsPreview;FunctionPointers;ExtendedPartialMethods" }
        };
        var analyzerResult = CreateAnalyzerResultWithProperties(properties);

        // Act
        var parseOptions = analyzerResult.GetParseOptions(CreateStrykerOptions());

        // Assert
        parseOptions.Features.ShouldContain(f => f.Key == "InterceptorsPreview" && f.Value == "true");
        parseOptions.Features.ShouldContain(f => f.Key == "FunctionPointers" && f.Value == "true");
        parseOptions.Features.ShouldContain(f => f.Key == "ExtendedPartialMethods" && f.Value == "true");
        parseOptions.Features.Count().ShouldBe(3);
    }

    [TestMethod]
    public void GetParseOptions_ShouldNotDuplicateInterceptorsPreview_WhenBothFeaturesAndNamespacesAreSet()
    {
        // Arrange
        var properties = new Dictionary<string, string>
        {
            { "Features", "InterceptorsPreview" },
            { "InterceptorsNamespaces", "Microsoft.Extensions.DependencyInjection;System.Text.Json" }
        };
        var analyzerResult = CreateAnalyzerResultWithProperties(properties);

        // Act
        var parseOptions = analyzerResult.GetParseOptions(CreateStrykerOptions());

        // Assert
        var interceptorsPreviewCount = parseOptions.Features.Count(f => f.Key == "InterceptorsPreview");
        interceptorsPreviewCount.ShouldBe(1);
        parseOptions.Features.ShouldContain(f => f.Key == "InterceptorsNamespaces" && f.Value == "Microsoft.Extensions.DependencyInjection;System.Text.Json");
        parseOptions.Features.Count().ShouldBe(2);
    }

    [TestMethod]
    [DataRow("", "")]
    [DataRow(" ", " ")]
    [DataRow(null, null)]
    public void GetParseOptions_ShouldNotEnableFeatures_WhenPropertiesAreEmptyOrNull(string featuresValue, string interceptorsNamespacesValue)
    {
        // Arrange
        var properties = new Dictionary<string, string>();
        if (featuresValue != null)
        {
            properties["Features"] = featuresValue;
        }
        if (interceptorsNamespacesValue != null)
        {
            properties["InterceptorsNamespaces"] = interceptorsNamespacesValue;
        }
        var analyzerResult = CreateAnalyzerResultWithProperties(properties);

        // Act
        var parseOptions = analyzerResult.GetParseOptions(CreateStrykerOptions());

        // Assert
        parseOptions.Features.ShouldBeEmpty();
    }

    [TestMethod]
    [DataRow(" InterceptorsPreview ; FunctionPointers ; ExtendedPartialMethods ")]
    [DataRow("InterceptorsPreview  ;  FunctionPointers  ;  ExtendedPartialMethods")]
    public void GetParseOptions_ShouldHandleWhitespaceInFeatures(string featuresValue)
    {
        // Arrange
        var properties = new Dictionary<string, string> { { "Features", featuresValue } };
        var analyzerResult = CreateAnalyzerResultWithProperties(properties);

        // Act
        var parseOptions = analyzerResult.GetParseOptions(CreateStrykerOptions());

        // Assert
        parseOptions.Features.ShouldContain(f => f.Key == "InterceptorsPreview");
        parseOptions.Features.ShouldContain(f => f.Key == "FunctionPointers");
        parseOptions.Features.ShouldContain(f => f.Key == "ExtendedPartialMethods");
        parseOptions.Features.Count().ShouldBe(3);
    }

    [TestMethod]
    [DataRow("InterceptorsPreview;;FunctionPointers;;;")]
    [DataRow("InterceptorsPreview;;;;FunctionPointers")]
    [DataRow(";;InterceptorsPreview;FunctionPointers;;")]
    public void GetParseOptions_ShouldIgnoreEmptyFeatures(string featuresValue)
    {
        // Arrange
        var properties = new Dictionary<string, string> { { "Features", featuresValue } };
        var analyzerResult = CreateAnalyzerResultWithProperties(properties);

        // Act
        var parseOptions = analyzerResult.GetParseOptions(CreateStrykerOptions());

        // Assert
        parseOptions.Features.Count().ShouldBe(2);
        parseOptions.Features.ShouldContain(f => f.Key == "InterceptorsPreview");
        parseOptions.Features.ShouldContain(f => f.Key == "FunctionPointers");
    }

    private static IAnalyzerResult CreateAnalyzerResultWithProperties(Dictionary<string, string> properties, string[] preprocessorSymbols = null)
    {
        var analyzerResult = Mock.Of<IAnalyzerResult>();
        Mock.Get(analyzerResult)
            .SetupGet(g => g.Properties)
            .Returns(properties);
        Mock.Get(analyzerResult)
            .SetupGet(g => g.PreprocessorSymbols)
            .Returns(preprocessorSymbols ?? ["NET10_0"]);
        return analyzerResult;
    }

    private static StrykerOptions CreateStrykerOptions(LanguageVersion languageVersion = LanguageVersion.CSharp13) =>
        new() { LanguageVersion = languageVersion };
}
