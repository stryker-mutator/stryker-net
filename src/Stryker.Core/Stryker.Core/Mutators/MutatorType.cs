using System;
using System.ComponentModel;
using System.Linq;

namespace Stryker.Core.Mutators
{
    public enum MutatorType
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
        String
    }

    public static class MutatorTypeExtension
    {
        public static string GetDescription(this MutatorType e)
        {
            Type type = e.GetType();
            Array values = Enum.GetValues(type);

            foreach (int val in values)
            {
                var memInfo = type.GetMember(type.GetEnumName(val));

                if (memInfo[0]
                    .GetCustomAttributes(typeof(DescriptionAttribute), false)
                    .FirstOrDefault() is DescriptionAttribute descriptionAttribute)
                {
                    return descriptionAttribute.Description;
                }
            }

            return "";
        }
    }
}
