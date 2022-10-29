using Stryker.Core.Attributes;

namespace Stryker.Core.UnitTest.Mutators
{
    public enum TestMutator
    {
        [Description("Simple mutator")]
        Simple,
        [Description("Two descriptions mutator")]
        [Description("Multi-description mutator")]
        MultipleDescriptions
    }
}
