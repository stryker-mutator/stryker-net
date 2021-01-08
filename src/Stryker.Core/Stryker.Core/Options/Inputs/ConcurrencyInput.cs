using System;
using Microsoft.Extensions.Logging;
using Stryker.Core.Exceptions;

namespace Stryker.Core.Options.Inputs
{
    public class ConcurrencyInput : OptionDefinition<int>
    {
        public override StrykerOption Type => StrykerOption.Concurrency;
        public override int DefaultValue => Environment.ProcessorCount;

        protected override string Description => @"By default Stryker tries to make the most of your CPU, by spawning as many parallel processes as you have CPU cores.
    This setting allows you to override this default behavior.
    Reasons you might want to lower this setting:
                                                                 
        - Your test runner starts a browser (another CPU-intensive process)
        - You're running on a shared server
        - You are running stryker in the background while doing other work";

        public ConcurrencyInput() { }
        public ConcurrencyInput(ILogger logger, string maxConcurrency)
        {
            if (maxConcurrency is { })
            {
                var concurrency = int.Parse(maxConcurrency);
                if (concurrency < 1)
                {
                    throw new StrykerInputException("Concurrency must be at least 1.");
                }

                if (concurrency > DefaultValue)
                {
                    logger.LogWarning("Using a concurrency of {concurrency} which is more than recommended {safeConcurrencyCount} for normal system operation. " +
                        "This might have an impact on performance.", concurrency, DefaultValue);
                }

                if (concurrency is 1)
                {
                    logger.LogWarning("Stryker is running in single threaded mode due to concurrency being set to 1.");
                }

                Value = concurrency;
            }
        }
    }
}
