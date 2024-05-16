using Stryker.TestRunners.MsTest.Discovery;

namespace Stryker.TestRunners.MsTest;

public class MsTestRunnerWrapper
{
    public static async Task<bool> ContainsTestsAsync(string assemblyPath)
    {
        var testAssembly = TestAssemblyLoader.Load(assemblyPath);

        if (testAssembly is null)
        {
            return false;
        }

        var testFramework = testAssembly.TestFramework;
        var app = await testFramework.Build(testAssembly.Assembly, [Constants.Modes.Discovery, Constants.RunOptions.NoBanner], null);
        var result = await testFramework.Run(app);

        return result != Constants.ExitCodes.NoTests;
    }

    public static async Task DiscoverTestsAsync(string assemblyPath)
    {
        var testAssembly = TestAssemblyLoader.Load(assemblyPath);

        if (testAssembly is null)
        {
            return;
        }

        var testFramework = testAssembly.TestFramework;
        var app = await testFramework.Build(testAssembly.Assembly, [Constants.RunOptions.NoBanner], null);
        await testFramework.Run(app);
    }
}

