using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using LibGit2Sharp;
using Stryker.Abstractions.Exceptions;
using Stryker.Abstractions.Options;
using Stryker.Abstractions.Testing;
using Stryker.Core.Baseline.Providers;
using Stryker.Utilities;

namespace Stryker.Core.DiffProviders;

public class GitDiffProvider : IDiffProvider
{
    public ITestSet Tests { get; }
    private readonly IStrykerOptions _options;
    private readonly IGitInfoProvider _gitInfoProvider;
    private readonly IEnumerable<string> _testProjectPaths;

    public GitDiffProvider(IStrykerOptions options, ITestSet tests, IGitInfoProvider gitInfoProvider = null,
        IEnumerable<string> testProjectPaths = null)
    {
        Tests = tests;
        _options = options;
        _gitInfoProvider = gitInfoProvider ?? new GitInfoProvider(options);
        _testProjectPaths = testProjectPaths;
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
        if (testProjects.Count == 0)
        {
            // Use the test projects identified during analysis (e.g. in solution mode), before
            // falling back to the working directory.
            testProjects.AddRange(_testProjectPaths ?? []);
            if (testProjects.Count == 0)
            {
                testProjects.Add(_options.ProjectPath);
            }
        }

        var testPaths = testProjects.Select(ToTestDirectoryPath).ToArray();

        foreach (var patchChanges in repository.Diff.Compare<Patch>(commit.Tree, DiffTargets.WorkingDirectory))
        {
            var diffPath = FilePathUtils.NormalizePathSeparators(Path.Combine(_gitInfoProvider.RepositoryPath, patchChanges.Path));

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

    // A test project can be provided as a project file path or as a directory; in both cases the
    // files inside its directory are test files.
    private static string ToTestDirectoryPath(string testProject)
    {
        var path = FilePathUtils.NormalizePathSeparators(testProject);
        if (path.EndsWith("sproj", StringComparison.OrdinalIgnoreCase))
        {
            path = Path.GetDirectoryName(path);
        }

        return path.EndsWith(Path.DirectorySeparatorChar)
            ? path
            : path + Path.DirectorySeparatorChar;
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
