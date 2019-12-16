﻿using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestPlatform.ObjectModel;
using Stryker.Core.Initialisation;
using Stryker.Core.Logging;
using Stryker.Core.Options;
using Stryker.Core.ToolHelpers;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Threading;
using System.Threading.Tasks;
using Stryker.Core.Mutants;

namespace Stryker.Core.TestRunners.VsTest
{
    public class VsTestRunnerPool : ITestRunner
    {
        private readonly OptimizationFlags _flags;
        private readonly AutoResetEvent _runnerAvailableHandler = new AutoResetEvent(false);
        private readonly ConcurrentBag<VsTestRunner> _availableRunners = new ConcurrentBag<VsTestRunner>();
        private readonly ICollection<TestCase> _discoveredTests;
        private readonly VsTestHelper _helper = new VsTestHelper();
        private readonly ILogger _logger;

        public VsTestRunnerPool(StrykerOptions options, OptimizationFlags flags, ProjectInfo projectInfo)
        {
            _logger = ApplicationLogging.LoggerFactory.CreateLogger<VsTestRunnerPool>();

            _flags = flags;
            using (var runner = new VsTestRunner(options, _flags, projectInfo, null, null))
            {
                _discoveredTests = runner.DiscoverTests();
            }

            Parallel.For(0, options.ConcurrentTestrunners, (i, loopState) =>
            {
                _availableRunners.Add(new VsTestRunner(options, _flags, projectInfo, _discoveredTests, CoverageMutants, helper: _helper));
            });
        }

        public TestCoverageInfos CoverageMutants { get; private set; } = new TestCoverageInfos();

        public IEnumerable<TestDescription> Tests => _discoveredTests.Select(x => (TestDescription) x);

        public TestRunResult RunAll(int? timeoutMs, IReadOnlyMutant mutant)
        {
            var runner = TakeRunner();
            TestRunResult result;

            try
            {
                result = runner.RunAll(timeoutMs, mutant);
            }
            finally
            {
                ReturnRunner(runner);
            }
            return result;
        }

        public TestRunResult CaptureCoverage(bool cantUsePipe, bool cantUseUnloadAppDomain)
        {
            var needCoverage = _flags.HasFlag(OptimizationFlags.CoverageBasedTest) || _flags.HasFlag(OptimizationFlags.SkipUncoveredMutants);
            if (needCoverage && _flags.HasFlag(OptimizationFlags.CaptureCoveragePerTest))
            {
                return CaptureCoveragePerIsolatedTests(cantUsePipe);
            }

            var runner = TakeRunner();
            TestRunResult result;

            try
            {
                if (needCoverage)
                {
                    result = runner.CaptureCoverage(cantUsePipe, cantUseUnloadAppDomain);
                    CoverageMutants = runner.CoverageMutants;
                }
                else
                {
                    result = runner.RunAll(null, null);
                }
            }
            finally
            {
                ReturnRunner(runner);
            }
            return result;
        }

        private TestRunResult CaptureCoveragePerIsolatedTests(bool cantUsePipe)
        {
            var options = new ParallelOptions { MaxDegreeOfParallelism = _availableRunners.Count };

            Parallel.ForEach(_discoveredTests, options, testCase =>
            {
                var runner = TakeRunner();
                try
                {
                    runner.CoverageForTest(testCase, cantUsePipe);
                }
                catch (Exception e)
                {
                    _logger.LogError("Something went wrong while capturing coverage", e);
                }
                finally
                {
                    ReturnRunner(runner);
                }
            });

            return new TestRunResult { Success = true };
        }

        private VsTestRunner TakeRunner()
        {
            VsTestRunner runner;
            while (!_availableRunners.TryTake(out runner))
            {
                _runnerAvailableHandler.WaitOne();
            }
            return runner;
        }

        private void ReturnRunner(VsTestRunner runner)
        {
            _availableRunners.Add(runner);
            _runnerAvailableHandler.Set();
        }

        public void Dispose()
        {
            foreach (var runner in _availableRunners)
            {
                runner.Dispose();
            }
            _helper.Cleanup();
            _runnerAvailableHandler.Dispose();
        }

        public int DiscoverNumberOfTests()
        {
            return _discoveredTests.Count();
        }
    }
}
