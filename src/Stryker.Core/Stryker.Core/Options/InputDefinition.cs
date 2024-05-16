using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Stryker.Core.Options;

public interface IInput
{
    string HelpText { get; }
}

public interface IInput<T> : IInput
{
    T SuppliedInput { get; set; }
}

/// <summary>
/// Definition for options that have a different type for the input and the option
/// </summary>
/// <typeparam name="TInput">The type of the input</typeparam>
/// <typeparam name="TValue">The type of the option</typeparam>
public abstract class Input<TInput> : IInput<TInput>
{
    /// <summary>
    /// The default value for the option when no custom value has been supplied, will also be displayed formatted in the help text
    /// </summary>
    public abstract TInput Default { get; }
    public string HelpText => Description + DefaultText + AllowedOptionsText;
    public string DefaultText => FormatDefaultText();
    public string AllowedOptionsText => FormatAllowedInputText();

    /// <summary>
    /// The description will be displayed in the help text
    /// </summary>
    protected abstract string Description { get; }
    /// <summary>
    /// The allowed options will be displayed in the help text
    /// </summary>
    protected virtual IEnumerable<string> AllowedOptions => Enumerable.Empty<string>();

    /// <summary>
    /// The user supplied input value
    /// </summary>
    public TInput SuppliedInput { get; set; } = default;

    protected IEnumerable<string> EnumToStrings(Type enumType) => Enum.GetNames(enumType).Select(e => e.ToString());

    private string FormatAllowedInputText()
    {
        if (AllowedOptions.Any())
        {
            var nonDefaultOptions = string.Join(", ", AllowedOptions);

            if (!string.IsNullOrWhiteSpace(nonDefaultOptions))
            {
                return " | allowed: " + nonDefaultOptions;
            }
        }

        return "";
    }

    private string FormatDefaultText()
    {
        if (Default is not null)
        {
            var optionsString = new StringBuilder();
            optionsString.Append(" | default: ");

            var enumerable = Default as IEnumerable<object>;
            if (Default is not string && enumerable is not null)
            {

                optionsString.Append("[");
                var count = 0;
                foreach (var item in enumerable)
                {
                    optionsString.Append("'");
                    optionsString.Append(item);
                    optionsString.Append("'");
                    if (++count < enumerable.Count())
                    {
                        optionsString.Append(", ");
                    }
                }

                optionsString.Append("]");

                return optionsString.ToString();
            }
            optionsString.Append("'");
            optionsString.Append(Default.ToString());
            optionsString.Append("'");
            return optionsString.ToString();
        }
        return "";
    }
}
