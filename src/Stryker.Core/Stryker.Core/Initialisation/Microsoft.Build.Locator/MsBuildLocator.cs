// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;

namespace Microsoft.Build.Locator
{
    public static class MSBuildLocator
    {
        private const string MSBuildPublicKeyToken = "b03f5f7f11d50a3a";

        private static readonly string[] s_msBuildAssemblies =
        {
        "Microsoft.Build",
        "Microsoft.Build.Framework",
        "Microsoft.Build.Tasks.Core",
        "Microsoft.Build.Utilities.Core"
    };

        private static ResolveEventHandler registeredHandler;

        // Used to determine when it's time to unregister the registeredHandler.
        private static int numResolvedAssemblies;

        /// <summary>
        ///     Gets a value indicating whether an instance of MSBuild is currently registered.
        /// </summary>
        public static bool IsRegistered => registeredHandler != null;

        /// <summary>
        ///     Gets a value indicating whether an instance of MSBuild can be registered.
        /// </summary>
        /// <remarks>
        ///     If any Microsoft.Build assemblies are already loaded into the current AppDomain, the value will be false.
        /// </remarks>
        public static bool CanRegister => !IsRegistered && !LoadedMsBuildAssemblies.Any();

        private static IEnumerable<Assembly> LoadedMsBuildAssemblies => AppDomain.CurrentDomain.GetAssemblies().Where(IsMSBuildAssembly);

        /// <summary>
        ///     Query for all Visual Studio instances.
        /// </summary>
        /// <remarks>
        ///     Only includes Visual Studio 2017 (v15.0) and higher.
        /// </remarks>
        /// <returns>Enumeration of all Visual Studio instances detected on the machine.</returns>
        public static IEnumerable<VisualStudioInstance> QueryVisualStudioInstances()
        {
            return QueryVisualStudioInstances(VisualStudioInstanceQueryOptions.Default);
        }

        /// <summary>
        ///     Query for Visual Studio instances matching the given options.
        /// </summary>
        /// <remarks>
        ///     Only includes Visual Studio 2017 (v15.0) and higher.
        /// </remarks>
        /// <param name="options">Query options for Visual Studio instances.</param>
        /// <returns>Enumeration of Visual Studio instances detected on the machine.</returns>
        public static IEnumerable<VisualStudioInstance> QueryVisualStudioInstances(
            VisualStudioInstanceQueryOptions options)
        {
            return GetInstances().Where(i => i.DiscoveryType.HasFlag(options.DiscoveryTypes));
        }

        /// <summary>
        ///     Discover instances of Visual Studio and register the first one. See <see cref="RegisterInstance" />.
        /// </summary>
        /// <returns>Instance of Visual Studio found and registered.</returns>
        public static VisualStudioInstance RegisterDefaults()
        {
            var instance = GetInstances().FirstOrDefault();
            if (instance == null)
            {
                var error = "No instances of MSBuild could be detected." +
                            Environment.NewLine +
                            $"Try calling {nameof(RegisterInstance)} or {nameof(RegisterMSBuildPath)} to manually register one.";

                throw new InvalidOperationException(error);
            }

            RegisterInstance(instance);

            return instance;
        }

        /// <summary>
        ///     Add assembly resolution for Microsoft.Build core dlls in the current AppDomain from the specified
        ///     instance of Visual Studio. See <see cref="QueryVisualStudioInstances()" /> to discover Visual Studio
        ///     instances or use <see cref="RegisterDefaults" />.
        /// </summary>
        /// <param name="instance"></param>
        public static void RegisterInstance(VisualStudioInstance instance)
        {
            if (instance == null)
            {
                throw new ArgumentNullException(nameof(instance));
            }

            RegisterMSBuildPath(instance.MSBuildPath);
        }

        /// <summary>
        ///     Add assembly resolution for Microsoft.Build core dlls in the current AppDomain from the specified
        ///     path.
        /// </summary>
        /// <param name="msbuildPath">
        ///     Path to the directory containing a deployment of MSBuild binaries.
        ///     A minimal MSBuild deployment would be the publish result of the Microsoft.Build.Runtime package.
        ///
        ///     In order to restore and build real projects, one needs a deployment that contains the rest of the toolchain (nuget, compilers, etc.).
        ///     Such deployments can be found in installations such as Visual Studio or dotnet CLI.
        /// </param>
        public static void RegisterMSBuildPath(string msbuildPath)
        {
            if (string.IsNullOrWhiteSpace(msbuildPath))
            {
                throw new ArgumentException("Value may not be null or whitespace", nameof(msbuildPath));
            }

            if (!Directory.Exists(msbuildPath))
            {
                throw new ArgumentException($"Directory \"{msbuildPath}\" does not exist", nameof(msbuildPath));
            }

            if (!CanRegister)
            {
                var loadedAssemblyList = string.Join(Environment.NewLine, LoadedMsBuildAssemblies.Select(a => a.GetName()));

                var error = $"{typeof(MSBuildLocator)}.{nameof(RegisterInstance)} was called, but MSBuild assemblies were already loaded." +
                    Environment.NewLine +
                    $"Ensure that {nameof(RegisterInstance)} is called before any method that directly references types in the Microsoft.Build namespace has been called." +
                    Environment.NewLine +
                    "Loaded MSBuild assemblies: " +
                    loadedAssemblyList;

                throw new InvalidOperationException(error);
            }

            // AssemblyResolve event can fire multiple times for the same assembly, so keep track of what's already been loaded.
            var loadedAssemblies = new Dictionary<string, Assembly>(s_msBuildAssemblies.Length);

            // Saving the handler in a static field so it can be unregistered later.
            registeredHandler = (_, eventArgs) =>
            {
                // Assembly resolution is not thread-safe.
                lock (loadedAssemblies)
                {
                    var assemblyNameString = eventArgs.Name;
                    if (loadedAssemblies.TryGetValue(assemblyNameString, out var assembly))
                    {
                        return assembly;
                    }

                    var assemblyName = new AssemblyName(eventArgs.Name);
                    if (IsMSBuildAssembly(assemblyName))
                    {
                        var targetAssembly = Path.Combine(msbuildPath, assemblyName.Name + ".dll");
                        if (File.Exists(targetAssembly))
                        {
                            // Automatically unregister the handler once all supported assemblies have been loaded.
                            if (Interlocked.Increment(ref numResolvedAssemblies) == s_msBuildAssemblies.Length)
                            {
                                Unregister();
                            }

                            assembly = Assembly.LoadFrom(targetAssembly);
                            loadedAssemblies.Add(assemblyNameString, assembly);
                            return assembly;
                        }
                    }

                    return null;
                }
            };

            AppDomain.CurrentDomain.AssemblyResolve += registeredHandler;
        }

        /// <summary>
        ///     Remove assembly resolution previously registered via <see cref="RegisterInstance" />, <see cref="RegisterMSBuildPath" />, or <see cref="RegisterDefaults" />.
        /// </summary>
        /// <remarks>
        ///     This will automatically be called once all supported assemblies are loaded into the current AppDomain and so generally is not necessary to call directly.
        /// </remarks>
        public static void Unregister()
        {
            if (!IsRegistered)
            {
                var error = $"{typeof(MSBuildLocator)}.{nameof(Unregister)} was called, but no MSBuild instance is registered." + Environment.NewLine;
                if (numResolvedAssemblies == 0)
                {
                    error += $"Ensure that {nameof(RegisterInstance)}, {nameof(RegisterMSBuildPath)}, or {nameof(RegisterDefaults)} is called before calling this method.";
                }
                else
                {
                    error += "Unregistration automatically occurs once all supported assemblies are loaded into the current AppDomain and so generally is not necessary to call directly.";
                }

                error += Environment.NewLine +
                            $"{nameof(IsRegistered)} should be used to determine whether calling {nameof(Unregister)} is a valid operation.";

                throw new InvalidOperationException(error);
            }

            AppDomain.CurrentDomain.AssemblyResolve -= registeredHandler;
        }

        private static bool IsMSBuildAssembly(Assembly assembly) => IsMSBuildAssembly(assembly.GetName());

        private static bool IsMSBuildAssembly(AssemblyName assemblyName)
        {
            if (!s_msBuildAssemblies.Contains(assemblyName.Name, StringComparer.OrdinalIgnoreCase))
            {
                return false;
            }

            var publicKeyToken = assemblyName.GetPublicKeyToken();
            if (publicKeyToken == null || publicKeyToken.Length == 0)
            {
                return false;
            }

            var sb = new StringBuilder();
            foreach (var b in publicKeyToken)
            {
                sb.Append($"{b:x2}");
            }

            return sb.ToString().Equals(MSBuildPublicKeyToken, StringComparison.OrdinalIgnoreCase);
        }

        private static IEnumerable<VisualStudioInstance> GetInstances()
        {
            var devConsole = GetDevConsoleInstance();
            if (devConsole != null)
                yield return devConsole;

            foreach (var instance in VisualStudioLocationHelper.GetInstances())
                yield return instance;
        }

        private static VisualStudioInstance GetDevConsoleInstance()
        {
            var path = Environment.GetEnvironmentVariable("VSINSTALLDIR");
            if (!string.IsNullOrEmpty(path))
            {
                var versionString = Environment.GetEnvironmentVariable("VSCMD_VER");
                Version version;
                Version.TryParse(versionString, out version);

                if (version == null && versionString?.Contains('-') == true)
                {
                    versionString = versionString.Substring(0, versionString.IndexOf('-'));
                    Version.TryParse(versionString, out version);
                }

                if (version == null)
                {
                    versionString = Environment.GetEnvironmentVariable("VisualStudioVersion");
                    Version.TryParse(versionString, out version);
                }

                return new VisualStudioInstance("DEVCONSOLE", path, version, DiscoveryType.DeveloperConsole);
            }

            return null;
        }
    }
}
