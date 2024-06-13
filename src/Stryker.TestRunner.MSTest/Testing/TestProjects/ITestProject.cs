using Microsoft.Testing.Platform.Extensions.Messages;
using Stryker.TestRunner.MSTest.Testing.Results;
using Stryker.TestRunner.MSTest.Testing.Tests;

namespace Stryker.TestRunner.MSTest.Testing.TestProjects;
internal interface ITestProject
{
    Task<int> Discover(DiscoveryResult discoveryResult, string assemblyPath);
    Task<int> InitialTestRun(DiscoveryResult discoveryResult, List<TestNode> executed);
    Task<int> CoverageRun(CoverageCollector coverageCollector);
    Task<int> MutantRun();
}
