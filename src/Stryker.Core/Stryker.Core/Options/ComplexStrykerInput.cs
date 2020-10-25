using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Stryker.Core.Options
{
    public abstract class ComplexStrykerInput<TInput, TValue>
    {
        public abstract StrykerInput Type { get; }

        public TInput StaticDefaultInput => DefaultInput;
        public string StaticHelptext => HelpText;

        public static string HelpText { get; protected set; } = string.Empty;
        public static TValue DefaultValue { get; protected set; } = default;

        private static TInput _defaultInput = default;
        public static TInput DefaultInput
        {
            get
            {
                if (_defaultInput is null && DefaultValue is TInput defaultValueAsTInput)
                {
                    return defaultValueAsTInput;
                }
                return _defaultInput;
            }
            protected set
            {
                _defaultInput = value;
            }
        }

        private TValue _value = default;
        /// <summary>
        /// The value of the option. Returns <see cref="DefaultValue"/> if current value equals <code>default(T)</code>
        /// </summary>
        public TValue Value
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

        public static string FormatOptions(TInput @default, IEnumerable<TValue> options)
        {
            return FormatOptions(new List<TInput> { @default }, options, new List<TValue>());
        }

        public static string FormatOptions(IEnumerable<TInput> @default, IEnumerable<TValue> options, IEnumerable<TValue> deprecated)
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
