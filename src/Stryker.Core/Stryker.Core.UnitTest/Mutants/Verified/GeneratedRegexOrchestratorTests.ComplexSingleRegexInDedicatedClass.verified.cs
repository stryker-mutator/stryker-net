using System.Text.RegularExpressions;
namespace StrykerNet.UnitTest.Mutants.TestResources;
public partial class R {
    private static Regex AbcGeneratedRegex() => (StrykerNamespace.MutantControl.IsActive(13)?AbcGeneratedRegex_RegexQuantifierRemovalMutation_uJA6aLkSDZf1nNEqTTrmUwkVMpUC():(StrykerNamespace.MutantControl.IsActive(12)?AbcGeneratedRegex_RegexPredefinedCharacterClassToCharacterClassWithItsNegationChange_MDzkRwclKiIgJcKRendsP7fvRxsC():(StrykerNamespace.MutantControl.IsActive(11)?AbcGeneratedRegex_RegexPredefinedCharacterClassNullification_zvabPNkNXi5E4WVy9DbXSDBtJBcC():(StrykerNamespace.MutantControl.IsActive(10)?AbcGeneratedRegex_RegexGreedyQuantifierToReluctantQuantifierModification_w4583iXfF3f5E1wYY0vUHvf89GsC():(StrykerNamespace.MutantControl.IsActive(9)?AbcGeneratedRegex_RegexGreedyQuantifierQuantityMutation_SxSBojTcWfK90GNEzOYCcKxKPEMC():(StrykerNamespace.MutantControl.IsActive(8)?AbcGeneratedRegex_RegexGreedyQuantifierQuantityMutation_EemMBA83KH5r9vsxwuYTbZItICIC():(StrykerNamespace.MutantControl.IsActive(7)?AbcGeneratedRegex_RegexGreedyQuantifierQuantityMutation_0IPy0NkuaZuus5vyaBNObDf8YBEC():(StrykerNamespace.MutantControl.IsActive(6)?AbcGeneratedRegex_RegexCharacterClassToAnycharChange_ChZb0RJfdWiCoO7uZWgXV9GKRWoC():(StrykerNamespace.MutantControl.IsActive(5)?AbcGeneratedRegex_RegexCharacterClassShorthandNegationMutation_MlYZgMyXCSRTuWJOS9wBgXbB5tcC():(StrykerNamespace.MutantControl.IsActive(4)?AbcGeneratedRegex_RegexCharacterClassNegationMutation_sUNdnI9ukFndVf8l6TkBoumJJRQC():(StrykerNamespace.MutantControl.IsActive(3)?AbcGeneratedRegex_RegexCharacterClassChildRemoval_yxJAlmw5XA356qagdqMKq3btNCEC():(StrykerNamespace.MutantControl.IsActive(2)?AbcGeneratedRegex_RegexCharacterClassChildRemoval_e4iNMNHMRl5IY1AYBopi97YjMQwC():(StrykerNamespace.MutantControl.IsActive(1)?AbcGeneratedRegex_RegexCharacterClassChildRemoval_5XPHLFUGLBs4SAQEaHNXnV9D4MkC():(StrykerNamespace.MutantControl.IsActive(0)?AbcGeneratedRegex_RegexAnchorRemovalMutation_SdtdlVHq9QAJyltvbdr0AfzHrW4C():AbcGeneratedRegex_Original()))))))))))))));
    [GeneratedRegex(@"^[abc]\d?", RegexOptions.IgnoreCase, "en-US")]
    private static partial Regex AbcGeneratedRegex_Original();
    [GeneratedRegex("[abc]\\d?", RegexOptions.IgnoreCase, "en-US")]
    private static partial Regex AbcGeneratedRegex_RegexAnchorRemovalMutation_SdtdlVHq9QAJyltvbdr0AfzHrW4C();
    [GeneratedRegex("^[ab]\\d?", RegexOptions.IgnoreCase, "en-US")]
    private static partial Regex AbcGeneratedRegex_RegexCharacterClassChildRemoval_5XPHLFUGLBs4SAQEaHNXnV9D4MkC();
    [GeneratedRegex("^[ac]\\d?", RegexOptions.IgnoreCase, "en-US")]
    private static partial Regex AbcGeneratedRegex_RegexCharacterClassChildRemoval_e4iNMNHMRl5IY1AYBopi97YjMQwC();
    [GeneratedRegex("^[bc]\\d?", RegexOptions.IgnoreCase, "en-US")]
    private static partial Regex AbcGeneratedRegex_RegexCharacterClassChildRemoval_yxJAlmw5XA356qagdqMKq3btNCEC();
    [GeneratedRegex("^[^abc]\\d?", RegexOptions.IgnoreCase, "en-US")]
    private static partial Regex AbcGeneratedRegex_RegexCharacterClassNegationMutation_sUNdnI9ukFndVf8l6TkBoumJJRQC();
    [GeneratedRegex("^[abc]\\D?", RegexOptions.IgnoreCase, "en-US")]
    private static partial Regex AbcGeneratedRegex_RegexCharacterClassShorthandNegationMutation_MlYZgMyXCSRTuWJOS9wBgXbB5tcC();
    [GeneratedRegex("^[\\w\\W]\\d?", RegexOptions.IgnoreCase, "en-US")]
    private static partial Regex AbcGeneratedRegex_RegexCharacterClassToAnycharChange_ChZb0RJfdWiCoO7uZWgXV9GKRWoC();
    [GeneratedRegex("^[abc]\\d{0,0}", RegexOptions.IgnoreCase, "en-US")]
    private static partial Regex AbcGeneratedRegex_RegexGreedyQuantifierQuantityMutation_0IPy0NkuaZuus5vyaBNObDf8YBEC();
    [GeneratedRegex("^[abc]\\d{0,2}", RegexOptions.IgnoreCase, "en-US")]
    private static partial Regex AbcGeneratedRegex_RegexGreedyQuantifierQuantityMutation_EemMBA83KH5r9vsxwuYTbZItICIC();
    [GeneratedRegex("^[abc]\\d{1,1}", RegexOptions.IgnoreCase, "en-US")]
    private static partial Regex AbcGeneratedRegex_RegexGreedyQuantifierQuantityMutation_SxSBojTcWfK90GNEzOYCcKxKPEMC();
    [GeneratedRegex("^[abc]\\d??", RegexOptions.IgnoreCase, "en-US")]
    private static partial Regex AbcGeneratedRegex_RegexGreedyQuantifierToReluctantQuantifierModification_w4583iXfF3f5E1wYY0vUHvf89GsC();
    [GeneratedRegex("^[abc]d?", RegexOptions.IgnoreCase, "en-US")]
    private static partial Regex AbcGeneratedRegex_RegexPredefinedCharacterClassNullification_zvabPNkNXi5E4WVy9DbXSDBtJBcC();
    [GeneratedRegex("^[abc][\\d\\D]?", RegexOptions.IgnoreCase, "en-US")]
    private static partial Regex AbcGeneratedRegex_RegexPredefinedCharacterClassToCharacterClassWithItsNegationChange_MDzkRwclKiIgJcKRendsP7fvRxsC();
    [GeneratedRegex("^[abc]\\d", RegexOptions.IgnoreCase, "en-US")]
    private static partial Regex AbcGeneratedRegex_RegexQuantifierRemovalMutation_uJA6aLkSDZf1nNEqTTrmUwkVMpUC();
}