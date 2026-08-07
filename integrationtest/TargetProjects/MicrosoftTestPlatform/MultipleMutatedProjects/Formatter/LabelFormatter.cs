namespace Formatter;

/// <summary>
/// The second mutated assembly loaded by the single test host of this fixture.
/// It deliberately does not reference <c>Calculator</c>: the two assemblies must stay independent
/// so the coverage of one can never stand in for the coverage of the other.
/// Every member is exercised by the tests so that no mutant is left uncovered.
/// </summary>
public static class LabelFormatter
{
    public static int Length(string? label) => label is null ? 0 : label.Length;

    public static bool NeedsTruncation(string? label, int maxLength) => Length(label) > maxLength;

    public static string Truncate(string label, int maxLength) =>
        label.Length <= maxLength ? label : label.Substring(0, maxLength);
}
