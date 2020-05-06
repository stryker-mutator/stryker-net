using System;
using System.ComponentModel;
using System.Globalization;
using System.Linq;

namespace Stryker.Core.Mutators
{
    public enum Mutator
    {
        [Description("Arithmetic operators")]
        Arithmetic,
        [Description("Equality operators")]
        Equality,
        [Description("Boolean literals")]
        Boolean,
        [Description("Logical operators")]
        Logical,
        [Description("Assignment statements")]
        Assignment,
        [Description("Unary operators")]
        Unary,
        [Description("Update operators")]
        Update,
        [Description("Checked statements")]
        Checked,
        [Description("Linq methods")]
        Linq,
        [Description("String literals")]
        String,
        [Description("Bitwise operators")]
        Bitwise,
        [Description("Array initializer")]
        Initializer
    }
}
