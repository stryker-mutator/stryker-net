using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Stryker.Core.Options
{
    public abstract class ComplexStrykerInput<T, Y>
    {
        public abstract StrykerInput Type { get; }
        public static string HelpText { get; protected set; } = string.Empty;
        public static T DefaultValue { get; protected set; } = default;

        private static Y _defaultInput = default;
        public static Y DefaultInput
        {
            get
            {
                if (_defaultInput is null && DefaultValue is Y defaultValueAsY)
                {
                    return defaultValueAsY;
                }
                return _defaultInput;
            }
            protected set
            {
                _defaultInput = value;
            }
        }

        private T _value = default;
        /// <summary>
        /// The value of the option. Returns <see cref="DefaultValue"/> if current value equals <code>default(T)</code>
        /// </summary>
        public T Value
        {
            get
            {
                return _value?.Equals(default) ?? false ? DefaultValue : _value;
            }
            protected set
            {
                _value = value;
            }
        }

        public static string FormatOptionsString(Y @default, IEnumerable<T> options)
        {
            return FormatOptionsString(new List<Y> { @default }, options, new List<T>());
        }

        public static string FormatOptionsString<U,V>(IEnumerable<U> @default, IEnumerable<V> options, IEnumerable<V> deprecated)
        {
            StringBuilder optionsString = new StringBuilder();

            optionsString.Append($"Options[ (default)[ {string.Join(", ", @default)} ], ");
            string nonDefaultOptions = string.Join(
            ", ",
            options
            .Where(o => !@default.Any(d => d.ToString() == o.ToString()))
            .Where(o => !deprecated.Any(d => d.ToString() == o.ToString())));

            string deprecatedOptions = "";
            if (deprecated.Any())
            {
                deprecatedOptions = "(deprecated) " + string.Join(", (deprecated) ", options.Where(o => deprecated.Any(d => d.ToString() == o.ToString())));
            }

            optionsString.Append(string.Join(", ", nonDefaultOptions, deprecatedOptions));
            optionsString.Append(" ]");

            return optionsString.ToString();
        }
    }
}
