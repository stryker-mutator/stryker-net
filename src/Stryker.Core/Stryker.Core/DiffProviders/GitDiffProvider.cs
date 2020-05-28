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
        private readonly IRepository _repository;
        private readonly string _repositoryPath;

        public GitDiffProvider(StrykerOptions options, IRepository repository = null, string repositoryPath = null)
        {
            _options = options;

            if (repository != null)
            {
                _repositoryPath = repositoryPath;
            }
            else
            {
                _repositoryPath = Repository.Discover(_options?.BasePath)?.Split(".git")[0];
            }

            if (repository != null)
            {
                _repository = repository;
            }
            else
            {
                if (string.IsNullOrEmpty(_repositoryPath))
                {
                    throw new StrykerInputException("Could not locate git repository. Unable to determine git diff to filter mutants. Did you run inside a git repo? If not please disable the --diff feature.");
                }

                _repository = new Repository(_repositoryPath);
            }
        }

        public DiffResult ScanDiff()
        {
            var diffResult = new DiffResult()
            {
                ChangedFiles = new Collection<string>(),
                TestsChanged = false
            };

            // A git repository has been detected, calculate the diff to filter
            var commit = DetermineCommit();

            foreach (var patchChanges in _repository.Diff.Compare<Patch>(commit.Tree, DiffTargets.Index | DiffTargets.WorkingDirectory))
            {
                string diffPath = FilePathUtils.NormalizePathSeparators(Path.Combine(_repositoryPath, patchChanges.Path));
                diffResult.ChangedFiles.Add(diffPath);
                if (diffPath.StartsWith(_options.BasePath))
                {
                    diffResult.TestsChanged = true;
                }
            }
            return diffResult;
        }

        private Commit DetermineCommit()
        {
            var commit = GetCommit();

            if (commit == null)
            {
                Checkout();
                commit = GetCommit();
            }

            if (commit == null)
            {
                throw new StrykerInputException($"No Branch or commit found with given source {_options.GitSource}. Please provide a different --git-source or remove this option.");
            }

            return commit;
        }


        private Commit GetCommit()
        {
            Branch sourceBranch = null;
            foreach (var branch in _repository.Branches)
            {
                try
                {
                    if (branch.UpstreamBranchCanonicalName == _options.GitSource || branch.FriendlyName == _options.GitSource)
                    {
                        sourceBranch = branch;
                        break;
                    }
                }
                catch (ArgumentNullException)
                {
                    // Internal error thrown by libgit2sharp which happens when there is no upstream on a branch.
                    continue;
                }
            }

            if (sourceBranch != null)
            {
                return sourceBranch.Tip;
            }

            if (_options.GitSource.Length == 40)
            {
                var commit = _repository.Lookup(new ObjectId(_options.GitSource)) as Commit;

                if (commit != null)
                {
                    return commit;
                }
            }

            return null;
        }


        public void Checkout()
        {
            var branch = _repository.CreateBranch(_options.GitSource, $"origin/{_options.GitSource}");

            Commands.Checkout(_repository, branch);

            var currentBranch = _repository.CreateBranch(_options.ProjectVersion, $"origin/{_options.ProjectVersion}");

            Commands.Checkout(_repository, currentBranch);
        }
    }
}
