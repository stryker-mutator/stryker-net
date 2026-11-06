using System.Globalization;

namespace MtpParallelAttributionMini;

/// <summary>
///     Formats dates in the British <c>dd/MM/yyyy</c> form used throughout the documents.
/// </summary>
internal static class DateFormatting
{
    // The static initializer throws when a string mutant empties the literal. That cached
    // TypeInitializationException is what makes a reused MTP test host fail every later mutant,
    // the trigger behind stryker-mutator/stryker-net#3832 (the upstream repro does this with a NodaTime pattern).
    private static readonly char Separator = char.Parse("/");

    public static string ToBritish(DateOnly date) =>
        date.ToString($"dd'{Separator}'MM'{Separator}'yyyy", CultureInfo.InvariantCulture);
}
