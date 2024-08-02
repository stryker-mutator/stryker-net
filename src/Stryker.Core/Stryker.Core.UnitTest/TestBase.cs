using Microsoft.Extensions.Logging;
using Stryker.Configuration.Logging;

namespace Stryker.Configuration.UnitTest;
public abstract class TestBase
{
    protected TestBase() =>
        // initialize loggerfactory to prevent exceptions
        ApplicationLogging.LoggerFactory = new LoggerFactory();
}
