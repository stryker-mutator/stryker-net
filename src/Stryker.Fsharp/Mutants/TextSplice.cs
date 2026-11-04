using Stryker.Abstractions;

namespace Stryker.Fsharp.Mutants;

/// <summary>A text replacement of one source range by its mutant-controlled form.</summary>
internal sealed record TextSplice(int MutantId, MutationLocation Range, string OriginalText, string MutatedText);
