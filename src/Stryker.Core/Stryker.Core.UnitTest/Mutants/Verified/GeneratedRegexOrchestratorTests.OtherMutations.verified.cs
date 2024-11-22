using System.Text.RegularExpressions;
namespace StrykerNet.UnitTest.Mutants.TestResources;
public partial class R {
    private static Regex AbcGeneratedRegex() => (StrykerNamespace.MutantControl.IsActive(4)?AbcGeneratedRegex_RegexAnchorRemovalMutation_XvqOBYS3t1u4AI1iIc6VTTMEhssC():(StrykerNamespace.MutantControl.IsActive(3)?AbcGeneratedRegex_RegexAnchorRemovalMutation_KqIxB18xYu8ALD4NBR827tks3jYC():AbcGeneratedRegex_Original()));
    [GeneratedRegex(@"^abc$", RegexOptions.IgnoreCase, "en-US")]
    private static partial Regex AbcGeneratedRegex_Original();
    [GeneratedRegex("^abc", RegexOptions.IgnoreCase, "en-US")]
    private static partial Regex AbcGeneratedRegex_RegexAnchorRemovalMutation_KqIxB18xYu8ALD4NBR827tks3jYC();
    [GeneratedRegex("abc$", RegexOptions.IgnoreCase, "en-US")]
    private static partial Regex AbcGeneratedRegex_RegexAnchorRemovalMutation_XvqOBYS3t1u4AI1iIc6VTTMEhssC();
    
    public void M() {if(StrykerNamespace.MutantControl.IsActive(0)){}else{
       var toMutate = (StrykerNamespace.MutantControl.IsActive(1)?"":"a string is here");
       var mutation2 = (StrykerNamespace.MutantControl.IsActive(2)?toMutate.PadRight(5):toMutate.PadLeft(5));
    }
}}