using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Buildalyzer;
using Buildalyzer.Environment;
using Microsoft.Extensions.Logging;
using Stryker.Abstractions.Exceptions;
using Stryker.Utilities.Buildalyzer;

namespace Stryker.Core.Initialisation;

public class ProjectAnalyzerContext
{
    private readonly IProjectAnalyzer _analyzer;
    private readonly TargetsForMutation _targetsForMutation;
    private readonly string _msBuildPath;
    private readonly string _configuration;
    private readonly string _platform;
    private readonly string _framework;
    private readonly ILogger _logger;
    private readonly StringWriter _buildLogger;
    public IAnalyzerResults AnalyzerLastResults { get; private set; }
    private IEnumerable<IAnalyzerResults> _filteredResults;
    private string[] _targetFrameworks;

    public ProjectAnalyzerContext(IBuildalyzerProvider buildalyzerProvider,
        string projectFile,
        string msBuildPath,
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
        _msBuildPath = msBuildPath;
        _configuration = configuration;
        _platform = platform;
        _framework = framework;
        _logger = logger;
    }

    public string LastBuildLog => _buildLogger.ToString();

    public string ProjectFileName { get; }

    public IAnalyzerResults Analyze(bool withRestore = false, bool forceFramework = false)
    {
        if (withRestore && AnalyzerLastResults?.Any(ar => ar.TargetsFullFramework()) == true)
        {
            _targetsForMutation.RestoreSolution(AnalyzerLastResults);
        }
        _buildLogger.GetStringBuilder().Clear();
        var env = new EnvironmentOptions
        {
            Restore = withRestore
        };
        if (!string.IsNullOrEmpty(_msBuildPath))
        {
            // we need to forward this path to buildalyzer
            env.EnvironmentVariables[EnvironmentVariables.MSBUILD_EXE_PATH] = _msBuildPath;
        }
        if (!string.IsNullOrEmpty(_configuration))
        {
            env.GlobalProperties["Configuration"] = _configuration;
        }
        if (!string.IsNullOrEmpty(_platform))
        {
            env.GlobalProperties["Platform"] = _platform;
        }

        AnalyzerLastResults = forceFramework ? _analyzer.Build(_framework, env) : _analyzer.Build(env);
        InitializeTargetFrameworks();
        return AnalyzerLastResults;
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
            if (!string.IsNullOrEmpty(_framework))
            {
                projectFileTargetFrameworks=[_framework];
                _logger.LogWarning("Failed to identify target frameworks for project {ProjectFilePath}. Assuming selected framework ({framework}) is present.", ProjectFileName, _framework);
            }
            else
            {
                projectFileTargetFrameworks = AnalyzerLastResults.Select(br => br.TargetFramework).ToArray();
                _logger.LogWarning("Failed to identify target frameworks for project {ProjectFilePath}. Using analysis results: {frameworks}", ProjectFileName, string.Join(',', projectFileTargetFrameworks));
            }
        }

        _targetFrameworks = projectFileTargetFrameworks;
    }

    public IEnumerable<string> FailedFrameworks => _targetFrameworks.Where(tf =>
        !AnalyzerLastResults.Any( ar => ar.TargetFramework == tf && ar.IsValid()));

    public bool IsTest => AnalyzerLastResults?.IsTestProject() == true;

    public bool HasValidResults() => AnalyzerLastResults != null && AnalyzerLastResults.IsValidFor(_targetFrameworks);

    public bool IsTestProject() => AnalyzerLastResults != null && AnalyzerLastResults.IsTestProject();

    public IEnumerable<string> GetProjectReferences() => AnalyzerLastResults?.SelectMany(r => r.ProjectReferences).Distinct();

    public IAnalyzerResult SelectAnalyzerResult()
    {
        var validResults = AnalyzerLastResults.ToList();
        if (validResults.Count == 0)
        {
            throw new InputException($"No valid project analysis results could be found for '{ProjectFileName}'.");
        }

        if (_framework is null)
        {
            // we try to avoid desktop versions
            return PickFrameworkVersion();
        }

        var resultForRequestedFramework = validResults.Find(a => a.TargetFramework == _framework);
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
                _framework, ProjectFileName, singleAnalyzerResult.TargetFramework);
            return singleAnalyzerResult;
        }

        var firstAnalyzerResult = PickFrameworkVersion();
        var availableFrameworks = validResults.Select(a => a.TargetFramework).Distinct();
        _logger.LogWarning(
            """
            Could not find a valid analysis for target {0} for project '{1}'.
            The available target frameworks are: {2}.
                 selected version is {3}.
            """, _framework, ProjectFileName, string.Join(',', availableFrameworks), firstAnalyzerResult.TargetFramework);

        return firstAnalyzerResult;

        IAnalyzerResult PickFrameworkVersion()
        {
            return validResults.Find(a => a.IsValid() && !a.TargetsFullFramework()) ?? validResults[0];
        }
    }

    private static readonly HashSet<string> ImportantProperties =
        ["Configuration", "Platform", "AssemblyName", "Configurations", "TargetPath", "OS"];

    public void LogAnalyzerResult()
    {
        var log = new StringBuilder();
        try
        {
            log.AppendLine("**** Buildalyzer result ****");
            log.AppendLine($"Project: {ProjectFileName}");
            if (AnalyzerLastResults.Count == 0)
            {
                _logger.LogTrace("No analyzer results to log. This indicates an early failure in analysis, check build log for details.");
                return;
            }
            // dump all properties as it can help diagnosing build issues for user project.
            foreach (var analyzerResult in AnalyzerLastResults)
            {
                log.AppendLine($"TargetFramework: {analyzerResult.TargetFramework}");
                log.AppendLine($"Succeeded: {analyzerResult.Succeeded}");
                log.AppendLine($"Compiler command: {analyzerResult.Command}");

                var properties = analyzerResult.Properties;
                foreach (var property in ImportantProperties)
                {
                    log.AppendLine($"Property {property}={properties.GetValueOrDefault(property) ?? "\"'undefined'\""}");
                }
                if (analyzerResult.SourceFiles.Length == 0)
                {
                    log.AppendLine("** No source files identified **");
                }
                else
                {
                    log.AppendLine(
                        $"{analyzerResult.SourceFiles.Length} source files: {string.Join(',', analyzerResult.SourceFiles)}");
                }
                if (analyzerResult.References.Length == 0)
                {
                    log.AppendLine("** No references Identified **");
                }
                else
                {
                    foreach (var reference in analyzerResult.References)
                    {
                        log.AppendLine($"References: {Path.GetFileName(reference)} (in {Path.GetDirectoryName(reference)})");
                    }
                }

                if (_logger.IsEnabled(LogLevel.Trace))
                {
                    // dumps all other properties as well, as they can be useful for diagnosing build issues
                    foreach (var property in properties.Where( p => !ImportantProperties.Contains(p.Key) ))
                    {
                        log.AppendLine($"Property {property.Key}={property.Value.Replace(Environment.NewLine, "\\n")}");
                    }
                }

                log.AppendLine();
            }
        }
        finally
        {
            log.AppendLine("**** End Buildalyzer result ****");
            _logger.LogDebug(log.ToString());
        }
    }

    public bool BuildsAnAssembly() => AnalyzerLastResults?.Any(p => p.BuildsAnAssembly()) == true;

    public bool FindMatchingVariant(string assemblyPath, out IAnalyzerResult analyzerResult)
    {
        analyzerResult= AnalyzerLastResults.FirstOrDefault( r=>
                            string.Compare(assemblyPath, r.GetAssemblyFileName(), StringComparison.OrdinalIgnoreCase) == 0
                || string.Compare(assemblyPath, r.GetReferenceAssemblyPath(), StringComparison.OrdinalIgnoreCase) == 0);
        return analyzerResult != null;
    }
}
