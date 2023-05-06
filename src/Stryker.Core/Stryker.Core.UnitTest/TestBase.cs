using Microsoft.Extensions.Logging;
using Stryker.Core.Logging;

namespace Stryker.Core.UnitTest;

public abstract class TestBase
{
    protected TestBase() =>
        // initialize loggerfactory to prevent exceptions
        ApplicationLogging.LoggerFactory = new LoggerFactory();
}
