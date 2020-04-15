using LibGit2Sharp;
using Stryker.Core.Exceptions;
using Stryker.Core.Options;
using Stryker.Core.Testing;
using System;
using System.Collections.Generic;
using System.Text;

namespace Stryker.Core.DashboardCompare
{
    public class GitBranchProvider : IBranchProvider
    {
        private readonly StrykerOptions _options;
        public GitBranchProvider(StrykerOptions options)
        {
            _options = options;
        }

        public string GetCurrentBranchCanonicalName()
        {
            string repositoryPath = Repository.Discover(_options.BasePath)?.Split(".git")[0];

            if (string.IsNullOrEmpty(repositoryPath))
            {
                throw new StrykerInputException("Could not locate git repository. Unable to determine git diff to filter mutants. Did you run inside a git repo? If not please disable the --diff feature.");
            }

            using (var repo = new Repository(repositoryPath))
            {
                foreach(var branch in repo.Branches)
                {
                    if (branch.IsCurrentRepositoryHead)
                    {
                       return branch.UpstreamBranchCanonicalName;                       
                    }
                }
            }

            return String.Empty;
        }
    }
}
