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
                return _mutantToTests[mutationId].Cast<T>().ToList();
            }

            return null;
        }

        public void DeclareMappingForATest(object discoveredTest, IEnumerable<int> captureCoverage)
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
                            _mutantToTests.Add(id, new List<object>{discoveredTest});
                        }
                    }   
                }
            }
        }
    }
}
