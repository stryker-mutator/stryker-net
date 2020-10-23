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
                logger.LogWarning("Using {maxConcurrentTestRunners} testrunners which is more than recommended {safeProcessorCount} for normal system operation. This can have an impact on performance.", maxConcurrentTestRunners, safeProcessorCount);
            }

            if (maxConcurrentTestRunners == 1)
            {
                logger.LogWarning("Stryker is running in single threaded mode due to max concurrent testrunners being set to 1.");
            }

            Value = maxConcurrentTestRunners.Value;
        }

        public override StrykerOption Type => StrykerOption.ConcurrentTestrunners;
        public override string HelpText => @"Mutation testing is time consuming. 
    By default Stryker tries to make the most of your CPU, by spawning as many test runners as you have CPU cores.
    This setting allows you to override this default behavior.

    Reasons you might want to lower this setting:
                                                                 
        - Your test runner starts a browser (another CPU-intensive process)
        - You're running on a shared server
        - You are running stryker in the background while doing other work";
        public override int DefaultValue => Math.Max(Environment.ProcessorCount / 2, 1);
    }
}
