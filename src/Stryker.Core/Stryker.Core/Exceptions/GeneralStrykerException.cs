using System;
using System.Text;

namespace Stryker.Core.Exceptions
{
    /// <summary>
    /// Represents errors which are caused by known exceptions in Stryker.
    /// </summary>
    public class GeneralStrykerException : Exception
    {
        public GeneralStrykerException(string message)
            : base(message)
        {
        }

        public GeneralStrykerException(string message, Exception innerException) : base(message, innerException)
        {
        }

        public override string ToString()
        {
            var builder = new StringBuilder();
            builder.AppendLine();
            builder.AppendLine();
            builder.AppendLine(Message);
            builder.AppendLine();
            if (InnerException is not null)
            {
                builder.AppendLine();
                builder.AppendLine("Inner Exception: ");
                builder.AppendLine(InnerException.Message);
                builder.AppendLine(InnerException.StackTrace);
            }
            return builder.ToString();
        }
    }
}
