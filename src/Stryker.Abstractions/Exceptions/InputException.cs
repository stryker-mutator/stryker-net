using System;
using System.Text;

namespace Stryker.Abstractions.Exceptions;

/// <summary>
/// Represents errors which are related to user input.
/// Everything that the user can fix themselves should be shown to them
///  using this kind of exception.
/// </summary>
public class InputException : Exception
{
    public string Details { get; }
        
    public InputException(string message)
        : base(message)
    {
    }

    public InputException(string message, string details) : base(message) => Details = details;

    public override string ToString()
    {
        var builder = new StringBuilder();
        builder.AppendLine();
        builder.AppendLine();
        builder.AppendLine(Message);
        builder.AppendLine();
        if (!string.IsNullOrEmpty(Details))
        {
            builder.AppendLine(Details);
        }
        return builder.ToString();
    }

}
