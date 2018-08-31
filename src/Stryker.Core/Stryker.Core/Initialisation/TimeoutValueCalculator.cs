using Microsoft.Extensions.Logging;
using Stryker.Core.Logging;

namespace Stryker.Core.Initialisation
{
    public class TimeoutValueCalculator
    {
        private ILogger _logger { get; set; }

        public TimeoutValueCalculator()
        {
            _logger = ApplicationLogging.LoggerFactory.CreateLogger<TimeoutValueCalculator>();
        }

        public int CalculateTimeoutValue(int initialTestrunDurationMS, int extraMS)
        {
            var timeout = (int)(initialTestrunDurationMS * 1.5) + extraMS;

            _logger.LogInformation("Using {0} ms as testrun timeout", timeout);
            return timeout;
        }
    }
}
