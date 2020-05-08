using LibGit2Sharp;
using Stryker.Core.Exceptions;
using Stryker.Core.Options;
using System;
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
                TestFilesChanged = new Collection<string>()
            };

            // A git repository has been detected, calculate the diff to filter
            using (var repository = new Repository(repositoryPath))
            {
                var commit = DetermineCommit(repository);

                foreach (var patchChanges in repository.Diff.Compare<Patch>(commit.Tree, DiffTargets.Index | DiffTargets.WorkingDirectory))
                {
                    string diffPath = FilePathUtils.NormalizePathSeparators(Path.Combine(repositoryPath, patchChanges.Path));
                    diffResult.ChangedFiles.Add(diffPath);
                    if (diffPath.StartsWith(_options.BasePath))
                    {
                        diffResult.TestFilesChanged.Add(diffPath);
                    }
                }


            }
            return diffResult;
        }

        private Commit DetermineCommit(Repository repository)
        {
            var sourceBranch = repository.Branches[_options.GitSource];

            if (sourceBranch != null)
            {
                return sourceBranch.Tip;
            }

            var commit = repository.Lookup(new ObjectId(_options.GitSource)) as Commit;

            if (commit != null)
            {
                return commit;
            }

            throw new StrykerInputException($"No Branch or commit found with given source {_options.GitSource}. Please provide a different --git-source or remove this option.");
        }
    }
}
