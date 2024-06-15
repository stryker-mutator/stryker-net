using System.IO.Abstractions;
using System.Reflection;
using Stryker.Shared.Exceptions;
using Stryker.TestRunner.MSTest.Testing.TestProjects;

namespace Stryker.TestRunner.MSTest.Setup;
internal class TestProjectLoader
{
    // Caches loaded assemblies
    private readonly Dictionary<string, ITestProject> _projects = [];
    private readonly Dictionary<string, ITestProject> _shadowProjects = [];

    private readonly IFileSystem _fileSystem;
    private readonly AssemblyCopy _assemblyCopy;
   
    public TestProjectLoader(IFileSystem? fileSystem)
    {
        _fileSystem = fileSystem ?? new FileSystem();
        _assemblyCopy = new AssemblyCopy(_fileSystem);
    }

    public ITestProject Load(string path)
    {
        var isCached = _projects.TryGetValue(path, out var project);

        if (isCached)
        {
            return project!;
        }

        var assembly = LoadAssembly(path);
        var testProject = LoadTestProject(assembly);

        _projects.Add(path, testProject);
        return testProject;
    }

    public ITestProject LoadCopy(string path)
    {
        var isCached = _shadowProjects.TryGetValue(path, out var project);
       
        if (isCached)
        {
            return project!;
        }

        // Find all .csproj files so we only copy referenced assemblies
        var root = DirectoryScanner.FindSolutionRoot(path);
        var csprojFiles = DirectoryScanner.FindCsprojFiles(root);

        // Makes a copy of the current assemblies
        var projectNames = _assemblyCopy.CopyProjects(csprojFiles, path);
        var copyPath = _assemblyCopy.CopyUnitTestProject(path, projectNames);

        // Loads the copy into memory
        var testAssembly = LoadAssembly(copyPath);
        var testProject = LoadTestProject(testAssembly);

        _shadowProjects.Add(path, testProject);
        return testProject;
    }

    private static Assembly LoadAssembly(string path)
    {
        try
        {
            // Loads the assembly in the default AssemblyLoadContext.
            // Locks the corresponding file for any IO-operations.
            return Assembly.LoadFrom(path);
        }
        catch
        {
            throw new GeneralStrykerException($"Could not load assembly from path: {path}");
        }
    }

    private static ITestProject LoadTestProject(Assembly assembly)
    {
        foreach (var reference in assembly.GetReferencedAssemblies())
        {
            if (reference.FullName?.Contains(MsTestProject.EntryPoint, StringComparison.OrdinalIgnoreCase) is true)
            {
                return MsTestProject.Create(assembly);
            }
        }

        throw new GeneralStrykerException("No supported test adapter found");
    }
}
