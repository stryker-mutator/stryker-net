using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Stryker.Core.Exceptions;
using Stryker.Core.Initialisation;
using Stryker.Core.Logging;
using Stryker.Core.MutationTest;
using Stryker.Core.Options;
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
        private IInitialisationProcess _initialisationProcess;
        private MutationTestInput _input;
        private IMutationTestProcess _mutationTestProcess;
        private readonly IFileSystem _fileSystem;

        public StrykerRunner(IInitialisationProcess initialisationProcess = null, IMutationTestProcess mutationTestProcess = null, IFileSystem fileSystem = null)
        {
            _initialisationProcess = initialisationProcess;
            _mutationTestProcess = mutationTestProcess;
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

            try
            {
                // initialize
                _reporter = ReporterFactory.Create(options);
                _initialisationProcess = _initialisationProcess ?? new InitialisationProcess();
                _input = _initialisationProcess.Initialize(options);

                _mutationTestProcess = _mutationTestProcess ?? new MutationTestProcess(
                    mutationTestInput: _input,
                    reporter: _reporter,
                    mutationTestExecutor: new MutationTestExecutor(_input.TestRunner),
                    options: options);

                // initial test
                _input.TimeoutMs = _initialisationProcess.InitialTest(options, out var nbTests);

                // mutate
                _mutationTestProcess.Mutate();

                if (options.Optimizations.HasFlag(OptimizationFlags.SkipUncoveredMutants) || options.Optimizations.HasFlag(OptimizationFlags.CoverageBasedTest))
                {
                    logger.LogInformation($"Capture mutant coverage using '{options.OptimizationMode}' mode.");
                    // coverage
                    var coverage = _initialisationProcess.GetCoverage(options);
                    coverage.UpdateMutants(_input.ProjectInfo.ProjectContents.Mutants, nbTests);
                }

                // test mutations and return results
                return _mutationTestProcess.Test(options);
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
    }
}
