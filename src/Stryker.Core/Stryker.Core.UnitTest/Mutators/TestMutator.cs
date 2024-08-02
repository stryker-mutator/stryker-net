using Stryker.Configuration.Attributes;

namespace Stryker.Configuration.UnitTest.Mutators
{
    public enum TestMutator
    {
        [MutatorDescription("Simple mutator")]
        Simple,
        [MutatorDescription("Two descriptions mutator")]
        [MutatorDescription("Multi-description mutator")]
        MultipleDescriptions
    }
}
