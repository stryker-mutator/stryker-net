using System;
using System.Collections.Generic;
using System.Linq;
using Moq;
using Stryker.Core.Initialisation;
using Stryker.Core.Mutants;
using Stryker.Core.Options;
using Stryker.Core.TestRunners;
using Stryker.Core.TestRunners.VsTest;

namespace Stryker.Core.UnitTest.MutationTest
{
    /// <summary>
    /// This class simplifies the creation of run scenarios
    /// </summary>
    internal class FullRunScenario
    {
        private readonly Dictionary<int, Mutant> _mutants = new();
        private readonly Dictionary<int, TestDescription> _tests = new();

        private readonly Dictionary<int, TestGuidsList> _coverageResult = new();
        private readonly Dictionary<int, TestGuidsList> _failedTestsPerRun = new();
        private readonly Dictionary<Guid, List<int>> _testCoverage = new();
        private const int InitialRunId = -1;
        private OptimizationModes _mode = OptimizationModes.CoverageBasedTest | OptimizationModes.SkipUncoveredMutants;

        public TestSet TestSet { get; } = new();
        public IDictionary<int, Mutant> Mutants => _mutants;

        /// <summary>
        /// Create a mutant
        /// </summary>
        /// <param name="id">expected mutant Id, autogenerated by default</param>
        /// <returns>A mutant instance</returns>
        public Mutant CreateMutant(int id = -1)
        {
            if (id == -1)
            {
                id = _mutants.Keys.Append(-1).Max() + 1;
            }
            var mutant = new Mutant { Id = id };
            _mutants[id] = mutant;
            return mutant;
        }

        public void CreateMutants(params int[] ids)
        {
            foreach (var id in ids)
            {
                CreateMutant(id);
            }
        }

        public IEnumerable<Mutant> GetMutants() => _mutants.Values;

        public IEnumerable<Mutant> GetCoveredMutants() => _coverageResult.Keys.Select(i => _mutants[i]);

        public MutantStatus GetMutantStatus(int id) => _mutants[id].ResultStatus;

        public void DeclareCoverageForMutant(int mutantId, params int[] testIds)
        {
            _coverageResult[mutantId] = GetGuidList(testIds);
            foreach (var testId in testIds.Length == 0 ? _tests.Keys.ToArray() : testIds)
            {
                var id = _tests[testId].Id;
                if (!_testCoverage.ContainsKey(id))
                {
                    _testCoverage[id] = new List<int>();
                }
                _testCoverage[id].Add(mutantId);
            }
        }

        public void DeclareTestsFailingAtInit(params int[] ids) => DeclareTestsFailingWhenTestingMutant(InitialRunId, ids);

        public void SetMode(OptimizationModes mode) => _mode = mode;

        public void DeclareTestsFailingWhenTestingMutant(int id, params int[] ids)
        {
            var testsGuidList = GetGuidList(ids);
            if (!testsGuidList.IsIncludedIn(GetCoveringTests(id)))
            {
                // just check we are not doing something stupid
                throw new ApplicationException(
                    $"you tried to declare a failing test but it does not cover mutant {id}");
            }
            _failedTestsPerRun[id] = testsGuidList;
        }

        /// <summary>
        /// Create a test
        /// </summary>
        /// <param name="id"></param>
        /// <param name="name"></param>
        /// <param name="file"></param>
        /// <returns></returns>
        public TestDescription CreateTest(int id = -1, string name = null, string file = "TestFile.cs")
        {
            if (id == -1)
            {
                id = _tests.Keys.Append(-1).Max() + 1;
            }

            var test = new TestDescription(Guid.NewGuid(), name ?? $"test {id}", file);
            _tests[id] = test;
            TestSet.RegisterTests(new[] { test });
            return test;
        }

        public void CreateTests(params int[] ids)
        {
            foreach (var id in ids)
            {
                CreateTest(id);
            }
        }

        /// <summary>
        /// Returns a list of test Guids
        /// </summary>
        /// <param name="ids"></param>
        /// <returns></returns>
        public TestGuidsList GetGuidList(params int[] ids)
        {
            var selectedIds = ids.Length > 0 ? ids.Select(i => _tests[i]) : _tests.Values;
            return new TestGuidsList(selectedIds.Select(t => t.Id));
        }

        private TestGuidsList GetFailedTests(int runId)
        {
            if (_failedTestsPerRun.TryGetValue(runId, out var list))
            {
                return list;
            }
            return TestGuidsList.NoTest();
        }

        private TestGuidsList GetCoveringTests(int id)
        {

            // if this is the initial test run, we must return the complete list of tests.
            if (id == InitialRunId)
            {
                return new TestGuidsList(_tests.Values.Select(t => t.Id));
            }

            if (!_mode.HasFlag(OptimizationModes.CoverageBasedTest))
            {
                return TestGuidsList.EveryTest();
            }

            return _coverageResult.TryGetValue(id, out var list) ? list : TestGuidsList.NoTest();
        }

        private TestRunResult GetRunResult(int id) => new(Enumerable.Empty<VsTestDescription>(), GetCoveringTests(id), GetFailedTests(id), TestGuidsList.NoTest(), string.Empty, Enumerable.Empty<string>(), TimeSpan.Zero);

        public TestRunResult GetInitialRunResult() => GetRunResult(InitialRunId);

        public Mock<ITestRunner> GetTestRunnerMock()
        {
            var runnerMock = new Mock<ITestRunner>();
            var successResult = new TestRunResult(
                Enumerable.Empty<VsTestDescription>(),
                GetGuidList(),
                TestGuidsList.NoTest(),
                TestGuidsList.NoTest(),
                string.Empty,
                Enumerable.Empty<string>(),
                TimeSpan.Zero);
            runnerMock.Setup(x => x.DiscoverTests()).Returns(TestSet);
            runnerMock.Setup(x => x.InitialTest()).Returns(GetRunResult(InitialRunId));
            runnerMock.Setup(x => x.CaptureCoverage())
                .Returns(() =>
                {
                    var result = new List<CoverageRunResult>(_tests.Count);
                    foreach (var (guid, mutations) in _testCoverage)
                    {
                        result.Add(new CoverageRunResult(guid, CoverageConfidence.Normal,
                            mutations,
                            Enumerable.Empty<int>(),
                            Enumerable.Empty<int>()));
                    }
                    return result;
                });
            runnerMock.Setup(x => x.TestMultipleMutants(It.IsAny<ITimeoutValueCalculator>(),
                    It.IsAny<IReadOnlyList<Mutant>>(), It.IsAny<TestUpdateHandler>())).
                Callback((Action<ITimeoutValueCalculator, IReadOnlyList<Mutant>, TestUpdateHandler>)((test1, list,
                    update) =>
                {
                    foreach (var m in list)
                    {
                        update(list, GetFailedTests(m.Id), GetCoveringTests(m.Id), TestGuidsList.NoTest());
                    }
                }))
                .Returns(successResult);
            return runnerMock;
        }
    }
}
