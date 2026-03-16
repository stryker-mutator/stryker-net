using System;
using System.Collections.Generic;
using System.Linq;
using Buildalyzer;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Stryker.Abstractions.Options;

namespace Stryker.Utilities.Buildalyzer;

public static class IAnalyzerResultCSharpExtensions
{
    private const string InterceptorsNamespacesKey = "InterceptorsNamespaces";
    private const string InterceptorsPreviewNamespacesKey = "InterceptorsPreviewNamespaces";

    public static CSharpCompilationOptions GetCompilationOptions(this IAnalyzerResult analyzerResult)
    {
        var compilationOptions = new CSharpCompilationOptions(analyzerResult.GetOutputKind())
            .WithNullableContextOptions(analyzerResult.GetNullableContextOptions())
            .WithAllowUnsafe(analyzerResult.GetPropertyOrDefault("AllowUnsafeBlocks", true))
            .WithAssemblyIdentityComparer(DesktopAssemblyIdentityComparer.Default)
            .WithConcurrentBuild(true)
            .WithModuleName(analyzerResult.GetAssemblyName())
            .WithOverflowChecks(analyzerResult.GetPropertyOrDefault("CheckForOverflowUnderflow", false))
            .WithSpecificDiagnosticOptions(analyzerResult.GetDiagnosticOptions())
            .WithWarningLevel(analyzerResult.GetWarningLevel());

        if (analyzerResult.IsSignedAssembly() && analyzerResult.GetAssemblyOriginatorKeyFile() is var keyFile && keyFile is not null)
        {
            compilationOptions = compilationOptions.WithCryptoKeyFile(keyFile)
                .WithStrongNameProvider(new DesktopStrongNameProvider())
                .WithDelaySign(analyzerResult.IsDelayedSignedAssembly());
        }
        return compilationOptions;
    }

    public static CSharpParseOptions GetParseOptions(this IAnalyzerResult analyzerResult, IStrykerOptions options)
    {
        var parseOptions = new CSharpParseOptions(
            options.LanguageVersion,
            DocumentationMode.None,
            preprocessorSymbols: analyzerResult.PreprocessorSymbols
        );

        var features = ExtractCSharpFeatures(analyzerResult);

        if (features.Count > 0)
        {
            parseOptions = parseOptions.WithFeatures(features);
        }

        return parseOptions;
    }

    private static List<KeyValuePair<string, string>> ExtractCSharpFeatures(IAnalyzerResult analyzerResult)
    {
        var features = new List<KeyValuePair<string, string>>();

        var projectFeatures = analyzerResult.GetPropertyOrDefault("Features");
        if (!string.IsNullOrWhiteSpace(projectFeatures))
        {
            foreach (var feature in projectFeatures.Split(';', StringSplitOptions.RemoveEmptyEntries))
            {
                var trimmedFeature = feature.Trim();
                if (!string.IsNullOrWhiteSpace(trimmedFeature))
                {
                    features.Add(new KeyValuePair<string, string>(trimmedFeature, "true"));
                }
            }
        }

        var interceptorsNamespaces = new List<string?>
        {
            analyzerResult.GetPropertyOrDefault(InterceptorsNamespacesKey),
            analyzerResult.GetPropertyOrDefault(InterceptorsPreviewNamespacesKey)
        };
        var combinedNamespaces = string.Join(";", interceptorsNamespaces.Where(ns => !string.IsNullOrWhiteSpace(ns)));

        if (!string.IsNullOrWhiteSpace(combinedNamespaces))
            {
            features.Add(new KeyValuePair<string, string>(InterceptorsNamespacesKey, combinedNamespaces));
        }

        return features;
    }

    private static NullableContextOptions GetNullableContextOptions(this IAnalyzerResult analyzerResult)
    {
        Enum.TryParse(analyzerResult.GetPropertyOrDefault("Nullable", "disable"), true, out NullableContextOptions nullableOptions);
        return nullableOptions;
    }
}
