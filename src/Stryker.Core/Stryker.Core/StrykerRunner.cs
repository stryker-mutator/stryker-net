using Microsoft.Extensions.Logging;
using Stryker.Core.Initialisation;
using Stryker.Core.Logging;
using Stryker.Core.MutationTest;
using Stryker.Core.Mutators;
using Stryker.Core.Options;
using Stryker.Core.Reporters;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Stryker.Core
{
    public class StrykerRunner : IStrykerRunner
    {
        private IReporter _reporter { get; set; }
        private IInitialisationProcess _initialisationProcess { get; set; }
        private string _basePath { get; set; }
        private MutationTestInput _input { get; set; }
        private IMutationTestProcess _mutationTestProcess { get; set; }

        public StrykerRunner(IInitialisationProcess initialisationProcess = null, IMutationTestProcess mutationTestProcess = null)
        {
            _initialisationProcess = initialisationProcess;
            _mutationTestProcess = mutationTestProcess;
        }

        /// <summary>
        /// Starts a mutation test run
        /// </summary>
        /// <exception cref="StrykerException">For managed exceptions</exception>
        /// <param name="options">The user options</param>
        public void RunMutationTest(StrykerOptions options)
        {
            // start stopwatch
            var stopwatch = new Stopwatch();
            stopwatch.Start();

            // setup logging
            ApplicationLogging.ConfigureLogger(options.LogOptions);
            var logger = ApplicationLogging.LoggerFactory.CreateLogger<StrykerRunner>();

            try
            {
                // validate options
                var validatedOptions = new StrykerOptionsValidator().Validate(options);

                // initialyze 
                _reporter = ReporterFactory.Create(validatedOptions.Reporter);
                _initialisationProcess = _initialisationProcess ?? new InitialisationProcess(_reporter);
                _input = _initialisationProcess.Initialize(validatedOptions);

                _mutationTestProcess = _mutationTestProcess ?? new MutationTestProcess(
                    mutationTestInput: _input,
                    mutators: new List<IMutator> ()
                        {
                            // the default list of mutators
                            new BinaryExpressionMutator(),
                            new BooleanMutator(),
                            new AssignmentStatementMutator(),
                            new PrefixUnaryMutator(),
                            new PostfixUnaryMutator(),
                            new CheckedMutator()
                        },
                    reporter: _reporter,
                    mutationTestExecutor: new MutationTestExecutor(_input.TestRunner, _input.TimeoutMS));

                // mutate
                _mutationTestProcess.Mutate();

                // test mutations
                _mutationTestProcess.Test();
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "An error occurred during the mutationtest run ");
                throw;
            }
            finally {
                // log duration
                stopwatch.Stop();
                logger.LogInformation("Time Elapsed {0}", stopwatch.Elapsed);
            }
        }
    }
}
