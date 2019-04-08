using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Extensions.Logging;
using Stryker.Core.Logging;

namespace Stryker.Core.TestRunners
{
    public class TestCoverageInfos
    {
        private readonly Dictionary<int, IList<object>> _mutantToTests = new Dictionary<int, IList<object>>();
        private readonly IList<object> _testsWithoutCoverageInfos = new List<object>();
        private static ILogger Logger { get; }
        public IEnumerable<int> CoveredMutants => _mutantToTests.Keys;

        static TestCoverageInfos()
        {
            Logger = ApplicationLogging.LoggerFactory.CreateLogger<TestCoverageInfos>();
        }

        public ICollection<T> GetTests<T>(int mutationId)
        {
            if (_mutantToTests.ContainsKey(mutationId))
            {
                return _mutantToTests[mutationId].Union(_testsWithoutCoverageInfos).Cast<T>().ToList();
            }

            return null;
        }

        public void DeclareMappingForATest(object discoveredTest, IEnumerable<int> captureCoverage)
        {
            if (captureCoverage == null)
            {
                // no coverage info available, the test will have to be run for each mutant
                _testsWithoutCoverageInfos.Add(discoveredTest);
            }
            else
            {
                foreach (var id in captureCoverage)
                {
                    if (_mutantToTests.ContainsKey(id))
                    {
                        _mutantToTests[id].Add(discoveredTest);
                    }
                    else
                    {
                        _mutantToTests.Add(id, new List<object>{discoveredTest});
                    }
                }   
            }
        }

        public void Log()
        {
            Logger.LogDebug("Mutant => Tests Coverage information");
            foreach (var (mutantId, tests) in _mutantToTests)
            {
                var list = new StringBuilder();
                list.AppendJoin(",", tests.Select(x => x.ToString()));
                Logger.LogDebug($"Mutant '{mutantId}' covered by [{list}].");
            }
            Logger.LogDebug("* Systematic Tests**");
            foreach (var testsWithoutCoverageInfo in _testsWithoutCoverageInfos)
            {
                Logger.LogDebug(testsWithoutCoverageInfo.ToString());
            }
            Logger.LogDebug("*****************");

        }

        public void DeclareCoveredMutants(IEnumerable<int> runnerCoveredMutants)
        {
            foreach (var runnerCoveredMutant in runnerCoveredMutants)
            {
                if (!_mutantToTests.ContainsKey(runnerCoveredMutant))
                {
                    _mutantToTests.Add(runnerCoveredMutant, null);
                }
            }
        }
    }
}
