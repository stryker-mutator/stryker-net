using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using LibGit2Sharp;
using Microsoft.VisualStudio.TestPlatform.CoreUtilities.Extensions;
using Spectre.Console;
using Stryker.Abstractions.Exceptions;
using Stryker.Abstractions.Options;
using Stryker.Core.Baseline.Providers;
using Stryker.Core.Mutants;
using Stryker.Utilities;

namespace Stryker.Core.DiffProviders
{
    public class GitDiffProvider : IDiffProvider
    {
        public TestSet Tests { get; }
        private readonly IStrykerOptions _options;
        private readonly IGitInfoProvider _gitInfoProvider;

        public GitDiffProvider(IStrykerOptions options, TestSet tests, IGitInfoProvider gitInfoProvider = null)
        {
            Tests = tests;
            _options = options;
            _gitInfoProvider = gitInfoProvider ?? new GitInfoProvider(options);
        }

        public DiffResult ScanDiff()
        {
            var diffResult = new DiffResult()
            {
                ChangedSourceFiles = new Dictionary<string, List<int>>(),
                ChangedTestFiles = new Dictionary<string, List<int>>(),
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
                var diffPath = FilePathUtils.NormalizePathSeparators(Path.Combine(_gitInfoProvider.RepositoryPath, patchChanges.Path));

                if (testPaths.Any(testPath => diffPath.StartsWith(testPath)))
                {
                    for (var i = 0; i < patchChanges.AddedLines.Count; i++)
                    {
                        if (!diffResult.ChangedTestFiles.ContainsKey(diffPath))
                        {
                            diffResult.ChangedTestFiles.Add(diffPath, [patchChanges.AddedLines[i].LineNumber]);

                        }
                        else

                        {
                            var entry = diffResult.ChangedTestFiles[diffPath];
                            entry.Add(patchChanges.AddedLines[i].LineNumber);
                        }
                    }
                }
                else
                {
                    if (patchChanges.AddedLines.Count > 0)
                    {
                        for (var i = 0; i < patchChanges.AddedLines.Count; i++)
                        {
                            if (!diffResult.ChangedSourceFiles.ContainsKey(diffPath))
                            {
                                diffResult.ChangedSourceFiles.Add(diffPath, [patchChanges.AddedLines[i].LineNumber]);

                            }
                            else

                            {
                                var entry = diffResult.ChangedSourceFiles[diffPath];
                                entry.Add(patchChanges.AddedLines[i].LineNumber);
                            }
                        }

                    }
                }
            }

            RemoveFilteredOutFiles(diffResult);

            return diffResult;
        }

        private void RemoveFilteredOutFiles(DiffResult diffResult)
        {
            foreach (var glob in _options.DiffIgnoreChanges.Select(d => d.Glob))
            {
                diffResult.ChangedSourceFiles = diffResult.ChangedSourceFiles.Where(diffResultFile => !glob.IsMatch(diffResultFile.Key)).ToDictionary();
                diffResult.ChangedTestFiles = diffResult.ChangedTestFiles.Where(diffResultFile => !glob.IsMatch(diffResultFile.Key)).ToDictionary();
            }
        }
    }
}
