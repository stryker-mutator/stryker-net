namespace Stryker.RegexMutators;
using RegexParser.Nodes;

public class RegexMutation
{
    public RegexNode OriginalNode { get; set; }
    public RegexNode ReplacementNode { get; set; }
    public string ReplacementPattern { get; set; }
    public string DisplayName { get; set; }
    public string Description { get; set; }
}
