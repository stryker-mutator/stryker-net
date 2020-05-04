﻿using LibGit2Sharp;
using Stryker.Core.Exceptions;
using Stryker.Core.Options;
using System.Collections.ObjectModel;
using System.IO;

namespace Stryker.Core.DiffProviders
{
    public class GitDiffProvider : IDiffProvider
    {
        private readonly StrykerOptions _options;

        public GitDiffProvider(StrykerOptions options)
        {
            _options = options;
        }



        public DiffResult ScanDiff()
        {
            string repositoryPath = Repository.Discover(_options.BasePath)?.Split(".git")[0];

            if (string.IsNullOrEmpty(repositoryPath))
            {
                throw new StrykerInputException("Could not locate git repository. Unable to determine git diff to filter mutants. Did you run inside a git repo? If not please disable the --diff feature.");
            }
            var diffResult = new DiffResult()
            {
                ChangedFiles = new Collection<string>(),
                TestsChanged = false
            };

            // A git repository has been detected, calculate the diff to filter
            using (var repository = new Repository(repositoryPath))
            {
                var sourceBranch = repository.Branches[_options.GitSource];

                if (sourceBranch == null)
                {
                    throw new StrykerInputException("Git source branch does not exist. Please set another source branch or remove the --git-source option.");
                }

                var commit = DetermineCommit(repository, sourceBranch);

                foreach (var patchChanges in repository.Diff.Compare<Patch>(commit.Tree, DiffTargets.Index | DiffTargets.WorkingDirectory))
                {
                    string diffPath = FilePathUtils.NormalizePathSeparators(Path.Combine(repositoryPath, patchChanges.Path));
                    diffResult.ChangedFiles.Add(diffPath);
                    if (diffPath.StartsWith(_options.BasePath))
                    {
                        diffResult.TestsChanged = true;
                    }
                }


            }
            return diffResult;
        }

        private Commit DetermineCommit(Repository repository, Branch sourceBranch)
        {
            if (_options.DiffCommit != null)
            {
                var targetCommit = repository.Lookup(new ObjectId(_options.DiffCommit)) as Commit;

                if (targetCommit != null)
                {
                    return targetCommit;
                }
                else
                {
                    throw new StrykerInputException("No commit was found with the given hash. Please provide another hash or remove the --diff-commit option.");
                }
            }
            else
            {
                return sourceBranch.Tip;
            }
        }
    }
}
