using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Stryker.Core.Options
{
    public abstract class ComplexStrykerInput<TInput, TValue>
    {
        public abstract StrykerInput Type { get; }
        public virtual TValue DefaultValue { get; } = default;

        public string HelpText => Description + HelpOptions;
        protected abstract string Description { get; }
        protected virtual string HelpOptions => $"{ (DefaultInput is { } ? $" | default: { DefaultInput }" : "") }";

        public abstract TInput DefaultInput { get; }

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

        protected string FormatEnumHelpOptions() => FormatEnumHelpOptions(new List<string> { DefaultInput.ToString() }, DefaultValue.GetType());
        protected string FormatEnumHelpOptions(IEnumerable<string> defaultInputs, Type enumType) => FormatHelpOptions(defaultInputs, Enum.GetNames(enumType).Select(e => e.ToString()));

        protected string FormatHelpOptions(string allowedInput) => FormatHelpOptions(new List<string> { DefaultValue.ToString() }, new List<string> { allowedInput });
        protected string FormatHelpOptions(IEnumerable<string> allowedInputs) => FormatHelpOptions(new List<string> { DefaultValue.ToString() }, allowedInputs);
        protected string FormatHelpOptions(string defaultInput, string allowedInput) => FormatHelpOptions(new List<string> { defaultInput }, new List<string> { allowedInput });
        protected string FormatHelpOptions(string defaultInputs, IEnumerable<string> allowedInputs) => FormatHelpOptions(new List<string> { defaultInputs }, allowedInputs);

        protected string FormatHelpOptions(IEnumerable<string> defaultInputs, IEnumerable<string> allowedInputs)
        {
            var optionsString = new StringBuilder();

            optionsString.Append($" | default: ({string.Join(", ", defaultInputs)}), ");
            var nonDefaultOptions = string.Join(
            ", ",
            allowedInputs
            .Where(o => !defaultInputs.Any(d => d.ToString() == o.ToString())));

            optionsString.Append(string.Join(", ", nonDefaultOptions));
            //optionsString.Append(" ]");

            return optionsString.ToString();
        }
    }
}
