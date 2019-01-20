using Microsoft.Extensions.Logging;
using Stryker.Core.Exceptions;
using Stryker.Core.Initialisation;
using Stryker.Core.Logging;
using Stryker.Core.MutationTest;
using Stryker.Core.Options;
using Stryker.Core.Reporters;
using System;
using System.Diagnostics;
using System.IO;
using System.IO.Abstractions;

namespace Stryker.Core
{
    public interface IStrykerRunner
    {
        StrykerRunResult RunMutationTest(StrykerOptions options);
    }

    public class StrykerRunner : IStrykerRunner
    {
        private IReporter _reporter { get; set; }
        private IInitialisationProcess _initialisationProcess { get; set; }
        private string _basePath { get; set; }
        private MutationTestInput _input { get; set; }
        private IMutationTestProcess _mutationTestProcess { get; set; }
        private IFileSystem _fileSystem { get; set; }

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
        public StrykerRunResult RunMutationTest(StrykerOptions options)
        {
            // start stopwatch
            var stopwatch = new Stopwatch();
            stopwatch.Start();

            // Create output dir with gitignore
            string strykerOutputDirectory = Path.Combine(options.BasePath, "StrykerOutput");
            _fileSystem.Directory.CreateDirectory(strykerOutputDirectory);
            _fileSystem.File.Create(Path.Combine(strykerOutputDirectory, ".gitignore")).Close();
            using (var file = _fileSystem.File.CreateText(Path.Combine(strykerOutputDirectory, ".gitignore")))
            {
                file.WriteLine("*");
            }

            // setup logging
            ApplicationLogging.ConfigureLogger(options.LogOptions);
            var logger = ApplicationLogging.LoggerFactory.CreateLogger<StrykerRunner>();

            try
            {
                // initialize 
                _reporter = ReporterFactory.Create(options);
                _initialisationProcess = _initialisationProcess ?? new InitialisationProcess();
                _input = _initialisationProcess.Initialize(options);

                _mutationTestProcess = _mutationTestProcess ?? new MutationTestProcess(
                    mutationTestInput: _input,
                    reporter: _reporter,
                    mutationTestExecutor: new MutationTestExecutor(_input.TestRunner));

                // mutate
                _mutationTestProcess.Mutate(options);

                // initial test
                _input.TimeoutMS =_initialisationProcess.InitialTest(options);
              
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
