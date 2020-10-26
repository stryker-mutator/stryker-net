using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Stryker.Core.Options
{
    public abstract class ComplexStrykerInput<TInput, TValue>
    {
        public abstract StrykerInput Type { get; }
        public virtual TValue DefaultValue { get; } = default;

        private TInput _defaultInput = default;
        public TInput DefaultInput
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

        protected static string FormatHelpOptions(string defaultInput, string allowedInput) => FormatHelpOptions(new List<string> { defaultInput }, new List<string> { allowedInput });
        protected static string FormatHelpOptions(string defaultInputs, IEnumerable<string> allowedInputs) => FormatHelpOptions(new List<string> { defaultInputs }, allowedInputs);

        protected static string FormatHelpOptions(IEnumerable<string> defaultInputs, IEnumerable<string> allowedInputs)
        {
            StringBuilder optionsString = new StringBuilder();

            optionsString.Append($" | [default = ( {string.Join(", ", defaultInputs)} )], ");
            string nonDefaultOptions = string.Join(
            ", ",
            allowedInputs
            .Where(o => !defaultInputs.Any(d => d.ToString() == o.ToString())));

            optionsString.Append(string.Join(", ", nonDefaultOptions));
            //optionsString.Append(" ]");

            return optionsString.ToString();
        }
    }
}
