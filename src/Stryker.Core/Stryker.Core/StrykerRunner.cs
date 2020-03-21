using Buildalyzer;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Stryker.Core.Exceptions;
using Stryker.Core.Initialisation;
using Stryker.Core.Logging;
using Stryker.Core.Mutants;
using Stryker.Core.MutationTest;
using Stryker.Core.Options;
using Stryker.Core.ProjectComponents;
using Stryker.Core.Reporters;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.Abstractions;
using System.Linq;

namespace Stryker.Core
{
    public interface IStrykerRunner
    {
        StrykerRunResult RunMutationTest(StrykerOptions options, IEnumerable<LogMessage> initialLogMessages = null);
    }

    public class StrykerRunner : IStrykerRunner
    {
        private IReporter _reporter;
        private ICollection<IMutationTestProcess> _mutationTestProcesses;
        private readonly IFileSystem _fileSystem;

        public StrykerRunner(IInitialisationProcess initialisationProcess = null, ICollection<IMutationTestProcess> mutationTestProcess = null, IFileSystem fileSystem = null)
        {
            //_initialisationProcess = initialisationProcess;
            _mutationTestProcesses = mutationTestProcess ?? new List<IMutationTestProcess>();
            _fileSystem = fileSystem ?? new FileSystem();
        }

        /// <summary>
        /// Starts a mutation test run
        /// </summary>
        /// <exception cref="StrykerInputException">For managed exceptions</exception>
        /// <param name="options">The user options</param>
        /// <param name="initialLogMessages">
        /// Allows to pass log messages that occured before the mutation test.
        /// The messages will be written to the logger after it was configured.
        /// </param>
        public StrykerRunResult RunMutationTest(StrykerOptions options, IEnumerable<LogMessage> initialLogMessages = null)
        {
            // start stopwatch
            var stopwatch = new Stopwatch();
            stopwatch.Start();

            // Create output dir with gitignore
            _fileSystem.Directory.CreateDirectory(options.OutputPath);
            _fileSystem.File.Create(Path.Combine(options.OutputPath, ".gitignore")).Close();
            using (var file = _fileSystem.File.CreateText(Path.Combine(options.OutputPath, ".gitignore")))
            {
                file.WriteLine("*");
            }

            // setup logging
            ApplicationLogging.ConfigureLogger(options.LogOptions, initialLogMessages);
            var logger = ApplicationLogging.LoggerFactory.CreateLogger<StrykerRunner>();

            logger.LogDebug("Stryker started with options: {0}",
                JsonConvert.SerializeObject(options, new StringEnumConverter()));

            _reporter = ReporterFactory.Create(options);

            try
            {
                if (options.SolutionPath != null)
                {
                    var manager = new AnalyzerManager(options.SolutionPath);

                    var projects = manager.Projects.Where(x => !x.Value.ProjectFile.PackageReferences.Any(y => y.Name.ToLower().Contains("test"))).Select(x => x.Value).ToList();
                    var testProjects = manager.Projects.Select(x => x.Value).Except(projects).Select(x => x.Build().Results.FirstOrDefault()).ToList();
                    foreach (var project in projects)
                    {
                        var relatedTestProjects = testProjects.Where(x => x.ProjectReferences.Any(y => y == project.ProjectFile.Path)).ToList();
                        if (relatedTestProjects.Any())
                        {
                            PrepareProject(options.ToProjectOptions(project.ProjectFile.Path, project.ProjectFile.Path, relatedTestProjects.Select(x => x.ProjectFilePath)));
                        }
                    }
                }

                var rootComponent = new FolderComposite();

                foreach(var project in _mutationTestProcesses)
                {
                    rootComponent.Add(project.Input.ProjectInfo.ProjectContents);
                }


                logger.LogInformation("{0} mutants ready for test", rootComponent.TotalMutants.Count());

                if (options.Optimizations.HasFlag(OptimizationFlags.SkipUncoveredMutants) || options.Optimizations.HasFlag(OptimizationFlags.CoverageBasedTest))
                {
                    logger.LogInformation($"Capture mutant coverage using '{options.OptimizationMode}' mode.");
                    // coverage
                    foreach (var project in _mutationTestProcesses)
                    {
                        project.GetCoverage();
                    }
                }
                _reporter.OnMutantsCreated(rootComponent);

                var allMutants = rootComponent.Mutants.ToList();
                var mutantsNotRun = allMutants.Where(x => x.ResultStatus == MutantStatus.NotRun).ToList();

                if (!mutantsNotRun.Any())
                {
                    if (allMutants.Any(x => x.ResultStatus == MutantStatus.Ignored))
                    {
                       logger.LogWarning("It looks like all mutants with tests were excluded. Try a re-run with less exclusion!");
                    }
                    if (allMutants.Any(x => x.ResultStatus == MutantStatus.NoCoverage))
                    {
                        logger.LogWarning("It looks like all non-excluded mutants are not covered by a test. Go add some tests!");
                    }
                    if (!allMutants.Any())
                    {
                        logger.LogWarning("It\'s a mutant-free world, nothing to test.");
                        return new StrykerRunResult(options, double.NaN);
                    }
                }

                var mutantsToTest = mutantsNotRun.Where(x => x.ResultStatus != MutantStatus.Ignored && x.ResultStatus != MutantStatus.NoCoverage);
                _reporter.OnStartMutantTestRun(mutantsNotRun);

                foreach (var project in _mutationTestProcesses)
                {
                    // test mutations
                    project.Test(options, project.Input.ProjectInfo.ProjectContents.Mutants.Where(x => x.ResultStatus == MutantStatus.NotRun).ToList());
                }

                _reporter.OnAllMutantsTested(rootComponent);

                return new StrykerRunResult(options, rootComponent.GetMutationScore());
            }
            catch (Exception ex) when (!(ex is StrykerInputException))
            {
                logger.LogError(ex, "An error occurred during the mutation test run ");
                throw;
            }
            finally
            {
                // log duration
                stopwatch.Stop();
                logger.LogInformation("Time Elapsed {0}", stopwatch.Elapsed);
            }
        }

        private void PrepareProject(StrykerProjectOptions options)
        {
            // initialize
            var initialisationProcess = new InitialisationProcess();
            var input = initialisationProcess.Initialize(options);

            var process = new MutationTestProcess(
                mutationTestInput: input,
                reporter: _reporter,
                mutationTestExecutor: new MutationTestExecutor(input.TestRunner),
                options: options);
            _mutationTestProcesses.Add(process);

            // initial test
            input.TimeoutMs = initialisationProcess.InitialTest(options, out var nbTests);

            // mutate
            process.Mutate();
        }
    }
}
