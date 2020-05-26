﻿using LibGit2Sharp;
using Stryker.Core.Exceptions;
using Stryker.Core.Options;
using System;
using System.Collections.ObjectModel;
using System.IO;

namespace Stryker.Core.DiffProviders
{
    public class GitDiffProvider : IDiffProvider, IDisposable
    {
        private readonly StrykerOptions _options;
        private readonly IRepository _repository;
        private string _repositoryPath
        {
            get
            {
                return Repository.Discover(_options?.BasePath)?.Split(".git")[0];
            }
        }

        public GitDiffProvider(StrykerOptions options, IRepository repository = null)
        {
            _options = options;
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
            var sourceBranch = _repository.Branches[_options.GitSource];

            if (sourceBranch != null)
            {
                return sourceBranch.Tip;
            }

            var commit = _repository.Lookup(new ObjectId(_options.GitSource)) as Commit;

            if (commit != null)
            {
                return commit;
            }

            throw new StrykerInputException($"No Branch or commit found with given source {_options.GitSource}. Please provide a different --git-source or remove this option.");
        }

        public void Dispose()
        {
            _repository?.Dispose();
        }
    }
}
