using System.Text.RegularExpressions;
namespace StrykerNet.UnitTest.Mutants.TestResources;
public partial class R {
    private static Regex AbcGeneratedRegex  => (StrykerNamespace.MutantControl.IsActive(6)?AbcGeneratedRegex_RegexQuantifierRemovalMutation_wRDyzHH2CJ7sUB0tpS8lR26p8WUC:(StrykerNamespace.MutantControl.IsActive(5)?AbcGeneratedRegex_RegexPredefinedCharacterClassToCharacterClassWithItsNegationChange_1luZwUASUXVZkgVBGCAx1ChgkVgC:(StrykerNamespace.MutantControl.IsActive(4)?AbcGeneratedRegex_RegexPredefinedCharacterClassNullification_DO1jh3gLUbqKucZGFaAiP5s6FBAC:(StrykerNamespace.MutantControl.IsActive(3)?AbcGeneratedRegex_RegexGreedyQuantifierToReluctantQuantifierModification_mI5VM2LQbh8xoAzLnPAhj32mXecC:(StrykerNamespace.MutantControl.IsActive(2)?AbcGeneratedRegex_RegexCharacterClassShorthandNegationMutation_NLOKz2w30PQRgJAAgumztVCC96gC:(StrykerNamespace.MutantControl.IsActive(1)?AbcGeneratedRegex_RegexAnchorRemovalMutation_QUnzaJ2FZicwB8ov7nGGyh3tpl8C:(StrykerNamespace.MutantControl.IsActive(0)?AbcGeneratedRegex_RegexAnchorRemovalMutation_B3qcxCMGyAqchkutVdC9fBZzdU8C:AbcGeneratedRegex_Original)))))));
    [GeneratedRegex(@"\b\w{5}\b")]
    private static partial Regex AbcGeneratedRegex_Original{ get; }
    [GeneratedRegex("\\w{5}\\b")]
    private static partial Regex AbcGeneratedRegex_RegexAnchorRemovalMutation_B3qcxCMGyAqchkutVdC9fBZzdU8C{ get; }
    [GeneratedRegex("\\b\\w{5}")]
    private static partial Regex AbcGeneratedRegex_RegexAnchorRemovalMutation_QUnzaJ2FZicwB8ov7nGGyh3tpl8C{ get; }
    [GeneratedRegex("\\b\\W{5}\\b")]
    private static partial Regex AbcGeneratedRegex_RegexCharacterClassShorthandNegationMutation_NLOKz2w30PQRgJAAgumztVCC96gC{ get; }
    [GeneratedRegex("\\b\\w{5}?\\b")]
    private static partial Regex AbcGeneratedRegex_RegexGreedyQuantifierToReluctantQuantifierModification_mI5VM2LQbh8xoAzLnPAhj32mXecC{ get; }
    [GeneratedRegex("\\bw{5}\\b")]
    private static partial Regex AbcGeneratedRegex_RegexPredefinedCharacterClassNullification_DO1jh3gLUbqKucZGFaAiP5s6FBAC{ get; }
    [GeneratedRegex("\\b[\\w\\W]{5}\\b")]
    private static partial Regex AbcGeneratedRegex_RegexPredefinedCharacterClassToCharacterClassWithItsNegationChange_1luZwUASUXVZkgVBGCAx1ChgkVgC{ get; }
    [GeneratedRegex("\\b\\w\\b")]
    private static partial Regex AbcGeneratedRegex_RegexQuantifierRemovalMutation_wRDyzHH2CJ7sUB0tpS8lR26p8WUC{ get; }
}