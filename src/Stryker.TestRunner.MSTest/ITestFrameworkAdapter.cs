using Microsoft.Testing.Platform.Builder;
using System.Reflection;

namespace Stryker.TestRunner.MSTest;
internal interface ITestFrameworkAdapter
{
    /// <summary>
    /// Build the test application for framework implenting this interface
    /// </summary>
    /// <param name="assembly">The assembly where the tetframework will operate in</param>
    /// <param name="activeMutation">The mutation that should be active</param>
    /// <returns>A test application</returns>
    Task<ITestApplication?> Build(Assembly assembly, string activeMutation);

    /// <summary>
    /// Run the test application
    /// </summary>
    /// <param name="app">The application that will be runned</param>
    /// <returns>A task</returns>
    Task Run(ITestApplication app);
}
