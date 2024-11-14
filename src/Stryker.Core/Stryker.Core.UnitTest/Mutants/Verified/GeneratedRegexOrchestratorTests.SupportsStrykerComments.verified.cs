using System.Text.RegularExpressions;
namespace StrykerNet.UnitTest.Mutants.TestResources;
public partial class R {
    // stryker disable Regex
    [GeneratedRegex(@"\b\w{5}\b")]
    private static partial Regex AbcGeneratedRegex();
}