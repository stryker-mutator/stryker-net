using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Buildalyzer;
using Microsoft.Extensions.Logging;
using Stryker.Core.Exceptions;
using Stryker.Core.Initialisation.Buildalyzer;
using Stryker.Core.Logging;
using Stryker.Core.Testing;

namespace Stryker.Core.Initialisation;

public interface IProjectFileReader
{
    IAnalyzerManager GetAnalyzerManager(string solutionFilePath = null);
    IAnalyzerResult SelectAnalyzerResult(IEnumerable<IAnalyzerResult> analyzerResults, string targetFramework);
}

/// <summary>
/// This class is an abstraction of Buildalyzers AnalyzerManager to make the rest of our code better testable. Mocking AnalyzerManager is really ugly and we want to avoid it.
/// </summary>
public class ProjectFileReader : IProjectFileReader
{
    private readonly IBuildalyzerProvider _analyzerProvider;
    private IAnalyzerManager _analyzerManager;
    private readonly ILogger _logger;
    private readonly StringWriter _buildalyzerLog = new();

    public ProjectFileReader(IBuildalyzerProvider analyzerProvider = null)
    {
        _analyzerProvider = analyzerProvider ?? new BuildalyzerProvider();
        _logger = ApplicationLogging.LoggerFactory.CreateLogger<ProjectFileReader>();
    }

    public IAnalyzerManager GetAnalyzerManager(string solutionFilePath = null)
    {
        _analyzerManager ??= _analyzerProvider.Provide(solutionFilePath, new AnalyzerManagerOptions { LogWriter = _buildalyzerLog });
        return _analyzerManager;
    }

    public IAnalyzerResult SelectAnalyzerResult(IEnumerable<IAnalyzerResult> analyzerResults, string targetFramework)
    {
        var validResults = analyzerResults.Where(a => a.TargetFramework is not null).ToList();
        if (validResults.Count == 0)
        {
            _logger.LogError("Project analysis failed. The MsBuild log is below.");
            _logger.LogError(_buildalyzerLog.ToString());
            throw new InputException("No valid project analysis results could be found.");
        }

        if (targetFramework is null)
        {
            return validResults.First();
        }

        var resultForRequestedFramework = validResults.FirstOrDefault(a => a.TargetFramework == targetFramework);
        if (resultForRequestedFramework is not null)
        {
            return resultForRequestedFramework;
        }

        var firstAnalyzerResult = validResults.First();
        var availableFrameworks = validResults.Select(a => a.TargetFramework).Distinct();
        var firstFramework = firstAnalyzerResult.TargetFramework;
        _logger.LogWarning(
            $"Could not find a project analysis for the chosen target framework {targetFramework}. \n" +
            $"The available target frameworks are: {string.Join(',', availableFrameworks)}. \n" +
            $"The first available framework will be selected, which is {firstFramework}.");

        return firstAnalyzerResult;
    }

    private static readonly HashSet<string> importantProperties =
        ["Configuration", "Platform", "AssemblyName", "Configurations"];

    private void LogAnalyzerResult(IAnalyzerResult analyzerResult)
    {
        // do not log if trace is not enabled
        if (!_logger.IsEnabled(LogLevel.Trace))
        {
            return;
        }
        // dump all properties as it can help diagnosing build issues for user project.
        _logger.LogTrace("**** Buildalyzer result ****");

        _logger.LogTrace("Project: {0}", analyzerResult.ProjectFilePath);
        _logger.LogTrace("TargetFramework: {0}", analyzerResult.TargetFramework);
        _logger.LogTrace("Succeeded: {0}", analyzerResult.Succeeded);

        var properties = analyzerResult.Properties ?? new Dictionary<string, string>();
        foreach (var property in importantProperties)
        {
            _logger.LogTrace("Property {0}={1}", property, properties.GetValueOrDefault(property)??"'undefined'");
        }
        foreach (var sourceFile in analyzerResult.SourceFiles ?? Enumerable.Empty<string>())
        {
            _logger.LogTrace("SourceFile {0}", sourceFile);
        }
        foreach (var reference in analyzerResult.References ?? Enumerable.Empty<string>())
        {
            _logger.LogTrace("References: {0} (in {1})", Path.GetFileName(reference), Path.GetDirectoryName(reference));
        }

        foreach (var property in properties)
        {
            if (importantProperties.Contains(property.Key)) continue; // already logged 
            _logger.LogTrace("Property {0}={1}", property.Key, property.Value.Replace(Environment.NewLine, "\\n"));
        }

        _logger.LogTrace("**** Buildalyzer result ****");
    }
}
