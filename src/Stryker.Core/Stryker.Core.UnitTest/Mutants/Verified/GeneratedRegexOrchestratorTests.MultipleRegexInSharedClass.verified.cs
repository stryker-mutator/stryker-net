using System.Text.RegularExpressions;
namespace StrykerNet.UnitTest.Mutants.TestResources;
public partial class R {
    private static Regex AbcGeneratedRegex() => (StrykerNamespace.MutantControl.IsActive(3)?AbcGeneratedRegex_RegexAnchorRemovalMutation_XvqOBYS3t1u4AI1iIc6VTTMEhssC():(StrykerNamespace.MutantControl.IsActive(2)?AbcGeneratedRegex_RegexAnchorRemovalMutation_KqIxB18xYu8ALD4NBR827tks3jYC():AbcGeneratedRegex_Original()));
    [GeneratedRegex(@"^abc$", RegexOptions.IgnoreCase, "en-US")]
    private static partial Regex AbcGeneratedRegex_Original();
    [GeneratedRegex("^abc", RegexOptions.IgnoreCase, "en-US")]
    private static partial Regex AbcGeneratedRegex_RegexAnchorRemovalMutation_KqIxB18xYu8ALD4NBR827tks3jYC();
    [GeneratedRegex("abc$", RegexOptions.IgnoreCase, "en-US")]
    private static partial Regex AbcGeneratedRegex_RegexAnchorRemovalMutation_XvqOBYS3t1u4AI1iIc6VTTMEhssC();
    
    static string Value => (StrykerNamespace.MutantControl.IsActive(0)?"":(StrykerNamespace.MutantControl.IsActive(1)?"":"test ").Substring(2));
    
    private static Regex AbcdGeneratedRegex() => (StrykerNamespace.MutantControl.IsActive(17)?AbcdGeneratedRegex_RegexQuantifierRemovalMutation_uJA6aLkSDZf1nNEqTTrmUwkVMpUC():(StrykerNamespace.MutantControl.IsActive(16)?AbcdGeneratedRegex_RegexPredefinedCharacterClassToCharacterClassWithItsNegationChange_MDzkRwclKiIgJcKRendsP7fvRxsC():(StrykerNamespace.MutantControl.IsActive(15)?AbcdGeneratedRegex_RegexPredefinedCharacterClassNullification_zvabPNkNXi5E4WVy9DbXSDBtJBcC():(StrykerNamespace.MutantControl.IsActive(14)?AbcdGeneratedRegex_RegexGreedyQuantifierToReluctantQuantifierModification_w4583iXfF3f5E1wYY0vUHvf89GsC():(StrykerNamespace.MutantControl.IsActive(13)?AbcdGeneratedRegex_RegexGreedyQuantifierQuantityMutation_SxSBojTcWfK90GNEzOYCcKxKPEMC():(StrykerNamespace.MutantControl.IsActive(12)?AbcdGeneratedRegex_RegexGreedyQuantifierQuantityMutation_EemMBA83KH5r9vsxwuYTbZItICIC():(StrykerNamespace.MutantControl.IsActive(11)?AbcdGeneratedRegex_RegexGreedyQuantifierQuantityMutation_0IPy0NkuaZuus5vyaBNObDf8YBEC():(StrykerNamespace.MutantControl.IsActive(10)?AbcdGeneratedRegex_RegexCharacterClassToAnycharChange_ChZb0RJfdWiCoO7uZWgXV9GKRWoC():(StrykerNamespace.MutantControl.IsActive(9)?AbcdGeneratedRegex_RegexCharacterClassShorthandNegationMutation_MlYZgMyXCSRTuWJOS9wBgXbB5tcC():(StrykerNamespace.MutantControl.IsActive(8)?AbcdGeneratedRegex_RegexCharacterClassNegationMutation_sUNdnI9ukFndVf8l6TkBoumJJRQC():(StrykerNamespace.MutantControl.IsActive(7)?AbcdGeneratedRegex_RegexCharacterClassChildRemoval_yxJAlmw5XA356qagdqMKq3btNCEC():(StrykerNamespace.MutantControl.IsActive(6)?AbcdGeneratedRegex_RegexCharacterClassChildRemoval_e4iNMNHMRl5IY1AYBopi97YjMQwC():(StrykerNamespace.MutantControl.IsActive(5)?AbcdGeneratedRegex_RegexCharacterClassChildRemoval_5XPHLFUGLBs4SAQEaHNXnV9D4MkC():(StrykerNamespace.MutantControl.IsActive(4)?AbcdGeneratedRegex_RegexAnchorRemovalMutation_SdtdlVHq9QAJyltvbdr0AfzHrW4C():AbcdGeneratedRegex_Original()))))))))))))));
    
    [GeneratedRegex(@"^[abc]\d?")]
    private static partial Regex AbcdGeneratedRegex_Original();
    
    [GeneratedRegex("[abc]\\d?")]
    private static partial Regex AbcdGeneratedRegex_RegexAnchorRemovalMutation_SdtdlVHq9QAJyltvbdr0AfzHrW4C();
    
    [GeneratedRegex("^[ab]\\d?")]
    private static partial Regex AbcdGeneratedRegex_RegexCharacterClassChildRemoval_5XPHLFUGLBs4SAQEaHNXnV9D4MkC();
    
    [GeneratedRegex("^[ac]\\d?")]
    private static partial Regex AbcdGeneratedRegex_RegexCharacterClassChildRemoval_e4iNMNHMRl5IY1AYBopi97YjMQwC();
    
    [GeneratedRegex("^[bc]\\d?")]
    private static partial Regex AbcdGeneratedRegex_RegexCharacterClassChildRemoval_yxJAlmw5XA356qagdqMKq3btNCEC();
    
    [GeneratedRegex("^[^abc]\\d?")]
    private static partial Regex AbcdGeneratedRegex_RegexCharacterClassNegationMutation_sUNdnI9ukFndVf8l6TkBoumJJRQC();
    
    [GeneratedRegex("^[abc]\\D?")]
    private static partial Regex AbcdGeneratedRegex_RegexCharacterClassShorthandNegationMutation_MlYZgMyXCSRTuWJOS9wBgXbB5tcC();
    
    [GeneratedRegex("^[\\w\\W]\\d?")]
    private static partial Regex AbcdGeneratedRegex_RegexCharacterClassToAnycharChange_ChZb0RJfdWiCoO7uZWgXV9GKRWoC();
    
    [GeneratedRegex("^[abc]\\d{0,0}")]
    private static partial Regex AbcdGeneratedRegex_RegexGreedyQuantifierQuantityMutation_0IPy0NkuaZuus5vyaBNObDf8YBEC();
    
    [GeneratedRegex("^[abc]\\d{0,2}")]
    private static partial Regex AbcdGeneratedRegex_RegexGreedyQuantifierQuantityMutation_EemMBA83KH5r9vsxwuYTbZItICIC();
    
    [GeneratedRegex("^[abc]\\d{1,1}")]
    private static partial Regex AbcdGeneratedRegex_RegexGreedyQuantifierQuantityMutation_SxSBojTcWfK90GNEzOYCcKxKPEMC();
    
    [GeneratedRegex("^[abc]\\d??")]
    private static partial Regex AbcdGeneratedRegex_RegexGreedyQuantifierToReluctantQuantifierModification_w4583iXfF3f5E1wYY0vUHvf89GsC();
    
    [GeneratedRegex("^[abc]d?")]
    private static partial Regex AbcdGeneratedRegex_RegexPredefinedCharacterClassNullification_zvabPNkNXi5E4WVy9DbXSDBtJBcC();
    
    [GeneratedRegex("^[abc][\\d\\D]?")]
    private static partial Regex AbcdGeneratedRegex_RegexPredefinedCharacterClassToCharacterClassWithItsNegationChange_MDzkRwclKiIgJcKRendsP7fvRxsC();
    
    [GeneratedRegex("^[abc]\\d")]
    private static partial Regex AbcdGeneratedRegex_RegexQuantifierRemovalMutation_uJA6aLkSDZf1nNEqTTrmUwkVMpUC();
}