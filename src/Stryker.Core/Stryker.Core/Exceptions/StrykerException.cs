using System;
using System.Collections.Generic;
using System.Text;

namespace Stryker.Core.Exceptions
{
    public abstract class StrykerException : Exception
    {
        public StrykerException(string message) : base(message)
        {

        }
        public StrykerException(string message, Exception innerException) : base (message, innerException)
        {

        }
    }
}
