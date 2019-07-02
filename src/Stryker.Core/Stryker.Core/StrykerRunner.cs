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
        private IReporter Reporter { get; set; }
        private IInitialisationProcess InitialisationProcess { get; set; }
        private MutationTestInput Input { get; set; }
        private IMutationTestProcess MutationTestProcess { get; set; }
        private IFileSystem FileSystem { get; }

        public StrykerRunner(IInitialisationProcess initialisationProcess = null, IMutationTestProcess mutationTestProcess = null, IFileSystem fileSystem = null)
        {
            InitialisationProcess = initialisationProcess;
            MutationTestProcess = mutationTestProcess;
            FileSystem = fileSystem ?? new FileSystem();
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
            FileSystem.Directory.CreateDirectory(options.OutputPath);
            FileSystem.File.Create(Path.Combine(options.OutputPath, ".gitignore")).Close();
            using (var file = FileSystem.File.CreateText(Path.Combine(options.OutputPath, ".gitignore")))
            {
                file.WriteLine("*");
            }

            // setup logging
            ApplicationLogging.ConfigureLogger(options.LogOptions);
            var logger = ApplicationLogging.LoggerFactory.CreateLogger<StrykerRunner>();

            logger.LogDebug("Stryker started with options: {0}",
                JsonConvert.SerializeObject(options, new StringEnumConverter()));

            try
            {
                // initialize 
                Reporter = ReporterFactory.Create(options);
                InitialisationProcess = InitialisationProcess ?? new InitialisationProcess();
                Input = InitialisationProcess.Initialize(options);

                MutationTestProcess = MutationTestProcess ?? new MutationTestProcess(
                    mutationTestInput: Input,
                    reporter: Reporter,
                    mutationTestExecutor: new MutationTestExecutor(Input.TestRunner));

                // initial test
                Input.TimeoutMs = InitialisationProcess.InitialTest(options);

                // mutate
                MutationTestProcess.Mutate(options);

                // coverage
                var coverage = InitialisationProcess.GetCoverage(options);

                MutationTestProcess.Optimize(coverage);

                // test mutations and return results
                return MutationTestProcess.Test(options);
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
