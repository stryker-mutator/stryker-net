using System.Collections.Generic;
using Stryker.Abstractions;

namespace Stryker.Fsharp.Mutants;

/// <summary>A text replacement of one source range by its mutant-controlled form.</summary>
internal sealed record TextSplice(int MutantId, MutationLocation Range, string OriginalText, string MutatedText)
{
    /// <summary><see cref="MutantId"/> of a splice that is not a mutant.</summary>
    public const int NoMutant = -1;

    /// <summary>
    /// A splice that wraps its range in the helper's static value tracker, so mutants reached while the value is
    /// built are reported as covered in a static context (the C# <c>MutantContext.TrackValue</c>).
    /// </summary>
    public static TextSplice TrackValue(MutationLocation range, string originalText) => new(NoMutant, range, originalText, originalText);

    public bool IsTrackValue => MutantId == NoMutant;
}

/// <summary>Where a splice's wrapped text sits in the mutated file: character offsets, end exclusive.</summary>
internal sealed record PlacedSplice(int MutantId, int StartOffset, int EndOffset);

/// <summary>Mutated file text and the final position of every splice in it.</summary>
internal sealed record AppliedSplices(string Text, IReadOnlyList<PlacedSplice> Placed);
