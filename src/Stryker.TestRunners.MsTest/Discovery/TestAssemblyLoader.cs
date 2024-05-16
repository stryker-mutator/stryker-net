using System.Diagnostics;
using System.Reflection;
using Stryker.TestRunners.MsTest.Discovery.FrameworkAdapters;

namespace Stryker.TestRunners.MsTest.Discovery;
internal class TestAssemblyLoader
{
    private static readonly Dictionary<string, TestAssembly> testAssemblies = [];

    public static TestAssembly? Load(string assemblyPath)
    {
        if (testAssemblies.TryGetValue(assemblyPath, out var testAssembly))
        {
            return testAssembly;
        }

        var assembly = LoadAssemblySafe(assemblyPath);

        if (assembly is null)
        {
            return null;
        }

        var testFramework = GetTestFrameworkAdapter(assembly);

        if (testFramework is null)
        {
            return null;
        }

        // Caches testassembly
        testAssembly = TestAssembly.Create(assembly, testFramework);
        testAssemblies.Add(assemblyPath, testAssembly);

        return testAssembly;
    }

    private static Assembly? LoadAssemblySafe(string assemblyPath)
    {
        try
        {
            return Assembly.LoadFrom(assemblyPath);
        }
        catch (Exception exception)
        {
            Debug.WriteLine(exception);
            return null;
        }
    }

    private static IFrameworkAdapter? GetTestFrameworkAdapter(Assembly assembly)
    {
        var referencedAssemblies = assembly.GetReferencedAssemblies();

        foreach (var referencedAssembly in referencedAssemblies)
        {
            var testFramework = DetermineTestFramework(referencedAssembly);

            if(testFramework is not null)
            {
                return testFramework;
            }
        }

        return null;
    }


    private static IFrameworkAdapter? DetermineTestFramework(AssemblyName assemblyName) => assemblyName.Name switch
    {
        null => null,
        _ when assemblyName.Name.Contains(MsTestAdapter.EntryPoint, StringComparison.OrdinalIgnoreCase) => MsTestAdapter.Create(),
        _ => null
    };
}
