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

        protected static string FormatOptions(TInput defaultInputs, TInput input)
        {
            return FormatOptions(new List<TInput> { defaultInputs }, new List<TInput> { input });
        }

        protected static string FormatOptions(TInput defaultInputs, IEnumerable<TInput> inputs)
        {
            return FormatOptions(new List<TInput> { defaultInputs }, inputs);
        }

        protected static string FormatOptions<To>(IEnumerable<To> defaultInputs, IEnumerable<To> allInputs)
        {
            StringBuilder optionsString = new StringBuilder();

            optionsString.Append($"Options[ (default)[ {string.Join(", ", defaultInputs)} ], ");
            string nonDefaultOptions = string.Join(
            ", ",
            allInputs
            .Where(o => !defaultInputs.Any(d => d.ToString() == o.ToString())));

            optionsString.Append(string.Join(", ", nonDefaultOptions));
            optionsString.Append(" ]");

            return optionsString.ToString();
        }
    }
}
