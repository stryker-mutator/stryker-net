using Microsoft.Extensions.Logging;

namespace Stryker.Core.Logging
{
    public static class ApplicationLogging
    {
        private static ILoggerFactory _factory = null;

        public static ILoggerFactory LoggerFactory
        {
            get => _factory;
            set => _factory = value;
        }
    }
}
