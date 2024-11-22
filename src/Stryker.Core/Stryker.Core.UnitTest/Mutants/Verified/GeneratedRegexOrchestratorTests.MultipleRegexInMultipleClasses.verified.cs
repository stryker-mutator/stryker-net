using System.Text.RegularExpressions;
namespace StrykerNet.UnitTest.Mutants.TestResources;
public partial class R1 {
    private static Regex Abc1GeneratedRegex() => (StrykerNamespace.MutantControl.IsActive(1)?Abc1GeneratedRegex_RegexAnchorRemovalMutation_XvqOBYS3t1u4AI1iIc6VTTMEhssC():(StrykerNamespace.MutantControl.IsActive(0)?Abc1GeneratedRegex_RegexAnchorRemovalMutation_KqIxB18xYu8ALD4NBR827tks3jYC():Abc1GeneratedRegex_Original()));
    [GeneratedRegex(@"^abc$", RegexOptions.IgnoreCase, "en-US")]
    private static partial Regex Abc1GeneratedRegex_Original();
    [GeneratedRegex("^abc", RegexOptions.IgnoreCase, "en-US")]
    private static partial Regex Abc1GeneratedRegex_RegexAnchorRemovalMutation_KqIxB18xYu8ALD4NBR827tks3jYC();
    [GeneratedRegex("abc$", RegexOptions.IgnoreCase, "en-US")]
    private static partial Regex Abc1GeneratedRegex_RegexAnchorRemovalMutation_XvqOBYS3t1u4AI1iIc6VTTMEhssC();
    
    private static Regex Abcd1GeneratedRegex() => (StrykerNamespace.MutantControl.IsActive(15)?Abcd1GeneratedRegex_RegexQuantifierRemovalMutation_uJA6aLkSDZf1nNEqTTrmUwkVMpUC():(StrykerNamespace.MutantControl.IsActive(14)?Abcd1GeneratedRegex_RegexPredefinedCharacterClassToCharacterClassWithItsNegationChange_MDzkRwclKiIgJcKRendsP7fvRxsC():(StrykerNamespace.MutantControl.IsActive(13)?Abcd1GeneratedRegex_RegexPredefinedCharacterClassNullification_zvabPNkNXi5E4WVy9DbXSDBtJBcC():(StrykerNamespace.MutantControl.IsActive(12)?Abcd1GeneratedRegex_RegexGreedyQuantifierToReluctantQuantifierModification_w4583iXfF3f5E1wYY0vUHvf89GsC():(StrykerNamespace.MutantControl.IsActive(11)?Abcd1GeneratedRegex_RegexGreedyQuantifierQuantityMutation_SxSBojTcWfK90GNEzOYCcKxKPEMC():(StrykerNamespace.MutantControl.IsActive(10)?Abcd1GeneratedRegex_RegexGreedyQuantifierQuantityMutation_EemMBA83KH5r9vsxwuYTbZItICIC():(StrykerNamespace.MutantControl.IsActive(9)?Abcd1GeneratedRegex_RegexGreedyQuantifierQuantityMutation_0IPy0NkuaZuus5vyaBNObDf8YBEC():(StrykerNamespace.MutantControl.IsActive(8)?Abcd1GeneratedRegex_RegexCharacterClassToAnycharChange_ChZb0RJfdWiCoO7uZWgXV9GKRWoC():(StrykerNamespace.MutantControl.IsActive(7)?Abcd1GeneratedRegex_RegexCharacterClassShorthandNegationMutation_MlYZgMyXCSRTuWJOS9wBgXbB5tcC():(StrykerNamespace.MutantControl.IsActive(6)?Abcd1GeneratedRegex_RegexCharacterClassNegationMutation_sUNdnI9ukFndVf8l6TkBoumJJRQC():(StrykerNamespace.MutantControl.IsActive(5)?Abcd1GeneratedRegex_RegexCharacterClassChildRemoval_yxJAlmw5XA356qagdqMKq3btNCEC():(StrykerNamespace.MutantControl.IsActive(4)?Abcd1GeneratedRegex_RegexCharacterClassChildRemoval_e4iNMNHMRl5IY1AYBopi97YjMQwC():(StrykerNamespace.MutantControl.IsActive(3)?Abcd1GeneratedRegex_RegexCharacterClassChildRemoval_5XPHLFUGLBs4SAQEaHNXnV9D4MkC():(StrykerNamespace.MutantControl.IsActive(2)?Abcd1GeneratedRegex_RegexAnchorRemovalMutation_SdtdlVHq9QAJyltvbdr0AfzHrW4C():Abcd1GeneratedRegex_Original()))))))))))))));
    
    [GeneratedRegex(@"^[abc]\d?", RegexOptions.IgnoreCase, "en-US")]
    private static partial Regex Abcd1GeneratedRegex_Original();
    
    [GeneratedRegex("[abc]\\d?", RegexOptions.IgnoreCase, "en-US")]
    private static partial Regex Abcd1GeneratedRegex_RegexAnchorRemovalMutation_SdtdlVHq9QAJyltvbdr0AfzHrW4C();
    
    [GeneratedRegex("^[ab]\\d?", RegexOptions.IgnoreCase, "en-US")]
    private static partial Regex Abcd1GeneratedRegex_RegexCharacterClassChildRemoval_5XPHLFUGLBs4SAQEaHNXnV9D4MkC();
    
    [GeneratedRegex("^[ac]\\d?", RegexOptions.IgnoreCase, "en-US")]
    private static partial Regex Abcd1GeneratedRegex_RegexCharacterClassChildRemoval_e4iNMNHMRl5IY1AYBopi97YjMQwC();
    
    [GeneratedRegex("^[bc]\\d?", RegexOptions.IgnoreCase, "en-US")]
    private static partial Regex Abcd1GeneratedRegex_RegexCharacterClassChildRemoval_yxJAlmw5XA356qagdqMKq3btNCEC();
    
    [GeneratedRegex("^[^abc]\\d?", RegexOptions.IgnoreCase, "en-US")]
    private static partial Regex Abcd1GeneratedRegex_RegexCharacterClassNegationMutation_sUNdnI9ukFndVf8l6TkBoumJJRQC();
    
    [GeneratedRegex("^[abc]\\D?", RegexOptions.IgnoreCase, "en-US")]
    private static partial Regex Abcd1GeneratedRegex_RegexCharacterClassShorthandNegationMutation_MlYZgMyXCSRTuWJOS9wBgXbB5tcC();
    
    [GeneratedRegex("^[\\w\\W]\\d?", RegexOptions.IgnoreCase, "en-US")]
    private static partial Regex Abcd1GeneratedRegex_RegexCharacterClassToAnycharChange_ChZb0RJfdWiCoO7uZWgXV9GKRWoC();
    
    [GeneratedRegex("^[abc]\\d{0,0}", RegexOptions.IgnoreCase, "en-US")]
    private static partial Regex Abcd1GeneratedRegex_RegexGreedyQuantifierQuantityMutation_0IPy0NkuaZuus5vyaBNObDf8YBEC();
    
    [GeneratedRegex("^[abc]\\d{0,2}", RegexOptions.IgnoreCase, "en-US")]
    private static partial Regex Abcd1GeneratedRegex_RegexGreedyQuantifierQuantityMutation_EemMBA83KH5r9vsxwuYTbZItICIC();
    
    [GeneratedRegex("^[abc]\\d{1,1}", RegexOptions.IgnoreCase, "en-US")]
    private static partial Regex Abcd1GeneratedRegex_RegexGreedyQuantifierQuantityMutation_SxSBojTcWfK90GNEzOYCcKxKPEMC();
    
    [GeneratedRegex("^[abc]\\d??", RegexOptions.IgnoreCase, "en-US")]
    private static partial Regex Abcd1GeneratedRegex_RegexGreedyQuantifierToReluctantQuantifierModification_w4583iXfF3f5E1wYY0vUHvf89GsC();
    
    [GeneratedRegex("^[abc]d?", RegexOptions.IgnoreCase, "en-US")]
    private static partial Regex Abcd1GeneratedRegex_RegexPredefinedCharacterClassNullification_zvabPNkNXi5E4WVy9DbXSDBtJBcC();
    
    [GeneratedRegex("^[abc][\\d\\D]?", RegexOptions.IgnoreCase, "en-US")]
    private static partial Regex Abcd1GeneratedRegex_RegexPredefinedCharacterClassToCharacterClassWithItsNegationChange_MDzkRwclKiIgJcKRendsP7fvRxsC();
    
    [GeneratedRegex("^[abc]\\d", RegexOptions.IgnoreCase, "en-US")]
    private static partial Regex Abcd1GeneratedRegex_RegexQuantifierRemovalMutation_uJA6aLkSDZf1nNEqTTrmUwkVMpUC();
}
public partial class R2 {
    private static Regex Abc2GeneratedRegex() => (StrykerNamespace.MutantControl.IsActive(18)?Abc2GeneratedRegex_RegexAnchorRemovalMutation_zGamrKwW6ANBC778udllQZ1MR7QC():(StrykerNamespace.MutantControl.IsActive(17)?Abc2GeneratedRegex_RegexAnchorRemovalMutation_SWsP0ttxcvtw60j1d0lOMxgnqR8C():(StrykerNamespace.MutantControl.IsActive(16)?Abc2GeneratedRegex_RegexAnchorRemovalMutation_oUokMRf9irbrUFWAuW1GF0UYUmEC():Abc2GeneratedRegex_Original())));
    [GeneratedRegex(@"^abc\b$", RegexOptions.IgnoreCase, "en-US")]
    private static partial Regex Abc2GeneratedRegex_Original();
    [GeneratedRegex("abc\\b$", RegexOptions.IgnoreCase, "en-US")]
    private static partial Regex Abc2GeneratedRegex_RegexAnchorRemovalMutation_oUokMRf9irbrUFWAuW1GF0UYUmEC();
    [GeneratedRegex("^abc\\b", RegexOptions.IgnoreCase, "en-US")]
    private static partial Regex Abc2GeneratedRegex_RegexAnchorRemovalMutation_SWsP0ttxcvtw60j1d0lOMxgnqR8C();
    [GeneratedRegex("^abc$", RegexOptions.IgnoreCase, "en-US")]
    private static partial Regex Abc2GeneratedRegex_RegexAnchorRemovalMutation_zGamrKwW6ANBC778udllQZ1MR7QC();
    
    private static Regex Abcd2GeneratedRegex() => (StrykerNamespace.MutantControl.IsActive(35)?Abcd2GeneratedRegex_RegexQuantifierRemovalMutation_KBl0fhby530iRtCWz8NjzdsGEVkC():(StrykerNamespace.MutantControl.IsActive(34)?Abcd2GeneratedRegex_RegexPredefinedCharacterClassToCharacterClassWithItsNegationChange_iXmXZGuK8Hx41XA1lnPmpjPknBsC():(StrykerNamespace.MutantControl.IsActive(33)?Abcd2GeneratedRegex_RegexPredefinedCharacterClassToCharacterClassWithItsNegationChange_agkiVP4RSi4UdRJIbItUiEnIup0C():(StrykerNamespace.MutantControl.IsActive(32)?Abcd2GeneratedRegex_RegexPredefinedCharacterClassNullification_ZEhYNkswW6B197LW5QCAEdF9nqMC():(StrykerNamespace.MutantControl.IsActive(31)?Abcd2GeneratedRegex_RegexPredefinedCharacterClassNullification_7eoO374QTRVGuB0S0klaEHAi9I4C():(StrykerNamespace.MutantControl.IsActive(30)?Abcd2GeneratedRegex_RegexGreedyQuantifierToReluctantQuantifierModification_xGhG3l5tSJUDy5Ifrk4lioWoRGYC():(StrykerNamespace.MutantControl.IsActive(29)?Abcd2GeneratedRegex_RegexGreedyQuantifierQuantityMutation_bW7wM7kgBZDh9DzrLDA38dt5ELcC():(StrykerNamespace.MutantControl.IsActive(28)?Abcd2GeneratedRegex_RegexGreedyQuantifierQuantityMutation_8D7T7YM8npB9DR3uvuLjBKGBfFwC():(StrykerNamespace.MutantControl.IsActive(27)?Abcd2GeneratedRegex_RegexGreedyQuantifierQuantityMutation_40dpPAGrBhiVjyajOobno7aAFlwC():(StrykerNamespace.MutantControl.IsActive(26)?Abcd2GeneratedRegex_RegexCharacterClassToAnycharChange_IlcC0WBD6RTPHTWzzI8h7GR6yDEC():(StrykerNamespace.MutantControl.IsActive(25)?Abcd2GeneratedRegex_RegexCharacterClassShorthandNegationMutation_JT1uBo8WX3bw1WvAlBzY08lwAdAC():(StrykerNamespace.MutantControl.IsActive(24)?Abcd2GeneratedRegex_RegexCharacterClassShorthandNegationMutation_cy4mCbUGK1XhyZGp20URzeZ16WwC():(StrykerNamespace.MutantControl.IsActive(23)?Abcd2GeneratedRegex_RegexCharacterClassNegationMutation_R1njA5T5mqbQX4QdgTtA84aAUf4C():(StrykerNamespace.MutantControl.IsActive(22)?Abcd2GeneratedRegex_RegexCharacterClassChildRemoval_YtvHnV0PYI0PHlPJvL6JKMfBS8sC():(StrykerNamespace.MutantControl.IsActive(21)?Abcd2GeneratedRegex_RegexCharacterClassChildRemoval_fVOHtzA3yf7EQENon6OG8iqeVrsC():(StrykerNamespace.MutantControl.IsActive(20)?Abcd2GeneratedRegex_RegexCharacterClassChildRemoval_4tRJqEzhSAWdB01rQVeDsQaY998C():(StrykerNamespace.MutantControl.IsActive(19)?Abcd2GeneratedRegex_RegexAnchorRemovalMutation_nrttgUdiLHGmkMufZczT1IN3JbcC():Abcd2GeneratedRegex_Original())))))))))))))))));
    
    [GeneratedRegex(@"^\d[abc]\d?", RegexOptions.IgnoreCase, "en-US")]
    private static partial Regex Abcd2GeneratedRegex_Original();
    
    [GeneratedRegex("\\d[abc]\\d?", RegexOptions.IgnoreCase, "en-US")]
    private static partial Regex Abcd2GeneratedRegex_RegexAnchorRemovalMutation_nrttgUdiLHGmkMufZczT1IN3JbcC();
    
    [GeneratedRegex("^\\d[ab]\\d?", RegexOptions.IgnoreCase, "en-US")]
    private static partial Regex Abcd2GeneratedRegex_RegexCharacterClassChildRemoval_4tRJqEzhSAWdB01rQVeDsQaY998C();
    
    [GeneratedRegex("^\\d[bc]\\d?", RegexOptions.IgnoreCase, "en-US")]
    private static partial Regex Abcd2GeneratedRegex_RegexCharacterClassChildRemoval_fVOHtzA3yf7EQENon6OG8iqeVrsC();
    
    [GeneratedRegex("^\\d[ac]\\d?", RegexOptions.IgnoreCase, "en-US")]
    private static partial Regex Abcd2GeneratedRegex_RegexCharacterClassChildRemoval_YtvHnV0PYI0PHlPJvL6JKMfBS8sC();
    
    [GeneratedRegex("^\\d[^abc]\\d?", RegexOptions.IgnoreCase, "en-US")]
    private static partial Regex Abcd2GeneratedRegex_RegexCharacterClassNegationMutation_R1njA5T5mqbQX4QdgTtA84aAUf4C();
    
    [GeneratedRegex("^\\D[abc]\\d?", RegexOptions.IgnoreCase, "en-US")]
    private static partial Regex Abcd2GeneratedRegex_RegexCharacterClassShorthandNegationMutation_cy4mCbUGK1XhyZGp20URzeZ16WwC();
    
    [GeneratedRegex("^\\d[abc]\\D?", RegexOptions.IgnoreCase, "en-US")]
    private static partial Regex Abcd2GeneratedRegex_RegexCharacterClassShorthandNegationMutation_JT1uBo8WX3bw1WvAlBzY08lwAdAC();
    
    [GeneratedRegex("^\\d[\\w\\W]\\d?", RegexOptions.IgnoreCase, "en-US")]
    private static partial Regex Abcd2GeneratedRegex_RegexCharacterClassToAnycharChange_IlcC0WBD6RTPHTWzzI8h7GR6yDEC();
    
    [GeneratedRegex("^\\d[abc]\\d{1,1}", RegexOptions.IgnoreCase, "en-US")]
    private static partial Regex Abcd2GeneratedRegex_RegexGreedyQuantifierQuantityMutation_40dpPAGrBhiVjyajOobno7aAFlwC();
    
    [GeneratedRegex("^\\d[abc]\\d{0,0}", RegexOptions.IgnoreCase, "en-US")]
    private static partial Regex Abcd2GeneratedRegex_RegexGreedyQuantifierQuantityMutation_8D7T7YM8npB9DR3uvuLjBKGBfFwC();
    
    [GeneratedRegex("^\\d[abc]\\d{0,2}", RegexOptions.IgnoreCase, "en-US")]
    private static partial Regex Abcd2GeneratedRegex_RegexGreedyQuantifierQuantityMutation_bW7wM7kgBZDh9DzrLDA38dt5ELcC();
    
    [GeneratedRegex("^\\d[abc]\\d??", RegexOptions.IgnoreCase, "en-US")]
    private static partial Regex Abcd2GeneratedRegex_RegexGreedyQuantifierToReluctantQuantifierModification_xGhG3l5tSJUDy5Ifrk4lioWoRGYC();
    
    [GeneratedRegex("^\\d[abc]d?", RegexOptions.IgnoreCase, "en-US")]
    private static partial Regex Abcd2GeneratedRegex_RegexPredefinedCharacterClassNullification_7eoO374QTRVGuB0S0klaEHAi9I4C();
    
    [GeneratedRegex("^d[abc]\\d?", RegexOptions.IgnoreCase, "en-US")]
    private static partial Regex Abcd2GeneratedRegex_RegexPredefinedCharacterClassNullification_ZEhYNkswW6B197LW5QCAEdF9nqMC();
    
    [GeneratedRegex("^\\d[abc][\\d\\D]?", RegexOptions.IgnoreCase, "en-US")]
    private static partial Regex Abcd2GeneratedRegex_RegexPredefinedCharacterClassToCharacterClassWithItsNegationChange_agkiVP4RSi4UdRJIbItUiEnIup0C();
    
    [GeneratedRegex("^[\\d\\D][abc]\\d?", RegexOptions.IgnoreCase, "en-US")]
    private static partial Regex Abcd2GeneratedRegex_RegexPredefinedCharacterClassToCharacterClassWithItsNegationChange_iXmXZGuK8Hx41XA1lnPmpjPknBsC();
    
    [GeneratedRegex("^\\d[abc]\\d", RegexOptions.IgnoreCase, "en-US")]
    private static partial Regex Abcd2GeneratedRegex_RegexQuantifierRemovalMutation_KBl0fhby530iRtCWz8NjzdsGEVkC();
}