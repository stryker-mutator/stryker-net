using System;

namespace Stryker.Core.Exceptions
{
    public class ValidationException : StrykerException
    {
        public ValidationException(string message) : base ("The given options are not valid", new Exception(message))
        {

        }
    }
}
