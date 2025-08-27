using System;
using Buildalyzer;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Stryker.Abstractions.Options;

namespace Stryker.Utilities.Buildalyzer;

public static class IAnalyzerResultCSharpExtensions
{
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

    public static CSharpParseOptions GetParseOptions(this IAnalyzerResult analyzerResult, IStrykerOptions options) => new CSharpParseOptions(options.LanguageVersion, DocumentationMode.None, preprocessorSymbols: analyzerResult.PreprocessorSymbols);

    private static NullableContextOptions GetNullableContextOptions(this IAnalyzerResult analyzerResult)
    {
        Enum.TryParse(analyzerResult.GetPropertyOrDefault("Nullable", "disable"), true, out NullableContextOptions nullableOptions);
        return nullableOptions;
    }
}
