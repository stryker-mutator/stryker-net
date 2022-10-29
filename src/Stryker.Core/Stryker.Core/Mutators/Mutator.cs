using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using Stryker.Core.Attributes;

namespace Stryker.Core.Mutators
{
    public enum Mutator
    {
        [Description("Statements")]
        Statement,
        [Description("Arithmetic operators")]
        Arithmetic,
        [Description("Block statements")]
        Block,
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
        Initializer,
        [Description("Regular expressions")]
        Regex
    }

    public static class EnumExtension
    {
        public static IEnumerable<string> GetDescriptions<T>(this T e) where T : IConvertible
        {
            if (e is not Enum) return null;
            var type = e.GetType();
            var values = Enum.GetValues(type);

            foreach (int val in values)
            {
                if (val == e.ToInt32(CultureInfo.InvariantCulture))
                {
                    var memInfo = type.GetMember(type.GetEnumName(val));

                    var descriptions = memInfo[0].GetCustomAttributes<DescriptionAttribute>(false).Select(descriptionAttribute => descriptionAttribute.Description);
                    return descriptions;
                }
            }

            return null;
        }
    }
}
