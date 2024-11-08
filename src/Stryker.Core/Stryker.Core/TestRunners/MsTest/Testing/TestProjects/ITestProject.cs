using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Testing.Platform.Extensions.Messages;
using Stryker.Core.TestRunners.MsTest.Testing.Results;
using Stryker.Core.TestRunners.MsTest.Testing.Tests;

namespace Stryker.Core.TestRunners.MsTest.Testing.TestProjects;

internal interface ITestProject
{
    Task<int> Discover(DiscoveryResult discoveryResult, List<TestNode> executed, string assemblyPath);
    Task<int> InitialTestRun(DiscoveryResult discoveryResult, List<TestNode> executed);
    Task<int> CoverageRun(CoverageCollector coverageCollector);
    Task<int> MutantRun(MutantController mutantController, IEnumerable<string>? testCases, List<TestNode> executed);
}
