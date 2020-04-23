using Stryker.Core.DashboardCompare;
using Stryker.Core.DiffProviders;
using Stryker.Core.Initialisation;
using Stryker.Core.Mutants;
using Stryker.Core.Options;
using Stryker.Core.ProjectComponents;
using Stryker.Core.Reporters.Json;
using System.Collections.Generic;
using System.Linq;

namespace Stryker.Core.MutantFilters
{
    public class DiffMutantFilter : IMutantFilter
    {
        private readonly DiffResult _diffResult;
        private const string _displayName = "git diff file filter";
        public string DisplayName => _displayName;

        public DiffMutantFilter(StrykerOptions options, IDiffProvider diffProvider)
        {
            if (options.DiffEnabled)
            {
                _diffResult = diffProvider.ScanDiff().Result;
            }
        }

        public IEnumerable<Mutant> FilterMutants(IEnumerable<Mutant> mutants, FileLeaf file, StrykerOptions options)
        {
            if (options.DiffCompareToDashboard && BaselineReport.Instance.Report == null)
            {
                return mutants;
            }

            if (options.DiffEnabled && !_diffResult.TestsChanged)
            {
                if (_diffResult.ChangedFiles.Contains(file.FullPath))
                {
                    return mutants;
                }
                return Enumerable.Empty<Mutant>();
            }
            return mutants;
        }
    }
}
