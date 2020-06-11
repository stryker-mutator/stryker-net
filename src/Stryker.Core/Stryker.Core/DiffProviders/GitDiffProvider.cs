﻿using LibGit2Sharp;
using Stryker.Core.DashboardCompare;
using Stryker.Core.Options;
using System.Collections.ObjectModel;
using System.IO;

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
                ChangedFiles = new Collection<string>(),
                TestsChanged = false
            };

            // A git repository has been detected, calculate the diff to filter
            var commit = _gitInfoProvider.DetermineCommit();

            if (commit == null)
            {
                throw new Stryker.Core.Exceptions.StrykerInputException("Could not determine a commit to check for diff. Please check you have provided the correct value for --git-source");
            }

            foreach (var patchChanges in _gitInfoProvider.Repository.Diff.Compare<Patch>(commit.Tree, DiffTargets.Index | DiffTargets.WorkingDirectory))
            {
                string diffPath = FilePathUtils.NormalizePathSeparators(Path.Combine(_gitInfoProvider.RepositoryPath, patchChanges.Path));
                diffResult.ChangedFiles.Add(diffPath);
                if (diffPath.StartsWith(_options.BasePath) && diffPath.EndsWith(".cs"))
                {
                    diffResult.TestsChanged = true;
                }
            }
            return diffResult;
        }
    }
}
