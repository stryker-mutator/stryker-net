using System;
using System.Runtime.Serialization;

namespace Stryker.Core
{
    [Serializable]
    class ReportNotFoundException : Exception
    {
        public ReportNotFoundException()
        {
        }

        public ReportNotFoundException(string message) : base(message)
        {
        }

        public ReportNotFoundException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected ReportNotFoundException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}