using System.Collections.Generic;
using System.IO;
using System.Linq;
using Buildalyzer;
using Buildalyzer.Environment;
using Microsoft.Extensions.Logging;
using Stryker.Abstractions.Exceptions;
using Stryker.Abstractions.Options;
using Stryker.Utilities.Buildalyzer;

namespace Stryker.Core.Initialisation;

public class ProjectAnalyzerContext
{
    private readonly IProjectAnalyzer _analyzer;
    private readonly TargetsForMutation _targetsForMutation;
    private readonly IStrykerOptions _options;
    private readonly string _configuration;
    private readonly string _platform;
    private readonly string _framework;
    private readonly ILogger _logger;
    private readonly StringWriter _buildLogger;
    private IAnalyzerResults _analyzerLastResults;
    private string[] _targetFrameworks;

    public ProjectAnalyzerContext(IBuildalyzerProvider buildalyzerProvider,
        string projectFile,
        IStrykerOptions options,
        string configuration,
        string platform,
        string framework,
        ILogger logger,
        TargetsForMutation targetsForMutation)
    {
        _buildLogger = new StringWriter();
        var manager = buildalyzerProvider.Provide(new AnalyzerManagerOptions{LogWriter = _buildLogger});
        var analyzer = manager.GetProject(projectFile);

        _analyzer = analyzer;
        ProjectFileName = projectFile;
        _targetsForMutation = targetsForMutation;
        _options = options;
        _configuration = configuration;
        _platform = platform;
        _framework = framework;
        _logger = logger;
    }

    public string LastBuildLog => _buildLogger.ToString();

    public string ProjectFileName { get; }

    public IAnalyzerResults Analyze(bool withRestore = false, bool forceFramework = false)
    {
        if (withRestore && _analyzerLastResults?.Any(ar => ar.TargetsFullFramework()) == true)
        {
            _targetsForMutation.RestoreSolution(_analyzerLastResults);
        }
        _buildLogger.GetStringBuilder().Clear();
        var env = new EnvironmentOptions
        {
            Restore = withRestore
        };
        if (!string.IsNullOrEmpty(_options.MsBuildPath))
        {
            // we need to forward this path to buildalyzer
            env.EnvironmentVariables[EnvironmentVariables.MSBUILD_EXE_PATH] = _options.MsBuildPath;
        }
        if (!string.IsNullOrEmpty(_configuration))
        {
            env.GlobalProperties["Configuration"] = _configuration;
        }
        if (!string.IsNullOrEmpty(_platform))
        {
            env.GlobalProperties["Platform"] = _platform;
        }

        _analyzerLastResults = forceFramework ? _analyzer.Build(_options.TargetFramework, env) : _analyzer.Build(env);
        InitializeTargetFrameworks();
        return _analyzerLastResults;
    }

    private void InitializeTargetFrameworks()
    {
        var projectFileTargetFrameworks = _analyzer.ProjectFile.TargetFrameworks;
        if (projectFileTargetFrameworks.Length > 0)
        {
            _logger.LogDebug("Project {ProjectFilePath} supported frameworks: {FrameworkList}.", ProjectFileName, string.Join(',', projectFileTargetFrameworks));
        }
        else
        {
            if (!string.IsNullOrEmpty(_options.TargetFramework))
            {
                projectFileTargetFrameworks=[_options.TargetFramework];
                _logger.LogWarning("Failed to identify target frameworks for project {ProjectFilePath}. Assuming selected framework ({framework}) is present.", ProjectFileName, _options.TargetFramework);
            }
            else
            {
                projectFileTargetFrameworks = _analyzerLastResults.Select(br => br.TargetFramework).ToArray();
                _logger.LogWarning("Failed to identify target frameworks for project {ProjectFilePath}. Using analysis results: {frameworks}", ProjectFileName, string.Join(',', projectFileTargetFrameworks));
            }
        }

        _targetFrameworks = projectFileTargetFrameworks;
    }

    public IEnumerable<string> FailedFrameworks => _targetFrameworks.Where(tf =>
        !_analyzerLastResults.Any( ar => ar.TargetFramework == tf && ar.IsValid()));

    public bool HasValidResults() => _analyzerLastResults != null && _analyzerLastResults.IsValidFor(_targetFrameworks);

    private IAnalyzerResult SelectAnalyzerResult(IEnumerable<IAnalyzerResult> analyzerResults, string targetFramework)
    {
        var validResults = analyzerResults.ToList();
        if (validResults.Count == 0)
        {
            throw new InputException($"No valid project analysis results could be found for '{ProjectFileName}'.");
        }

        if (targetFramework is null)
        {
            // we try to avoid desktop versions
            return PickFrameworkVersion();
        }

        var resultForRequestedFramework = validResults.Find(a => a.TargetFramework == targetFramework);
        if (resultForRequestedFramework is not null)
        {
            return resultForRequestedFramework;
        }
        // if there is only one available framework version, we log an info
        if (validResults.Count == 1)
        {
            var singleAnalyzerResult = validResults[0];
            _logger.LogInformation(
                "Could not find a valid analysis for target {0} for project '{1}'. Selected version is {2}.",
                targetFramework, ProjectFileName, singleAnalyzerResult.TargetFramework);
            return singleAnalyzerResult;
        }

        var firstAnalyzerResult = PickFrameworkVersion();
        var availableFrameworks = validResults.Select(a => a.TargetFramework).Distinct();
        _logger.LogWarning(
            """
            Could not find a valid analysis for target {0} for project '{1}'.
            The available target frameworks are: {2}.
                 selected version is {3}.
            """, targetFramework, ProjectFileName, string.Join(',', availableFrameworks), firstAnalyzerResult.TargetFramework);

        return firstAnalyzerResult;

        IAnalyzerResult PickFrameworkVersion()
        {
            return validResults.Find(a => a.IsValid() && !a.TargetsFullFramework()) ?? validResults[0];
        }
    }

}
