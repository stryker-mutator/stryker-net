using Microsoft.Extensions.Logging;
using Stryker.Core.Exceptions;
using System;

namespace Stryker.Core.Options.Options
{
    class ConcurrentTestrunnersOption : BaseStrykerOption<int>
    {
        public ConcurrentTestrunnersOption(int? maxConcurrentTestRunners, ILogger logger)
        {
            var safeProcessorCount = Math.Max(Environment.ProcessorCount / 2, 1);

            if (!maxConcurrentTestRunners.HasValue)
            {
                Value = safeProcessorCount;
                return;
            }

            if (maxConcurrentTestRunners < 1)
            {
                throw new StrykerInputException(
                    ErrorMessage,
                    "Amount of maximum concurrent testrunners must be greater than zero.");
            }

            if (maxConcurrentTestRunners > safeProcessorCount)
            {
                logger?.LogWarning("Using {0} testrunners which is more than recommended {1} for normal system operation. This can have an impact on performance.", maxConcurrentTestRunners, safeProcessorCount);
            }

            if (maxConcurrentTestRunners == 1)
            {
                logger?.LogWarning("Stryker is running in single threaded mode due to max concurrent testrunners being set to 1.");
            }

            Value = maxConcurrentTestRunners.GetValueOrDefault();
        }

        public override StrykerOption Type => StrykerOption.ConcurrentTestrunners;
        public override string HelpText => "";
        public override int DefaultValue => Math.Max(Environment.ProcessorCount / 2, 1);
    }
}
