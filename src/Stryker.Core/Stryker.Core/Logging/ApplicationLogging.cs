using Microsoft.Extensions.Logging;

namespace Stryker.Abstractions.Logging
{
    public static class ApplicationLogging
    {
        public static ILoggerFactory LoggerFactory { get; set; }
    }
}
