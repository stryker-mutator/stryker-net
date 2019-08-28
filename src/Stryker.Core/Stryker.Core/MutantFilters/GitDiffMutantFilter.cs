using System;
using System.Collections.Generic;
using System.Text;
using LibGit2Sharp;
using Stryker.Core.Mutants;
using Stryker.Core.Options;
using Stryker.Core.ProjectComponents;

namespace Stryker.Core.MutantFilters
{
    public class GitDiffMutantFilter : IMutantFilter
    {
        public string DisplayName => throw new NotImplementedException();

        public IEnumerable<Mutant> FilterMutants(IEnumerable<Mutant> mutants, FileLeaf file, StrykerOptions options)
        {
            using (var repo = new Repository(@"C:\Dev\Repos\stryker-net"))
            {
                foreach (TreeEntryChanges c in repo.Diff.Compare<TreeChanges>())
                {
                    Console.WriteLine(c);
                }
            }
            return mutants;
        }
    }
}
