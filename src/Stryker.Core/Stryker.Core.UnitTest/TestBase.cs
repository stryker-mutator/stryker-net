using System;
using Microsoft.Extensions.Logging;
using Stryker.Core.Logging;

namespace Stryker.Core.UnitTest
{
    public abstract class LoggingTestBase
    {
        protected LoggingTestBase()
        {
            ApplicationLogging.LoggerFactory = new LoggerFactory();
        }
    }
}
