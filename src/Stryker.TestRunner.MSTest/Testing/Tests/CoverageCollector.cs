using System.Reflection;
using Microsoft.Testing.Platform.Extensions.Messages;
using Stryker.Shared.Coverage;
using Stryker.TestRunner.MSTest.Testing.Results;

namespace Stryker.TestRunner.MSTest.Testing.Tests;
internal class CoverageCollector
{
    public const string AnyId = "*";

    private readonly IDictionary<string, int> _mutantTestedBy = new Dictionary<string, int>();
    private readonly DiscoveryResult _discoveryResult;

    private IList<int> _mutationCoveredOutsideTests = [];
    private IList<TestCoverageInfo> _testCoverageInfos = [];

    private CoverageCollector(DiscoveryResult discoveryResult, string helperNamespace, bool isCoverageRun)
    {
        _discoveryResult = discoveryResult;
        MutantControlClassName = $"{helperNamespace}.MutantControl";
        IsCoverageRun = isCoverageRun;
    }

    public static CoverageCollector CoverageRun(DiscoveryResult discoveryResult, string helperNamespace) => new(discoveryResult, helperNamespace, true); 

    public int ActiveMutation { get; private set; }

    public bool IsCoverageRun { get; private set; }

    public string MutantControlClassName { get; private set; }

    public FieldInfo? ActiveMutantField { get; set; } 

    public FieldInfo? CaptureCoverageField { get; set; }

    public MethodInfo? GetCoverageDataMethod { get; set; }

    public void SetActiveMutation(string id)
    {
        ActiveMutation = GetActiveMutantForTest(id);
        ActiveMutantField?.SetValue(null, ActiveMutation);
    }

    public IList<int>[]? RetrieveCoverData()
        => (IList<int>[]?)GetCoverageDataMethod?.Invoke(null, []);

    public void CaptureCoverageOutsideTests()
    {
        var covered = RetrieveCoverData();
        if (covered?[0] is not null)
        {
            _mutationCoveredOutsideTests =
                covered[1] is not null ? covered[0].Union(covered[1]).ToList() : [.. covered[0]];
        }
        else if (covered?[1] is not null)
        {
            _mutationCoveredOutsideTests = [.. covered[1]];
        }
    }

    public void PublishCoverageData(TestNode testNode)
    {
        var covered = RetrieveCoverData();

        if (covered is null)
        {
            return;
        }

        var testCoverageInfo = new TestCoverageInfo(
            testNode,
            covered[0],
            covered[1],
            _mutationCoveredOutsideTests);

        _testCoverageInfos.Add(testCoverageInfo);
        _mutationCoveredOutsideTests.Clear();
    }

    public IEnumerable<ICoverageRunResult> GetCoverageRunResult(bool perIsolatedTest)
    {
        var seenTestCases = new HashSet<string>();
        var defaultConfidence = perIsolatedTest ? CoverageConfidence.Exact : CoverageConfidence.Normal;
        var resultCache = new Dictionary<string, CoverageRunResult>();

        foreach (var testCoverageInfo in _testCoverageInfos)
        {
            var outcome = testCoverageInfo.TestNode.Properties.Single<TestNodeStateProperty>();

            if (outcome is not PassedTestNodeStateProperty && outcome is not FailedTestNodeStateProperty)
            {
                continue;
            }

            if (TryConvertToCoverageResult(testCoverageInfo, seenTestCases, defaultConfidence, out var coverageRunResult))
            {
                continue;
            }

            if (!resultCache.TryAdd(coverageRunResult!.TestId.ToString(), coverageRunResult))
            {
                resultCache[coverageRunResult.TestId.ToString()].Merge(coverageRunResult);
            }
        }

        return resultCache.Values;
    }

    private bool TryConvertToCoverageResult(TestCoverageInfo testCoverageInfo, ISet<string> seenTestCases, CoverageConfidence defaultConfidence, out CoverageRunResult? coverageRunResult)
    {
        var testCaseId = testCoverageInfo.TestNode.Uid.Value;

        if (!_discoveryResult.MsTests.TryGetValue(testCaseId, out var testDescription))
        {
            throw new NotImplementedException();
        }

        if(seenTestCases.Contains(testCaseId))
        {
            coverageRunResult = null;
            return true;
        }

        seenTestCases.Add(testDescription.Id.ToString());
        coverageRunResult = CoverageRunResult.Create(testCaseId, defaultConfidence, testCoverageInfo.CoveredMutations, testCoverageInfo.CoveredStaticMutations, testCoverageInfo.LeakedMutations);

        return false;
    }

    private int GetActiveMutantForTest(string id)
    {
        if (_mutantTestedBy.TryGetValue(id, out var test))
        {
            return test;
        }

        return _mutantTestedBy.TryGetValue(AnyId, out var value) ? value : -1;
    }
}
