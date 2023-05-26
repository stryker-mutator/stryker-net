using System;
using System.Runtime.Serialization;

namespace Stryker.Core.Exceptions
{
    /// <summary>
    /// Represents error when no test projects are found in the solution or configured for stryker.
    /// </summary>
    [Serializable]
    public class NoTestProjectsException : Exception
    {
        public NoTestProjectsException() : base("No test projects found. Please add a test project to your solution or fix your stryker config.") { }

        protected NoTestProjectsException(SerializationInfo info, StreamingContext context) : base(info, context) {}
    }
}
