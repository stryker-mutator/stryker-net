using LibGit2Sharp;
using Stryker.Core.Options;
using System.Collections.ObjectModel;
using System.IO;

namespace Stryker.Core.DiffProviders
{
    public class GitDiffProvider : IDiffProvider
    {
        private readonly DiffResult _diffResult;

        public GitDiffProvider(StrykerOptions options, IRepository repository, string repositoryPath)
        {
            _diffResult = new DiffResult()
            {
                ChangedFiles = new Collection<string>()
            };
            var sourceBranch = repository.Branches[options.GitSource];
            // Compare the sourcebranch by commits and open filesystem changes.
            foreach (var treeChanges in repository.Diff.Compare<TreeChanges>(sourceBranch.Tip.Tree, DiffTargets.Index | DiffTargets.WorkingDirectory))
            {
                string diffPath = FilePathUtils.NormalizePathSeparators(Path.Combine(repositoryPath, treeChanges.Path));
                _diffResult.ChangedFiles.Add(diffPath);
                if (diffPath.StartsWith(options.BasePath))
                {
                    _diffResult.TestsChanged = true;
                }
            }
        }

        public DiffResult ScanDiff()
        {
            return _diffResult;
        }
    }
}
