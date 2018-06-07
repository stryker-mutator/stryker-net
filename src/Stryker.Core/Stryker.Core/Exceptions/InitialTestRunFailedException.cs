using System;

namespace Stryker.Core.Exceptions
{
    public class InitialTestRunFailedException : StrykerException
    {
        public InitialTestRunFailedException(string message) : base(message)
        {
        }

        public InitialTestRunFailedException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}
