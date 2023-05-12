namespace Stryker.DataCollector
{
    using System;
    using System.Linq;

    internal static class Helpers
    {
        public static T ExtractAttribute<T>(this Type type) where T : Attribute
        {
            return type.GetCustomAttributes(typeof(T), false).First() as T;
        }
    }
}
