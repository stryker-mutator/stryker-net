namespace Stryker.Core.UnitTest;
using Microsoft.Extensions.Logging;
using Stryker.Core.Logging;

public abstract class TestBase
{
    protected TestBase() =>
        // initialize loggerfactory to prevent exceptions
        ApplicationLogging.LoggerFactory = new LoggerFactory();
}
