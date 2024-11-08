using Stryker.Utilities.Attributes;

namespace Stryker.Abstractions.Mutators;

public enum Mutator
{
    [MutatorDescription("Statements")]
    Statement,
    [MutatorDescription("Arithmetic operators")]
    Arithmetic,
    [MutatorDescription("Block statements")]
    Block,
    [MutatorDescription("Equality operators")]
    Equality,
    [MutatorDescription("Boolean literals")]
    Boolean,
    [MutatorDescription("Logical operators")]
    Logical,
    [MutatorDescription("Assignment statements")]
    Assignment,
    [MutatorDescription("Unary operators")]
    Unary,
    [MutatorDescription("Update operators")]
    Update,
    [MutatorDescription("Checked statements")]
    Checked,
    [MutatorDescription("Linq methods")]
    Linq,
    [MutatorDescription("String literals")]
    String,
    [MutatorDescription("Bitwise operators")]
    Bitwise,
    [MutatorDescription("Array initializer")]
    Initializer,
    [MutatorDescription("Regular expressions")]
    Regex,
    [MutatorDescription("Null coalescing")]
    NullCoalescing,
    [MutatorDescription("Math methods")]
    Math,
    [MutatorDescription("String Method")]
    StringMethod,
    [MutatorDescription("Conditional operators")]
    Conditional,
    [MutatorDescription("Collection expressions")]
    CollectionExpression
}
