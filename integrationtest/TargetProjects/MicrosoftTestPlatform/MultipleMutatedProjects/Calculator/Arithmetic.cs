using Formatter;

namespace Calculator;

/// <summary>
/// The upper half of the stack, and the only assembly the test project references directly.
/// The members that delegate to <see cref="LabelFormatter"/> are what pull the second mutated
/// assembly into the same test host, so both assemblies must report their own coverage.
/// Every member is exercised by the tests so that no mutant is left uncovered.
/// </summary>
public static class Arithmetic
{
    public static int Add(int left, int right) => left + right;

    public static bool IsPositive(int value) => value > 0;

    public static int Clamp(int value, int max) => value > max ? max : value;

    public static int LabelBudget(string? label, int max) => Clamp(LabelFormatter.Length(label), max);

    public static bool LabelFits(string? label, int max) => !LabelFormatter.NeedsTruncation(label, max);

    public static string ShortLabel(string label, int max) => LabelFormatter.Truncate(label, max);
}
