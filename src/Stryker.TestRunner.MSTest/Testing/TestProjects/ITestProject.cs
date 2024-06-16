using Microsoft.Testing.Platform.Extensions.Messages;
using Stryker.TestRunner.MSTest.Testing.Results;
using Stryker.TestRunner.MSTest.Testing.Tests;

namespace Stryker.TestRunner.MSTest.Testing.TestProjects;
internal interface ITestProject
{
    Task<int> Discover(DiscoveryResult discoveryResult, string assemblyPath);
    Task<int> InitialTestRun(DiscoveryResult discoveryResult, List<TestNode> executed);
    Task<int> CoverageRun(CoverageCollector coverageCollector, string testCase);
    Task<int> MutantRun(int mutantId, IEnumerable<string>? testCases, string helperNamespace, List<TestNode> executed);
}
