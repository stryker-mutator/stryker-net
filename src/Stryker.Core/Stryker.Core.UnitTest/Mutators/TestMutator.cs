using Stryker.Utilities.Attributes;

namespace Stryker.Abstractions.UnitTest.Mutators
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
