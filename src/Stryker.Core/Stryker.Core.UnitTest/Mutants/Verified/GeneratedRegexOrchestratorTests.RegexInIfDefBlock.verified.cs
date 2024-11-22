using System.Text.RegularExpressions;
namespace StrykerNet.UnitTest.Mutants.TestResources;
public partial class R {
#if GENERATED_REGEX
    private static Regex AbcGeneratedRegex() => (StrykerNamespace.MutantControl.IsActive(1)?AbcGeneratedRegex_RegexAnchorRemovalMutation_KqIxB18xYu8ALD4NBR827tks3jYC():(StrykerNamespace.MutantControl.IsActive(0)?AbcGeneratedRegex_RegexAnchorRemovalMutation_XvqOBYS3t1u4AI1iIc6VTTMEhssC():AbcGeneratedRegex_Original()));
    [GeneratedRegex(@"^abc$")]
    private static partial Regex AbcGeneratedRegex_Original();
    [GeneratedRegex("abc$")]
    private static partial Regex AbcGeneratedRegex_RegexAnchorRemovalMutation_XvqOBYS3t1u4AI1iIc6VTTMEhssC();
    [GeneratedRegex("^abc")]
    private static partial Regex AbcGeneratedRegex_RegexAnchorRemovalMutation_KqIxB18xYu8ALD4NBR827tks3jYC();
#else
    private static readonly Regex _abcGeneratedRegex = new Regex(@"^abc$");
    private static Regex AbcGeneratedRegex() => _abcGeneratedRegex;
#endif
}