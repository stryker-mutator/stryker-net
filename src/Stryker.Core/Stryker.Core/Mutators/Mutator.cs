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
        [Description("Statements")]
        Statement,
        [Description("Bitwise operators")]
        Bitwise,
        [Description("Array initializer")]
        Initializer,
        [Description("Regular expressions")]
        Regex
    }

    public static class EnumExtension
    {
        public static string GetDescription<T>(this T e) where T : IConvertible
        {
            if (e is not Enum) return null;
            var type = e.GetType();
            var values = Enum.GetValues(type);

            foreach (int val in values)
            {
                if (val == e.ToInt32(CultureInfo.InvariantCulture))
                {
                    var memInfo = type.GetMember(type.GetEnumName(val));

                    if (memInfo[0]
                        .GetCustomAttributes(typeof(DescriptionAttribute), false)
                        .FirstOrDefault() is DescriptionAttribute descriptionAttribute)
                    {
                        return descriptionAttribute.Description;
                    }
                }
            }

            return null;
        }
    }
}
