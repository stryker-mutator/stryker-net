using Microsoft.Extensions.Logging;

namespace Stryker.Shared.Logging;

public abstract class ApplicationLogging
{
    public static ILoggerFactory LoggerFactory { get; set; }
}
