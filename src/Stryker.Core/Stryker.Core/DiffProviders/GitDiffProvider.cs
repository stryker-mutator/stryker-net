using LibGit2Sharp;
using Stryker.Core.DashboardCompare;
using Stryker.Core.Exceptions;
using Stryker.Core.Options;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;

namespace Stryker.Core.DiffProviders
{
    public class GitDiffProvider : IDiffProvider
    {
        private readonly StrykerOptions _options;
        private readonly IGitInfoProvider _gitInfoProvider;

        public GitDiffProvider(StrykerOptions options, IGitInfoProvider gitInfoProvider = null)
        {
            _options = options;
            _gitInfoProvider = gitInfoProvider ?? new GitInfoProvider(options);
        }

        public DiffResult ScanDiff()
        {
            var diffResult = new DiffResult()
            {
                ChangedSourceFiles = new Collection<string>(),
                ChangedTestFiles = new Collection<string>()
            };

            // A git repository has been detected, calculate the diff to filter
            var repository = _gitInfoProvider.Repository;
            var commit = _gitInfoProvider.DetermineCommit();

            if (commit == null)
            {
                throw new InputException("Could not determine a commit to check for diff. Please check you have provided the correct value for --git-source");
            }

            foreach (var patchChanges in repository.Diff.Compare<Patch>(commit.Tree, DiffTargets.WorkingDirectory))
            {
                string diffPath = FilePathUtils.NormalizePathSeparators(Path.Combine(_gitInfoProvider.RepositoryPath, patchChanges.Path));

                if (diffPath.EndsWith("stryker-config.json"))
                {
                    continue;
                }

                var fullName = _options.BasePath.EndsWith(Path.DirectorySeparatorChar)
                    ? _options.BasePath
                    : _options.BasePath + Path.DirectorySeparatorChar;

                if (diffPath.StartsWith(fullName))
                {
                    diffResult.ChangedTestFiles.Add(diffPath);
                }
                else
                {
                    diffResult.ChangedSourceFiles.Add(diffPath);
                }
            }
            RemoveFilteredOutFiles(diffResult);

            return diffResult;
        }

        private void RemoveFilteredOutFiles(DiffResult diffResult)
        {
            foreach (FilePattern filePattern in _options.DiffIgnoreFilePatterns)
            {
                diffResult.ChangedSourceFiles = diffResult.ChangedSourceFiles.Where(diffResultFile => !filePattern.Glob.IsMatch(diffResultFile)).ToList();
                diffResult.ChangedTestFiles = diffResult.ChangedTestFiles.Where(diffResultFile => !filePattern.Glob.IsMatch(diffResultFile)).ToList();
            }
        }
    }
}
