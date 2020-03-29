using Buildalyzer;
using Buildalyzer.Construction;
using Microsoft.Build.Construction;
using Microsoft.Extensions.Logging;
using Stryker.Core.Initialisation;
using Stryker.Core.Logging;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Stryker.Core.Testing
{
    /// <summary>
    /// This is an interface to mock buildalyzer classes
    /// </summary>
    public interface IBuildalyzerProvider
    {
        IBuildalyzer Provide(AnalyzerManagerOptions options = null);
        IBuildalyzer Provide(string solutionFilePath, AnalyzerManagerOptions options = null);
    }

    public class BuildalyzerProvider : IBuildalyzerProvider
    {
        public IBuildalyzer Provide(AnalyzerManagerOptions options = null)
        {
            return new Buildalyzer(options);
        }

        public IBuildalyzer Provide(string solutionFilePath, AnalyzerManagerOptions options = null)
        {
            return new Buildalyzer(solutionFilePath, options);
        }
    }

    public interface IBuildalyzer
    {
        ILoggerFactory LoggerFactory { get; }
        IReadOnlyDictionary<string, IBuildalyzerProjectAnalyzer> Projects { get; }
        SolutionFile SolutionFile { get; }
        string SolutionFilePath { get; }
        IBuildalyzerProjectAnalyzer GetProject(string projectFilePath);
        void RemoveGlobalProperty(string key);
        void SetEnvironmentVariable(string key, string value);
        void SetGlobalProperty(string key, string value);
    }

    public class Buildalyzer : IBuildalyzer
    {
        private AnalyzerManager manager;

        public Buildalyzer(AnalyzerManagerOptions options = null)
        {
            manager = new AnalyzerManager(options);
        }

        public Buildalyzer(string solutionFilePath, AnalyzerManagerOptions options = null)
        {
            manager = new AnalyzerManager(solutionFilePath, options);
        }

        public IReadOnlyDictionary<string, IBuildalyzerProjectAnalyzer> Projects => Cast(manager.Projects);
        public ILoggerFactory LoggerFactory => manager.LoggerFactory;
        public string SolutionFilePath => manager.SolutionFilePath;
        public SolutionFile SolutionFile => manager.SolutionFile;

        public IBuildalyzerProjectAnalyzer GetProject(string projectFilePath)
        {
            return new BuildalyzerProjectAnalyzer(manager.GetProject(projectFilePath));
        }

        public void RemoveGlobalProperty(string key)
        {
            manager.RemoveGlobalProperty(key);
        }

        public void SetEnvironmentVariable(string key, string value)
        {
            manager.SetEnvironmentVariable(key, value);
        }

        public void SetGlobalProperty(string key, string value)
        {
            manager.SetGlobalProperty(key, value);
        }

        private IReadOnlyDictionary<string, IBuildalyzerProjectAnalyzer> Cast(IReadOnlyDictionary<string, ProjectAnalyzer> source)
        {
            return source.Select(x =>
                new KeyValuePair<string, IBuildalyzerProjectAnalyzer>(x.Key, new BuildalyzerProjectAnalyzer(x.Value))
            ).ToDictionary(x => x.Key, x => x.Value);
        }
    }

    public interface IBuildalyzerProjectAnalyzer
    {
        IReadOnlyDictionary<string, string> EnvironmentVariables { get; }
        IReadOnlyDictionary<string, string> GlobalProperties { get; }
        ILogger<ProjectAnalyzer> Logger { get; }
        AnalyzerManager Manager { get; }
        ProjectFile ProjectFile { get; }
        ProjectInSolution ProjectInSolution { get; }
        string SolutionDirectory { get; }

        IProjectAnalyzerResult Build();
        void RemoveGlobalProperty(string key);
        void SetEnvironmentVariable(string key, string value);
        void SetGlobalProperty(string key, string value);
    }

    public class BuildalyzerProjectAnalyzer : IBuildalyzerProjectAnalyzer
    {
        private ProjectAnalyzer _analyzer;
        private ILogger<ProjectAnalyzer> _logger;

        public BuildalyzerProjectAnalyzer(ProjectAnalyzer projectAnalyzer)
        {
            _analyzer = projectAnalyzer;
            _logger = ApplicationLogging.LoggerFactory.CreateLogger<ProjectAnalyzer>();
        }

        public IReadOnlyDictionary<string, string> EnvironmentVariables => _analyzer.EnvironmentVariables;
        public IReadOnlyDictionary<string, string> GlobalProperties => _analyzer.GlobalProperties;
        public ProjectInSolution ProjectInSolution => _analyzer.ProjectInSolution;
        public string SolutionDirectory => _analyzer.SolutionDirectory;
        public ProjectFile ProjectFile => _analyzer.ProjectFile;
        public AnalyzerManager Manager => _analyzer.Manager;
        public ILogger<ProjectAnalyzer> Logger => _logger;

        public IProjectAnalyzerResult Build()
        {
            return new ProjectAnalyzerResult(_logger, _analyzer.Build().First());
        }

        public void RemoveGlobalProperty(string key)
        {
            _analyzer.RemoveGlobalProperty(key);
        }

        public void SetEnvironmentVariable(string key, string value)
        {
            _analyzer.SetEnvironmentVariable(key, value);
        }

        public void SetGlobalProperty(string key, string value)
        {
            _analyzer.SetGlobalProperty(key, value);
        }
    }
}
