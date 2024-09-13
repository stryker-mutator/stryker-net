using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using Stryker.Utilities.Attributes;

namespace Stryker.Utilities;

public static class EnumExtension
{
    public static IEnumerable<string> GetDescriptions<T>(this T e) where T : IConvertible
    {
        if (e is not Enum)
            return Array.Empty<string>();
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
