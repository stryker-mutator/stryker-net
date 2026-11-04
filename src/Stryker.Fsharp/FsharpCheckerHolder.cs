using FSharp.Compiler.CodeAnalysis;

namespace Stryker.Fsharp;

/// <summary>One checker for the whole run: it owns FCS caches and is expensive to create.</summary>
internal static class FsharpCheckerHolder
{
    // C# cannot omit F# optional parameters; null selects each default. Only keepAssemblyContents is a deliberate choice.
    public static FSharpChecker Instance { get; } = FSharpChecker.Create(
        projectCacheSize: null,
        keepAssemblyContents: false,
        keepAllBackgroundResolutions: null,
        legacyReferenceResolver: null,
        tryGetMetadataSnapshot: null,
        suggestNamesForErrors: null,
        keepAllBackgroundSymbolUses: null,
        enableBackgroundItemKeyStoreAndSemanticClassification: null,
        enablePartialTypeChecking: null,
        parallelReferenceResolution: null,
        captureIdentifiersWhenParsing: null,
        documentSource: null,
        useTransparentCompiler: null,
        transparentCompilerCacheSizes: null);
}
