using System;
using System.Text;

namespace Stryker.Core.Exceptions;

/// <summary>
/// Represents errors which are caused by known exceptions in Stryker.
/// </summary>
public class GeneralStrykerException : Exception
{
    public string Details { get; }

    public GeneralStrykerException(string message)
        : base(message)
    {
    }

    public GeneralStrykerException(string message, Exception innerException) : base(message, innerException)
    {
    }

    public GeneralStrykerException(string message, string details) : base(message) => Details = details;

    public override string ToString()
    {
        var builder = new StringBuilder();
        builder.AppendLine();
        builder.AppendLine();
        builder.AppendLine(Message);
        builder.AppendLine();
        if (Details is not null)
        {
            builder.AppendLine(Details);
        }
        else if (InnerException is not null)
        {
            builder.AppendLine();
            builder.AppendLine("Inner Exception: ");
            builder.AppendLine(InnerException.Message);
            builder.AppendLine(InnerException.StackTrace);
        }
        return builder.ToString();
    }
}
