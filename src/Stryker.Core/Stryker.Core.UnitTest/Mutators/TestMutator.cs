using Stryker.Core.Attributes;

namespace Stryker.Core.UnitTest.Mutators
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
