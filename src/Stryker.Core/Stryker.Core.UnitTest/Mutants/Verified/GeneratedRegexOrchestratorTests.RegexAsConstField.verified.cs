using System.Text.RegularExpressions;
namespace StrykerNet.UnitTest.Mutants.TestResources;
public partial class R {
    public const string Regex = @"^abc$";
    
    private static Regex AbcGeneratedRegex() => (StrykerNamespace.MutantControl.IsActive(1)?AbcGeneratedRegex_RegexAnchorRemovalMutation_XvqOBYS3t1u4AI1iIc6VTTMEhssC():(StrykerNamespace.MutantControl.IsActive(0)?AbcGeneratedRegex_RegexAnchorRemovalMutation_KqIxB18xYu8ALD4NBR827tks3jYC():AbcGeneratedRegex_Original()));
    
    [GeneratedRegex(Regex)]
    private static partial Regex AbcGeneratedRegex_Original();
    
    [GeneratedRegex("^abc")]
    private static partial Regex AbcGeneratedRegex_RegexAnchorRemovalMutation_KqIxB18xYu8ALD4NBR827tks3jYC();
    
    [GeneratedRegex("abc$")]
    private static partial Regex AbcGeneratedRegex_RegexAnchorRemovalMutation_XvqOBYS3t1u4AI1iIc6VTTMEhssC();
}