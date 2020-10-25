using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Stryker.Core.Options
{
    public abstract class ComplexStrykerInput<TInput, TValue>
    {
        public string HelpText => Description + HelpOptions;

        public abstract StrykerInput Type { get; }
        public virtual TValue DefaultValue { get; } = default;

        protected abstract string Description { get; }
        protected abstract string HelpOptions { get; }

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

        protected string FormatHelpOptions(string defaultInputs, string allowedInput) => FormatHelpOptions(new List<string> { defaultInputs }, new List<string> { allowedInput });

        protected string FormatHelpOptions(string defaultInputs, IEnumerable<string> allowedInputs) => FormatHelpOptions(new List<string> { defaultInputs }, allowedInputs);

        protected string FormatHelpOptions(IEnumerable<string> defaultInputs, IEnumerable<string> allowedInputs)
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
