using System.Text.RegularExpressions;
namespace StrykerNet.UnitTest.Mutants.TestResources;
public partial interface R {
    [GeneratedRegex(@"a", RegexOptions.IgnoreCase, "en-US")]
    private static partial Regex AbcGeneratedRegex();
}