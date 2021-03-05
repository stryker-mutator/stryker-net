using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Stryker.Core.Options
{
    public interface IOptionDefinition
    {

    }


    /// <summary>
    /// Definition for options that have the same type for the input and the option 
    /// </summary>
    /// <typeparam name="TValue">The type of the option</typeparam>
    public abstract class OptionDefinition<TValue> : OptionDefinition<TValue, TValue>
    {

    }

    /// <summary>
    /// Definition for options that have a different type for the input and the option
    /// </summary>
    /// <typeparam name="TInput">The type of the input</typeparam>
    /// <typeparam name="TValue">The type of the option</typeparam>
    public abstract class OptionDefinition<TInput, TValue> : IOptionDefinition
    {
        /// <summary>
        /// The default value for the option when no custom value has been supplied
        /// </summary>
        public virtual TValue DefaultValue { get; } = default;

        public string HelpText => Description + HelpOptions;
        protected abstract string Description { get; }
        protected virtual string HelpOptions => $"{ (DefaultInput is { } ? $" | default: { DefaultInput }" : "") }";

        /// <summary>
        /// The default input value
        /// </summary>
        public abstract TInput DefaultInput { get; }
        /// <summary>
        /// The user supplied input value
        /// </summary>
        protected TInput SuppliedInput { get; } = default;

        private TValue _value = default;
        /// <summary>
        /// The value of the option. Returns <see cref="DefaultValue"/> if current value equals <code>default(T)</code>
        /// </summary>
        public TValue Value
        {
            get
            {
                var hasValue = _value?.Equals(default) ?? true;
                return hasValue ? DefaultValue : _value;
            }
            protected set
            {
                _value = value;
            }
        }

        protected string FormatEnumHelpOptions() => FormatEnumHelpOptions(new List<string> { DefaultInput.ToString() }, DefaultValue.GetType());
        protected string FormatEnumHelpOptions(IEnumerable<string> defaultInputs, Type enumType) => FormatHelpOptions(defaultInputs, Enum.GetNames(enumType).Select(e => e.ToString()));

        protected string FormatHelpOptions(string allowedInput) => FormatHelpOptions(new List<string> { DefaultValue.ToString() }, new List<string> { allowedInput });
        protected string FormatHelpOptions(string defaultInputs, IEnumerable<string> allowedInputs) => FormatHelpOptions(new List<string> { defaultInputs }, allowedInputs);

        protected string FormatHelpOptions(IEnumerable<string> defaultInputs, IEnumerable<string> allowedInputs)
        {
            var optionsString = new StringBuilder();

            optionsString.Append($" | default: ({string.Join(", ", defaultInputs)}), ");
            var nonDefaultOptions = string.Join(", ", allowedInputs.Where(o => !defaultInputs.Any(d => d.ToString() == o.ToString())));

            optionsString.Append(string.Join(", ", nonDefaultOptions));

            return optionsString.ToString();
        }
    }
}
