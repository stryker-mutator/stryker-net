using System;
using System.Collections.Generic;
using System.Linq;
using Buildalyzer;
using Microsoft.Extensions.Logging;
using Stryker.Abstractions.Options;
using Stryker.Solutions;
using Stryker.Utilities.Buildalyzer;

namespace Stryker.Core.Initialisation;

public class TargetsForMutation
{
    private List<string> _selectedProjects = [];
    private bool _solutionRestored;

    private readonly IStrykerOptions _options;
    private readonly INugetRestoreProcess _nugetRestoreProcess;
    private readonly IBuildalyzerProvider _buildalyzerProvider;
    private readonly ILogger _logger;

    public TargetsForMutation(SolutionFile? file, IStrykerOptions options, IBuildalyzerProvider buildalyzerProvider ,ILogger logger, INugetRestoreProcess nugetRestoreProcess)
    {
        _options = options;
        _buildalyzerProvider = buildalyzerProvider;
        _logger = logger;
        _nugetRestoreProcess = nugetRestoreProcess;
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
            if (_solutionRestored)
            {
                return;
            }

            if (Environment.OSVersion.Platform == PlatformID.Win32NT)
            {
                _logger.LogWarning("Project  analysis failed. Stryker will retry after a solution level nuget restore");
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

    /// <summary>
    /// Gets a project analysis context for the given project file, using the configuration and platform from the solution if available, otherwise using the configuration and platform from the options.
    /// </summary>
    /// <param name="projectFile">target project file</param>
    /// <returns>a <see cref="ProjectAnalyzerContext"/> instance for <param name="projectFile">.</param></returns>
    public ProjectAnalyzerContext GetProjectAnalysisContext(string projectFile)
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
        return new ProjectAnalyzerContext(_buildalyzerProvider, projectFile, _options.MsBuildPath, (configuration,
            platform, _options.TargetFramework), _logger, this);
    }
}
