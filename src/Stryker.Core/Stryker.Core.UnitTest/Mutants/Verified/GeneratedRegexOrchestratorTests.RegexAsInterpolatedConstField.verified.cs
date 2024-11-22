using System.Text.RegularExpressions;
namespace StrykerNet.UnitTest.Mutants.TestResources;
public partial class R {
    private static Regex AbcdeGeneratedRegex() => (StrykerNamespace.MutantControl.IsActive(1)?AbcdeGeneratedRegex_RegexAnchorRemovalMutation_yKlpAr1znwWgUdVhsnWjVnBCAZgC():(StrykerNamespace.MutantControl.IsActive(0)?AbcdeGeneratedRegex_RegexAnchorRemovalMutation_NmuwH41sWWaN87b6mJOyDYbGGhEC():AbcdeGeneratedRegex_Original()));
    [GeneratedRegex(C.Regex)]
    private static partial Regex AbcdeGeneratedRegex_Original();
    [GeneratedRegex("^abcde")]
    private static partial Regex AbcdeGeneratedRegex_RegexAnchorRemovalMutation_NmuwH41sWWaN87b6mJOyDYbGGhEC();
    [GeneratedRegex("abcde$")]
    private static partial Regex AbcdeGeneratedRegex_RegexAnchorRemovalMutation_yKlpAr1znwWgUdVhsnWjVnBCAZgC();
}
public static class C {
    public const string Inner = @"abcde";
    public const string Regex = $@"^{Inner}$";
}