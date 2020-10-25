using Microsoft.Extensions.Logging;
using Stryker.Core.Exceptions;
using System;

namespace Stryker.Core.Options.Inputs
{
    public class ConcurrentTestrunnersInput : SimpleStrykerInput<int>
    {
        static ConcurrentTestrunnersInput()
        {
            Description = @"Mutation testing is time consuming. 
    By default Stryker tries to make the most of your CPU, by spawning as many test runners as you have CPU cores.
    This setting allows you to override this default behavior.
    Reasons you might want to lower this setting:
                                                                 
        - Your test runner starts a browser (another CPU-intensive process)
        - You're running on a shared server
        - You are running stryker in the background while doing other work";
        DefaultValue = Math.Max(Environment.ProcessorCount / 2, 1);
    }

        public override StrykerInput Type => StrykerInput.Concurrency;

        public ConcurrentTestrunnersInput(ILogger logger, int? maxConcurrentTestRunners)
        {
            if (maxConcurrentTestRunners is { })
            {

                if (maxConcurrentTestRunners < 1)
                {
                    throw new StrykerInputException("Maximum concurrent testrunners must be at least 1.");
                }

                if (maxConcurrentTestRunners > DefaultValue)
                {
                    logger.LogWarning("Using {maxConcurrentTestRunners} testrunners which is more than recommended {safeProcessorCount} for normal system operation. " +
                        "This might have an impact on performance.", maxConcurrentTestRunners, DefaultValue);
                }

                if (maxConcurrentTestRunners is 1)
                {
                    logger.LogWarning("Stryker is running in single threaded mode due to max concurrent testrunners being set to 1.");
                }

                Value = maxConcurrentTestRunners.Value;
            }
        }
    }
}
