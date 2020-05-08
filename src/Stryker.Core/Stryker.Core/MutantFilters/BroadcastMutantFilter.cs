﻿using Microsoft.Extensions.Logging;
using Stryker.Core.Logging;
using Stryker.Core.Mutants;
using Stryker.Core.Options;
using Stryker.Core.ProjectComponents;
using System.Collections.Generic;
using System.Linq;

namespace Stryker.Core.MutantFilters
{
    public class BroadcastMutantFilter : IMutantFilter
    {
        public IEnumerable<IMutantFilter> MutantFilters { get; }
        public IProjectMutantFilter CoverageMutantFilter { get; }

        private readonly ILogger<BroadcastMutantFilter> _logger;

        public BroadcastMutantFilter(IEnumerable<IMutantFilter> mutantFilters, IProjectMutantFilter coverageMutantFilter)
        {
            MutantFilters = mutantFilters;
            CoverageMutantFilter = coverageMutantFilter;
            _logger = ApplicationLogging.LoggerFactory.CreateLogger<BroadcastMutantFilter>();
        }

        public string DisplayName => "broadcast filter";

        public IEnumerable<Mutant> FilterMutants(IEnumerable<Mutant> mutants, FileLeaf file, StrykerOptions options)
        {

            if (options.Optimizations.HasFlag(OptimizationFlags.SkipUncoveredMutants) || options.Optimizations.HasFlag(OptimizationFlags.CoverageBasedTest))
            {
                _logger.LogInformation($"Capture mutant coverage using '{options.OptimizationMode}' mode.");
                // coverage
                CoverageMutantFilter?.FilterMutants();
            }

            IEnumerable<Mutant> filteredMutants = mutants;

            foreach (var mutantFilter in MutantFilters)
            {
                filteredMutants = FilterMutantsPerFilter(mutantFilter, mutants, file, options).ToList();
            }

            var diffMutantFilter = MutantFilters.FirstOrDefault(x => x.GetType() == typeof(DiffMutantFilter));

            filteredMutants = diffMutantFilter?.FilterMutants(mutants, file, options);

            return filteredMutants;
        }


        public IEnumerable<Mutant> FilterMutantsPerFilter(IMutantFilter mutantFilter, IEnumerable<Mutant> mutants, FileLeaf file, StrykerOptions options)
        {
            DiffMutantFilter filterType = mutantFilter as DiffMutantFilter;

            if (filterType != null)
            {
                return mutants;

            }

            var current = mutantFilter.FilterMutants(mutants, file, options);

            foreach (var skippedMutant in mutants.Except(current))
            {
                if (skippedMutant.ResultStatus == MutantStatus.NotRun)
                {
                    skippedMutant.ResultStatus = MutantStatus.Ignored;
                    skippedMutant.ResultStatusReason = $"Removed by {mutantFilter.DisplayName}";
                }
            }

            return mutants;
        }
    }
}