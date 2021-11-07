#region --[Copyright]-----------------------------------------------------
//
// Author:
//   Anton Tykhyy (atykhyy@gmail.com)
//
// Copyright (c) 2021 Anton Tykhyy
//
// Licensed under the MIT/X11 license.
//
// Taken from: https://github.com/atykhyy/cecil-msbuild-helper/blob/e3b34ac224543aa52ea2cf6af3f17502f66dc874/MSBuildHelper.cs
// If we get more issues in the future, look to https://github.com/icsharpcode/ILSpy/blob/aa1906b8f54c78f06d41557214ff5bbff9fb2e26/ICSharpCode.Decompiler/Metadata/UniversalAssemblyResolver.cs
#endregion

#region --[Namespaces]----------------------------------------------------
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Mono.Cecil;
#endregion

namespace Stryker.Core.Initialisation
{
    /// <summary>
    /// Resolves assemblies and imports member references
    /// using the list of assembly references supplied by MSBuild through Buildalyzer.
    /// </summary>
    [ExcludeFromCodeCoverage] // This is not our code
    public class MSBuildAssemblyReferenceResolver : IAssemblyResolver, IMetadataImporterProvider, IReflectionImporterProvider
    {
        #region --[Fields: Private]---------------------------------------
        private readonly Dictionary<string, AssemblyDefinition> m_resolvedAssemblies = new Dictionary<string, AssemblyDefinition>();
        private readonly Dictionary<string, AssemblyNameReference> m_projectReferences = new Dictionary<string, AssemblyNameReference>();
        private readonly HashSet<Type> m_frameworkTypes = new HashSet<Type>();
        private readonly AssemblyNameReference m_frameworkReference;
        private readonly ReaderParameters m_readerParameters;

        private readonly static Version ZeroVersion = new Version("0.0.0.0");
        private readonly static ModuleDefinition SentinelModule = ModuleDefinition.CreateModule("<Sentinel>", 0);
        #endregion

        #region --[Constructors]------------------------------------------
        public MSBuildAssemblyReferenceResolver(IEnumerable<KeyValuePair<string, AssemblyNameReference>> references,
            ReaderParameters dependencyParams = null) : this(dependencyParams)
        {
            foreach (var item in references ?? throw new ArgumentNullException(nameof(references)))
                m_projectReferences.Add(item.Key, item.Value);

            m_frameworkReference = FindFrameworkReference();
        }

        private MSBuildAssemblyReferenceResolver(ReaderParameters dependencyParams)
        {
            m_readerParameters = dependencyParams ?? new ReaderParameters();
            m_readerParameters.AssemblyResolver = this;
        }
        #endregion

        #region --[Methods: Public]---------------------------------------
        /// <summary>
        /// Assigns self to be the assembly resolver and importer providers
        /// in <paramref name="targetParams"/>.
        /// </summary>
        public ReaderParameters WithProviders(ReaderParameters targetParams)
        {
            targetParams.AssemblyResolver = this;
            targetParams.MetadataImporterProvider = this;
            targetParams.ReflectionImporterProvider = this;
            return targetParams;
        }
        #endregion

        #region --[Interface: IAssemblyResolver]--------------------------
        AssemblyDefinition IAssemblyResolver.Resolve(AssemblyNameReference name)
        {
            return (this as IAssemblyResolver).Resolve(name, m_readerParameters);
        }

        AssemblyDefinition IAssemblyResolver.Resolve(AssemblyNameReference name, ReaderParameters parameters)
        {
            AssemblyDefinition assembly;
            if (m_resolvedAssemblies.TryGetValue(name.FullName, out assembly))
                return assembly;

            return ResolveAs(name, ResolveToCodeBase(name));
        }

        void IDisposable.Dispose()
        {
            foreach (var assembly in m_resolvedAssemblies.Values)
                assembly?.Dispose();
        }

        private string ResolveToCodeBase(AssemblyNameReference name)
        {
            // try to bind by full name first
            foreach (var kv in m_projectReferences)
                if (kv.Value.FullName == name.FullName)
                    return kv.Key;

            // try to bind by simple name as a fallback
            foreach (var kv in m_projectReferences)
                if (kv.Value.Name == name.Name)
                    return kv.Key;

            return null;
        }

        private AssemblyDefinition ResolveAs(AssemblyNameReference name, string fullPath)
        {
            AssemblyDefinition assembly;
            if (m_resolvedAssemblies.TryGetValue(name.FullName, out assembly))
                return assembly;

            if (fullPath != null)
                assembly = AssemblyDefinition.ReadAssembly(fullPath, m_readerParameters);

            m_resolvedAssemblies.Add(name.FullName, assembly);
            return assembly;
        }
        #endregion

        #region --[Interface: IMetadataImporterProvider]------------------
        IMetadataImporter IMetadataImporterProvider.GetMetadataImporter(ModuleDefinition module)
        {
            return new CrossTfmMetadataImporter(module, this);
        }

        sealed class CrossTfmMetadataImporter : DefaultMetadataImporter
        {
            private readonly MSBuildAssemblyReferenceResolver m_helper;

            public CrossTfmMetadataImporter(ModuleDefinition module, MSBuildAssemblyReferenceResolver helper) : base(module)
            {
                m_helper = helper;
            }

            protected override IMetadataScope ImportScope(TypeReference type)
            {
                return type.Module != SentinelModule && type.Scope is AssemblyNameReference reference ?
                    m_helper.ImportCrossTfmScope(module, type.FullName, false, reference) : base.ImportScope(type);
            }
        }
        #endregion

        #region --[Interface: IReflectionImporterProvider]----------------
        IReflectionImporter IReflectionImporterProvider.GetReflectionImporter(ModuleDefinition module)
        {
            return new CrossTfmReflectionImporter(module, this);
        }

        sealed class CrossTfmReflectionImporter : DefaultReflectionImporter
        {
            private readonly MSBuildAssemblyReferenceResolver m_helper;

            public CrossTfmReflectionImporter(ModuleDefinition module, MSBuildAssemblyReferenceResolver helper) : base(module)
            {
                m_helper = helper;
            }

            protected override IMetadataScope ImportScope(Type type)
            {
                AssemblyNameReference reference;
                if (!m_helper.m_frameworkTypes.Contains(type))
                {
                    // cook up a temporary AssemblyNameReference to avoid creating unwanted metadata table rows
                    var name = type.Assembly.GetName();
                    reference = new AssemblyNameReference(name.Name, name.Version)
                    {
                        Culture = name.CultureName,
                        PublicKeyToken = name.GetPublicKeyToken(),
                        HashAlgorithm = (AssemblyHashAlgorithm)name.HashAlgorithm,
                    };
                }
                else
                    reference = m_helper.m_frameworkReference;

                return m_helper.ImportCrossTfmScope(module, type.FullName.Replace('+', '/'), true, reference);
            }
        }
        #endregion

        #region --[Methods: Private]--------------------------------------
        private IMetadataScope ImportCrossTfmScope(ModuleDefinition module, string fullName, bool autoUnify, AssemblyNameReference reference)
        {
            var assembly = module.AssemblyResolver.Resolve(reference);
            if (assembly == null)
            {
                // can't resolve
                // use as-is and let downstream code deal with it
            }
            else
            if (IsFacadeAssembly(assembly))
            {
                var et = assembly.MainModule.ExportedTypes.FirstOrDefault(_ => _.FullName == fullName);
                if (et == null)
                    throw new KeyNotFoundException(String.Format(
                        "Framework type {0}!{1} was not found in target framework references", reference.Name, fullName));

                reference = (AssemblyNameReference)et.Scope;

                // netstandard forwards don't set mscorlib version
                // use compilation input
                if (reference.Version == ZeroVersion)
                    reference = m_projectReferences[ResolveToCodeBase(reference)];
            }
            else
            if (autoUnify && reference.Version != assembly.Name.Version)
            {
                // version mismatch between compilation inputs and what is loaded at runtime
                // prefer compilation input
                reference = assembly.Name;
            }

            // work around ModuleDefinition.MetadataImporter being internal:
            // import scope by means of importing a fake type reference
            // (from a fake sentinel module to avoid stack overflow)
            return module.ImportReference(new TypeReference("", "", SentinelModule, reference)).Scope;
        }

        private bool IsFacadeAssembly(AssemblyDefinition assembly)
        {
            // NB: in some cases reference items supplied by msbuild have a boolean Facade metadata, but not always
            // reference assemblies for both net4xx->netstandard and reverse cases are not marked with custom attributes
            // as a least bad alternative, I assume that a facade assembly has forwarders and no types except <Module>
            var m = assembly.MainModule;
            return m.HasExportedTypes && (!m.HasTypes || m.Types.Count == 1 && m.Types[0].Namespace == "" && m.Types[0].Name == "<Module>");
        }

        private AssemblyNameReference FindFrameworkReference()
        {
            foreach (var kv in m_projectReferences)
            {
                var filePath = kv.Key;
                var reference = kv.Value;

                // well known .NET framework and library public key tokens
                const ulong
                    MscorlibToken = 0x89e03419565c7ab7,
                    NetStdToken = 0x51dd2dcdff137bcc,
                    NetCoreToken = 0x8e79a7bed785ec7c,
                    StdlibToken = 0x3a0ad5117f5f3fb0;

                // prepare reverse map of framework types for reflection importer
                // NB: this code relies on having a netstandard dependency in project references
                if (reference.Name == "netstandard" && GetPublicKeyToken(reference) == NetStdToken &&
                    ResolveAs(reference, filePath) is AssemblyDefinition definition)
                {
                    var frameworkToken = GetPublicKeyToken(typeof(object).Assembly);
                    var frameworkAssemblies = new List<System.Reflection.Assembly>();
                    foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
                    {
                        // if a netstandard assembly is loaded, use it in preference of heuristic approach
                        var token = GetPublicKeyToken(assembly);
                        if (token == NetStdToken && assembly.GetName().Name == "netstandard")
                        {
                            frameworkAssemblies.Clear();
                            frameworkAssemblies.Add(assembly);
                            break;
                        }

                        if (token == frameworkToken ||
                            token == MscorlibToken ||
                            token == NetStdToken ||
                            token == NetCoreToken ||
                            token == StdlibToken)
                        {
                            frameworkAssemblies.Add(assembly);
                        }
                    }

                    foreach (var fullName in IsFacadeAssembly(definition) ?
                        definition.MainModule.ExportedTypes.Select(_ => _.FullName) :
                        definition.MainModule.Types.Select(_ => _.FullName))
                    {
                        var name = fullName.Replace('/', '+');
                        foreach (var fa in frameworkAssemblies)
                            if (fa.GetType(name) is Type type)
                                m_frameworkTypes.Add(type);
                    }

                    return reference;
                }
            }

            throw new InvalidOperationException("netstandard reference assembly not found in references");
        }

        private static ulong GetPublicKeyToken(AssemblyNameReference reference)
        {
            return ToUlongToken(reference.PublicKeyToken);
        }

        private static ulong GetPublicKeyToken(System.Reflection.Assembly assembly)
        {
            return ToUlongToken(assembly.GetName().GetPublicKeyToken());
        }

        private static ulong ToUlongToken(byte[] bytes)
        {
            return bytes != null && bytes.Length == 8 ? BitConverter.ToUInt64(bytes, 0) : 0;
        }
        #endregion
    }
}
