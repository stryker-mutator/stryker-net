using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Stryker.Core.Options
{
    public interface IInputDefinition
    {
        string HelpText;
    }


    /// <summary>
    /// Definition for options that have the same type for the input and the option 
    /// </summary>
    /// <typeparam name="TValue">The type of the option</typeparam>
    public abstract class InputDefinition<TValue> : InputDefinition<TValue, TValue>
    {

    }

    /// <summary>
    /// Definition for options that have a different type for the input and the option
    /// </summary>
    /// <typeparam name="TInput">The type of the input</typeparam>
    /// <typeparam name="TValue">The type of the option</typeparam>
    public abstract class InputDefinition<TInput, TValue> : IInputDefinition
    {
        /// <summary>
        /// The default value for the option when no custom value has been supplied
        /// </summary>
        public abstract TInput Default { get; }

        public string HelpText => Description + HelpOptions;
        protected abstract string Description { get; }
        protected virtual string HelpOptions => $"{ (Default is { } ? $" | default: { Default }" : "") }";

        /// <summary>
        /// The user supplied input value
        /// </summary>
        public TInput SuppliedInput { get; set; } = default;

        protected string FormatEnumHelpOptions() => FormatEnumHelpOptions(new List<string> { Default.ToString() }, Default.GetType());
        protected string FormatEnumHelpOptions(IEnumerable<string> defaultInputs, Type enumType) => FormatHelpOptions(defaultInputs, Enum.GetNames(enumType).Select(e => e.ToString()));

        protected string FormatHelpOptions(string allowedInput) => FormatHelpOptions(new List<string> { Default.ToString() }, new List<string> { allowedInput });
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
