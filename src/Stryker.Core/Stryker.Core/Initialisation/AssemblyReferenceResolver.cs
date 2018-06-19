using Microsoft.CodeAnalysis;
using Microsoft.Extensions.Logging;
using Stryker.Core.Logging;
using Stryker.Core.Testing;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Stryker.Core.Initialisation
{
    /// <summary>
    /// Resolving the MetadataReferences for compiling later
    /// This has to be done using msbuild because currently msbuild is the only reliable way of resolving all referenced assembly locations
    /// </summary>
    public class AssemblyReferenceResolver : IAssemblyReferenceResolver
    {
        private IProcessExecutor _processExecutor { get; set; }
        private IMetadataReferenceProvider _metadataReference { get; set; }
        private ILogger _logger { get; set; }

        public AssemblyReferenceResolver(IProcessExecutor processExecutor, IMetadataReferenceProvider metadataReference)
        {
            _processExecutor = processExecutor;
            _metadataReference = metadataReference;
            _logger = ApplicationLogging.LoggerFactory.CreateLogger<AssemblyReferenceResolver>();
        }

        public AssemblyReferenceResolver() : this(new ProcessExecutor(), new MetadataReferenceProvider()) { }

        /// <summary>
        /// Uses msbuild to resolve all references for the given test project
        /// </summary>
        /// <param name="projectFile">The test project file location</param>
        /// <returns>References</returns>
        public IEnumerable<PortableExecutableReference> ResolveReferences(string projectPath, string projectFileName, string projectUnderTestAssemblyName)
        {
            // Execute dotnet msbuild with the task PrintReferences
            var result = _processExecutor.Start(
                projectPath, 
                "dotnet", 
                $"msbuild {projectFileName} /nologo /t:PrintReferences");

            _logger.LogTrace(@"""{0} dotnet msbuild {1} /nologo /t:PrintReferences"" resulted in {2}", projectPath, projectFileName, result.Output);

            if (result.ExitCode != 0)
            {
                _logger.LogError(@"The task PrintReferences was not found in your project file. Please add the task to {0}", projectFileName);
                throw new ApplicationException($"The task PrintReferences was not found in your project file. Please add the task to {projectFileName}");
            }

            var rows = result.Output.Split(new string[] { Environment.NewLine.ToString() }, StringSplitOptions.None).Where(x => !string.IsNullOrWhiteSpace(x)).ToArray();

            // All rows except the last contain the project dependencies
            foreach (var reference in GetReferencePathsFromOutput(rows.Reverse().Skip(1))
                .Distinct()
                .Where(x => Path.GetFileNameWithoutExtension(x) != projectUnderTestAssemblyName))
            {
                _logger.LogDebug(@"Resolved reference {0}", reference.Trim());
                yield return _metadataReference.CreateFromFile(reference.Trim());
            }

            // the last part contains the package dependencies, seperated by the path seperator char
            foreach (var reference in GetAssemblyPathsFromOutput(rows.Last())
                .Distinct()
                .Where(x => Path.GetFileNameWithoutExtension(x) != projectUnderTestAssemblyName))
            {
                _logger.LogDebug(@"Resolved reference {0}", reference.Trim());
                yield return _metadataReference.CreateFromFile(reference.Trim());
            }
        }


        /// <summary>
        /// Subtracts all paths from PathSeperator seperated string
        /// </summary>
        /// <returns>All references this project has</returns>
        public IEnumerable<string> GetAssemblyPathsFromOutput(string paths)
        {
            Console.WriteLine("Pathseparator: " + Path.PathSeparator);
            Console.WriteLine("DirectorySeparatorChar: " + Path.DirectorySeparatorChar);
            foreach (var path in paths.Split(Path.PathSeparator))
            {
                Console.WriteLine("Filename: " + Path.GetFileNameWithoutExtension(paths));

                if (Path.GetExtension(path) == ".dll")
                {
                    yield return path;
                }
            }
        }

        /// <summary>
        /// Subtracts all paths from PathSeperator seperated string
        /// </summary>
        /// <returns>All references this project has</returns>
        public IEnumerable<string> GetReferencePathsFromOutput(IEnumerable<string> paths)
        {
            foreach (var pathPrintOutput in paths)
            {
                var path = pathPrintOutput.Split(new string[] { " -> " }, StringSplitOptions.None).Last();
                Console.WriteLine("Filename: " + Path.GetFileNameWithoutExtension(path));

                if (Path.GetExtension(path) == ".dll")
                {
                    yield return path;
                }
            }
        }
    }
}
