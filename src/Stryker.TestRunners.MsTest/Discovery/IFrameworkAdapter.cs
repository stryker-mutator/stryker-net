using System.Reflection;
using Microsoft.Testing.Platform.Builder;

namespace Stryker.TestRunners.MsTest.Discovery;
internal interface IFrameworkAdapter
{
    Task<ITestApplication> Build(Assembly assembly, string[] arguments, IEnumerable<string>? activeMutations);
    Task<int> Run(ITestApplication testApplication); 
}
