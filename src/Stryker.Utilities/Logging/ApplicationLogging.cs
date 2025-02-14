using Microsoft.Extensions.Logging;

namespace Stryker.Utilities.Logging;

public static class ApplicationLogging
{
    public static ILoggerFactory LoggerFactory { get; set; }
}
