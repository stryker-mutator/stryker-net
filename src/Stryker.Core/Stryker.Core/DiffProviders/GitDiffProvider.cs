using LibGit2Sharp;
using Stryker.Core.DashboardCompare;
using Stryker.Core.Exceptions;
using Stryker.Core.Options;
using System;
using System.Collections.ObjectModel;
using System.IO;

namespace Stryker.Core.DiffProviders
{
    using System.Linq;
    using System.Text.RegularExpressions;

    public class GitDiffProvider : IDiffProvider
    {
        private readonly StrykerOptions _options;
        private readonly IGitInfoProvider _gitInfoProvider;
        private static readonly Regex StrykerGeneratedFiles = new Regex(@"^.*[\/\\]?StrykerOutput[\/\\].*$", RegexOptions.Compiled);

        public GitDiffProvider(StrykerOptions options, IGitInfoProvider gitInfoProvider = null)
        {
            _options = options;
            _gitInfoProvider = gitInfoProvider ?? new GitInfoProvider(options);
        }

        public DiffResult ScanDiff()
        {
            var diffResult = new DiffResult()
            {
                ChangedFiles = new Collection<string>(),
                TestFilesChanged = new Collection<string>()
            };

            // A git repository has been detected, calculate the diff to filter
            var commit = _gitInfoProvider.DetermineCommit();
            var repository = _gitInfoProvider.Repository;

            if (commit == null)
            {
                throw new Stryker.Core.Exceptions.StrykerInputException("Could not determine a commit to check for diff. Please check you have provided the correct value for --git-source");
            }

            foreach (var patchChanges in repository.Diff.Compare<Patch>(commit.Tree, DiffTargets.Index | DiffTargets.WorkingDirectory))
            {
                string diffPath = FilePathUtils.NormalizePathSeparators(Path.Combine(_gitInfoProvider.RepositoryPath, patchChanges.Path));
                if (!StrykerGeneratedFiles.IsMatch(diffPath))
                {
                    diffResult.ChangedFiles.Add(diffPath);    
                }
                
                if (diffPath.StartsWith(_options.BasePath))
                {
                    diffResult.TestFilesChanged.Add(diffPath);
                }
            }

            RemoveFilteredOutFiles(diffResult);

            return diffResult;
        }

        private void RemoveFilteredOutFiles(DiffResult diffResult)
        {
            foreach(FilePattern filePattern in _options.DiffIgnoreFiles)
            {
                diffResult.ChangedFiles = diffResult.ChangedFiles.Where(diffResultFile => !filePattern.Glob.IsMatch(diffResultFile)).ToList();
                diffResult.TestFilesChanged = diffResult.TestFilesChanged.Where(diffResultFile => !filePattern.Glob.IsMatch(diffResultFile)).ToList();
            }
        }
    }
}
