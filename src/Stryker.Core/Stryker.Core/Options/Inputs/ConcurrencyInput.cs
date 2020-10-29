using System;
using Microsoft.Extensions.Logging;
using Stryker.Core.Exceptions;

namespace Stryker.Core.Options.Inputs
{
    public class ConcurrencyInput : SimpleStrykerInput<int>
    {
        public override StrykerInput Type => StrykerInput.Concurrency;
        public override int DefaultValue => Math.Max(Environment.ProcessorCount / 2, 1);

        protected override string Description => @"Mutation testing is time consuming. 
    By default Stryker tries to make the most of your CPU, by spawning as many parallel processes as you have CPU cores.
    This setting allows you to override this default behavior.
    Reasons you might want to lower this setting:
                                                                 
        - Your test runner starts a browser (another CPU-intensive process)
        - You're running on a shared server
        - You are running stryker in the background while doing other work";

        public ConcurrencyInput() { }
        public ConcurrencyInput(ILogger logger, int? maxConcurrentTestRunners)
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
