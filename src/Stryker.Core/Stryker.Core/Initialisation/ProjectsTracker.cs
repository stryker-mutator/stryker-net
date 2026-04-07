using System;
using System.Collections.Generic;
using System.IO.Abstractions;
using System.Linq;
using Buildalyzer;
using Microsoft.Extensions.Logging;
using Stryker.Abstractions.Options;
using Stryker.Solutions;
using Stryker.Utilities.Buildalyzer;

namespace Stryker.Core.Initialisation;

/// <summary>
/// This class is used to keep track of the solution and the projects that are selected for mutation during the initialization process.
/// </summary>
public class ProjectsTracker
{
    private List<string> _selectedProjects = [];
    private bool _solutionRestored;
    private bool _solutionBuilt;

    private readonly IStrykerOptions _options;
    private readonly INugetRestoreProcess _nugetRestoreProcess;
    private readonly IFileSystem _fileSystem;
    private readonly IBuildalyzerProvider _buildalyzerProvider;
    private readonly ILogger _logger;

    public ProjectsTracker(SolutionFile file, IStrykerOptions options, IBuildalyzerProvider buildalyzerProvider,
        INugetRestoreProcess nugetRestoreProcess, IFileSystem fileSystem, ILogger logger)
    {
        _options = options;
        _buildalyzerProvider = buildalyzerProvider;
        _logger = logger;
        _nugetRestoreProcess = nugetRestoreProcess;
        _fileSystem = fileSystem;
        Solution = file;
        SelectConfiguration();
    }

    private SolutionFile? Solution { get; }

    public string? SolutionFilePath => Solution?.FileName;

    public string Configuration { get; private set; }

    public string Platform { get; private set; }

    public string? TargetFramework { get; set; }

    public int ProjectCount => _selectedProjects.Count;

    public List<string> SelectedProjects => _selectedProjects;

    public void AddProjects(IEnumerable<string> projectFiles) => _selectedProjects.AddRange(projectFiles);

    private void SelectConfiguration()
    {
        var configuration = _options.Configuration;
        var platform = _options.Platform;
        if (Solution != null)
        {
            // we use the solution to determine the configuration and platform to use, as the project files may not contain all configurations and platforms that are defined in the solution
            (Configuration, Platform) = Solution.GetMatching(configuration, platform);
            if ((!string.IsNullOrEmpty(configuration) && configuration != Configuration) ||
                (!string.IsNullOrEmpty(platform) && platform != Platform))
            {
                _logger.LogWarning("Using solution configuration/platform '{ActualBuildType}|{ActualPlatform}' instead of requested '{Configuration}|{Platform}'.",
                    Configuration, Platform, configuration, platform);
            }
            else
            {
                _logger.LogInformation("Using solution configuration/platform '{Configuration}|{Platform}'.", Configuration, Platform);
            }
        }
        else
        {
            Configuration = configuration;
            // "Any CPU" is default platform at solution level, but in project files it is "AnyCPU", so we need to convert it to match the project files
            Platform = platform == "Any CPU" ? "AnyCPU" : platform;
            _logger.LogInformation("Using project configuration/platform '{Configuration}|{Platform}'.", Configuration??"`default`", Platform ?? "`default`");
        }
    }

    /// <summary>
    /// Select all projects from solution
    /// </summary>
    public void SelectAllProjects() => _selectedProjects = Solution?.GetProjects(Configuration, Platform).ToList() ?? [];

    // the method is at solution level because it needs only be called once
    internal void RestoreSolution(IAnalyzerResults results)
    {
        lock (_nugetRestoreProcess)
        {
            if (_solutionRestored || string.IsNullOrEmpty(SolutionFilePath))
            {
                return;
            }

            if (Environment.OSVersion.Platform == PlatformID.Win32NT)
            {
                _logger.LogWarning("Project analysis failed. Stryker will retry after a solution level nuget restore");
                var optionsMsBuildPath = _options.MsBuildPath ?? results.First().MsBuildPath();
                if (string.IsNullOrEmpty(optionsMsBuildPath))
                {
                    _logger.LogWarning("Failed to find MSBuild path from analysis results. Nuget restore may fail if MSBuild is not in PATH.");
                }
                _nugetRestoreProcess.RestorePackages(_options.SolutionPath, optionsMsBuildPath);
            }

            _solutionRestored = true;
        }
    }

    internal void BuildSolution(IInitialBuildProcess buildProcess, IEnumerable<IAnalyzerResult> results)
    {
        lock (_nugetRestoreProcess)
        {
            if (_solutionBuilt || string.IsNullOrEmpty(SolutionFilePath))
            {
                return;
            }

            var framework = results.Any(p => p.TargetsFullFramework());
            // Build the complete solution
            _logger.LogInformation("Building solution {SolutionPathName}.",
                _fileSystem.Path.GetRelativePath(_options.WorkingDirectory, SolutionFilePath));

            buildProcess.InitialBuild(
                framework,
                _fileSystem.Path.GetDirectoryName(SolutionFilePath),
                SolutionFilePath, Configuration, Platform,
                TargetFramework, _options.MsBuildPath ?? results.First().MsBuildPath()
                );
            _solutionBuilt = true;
        }
    }

    /// <summary>
    /// Gets a project analysis context for the given project file, using the configuration and platform from the solution if available, otherwise using the configuration and platform from the options.
    /// </summary>
    /// <param name="projectFile">target project file</param>
    /// <returns>a <see cref="ProjectSimulatedBuildHandler"/> instance for <param name="projectFile">.</param></returns>
    public ProjectSimulatedBuildHandler GetProjectAnalysisContext(string projectFile)
    {
        string configuration;
        string platform;
        if (Solution != null)
        {
            (configuration, platform) =
                Solution.GetProjectConfiguration(projectFile, Configuration, Platform);
        }
        else
        {
            (configuration, platform) = (Configuration, Platform);
        }
        return new ProjectSimulatedBuildHandler(_buildalyzerProvider, projectFile, _options.MsBuildPath, (configuration,
            platform, _options.TargetFramework), _logger, this);
    }
}
