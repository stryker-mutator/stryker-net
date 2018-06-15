using Microsoft.Extensions.Logging;
using Stryker.Core.Exceptions;
using Stryker.Core.Logging;
using Stryker.Core.Mutators;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace Stryker.Core.Options
{
    public class StrykerOptionsValidator
    {
        /// <summary>
        /// Validates the given values in the options and sets default values
        /// </summary>
        /// <exception cref="ValidationException">When a option is not valid</exception>
        /// <param name="options"></param>
        public ValidatedStrykerOptions Validate(StrykerOptions options)
        {
            var logger = ApplicationLogging.LoggerFactory.CreateLogger<StrykerOptionsValidator>();
            logger.LogDebug("Validating options {@options}", options);

            var validatedOptions = new ValidatedStrykerOptions(
                reporter: ValidateReporter(options.Reporter),
                projectUnderTestNameFilter: ValidateProjectUnderTestNameFilter(options.ProjectUnderTestNameFilter),
                basePath: ValidateBasePath(options.BasePath),
                additionalTimeoutMS: ValidateAdditionalTimeoutMS(options.AdditionalTimeoutMS),
                logOptions: options.LogOptions,
                mutators: ValidateMutators(options.Mutators));

            logger.LogDebug("Validated options {@validatedOptions}", validatedOptions);
            return validatedOptions;
        }

        private int ValidateAdditionalTimeoutMS(string additionalTimeoutMS)
        {
            if (string.IsNullOrEmpty(additionalTimeoutMS))
            {
                return 2000;
            }
            if (!int.TryParse(additionalTimeoutMS, out int value))
            {
                throw new ValidationException("The additional timeout value was not a number");
            }
            if (value < 0)
            {
                throw new ValidationException("The additional timeout value may not be negative");
            }
            return value;
        }

        private string ValidateBasePath(string basePath)
        {
            if(string.IsNullOrWhiteSpace(basePath))
            {
                throw new ValidationException("The basepath may not be null or empty");
            }
            return basePath;
        }

        private string ValidateProjectUnderTestNameFilter(string projectUnderTestNameFilter)
        {
            return projectUnderTestNameFilter;
        }

        private string ValidateReporter(string reporter)
        {
            if (string.IsNullOrWhiteSpace(reporter))
            {
                reporter = "Console";
            }
            else if (reporter != "Console" && reporter != "RapportOnly")
            {
                throw new ValidationException("The reporter options are [Console, RapportOnly]");
            }
            return reporter;
        }

        private ICollection<IMutator> ValidateMutators(ICollection<IMutator> mutators)
        {
            if (mutators == null || !mutators.Any())
            {
                mutators = new Collection<IMutator>
                {
                    // the default list of mutators
                    new BinaryExpressionMutator(),
                    new BooleanMutator(),
                };
            }
            return mutators;
        }
    }
}
