using Stryker.Abstractions;

namespace Stryker.Fsharp.Mutants;

/// <summary>A text replacement of one source range by its mutant-controlled form.</summary>
internal sealed record TextSplice(int MutantId, MutationLocation Range, string OriginalText, string MutatedText);

/// <summary>Where a splice's wrapped text sits in the mutated file: character offsets, end exclusive.</summary>
internal sealed record PlacedSplice(int MutantId, int StartOffset, int EndOffset);

/// <summary>Mutated file text and the final position of every splice in it.</summary>
internal sealed record AppliedSplices(string Text, IReadOnlyList<PlacedSplice> Placed);
