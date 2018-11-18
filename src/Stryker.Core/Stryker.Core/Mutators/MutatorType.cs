using System;
using System.ComponentModel;
using System.Globalization;
using System.Linq;

namespace Stryker.Core.Mutators
{
    public enum MutatorType
    {
        [Description("Binary operator")]
        Binary,
        [Description("Boolean literal")]
        Boolean,
        [Description("Logical operator")]
        Logical,
        [Description("Assignment expression")]
        Assignment,
        [Description("Binary operator")]
        Unary,
        [Description("Update operator")]
        Update,
        [Description("Checked expression")]
        Checked,
        [Description("Linq method")]
        Linq,
        [Description("String literal")]
        String
    }

    public static class EnumExtension {

        public static string GetDescription<T>(this T e) where T : IConvertible
        {
            if (e is Enum)
            {
                Type type = e.GetType();
                Array values = Enum.GetValues(type);

                foreach (int val in values)
                {
                    if (val == e.ToInt32(CultureInfo.InvariantCulture))
                    {
                        var memInfo = type.GetMember(type.GetEnumName(val));
                        var descriptionAttribute = memInfo[0]
                            .GetCustomAttributes(typeof(DescriptionAttribute), false)
                            .FirstOrDefault() as DescriptionAttribute;

                        if (descriptionAttribute != null)
                        {
                            return descriptionAttribute.Description;
                        }
                    }
                }
            }

            return null;
        }
    }
}
