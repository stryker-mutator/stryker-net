using LibGit2Sharp;
using Microsoft.Extensions.Logging;
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
    public class GitDiffMutantFilter : IMutantFilter
    {
        public string DisplayName => "Git diff mutation filter";
        private readonly ICollection<string> _changedFiles = new Collection<string>();
        private readonly ILogger _logger;

        public GitDiffMutantFilter(StrykerOptions options)
        {
            _logger = ApplicationLogging.LoggerFactory.CreateLogger<MutationTestProcess>();

            string repoPath = Repository.Discover(options.BasePath)?.Split(".git")[0];
            if (string.IsNullOrEmpty(repoPath))
            {
                _logger.LogWarning("Could not locate git repo. Unable to determine git diff to filter mutants.");
                return;
            }
            using (var repo = new Repository(repoPath))
            {
                var branchToCheck = repo.Branches["master"];
                foreach (var treeChanges in repo.Diff.Compare<TreeChanges>(branchToCheck.Tip.Tree, DiffTargets.Index | DiffTargets.WorkingDirectory))
                {
                    _changedFiles.Add(FilePathUtils.NormalizePathSeparators(Path.Combine(repoPath, treeChanges.OldPath)));
                }
            }
        }

        public IEnumerable<Mutant> FilterMutants(IEnumerable<Mutant> mutants, FileLeaf file, StrykerOptions options)
        {
            if (_changedFiles.Contains(file.FullPath))
            {
                return mutants;
            }
            return Enumerable.Empty<Mutant>();
        }
    }
}
