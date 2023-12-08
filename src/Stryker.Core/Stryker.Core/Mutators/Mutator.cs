using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using Stryker.Core.Attributes;

namespace Stryker.Core.Mutators;

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
    Math
}

public static class EnumExtension
{
    public static IEnumerable<string> GetDescriptions<T>(this T e) where T : IConvertible
    {
        if (e is not Enum) return Array.Empty<string>();
        var type = e.GetType();
        var values = Enum.GetValues(type);

        foreach (int val in values)
        {
            if (val != e.ToInt32(CultureInfo.InvariantCulture))
            {
                continue;
            }
            var memInfo = type.GetMember(type.GetEnumName(val));

            var descriptions = memInfo[0].GetCustomAttributes<MutatorDescriptionAttribute>(false).Select(descriptionAttribute => descriptionAttribute.Description);
            return descriptions;
        }

        return Array.Empty<string>();
    }
}
