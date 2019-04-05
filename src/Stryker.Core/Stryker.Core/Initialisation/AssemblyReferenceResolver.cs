﻿using Buildalyzer;
using Microsoft.CodeAnalysis;
using Microsoft.Extensions.Logging;
using Stryker.Core.Exceptions;
using Stryker.Core.Logging;
using Stryker.Core.Testing;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Stryker.Core.Initialisation
{
    public interface IAssemblyReferenceResolver
    {
        IEnumerable<PortableExecutableReference> LoadProjectReferences(string[] projectReferencePaths);
    }

    /// <summary>
    /// Resolving the MetadataReferences for compiling later
    /// This has to be done using msbuild because currently msbuild is the only reliable way of resolving all referenced assembly locations
    /// </summary>
    public class AssemblyReferenceResolver : IAssemblyReferenceResolver
    {
        private ILogger _logger { get; set; }

        public AssemblyReferenceResolver()
        {
            _logger = ApplicationLogging.LoggerFactory.CreateLogger<AssemblyReferenceResolver>();
        }

        /// <summary>
        /// Uses Buildalyzer to resolve all references for the given test project
        /// </summary>
        /// <param name="projectFile">The test project file location</param>
        /// <returns>References</returns>
        public IEnumerable<PortableExecutableReference> LoadProjectReferences(string[] projectReferencePaths)
        {
            foreach (var path in projectReferencePaths)
            {
                _logger.LogTrace("Resolved depedency {0}", path);
                yield return MetadataReference.CreateFromFile(path);
            }
        }
    }
}
