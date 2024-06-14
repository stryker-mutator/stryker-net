using System.IO.Abstractions;
using System.Reflection;
using System.Runtime.Loader;
using Stryker.Shared.Exceptions;
using Stryker.TestRunner.MSTest.Testing.TestProjects;

namespace Stryker.TestRunner.MSTest.Setup;
internal class TestProjectLoader
{
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
        var assembly = LoadAssembly(path);

        foreach (var reference in assembly.GetReferencedAssemblies())
        {
            if (reference.FullName?.Contains(MsTestProject.EntryPoint, StringComparison.OrdinalIgnoreCase) is true)
            {
                return MsTestProject.Create(assembly);
            } 
        }

        throw new GeneralStrykerException("No test adapter found, exiting...");
    }

    public ITestProject LoadCopy(string path)
    {
        var isCached = _shadowProjects.TryGetValue(path, out var project);
       
        if (isCached)
        {
            return project!;
        }

        var root = DirectoryScanner.FindSolutionRoot(path);
        var csprojFiles = DirectoryScanner.FindCsprojFiles(root);

        // Makes a copy of the current assemblies
        var projectNames = _assemblyCopy.CopyProjects(csprojFiles, path);
        var copyPath = _assemblyCopy.CopyUnitTestProject(path, projectNames);

        // Loads the copy into memory
        var testProject = Load(copyPath);

        _shadowProjects.Add(path, testProject);
        return testProject;
    }

    private Assembly LoadAssembly(string path)
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
}
