using System;
using System.Linq;

namespace Stryker.DataCollector
{
    internal static class Helpers
    {
        public static T ExtractAttribute<T>(this Type type) where T : Attribute
        {
            return type.GetCustomAttributes(typeof(T), false).First() as T;
        }
    }
}
