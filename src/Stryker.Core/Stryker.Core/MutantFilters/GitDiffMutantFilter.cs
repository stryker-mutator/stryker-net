using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using LibGit2Sharp;
using Stryker.Core.Mutants;
using Stryker.Core.Options;
using Stryker.Core.ProjectComponents;

namespace Stryker.Core.MutantFilters
{
    public class GitDiffMutantFilter : IMutantFilter
    {
        public string DisplayName => throw new NotImplementedException();
        private ICollection<string> _changedFiles = new Collection<string>();

        public GitDiffMutantFilter()
        {
            using (var repo = new Repository(@"C:\Dev\Repos\stryker-net"))
            {
                foreach (var treeChanges in repo.Diff.Compare<TreeChanges>())
                {
                    _changedFiles.Add(treeChanges.OldPath);
                }
            }
        }

        public IEnumerable<Mutant> FilterMutants(IEnumerable<Mutant> mutants, FileLeaf file, StrykerOptions options)
        {
            if (_changedFiles.Contains(file.FullPath))
            {
                return Enumerable.Empty<Mutant>();
            }
            return mutants;
        }
    }
}
