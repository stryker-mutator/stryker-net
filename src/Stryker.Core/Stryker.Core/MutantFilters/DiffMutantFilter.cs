using LibGit2Sharp;
using Microsoft.Extensions.Logging;
using Stryker.Core.DiffProviders;
using Stryker.Core.Logging;
using Stryker.Core.Mutants;
using Stryker.Core.MutationTest;
using Stryker.Core.Options;
using Stryker.Core.ProjectComponents;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;

namespace Stryker.Core.MutantFilters
{
    public class DiffMutantFilter : IMutantFilter
    {
        public string DisplayName => "git diff file filter";
        private readonly DiffResult _diffResult;

        public DiffMutantFilter(StrykerOptions options, IDiffProvider diffProvider)
        {
            if (options.DiffEnabled)
            {
                _diffResult = diffProvider.ScanDiff();
            }
        }

        public IEnumerable<Mutant> FilterMutants(IEnumerable<Mutant> mutants, FileLeaf file, StrykerOptions options)
        {
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
