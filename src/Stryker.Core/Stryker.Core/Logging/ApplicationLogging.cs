using Microsoft.Extensions.Logging;

namespace Stryker.Configuration.Logging
{
    public static class ApplicationLogging
    {
        public static ILoggerFactory LoggerFactory { get; set; }
    }
}
