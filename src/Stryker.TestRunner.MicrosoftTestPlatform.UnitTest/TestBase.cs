using Microsoft.Extensions.Logging;
using Stryker.Utilities.Logging;

namespace Stryker.TestRunner.MicrosoftTestPlatform.UnitTest;

public abstract class TestBase
{
    protected TestBase() =>
        ApplicationLogging.LoggerFactory = new LoggerFactory();
}
