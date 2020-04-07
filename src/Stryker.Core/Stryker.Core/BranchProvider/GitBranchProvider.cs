using LibGit2Sharp;
using Stryker.Core.Exceptions;
using Stryker.Core.Options;
using Stryker.Core.Testing;
using System;
using System.Collections.Generic;
using System.Text;

namespace Stryker.Core.BranchProvider
{
    public class GitBranchProvider : IBranchProvider
    {
        private readonly StrykerOptions _options;
        private readonly Chalk _chalk;
        public GitBranchProvider(StrykerOptions options)
        {
            _options = options;
            _chalk = new Chalk();
        }

        public void GetBranchSHA()
        {
            string repositoryPath = Repository.Discover(_options.BasePath)?.Split(".git")[0];

            if (string.IsNullOrEmpty(repositoryPath))
            {
                throw new StrykerInputException("Could not locate git repository. Unable to determine git diff to filter mutants. Did you run inside a git repo? If not please disable the --diff feature.");
            }

            using (var repo = new Repository(repositoryPath))
            {
                var currentBranchName = repo.Head.FriendlyName;
                var currentBranch = repo.Branches[currentBranchName];

                string currentBranchHash;
                if(currentBranch.IsRemote)
                {
                    _chalk.Green(true.ToString());
                }
                
            }
        }
    }
}
