using System;

namespace Stryker.DataCollector
{
    internal static class Helpers
    {
        public static T ExtractAttribute<T>(this Type type) where T : Attribute => type.GetCustomAttributes(typeof(T), false)[0] as T;
    }
}
