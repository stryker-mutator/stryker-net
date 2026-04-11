using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Buildalyzer;
using Buildalyzer.Environment;
using Microsoft.Extensions.Logging;
using Stryker.Utilities.Buildalyzer;

namespace Stryker.Core.Initialisation;

/// <summary>
/// Encapsulates the context of a project analysis, including the project analyzer, the last analysis results
///  for said project. It also provides reference discovery methods
/// </summary>
public class ProjectSimulatedBuildHandler
{
    private readonly IProjectAnalyzer _analyzer;
    private readonly ProjectsTracker _projectsTracker;
    private readonly string _msBuildPath;
    private readonly string _configuration;
    private readonly string _platform;
    private readonly string? _framework;
    private readonly ILogger _logger;
    private readonly StringWriter _buildLogger;
    private string[] _targetFrameworks=[];

    public ProjectSimulatedBuildHandler(IBuildalyzerProvider buildalyzerProvider,
        string projectFile,
        string msBuildPath,
        (string configuration,
        string platform,
        string? framework) target,
        ILogger logger,
        ProjectsTracker projectsTracker)
    {
        _buildLogger = new StringWriter();
        var manager = buildalyzerProvider.Provide(new AnalyzerManagerOptions{LogWriter = _buildLogger});
        var analyzer = manager.GetProject(projectFile);

        _analyzer = analyzer;
        ProjectFileName = projectFile;
        _projectsTracker = projectsTracker;
        _msBuildPath = msBuildPath;
        _configuration = target.configuration;
        _platform = target.platform;
        _framework = target.framework;
        _logger = logger;
    }

    public IAnalyzerResults AnalyzerLastResults { get; private set; } = new AnalyzerResults();

    public string LastBuildLog => _buildLogger.ToString();

    public string ProjectFileName { get; }

    public IAnalyzerResults Analyze(bool withRestore = false, bool forceFramework = false)
    {
        if (forceFramework && string.IsNullOrEmpty(_framework))
        {
            throw new InvalidOperationException("Cannot force framework when no framework is specified in options.");
        }

        if (withRestore && AnalyzerLastResults.Any(ar => ar.TargetsFullFramework()))
        {
            _projectsTracker.RestoreSolution(AnalyzerLastResults);
            withRestore= false;
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
                _logger.LogWarning("Failed to identify target frameworks for project {ProjectFilePath}. Assuming selected framework ({Framework}) is present.", ProjectFileName, _framework);
            }
            else
            {
                projectFileTargetFrameworks = AnalyzerLastResults.Select(br => br.TargetFramework).ToArray();
                _logger.LogWarning("Failed to identify target frameworks for project {ProjectFilePath}. Using analysis results: {Frameworks}", ProjectFileName, string.Join(',', projectFileTargetFrameworks));
            }
        }

        _targetFrameworks = projectFileTargetFrameworks;
    }

    public bool IsNetFramework => _analyzer.ProjectFile.RequiresNetFramework;

    public IEnumerable<string> FailedFrameworks => _targetFrameworks?.Where(tf =>
        !AnalyzerLastResults.Any( ar => ar.TargetFramework == tf && ar.IsValid())) ?? [];

    public bool IsTest => AnalyzerLastResults.IsTestProject();

    public bool HasValidResults() => AnalyzerLastResults.IsValidFor(_targetFrameworks);

    public bool IsTestProject() => AnalyzerLastResults.IsTestProject();

    public bool BuildsAnAssembly() => AnalyzerLastResults.Any(p => p.BuildsAnAssembly());

    public IEnumerable<string> GetProjectReferences() => AnalyzerLastResults.SelectMany(r => r.ProjectReferences).Distinct();

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
                DumpTestAnalyzerResult(log, analyzerResult);
            }
        }
        finally
        {
            log.AppendLine("**** End Buildalyzer result ****");
            _logger.LogDebug(log.ToString());
        }
    }

    private void DumpTestAnalyzerResult(StringBuilder log, IAnalyzerResult analyzerResult)
    {
        log.AppendLine($"TargetFramework: {analyzerResult.TargetFramework}");
        log.AppendLine($"Simulated build: {(analyzerResult.Succeeded ? "succeeded": "failed")}");
        log.AppendLine($"Stryker analysis: {(analyzerResult.IsValid() ? "succeeded": "failed")}");

        var properties = analyzerResult.Properties;
        foreach (var property in ImportantProperties)
        {
            log.AppendLine($"Property {property}={properties.GetValueOrDefault(property) ?? "\"'undefined'\""}");
        }
        DumpSourceFiles(log, analyzerResult);
        DumpReferences(log, analyzerResult);

        log.AppendLine($"Compiler command: {analyzerResult.Command}");

        if (_logger.IsEnabled(LogLevel.Trace))
        {
            // dumps all other properties as well, as they can be useful for diagnosing build issues
            var propertiesString = string.Join(", ", properties.
                Where(p => !ImportantProperties.Contains(p.Key)).
                Select(p => $"{p.Key}={p.Value.Replace(Environment.NewLine, "\\n")}"));
            log.AppendLine($"Other properties: {propertiesString}.");
        }

        log.AppendLine();
    }

    private static void DumpSourceFiles(StringBuilder log, IAnalyzerResult analyzerResult)
    {
        if (analyzerResult.SourceFiles.Length == 0)
        {
            log.AppendLine("** No source files identified **");
        }
        else
        {
            log.AppendLine(
                $"{analyzerResult.SourceFiles.Length} source files: {string.Join(',', analyzerResult.SourceFiles)}");
        }
    }

    private static void DumpReferences(StringBuilder log, IAnalyzerResult analyzerResult)
    {
        if (analyzerResult.References.Length == 0)
        {
            log.AppendLine("** No references Identified **");
        }
        else
        {
            foreach (var reference in analyzerResult.References)
            {
                var aliasText = string.Empty;
                if (analyzerResult.ReferenceAliases.TryGetValue(reference, out var aliases) && aliases.Length > 0)
                {
                    aliasText = $" aliases: {string.Join(", ", aliases)}";
                }
                log.AppendLine($"References: {Path.GetFileName(reference)}{aliasText} (in {Path.GetDirectoryName(reference)})");
            }
        }
    }

    public bool FindMatchingVariant(string assemblyPath, out IAnalyzerResult? analyzerResult)
    {
        analyzerResult= AnalyzerLastResults.FirstOrDefault( r=>
                            string.Compare(assemblyPath, r.GetAssemblyPath(), StringComparison.OrdinalIgnoreCase) == 0
                || string.Compare(assemblyPath, r.GetReferenceAssemblyPath(), StringComparison.OrdinalIgnoreCase) == 0);
        return analyzerResult != null;
    }
}
