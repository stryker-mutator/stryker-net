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
        private readonly IStrykerOptions _options;
        private readonly IGitInfoProvider _gitInfoProvider;

        public GitDiffProvider(IStrykerOptions options, IGitInfoProvider gitInfoProvider = null)
        {
            _options = options;
            _gitInfoProvider = gitInfoProvider ?? new GitInfoProvider(options);
        }

        public DiffResult ScanDiff()
        {
            var diffResult = new DiffResult()
            {
                ChangedSourceFiles = new Collection<ChangedFile>(),
                ChangedTestFiles = new Collection<ChangedFile>()
            };

            // A git repository has been detected, calculate the diff to filter
            var repository = _gitInfoProvider.Repository;
            var commit = _gitInfoProvider.DetermineCommit();

            if (commit == null)
            {
                throw new StrykerInputException("Could not determine a commit to check for diff. Please check you have provided the correct value for --git-source");
            }

            foreach (var patchChanges in repository.Diff.Compare<Patch>(commit.Tree, DiffTargets.WorkingDirectory))
            {
                string diffPath = FilePathUtils.NormalizePathSeparators(Path.Combine(_gitInfoProvider.RepositoryPath, patchChanges.Path));

                if (diffPath.EndsWith("stryker-config.json"))
                {
                    continue;
                }

                var addedLines = patchChanges.AddedLines;
                var deletedLines = patchChanges.DeletedLines;

                var changedFile = new ChangedFile { Path = diffPath, AddedLines = addedLines, DeletedLines = deletedLines };
                
                var fullName = _options.BasePath.EndsWith(Path.DirectorySeparatorChar)
                    ? _options.BasePath
                    : _options.BasePath + Path.DirectorySeparatorChar;

                if (diffPath.StartsWith(fullName))
                {
                    diffResult.ChangedTestFiles.Add(changedFile);
                }
                else
                {
                    diffResult.ChangedSourceFiles.Add(changedFile);
                }
            }
            RemoveFilteredOutFiles(diffResult);

            return diffResult;
        }

        private void RemoveFilteredOutFiles(DiffResult diffResult)
        {
            foreach (FilePattern filePattern in _options.DiffIgnoreFiles)
            {
                diffResult.ChangedSourceFiles = diffResult.ChangedSourceFiles.Where(diffResultFile => !filePattern.Glob.IsMatch(diffResultFile.Path)).ToList();
                diffResult.ChangedTestFiles = diffResult.ChangedTestFiles.Where(diffResultFile => !filePattern.Glob.IsMatch(diffResultFile.Path)).ToList();
            }
        }
    }
}
