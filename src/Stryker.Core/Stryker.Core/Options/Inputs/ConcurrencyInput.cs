using System;
using Microsoft.Extensions.Logging;
using Stryker.Core.Exceptions;
using Stryker.Core.Logging;

namespace Stryker.Core.Options.Inputs;

public class ConcurrencyInput : Input<int?>
{
    protected override string Description => @"By default Stryker tries to make the most of your CPU, by spawning as many parallel processes as you have CPU cores.
This setting allows you to override this default behavior.
Reasons you might want to lower this setting:

    - Your test runner starts a browser (another CPU-intensive process)
    - You're running on a shared server
    - You are running stryker in the background while doing other work";

    public override int? Default => Math.Max(Environment.ProcessorCount / 2, 1);

    public int Validate(ILogger<ConcurrencyInput> logger = null)
    {
        logger ??= ApplicationLogging.LoggerFactory.CreateLogger<ConcurrencyInput>();

        if (SuppliedInput is null)
        {
            if (Environment.ProcessorCount < 1)
            {
                logger.LogWarning("Processor count is not reported by the system, using concurrency of 1. Set a concurrency to remove this warning.");
            }

            return Default.Value;
        }

        if (SuppliedInput < 1)
        {
            throw new InputException("Concurrency must be at least 1.");
        }

        if (SuppliedInput > Default)
        {
            logger.LogWarning("Using a concurrency of {concurrency} which is more than recommended {safeConcurrencyCount} for normal system operation. " +
                "This might have an impact on performance.", SuppliedInput, Default);
        }

        logger.LogInformation("Stryker will use a max of {concurrency} parallel testsessions.", SuppliedInput.Value);

        if (SuppliedInput is 1)
        {
            logger.LogWarning("Stryker is running in single threaded mode due to concurrency being set to 1.");
        }

        return SuppliedInput.Value;
    }
}
