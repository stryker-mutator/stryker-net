using System.Text.RegularExpressions;
namespace StrykerNet.UnitTest.Mutants.TestResources;
public partial class R {
  public partial class R2 {
    private static Regex AbcGeneratedRegex() => (StrykerNamespace.MutantControl.IsActive(1)?AbcGeneratedRegex_RegexAnchorRemovalMutation_XvqOBYS3t1u4AI1iIc6VTTMEhssC():(StrykerNamespace.MutantControl.IsActive(0)?AbcGeneratedRegex_RegexAnchorRemovalMutation_KqIxB18xYu8ALD4NBR827tks3jYC():AbcGeneratedRegex_Original()));
    [GeneratedRegex(@"^abc$", RegexOptions.IgnoreCase, "en-US")]
    private static partial Regex AbcGeneratedRegex_Original();
    [GeneratedRegex("^abc", RegexOptions.IgnoreCase, "en-US")]
    private static partial Regex AbcGeneratedRegex_RegexAnchorRemovalMutation_KqIxB18xYu8ALD4NBR827tks3jYC();
    [GeneratedRegex("abc$", RegexOptions.IgnoreCase, "en-US")]
    private static partial Regex AbcGeneratedRegex_RegexAnchorRemovalMutation_XvqOBYS3t1u4AI1iIc6VTTMEhssC();
  }
}