using System.Reflection;
using Microsoft.Testing.Platform.Extensions.TestFramework;

namespace Stryker.TestRunners.MsTest.Discovery;
internal class TestAssembly
{
    public Assembly Assembly { get; private set; }
    public IFrameworkAdapter TestFramework { get; private set; }

    public static TestAssembly Create(Assembly assembly, IFrameworkAdapter testFramework) => new(assembly, testFramework);

    private TestAssembly(Assembly assembly, IFrameworkAdapter testFramework)
    {
        Assembly = assembly;
        TestFramework = testFramework;
    }
}
