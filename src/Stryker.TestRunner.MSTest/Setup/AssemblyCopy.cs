using System.IO.Abstractions;
using dnlib.DotNet;
using dnlib.DotNet.Writer;
using Stryker.Shared.Exceptions;

namespace Stryker.TestRunner.MSTest.Setup;
internal class AssemblyCopy
{
    public const string CopySuffix = "Copy";
    private readonly IFileSystem _fileSystem;

    public AssemblyCopy(IFileSystem fileSystem) => _fileSystem = fileSystem;
   
    public IEnumerable<string> CopyProjects(IEnumerable<string> csprojFilePaths, string testAssembly)
    {
        var csprojFiles = csprojFilePaths.Select(Path.GetFileNameWithoutExtension);
        var testDirectory = Path.GetDirectoryName(testAssembly) ?? string.Empty;

        List<string> projectNames = [];

        var dlls = csprojFiles
            .Select(file => $"{Path.Combine(testDirectory, file ?? string.Empty)}.dll")
            .Where(file => _fileSystem.File.Exists(file) && file != testAssembly);

        foreach (var dll in dlls)
        {
            var moduleContext = ModuleDef.CreateModuleContext();
            var module = ModuleDefMD.Load(dll, moduleContext);

            var moduleWriterOptions = new ModuleWriterOptions(module)
            {
                WritePdb = true
            };

            var projectName = Path.GetFileNameWithoutExtension(dll);
            projectNames.Add(projectName);

            var copyName = $"{projectName}.{CopySuffix}";
            var copyPath = $"{Path.Combine(testDirectory, copyName)}.dll";

            module.Assembly.Name = copyName;

            module.Write(copyPath, moduleWriterOptions);
        }

        return projectNames;
    }

    public string CopyUnitTestProject(string testAssembly, IEnumerable<string> projectNames)
    {
        if (!_fileSystem.File.Exists(testAssembly))
        {
            throw new GeneralStrykerException($"Could not load assembly from path: {testAssembly}");
        }

        var moduleContext = ModuleDef.CreateModuleContext();
        var module = ModuleDefMD.Load(testAssembly, moduleContext);

        var moduleWriterOptions = new ModuleWriterOptions(module)
        {
            WritePdb = true
        };

        var copyDirectory = Path.GetDirectoryName(testAssembly) ?? string.Empty;
        var copyName = $"{Path.GetFileNameWithoutExtension(testAssembly)}.{CopySuffix}";
        var copyPath = $"{Path.Combine(copyDirectory, copyName)}.dll";

        module.Assembly.Name = copyName;
        
        foreach(var projectName in projectNames)
        {
            var assemblyRef = module.GetAssemblyRefs().FirstOrDefault(x => x.Name.Contains(projectName));

            if (assemblyRef is null)
            {
                continue;
            }

            assemblyRef.Name = $"{projectName}.{CopySuffix}";
        }

        module.Write(copyPath, moduleWriterOptions);

        return copyPath;
    }
}
