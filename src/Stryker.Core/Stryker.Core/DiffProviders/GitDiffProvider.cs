using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using LibGit2Sharp;
using Stryker.Core.Baseline.Providers;
using Stryker.Core.Exceptions;
using Stryker.Core.Mutants;
using Stryker.Core.Options;

namespace Stryker.Core.DiffProviders
{
    public class GitDiffProvider : IDiffProvider
    {
        public TestSet Tests { get; }
        private readonly StrykerOptions _options;
        private readonly IGitInfoProvider _gitInfoProvider;

        public GitDiffProvider(StrykerOptions options, TestSet tests, IGitInfoProvider gitInfoProvider = null)
        {
            Tests = tests;
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
                throw new InputException("Could not find a commit to diff. Please check you have provided the correct committish for 'since'.");
            }

            var testProjects = _options.TestProjects.ToList();
            if (!testProjects.Any())
            {
                testProjects.Add(_options.ProjectPath);
            }

            var testPaths = testProjects
                .Select(testProject => testProject.EndsWith(Path.DirectorySeparatorChar)
                        ? testProject
                        : testProject + Path.DirectorySeparatorChar)
                .ToArray();

            foreach (var patchChanges in repository.Diff.Compare<Patch>(commit.Tree, DiffTargets.WorkingDirectory))
            {
                string diffPath = FilePathUtils.NormalizePathSeparators(Path.Combine(_gitInfoProvider.RepositoryPath, patchChanges.Path));

                if (testPaths.Any(testPath => diffPath.StartsWith(testPath)))
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
            foreach (var glob in _options.DiffIgnoreChanges.Select(d => d.Glob))
            {
                diffResult.ChangedSourceFiles = diffResult.ChangedSourceFiles.Where(diffResultFile => !glob.IsMatch(diffResultFile)).ToList();
                diffResult.ChangedTestFiles = diffResult.ChangedTestFiles.Where(diffResultFile => !glob.IsMatch(diffResultFile)).ToList();
            }
        }
    }
}
