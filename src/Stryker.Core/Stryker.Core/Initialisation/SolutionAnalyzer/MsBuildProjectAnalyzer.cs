using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.Build.Construction;
using Microsoft.Build.Definition;
using Microsoft.Build.Evaluation;
using Microsoft.Build.Exceptions;
using Microsoft.Build.Execution;
using Microsoft.Build.Framework;
using Microsoft.Build.Logging;
using Microsoft.Extensions.Logging;
using Stryker.Core.Logging;

namespace Stryker.Core.Initialisation.ProjectAnalyzer
{
    public class MsBuildProjectAnalyzer : IProjectAnalyzer
    {
        private Project _project;
        private readonly Microsoft.Extensions.Logging.ILogger _logger = ApplicationLogging.LoggerFactory.CreateLogger<MsBuildProjectAnalyzer>();
        private static object _lock = new object();

        public MsBuildProjectAnalyzer(ProjectInSolution projectInSolution)
        {
            string slnConfig = "Debug|Any CPU";
            var props = new Dictionary<string, string>()
            {
                {"Configuration", projectInSolution.ProjectConfigurations[slnConfig].ConfigurationName },
                {"Platform", projectInSolution.ProjectConfigurations[slnConfig].PlatformName },
                {"AbsolutePath", projectInSolution.AbsolutePath },
                {"RelativePath", projectInSolution.RelativePath }
            };
            LoadProject(props);
        }

        public MsBuildProjectAnalyzer(string projectFile)
        {
            var props = new Dictionary<string, string>()
            {
                {"Configuration", @"Debug" },
            //  {"Platform", projectInSolution.ProjectConfigurations[slnConfig].PlatformName },
                {"AbsolutePath", projectFile },
            };
            LoadProject(props);
        }

        public string ProjectFilePath { get { return _project.FullPath; } }


        private void LoadProject(Dictionary<string, string> props)
        {
            var projectOptions = new ProjectOptions() { GlobalProperties = props };

            try
            {
                _project = ProjectCollection.GlobalProjectCollection.LoadedProjects.FirstOrDefault(d => d.FullPath == props[@"AbsolutePath"]) ??
                                Project.FromFile(props[@"AbsolutePath"], new ProjectOptions());
            }
            catch (InvalidProjectFileException ex)
            {
                // Old project types such as ASP.NET 5 may not supported, based on what MSBuildLocator finds.
                // This can also happen when the SDK loaded is newer than BuildLocator's knowledge.  For example, BuildLocator does not know of NET5 until version 1.3.2.
                _logger.LogWarning(@"ERROR: " + props["RelativePath"] + ex.Message);
                _logger.LogWarning(@"This may be an old project type, or there is a problem with the SDK that MsBuildLocator chose.");
            }
        }

        private IReadOnlyDictionary<string, string> GetProperties()
        {
            var properties = new Dictionary<string, string>();
            if (_project != null)
            {
                foreach (ProjectProperty property in _project.AllEvaluatedProperties)
                {
                    // Note: this can have duplicates
                    properties.TryAdd(property.Name, property.EvaluatedValue);
                }
            }
            return properties;
        }

        private List<string> GetProjectReferences()
        {
            var references = new List<string>();
            if (_project != null)
            {
                foreach (ProjectItem item in _project.AllEvaluatedItems.Where(x => x.ItemType == @"ProjectReference"))
                {
                    // ProjectReference is a relative path
                    string projectReference = Path.Combine(_project.DirectoryPath, item.EvaluatedInclude);
                    projectReference = Path.GetFullPath(projectReference);
                    references.Add(projectReference);
                }
            }
            return references;
        }

        private List<string> GetAnalyzerReferences()
        {
            var references = new List<string>();
            if (_project != null)
            {
                foreach (ProjectItem item in _project.AllEvaluatedItems.Where(x => x.ItemType == @"Analyzer"))
                {
                    references.Add(item.EvaluatedInclude);
                }
            }
            return references;
        }
        private List<string> GetReferences()
        {
            var references = new List<string>();
            if (_project != null)
            {
                //TODO: We use Parallel.ForEach..., but only 1 BuildManager behind the scenes
                lock (_lock)
                {
                    //TODO: cache results
                    var manager = BuildManager.DefaultBuildManager;

                    var target = @"ResolveReferences";
                    var projectInstance = new ProjectInstance(ProjectFilePath);
                    var buildResult = manager.Build(
                        new BuildParameters()
                        {
                            DetailedSummary = true
                        },
                        new BuildRequestData(projectInstance, new string[] { target })
                    );

                    foreach (var item in buildResult[target].Items)
                    {
                        references.Add(item.ItemSpec);
                    }
                }
            }
            return references;
        }

        private List<string> GetPreprocessorValues()
        {
            var references = new List<string>();
            if (_project != null)
            {
                foreach (ProjectItem item in _project.AllEvaluatedItems.Where(x => x.ItemType == @"PreprocessorValue"))
                {
                    references.Add(item.EvaluatedInclude);
                }
            }
            return references;
        }


        private List<string> GetSourceFiles()
        {
            var sourceFiles = new List<string>();
            if (_project != null)
            {
                foreach (ProjectItem item in _project.AllEvaluatedItems.Where(x => x.ItemType == @"Compile"))
                {
                    string sourceFile = Path.Combine(_project.DirectoryPath, item.EvaluatedInclude);
                    sourceFiles.Add(sourceFile);
                }
            }
            return sourceFiles;
        }

        private string GetTargetFramework()
        {
            return _project.AllEvaluatedProperties.SingleOrDefault(x => x.Name == @"TargetFramework").EvaluatedValue;
        }


        public IAnalysisResult Analyze(string targetFramework)
        {
            string projectFilePath = _project.FullPath;
            IEnumerable<string> projectReferences = GetProjectReferences();
            IEnumerable<string> analyzerReferences = GetAnalyzerReferences();
            IEnumerable<string> references = GetReferences();
            IEnumerable<string> preprocessorValues = GetPreprocessorValues();
            IReadOnlyDictionary<string, string> properties = GetProperties();
            string[] sourceFiles = GetSourceFiles().ToArray();
            string targetFramework2 = GetTargetFramework(); // TODO: Filter by TargetFramework rather than everything munged together
            bool succeeded = _project != null;

            var result = new AnalyzerResult(projectFilePath, references, projectReferences, analyzerReferences, preprocessorValues, properties, sourceFiles, succeeded, targetFramework2);
            result.Log();
            return result;
        }
    }
}
