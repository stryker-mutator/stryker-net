using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Extensions.Logging;
using Stryker.Core.Logging;
using Stryker.Core.Mutants;

namespace Stryker.Core.TestRunners
{
    public class TestCoverageInfos
    {
        private readonly Dictionary<int, IList<TestDescription>> _mutantToTests = new Dictionary<int, IList<TestDescription>>();
        private readonly HashSet<int> _mutantsInStatic = new HashSet<int>();
        private readonly IList<TestDescription> _testsWithoutCoverageInfos = new List<TestDescription>();
        private static ILogger Logger { get; }

        public IEnumerable<int> CoveredMutants
        {
            get
            {
                lock (_mutantToTests)
                {
                    return _mutantToTests.Keys;
                }
            }
        }

        static TestCoverageInfos()
        {
            Logger = ApplicationLogging.LoggerFactory.CreateLogger<TestCoverageInfos>();
        }

        public bool NeedAllTests(IReadOnlyMutant mutant)
        {
            return mutant.IsStaticValue || this._mutantsInStatic.Contains(mutant.Id);
        }

        public ICollection<TestDescription> GetTests(IReadOnlyMutant mutant)
        {
            lock (_mutantToTests)
            {
                if (_mutantToTests.ContainsKey(mutant.Id))
                {
                    return _mutantToTests[mutant.Id].ToList();
                }
                
            }
            return null;
        }

        public void DeclareCoveredMutants(IEnumerable<int> list)
        {
            lock(_mutantToTests)
            {
                foreach (var mutant in list)
                {
                    _mutantToTests[mutant] = new List<TestDescription>();
                }
            }
        }

        public void DeclareMappingForATest(TestDescription discoveredTest, ICollection<int> captureCoverage, ICollection<int> staticMutants)
        {
            lock(_mutantToTests)
            { 
                if (captureCoverage == null)
                {
                    // no coverage info available, we keep track of it
                    Logger.LogDebug($"No covered mutants for {discoveredTest}.");
                    _testsWithoutCoverageInfos.Add(discoveredTest);
                }
                else
                {
                    Logger.LogDebug($"Covered mutants for {discoveredTest} are: {string.Join(", ", captureCoverage)}.");
                    foreach (var id in captureCoverage)
                    {
                        if (_mutantToTests.ContainsKey(id))
                        {
                            _mutantToTests[id].Add(discoveredTest);
                        }
                        else
                        {
                            _mutantToTests.Add(id, new List<TestDescription>{discoveredTest});
                        }
                    }
                    if (staticMutants.Any())
                    {
                        Logger.LogDebug($"Those are being executed in a static constructor context: {string.Join(", ", staticMutants)}.");
                        foreach (var staticMutant in staticMutants)
                        {
                            _mutantsInStatic.Add(staticMutant);
                        }
                    }
                }
            }
        }

        public long UpdateMutants(IEnumerable<Mutant> mutants, int testsCount)
        {
            var avoidedTests = 0L;
            Logger.LogDebug("Optimize test runs according to coverage info.");
            var report = new StringBuilder();
            var nonTested = mutants.Where(x =>
                x.ResultStatus == MutantStatus.NotRun && !CoveredMutants.Contains(x.Id)).ToList();
            foreach (var mutant in nonTested)
            {
                mutant.ResultStatus = MutantStatus.Survived;
                avoidedTests += testsCount;
            }

            foreach (var mutant in mutants)
            {
                var tests = this.GetTests(mutant);
                var mutantCoveringTest = new Dictionary<string, bool>();
                if (tests != null)
                { 
                    foreach (var test in tests)
                    {
                        mutantCoveringTest[test.Guid] = false;
                    }
                }
                mutant.CoveringTest = mutantCoveringTest;
                if (this.NeedAllTests(mutant))
                {
                    mutant.MustRunAllTests = true;
                    Logger.LogDebug($"Mutant {mutant.DisplayName} is related to a static value and we may not have reliable coverage info. No optimization done.");
                }
                else
                {
                    avoidedTests += testsCount - mutantCoveringTest.Count;
                }
            }

            report.AppendJoin(',', nonTested.Select(x => x.Id));
            Logger.LogInformation(nonTested.Count == 0
                ? "Congratulations, all mutants are covered by tests!"
                : $"{nonTested.Count} mutants are not reached by any tests and will survive! (Marked as {MutantStatus.Survived}).");

            var theoricalTestCount = (long)(testsCount) * mutants.Count();
            Logger.LogInformation($"Coverage analysis eliminated {1.0*avoidedTests/theoricalTestCount:P} of tests (i.e. {avoidedTests} tests out of {theoricalTestCount}).");

            return avoidedTests;
        }

    }
}
