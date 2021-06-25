using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Mono.Cecil;

namespace Stryker.Core.Initialisation
{
    // This (CrossPlatformAssemblyResolver) is a copy of Mono.Cecil's BaseAssemblyResolver with all the conditional compilation removed and changes made to "Resolve"
    // Original: https://github.com/jbevain/cecil/blob/7b8ee049a151204997eecf587c69acc2f67c8405/Mono.Cecil/BaseAssemblyResolver.cs
    // Author:
    //   Jb Evain (jbevain@gmail.com)
    //
    // Copyright (c) 2008 - 2015 Jb Evain
    // Copyright (c) 2008 - 2011 Novell, Inc.
    //
    // Licensed under the MIT/X11 license.
    //
    public class CrossPlatformAssemblyResolver : IAssemblyResolver
    {
        private static readonly bool _onMono = Type.GetType("Mono.Runtime") != null;
        private static readonly List<string> _directories = new List<string>(2) { ".", "bin" };

        // Maps file names of available trusted platform assemblies to their full paths.
        private static readonly Lazy<Dictionary<string, string>> TrustedPlatformAssemblies = new Lazy<Dictionary<string, string>>(CreateTrustedPlatformAssemblyMap);

        private List<string> _gacPaths;
        public event AssemblyResolveEventHandler ResolveFailure;

        public virtual AssemblyDefinition Resolve(AssemblyNameReference name) => Resolve(name, new ReaderParameters());

        public virtual AssemblyDefinition Resolve(AssemblyNameReference name, ReaderParameters parameters)
        {
            if (name == null)
            {
                throw new ArgumentNullException(nameof(name));
            }

            if (parameters == null)
            {
                throw new ArgumentNullException(nameof(parameters));
            }

            var assembly = SearchDirectory(name, _directories, parameters);
            if (assembly != null)
            {
                return assembly;
            }

            if (name.IsRetargetable)
            {
                // if the reference is retargetable, zero it
                name = new AssemblyNameReference(name.Name, new Version(0, 0, 0, 0))
                {
                    PublicKeyToken = new byte[0],
                };
            }

            //Try resolve as .NET core first (since stryker runs as .NET core, this is still the default)
            assembly = SearchTrustedPlatformAssemblies(name, parameters);
            if (assembly != null)
            {
                return assembly;
            }
            //If that fails, try as .NET framework
            else
            {
                var framework_dir = Path.GetDirectoryName(typeof(object).Module.FullyQualifiedName);
                var framework_dirs = _onMono
                    ? new[] { framework_dir, Path.Combine(framework_dir, "Facades") }
                    : new[] { framework_dir };

                if (IsZero(name.Version))
                {
                    assembly = SearchDirectory(name, framework_dirs, parameters);
                    if (assembly != null)
                    {
                        return assembly;
                    }
                }

                if (name.Name == "mscorlib")
                {
                    assembly = GetCorlib(name, parameters);
                    if (assembly != null)
                    {
                        return assembly;
                    }
                }

                assembly = GetAssemblyInGac(name, parameters);
                if (assembly != null)
                {
                    return assembly;
                }

                assembly = SearchDirectory(name, framework_dirs, parameters);
                if (assembly != null)
                {
                    return assembly;
                }
            }

            if (ResolveFailure != null)
            {
                assembly = ResolveFailure(this, name);
                if (assembly != null)
                {
                    return assembly;
                }
            }

            throw new AssemblyResolutionException(name);
        }

        private AssemblyDefinition GetAssembly(string file, ReaderParameters parameters)
        {
            if (parameters.AssemblyResolver == null)
            {
                parameters.AssemblyResolver = this;
            }

            return ModuleDefinition.ReadModule(file, parameters).Assembly;
        }

        private AssemblyDefinition SearchTrustedPlatformAssemblies(AssemblyNameReference name, ReaderParameters parameters)
        {
            if (name.IsWindowsRuntime)
            {
                return null;
            }

            return TrustedPlatformAssemblies.Value.TryGetValue(name.Name, out var path) ? GetAssembly(path, parameters) : null;
        }

        private static Dictionary<string, string> CreateTrustedPlatformAssemblyMap()
        {
            var result = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

            string paths;

            try
            {
                paths = (string)AppDomain.CurrentDomain.GetData("TRUSTED_PLATFORM_ASSEMBLIES");
            }
            catch
            {
                paths = null;
            }

            if (paths == null)
            {
                return result;
            }

            foreach (var path in paths.Split(Path.PathSeparator))
            {
                if (string.Equals(Path.GetExtension(path), ".dll", StringComparison.OrdinalIgnoreCase))
                {
                    result[Path.GetFileNameWithoutExtension(path)] = path;
                }
            }

            return result;
        }

        protected virtual AssemblyDefinition SearchDirectory(AssemblyNameReference name,
            IEnumerable<string> directories, ReaderParameters parameters)
        {
            var extensions = name.IsWindowsRuntime ? new[] { ".winmd", ".dll" } : new[] { ".exe", ".dll" };
            foreach (var directory in directories)
            {
                foreach (var extension in extensions)
                {
                    var file = Path.Combine(directory, name.Name + extension);
                    if (!File.Exists(file))
                    {
                        continue;
                    }

                    try
                    {
                        return GetAssembly(file, parameters);
                    }
                    catch (System.BadImageFormatException)
                    {
                        continue;
                    }
                }
            }

            return null;
        }

        private static bool IsZero(Version version) => version.Major == 0 && version.Minor == 0 && version.Build == 0 && version.Revision == 0;

        private AssemblyDefinition GetCorlib(AssemblyNameReference reference, ReaderParameters parameters)
        {
            var version = reference.Version;
            var corlib = typeof(object).Assembly.GetName();
            if (corlib.Version == version || IsZero(version))
            {
                return GetAssembly(typeof(object).Module.FullyQualifiedName, parameters);
            }

            var path = Directory.GetParent(Directory.GetParent(typeof(object).Module.FullyQualifiedName).FullName).FullName;

            if (_onMono)
            {
                if (version.Major == 1)
                {
                    path = Path.Combine(path, "1.0");
                }
                else if (version.Major == 2)
                {
                    if (version.MajorRevision == 5)
                    {
                        path = Path.Combine(path, "2.1");
                    }
                    else
                    {
                        path = Path.Combine(path, "2.0");
                    }
                }
                else if (version.Major == 4)
                {
                    path = Path.Combine(path, "4.0");
                }
                else
                {
                    throw new NotSupportedException("Version not supported: " + version);
                }
            }
            else
            {
                switch (version.Major)
                {
                    case 1:
                        path = version.MajorRevision == 3300 ? Path.Combine(path, "v1.0.3705") : Path.Combine(path, "v1.1.4322");
                        break;
                    case 2:
                        path = Path.Combine(path, "v2.0.50727");
                        break;
                    case 4:
                        path = Path.Combine(path, "v4.0.30319");
                        break;
                    default:
                        throw new NotSupportedException("Version not supported: " + version);
                }
            }

            var file = Path.Combine(path, "mscorlib.dll");
            if (File.Exists(file))
            {
                return GetAssembly(file, parameters);
            }

            if (_onMono && Directory.Exists(path + "-api"))
            {
                file = Path.Combine(path + "-api", "mscorlib.dll");
                if (File.Exists(file))
                {
                    return GetAssembly(file, parameters);
                }
            }

            return null;
        }

        private static List<string> GetGacPaths()
        {
            if (_onMono)
            {
                return GetDefaultMonoGacPaths();
            }

            var paths = new List<string>(2);
            var windir = Environment.GetEnvironmentVariable("WINDIR");
            if (windir == null)
            {
                return paths;
            }

            paths.Add(Path.Combine(windir, "assembly"));
            paths.Add(Path.Combine(windir, Path.Combine("Microsoft.NET", "assembly")));
            return paths;
        }

        private static List<string> GetDefaultMonoGacPaths()
        {
            var paths = new List<string>(1);
            var gac = GetCurrentMonoGac();
            if (gac != null)
            {
                paths.Add(gac);
            }

            var gac_paths_env = Environment.GetEnvironmentVariable("MONO_GAC_PREFIX");
            if (string.IsNullOrEmpty(gac_paths_env))
            {
                return paths;
            }

            var prefixes = gac_paths_env.Split(Path.PathSeparator);
            foreach (var prefix in prefixes)
            {
                if (string.IsNullOrEmpty(prefix))
                {
                    continue;
                }

                var gac_path = Path.Combine(Path.Combine(Path.Combine(prefix, "lib"), "mono"), "gac");
                if (Directory.Exists(gac_path) && !paths.Contains(gac))
                {
                    paths.Add(gac_path);
                }
            }

            return paths;
        }

        private static string GetCurrentMonoGac() => Path.Combine(Directory.GetParent(Path.GetDirectoryName(typeof(object).Module.FullyQualifiedName)).FullName, "gac");

        private AssemblyDefinition GetAssemblyInGac(AssemblyNameReference reference, ReaderParameters parameters)
        {
            if (reference.PublicKeyToken == null || reference.PublicKeyToken.Length == 0)
            {
                return null;
            }

            if (_gacPaths == null)
            {
                _gacPaths = GetGacPaths();
            }

            if (_onMono)
            {
                return GetAssemblyInMonoGac(reference, parameters);
            }

            return GetAssemblyInNetGac(reference, parameters);
        }

        private AssemblyDefinition GetAssemblyInMonoGac(AssemblyNameReference reference, ReaderParameters parameters)
        {
            for (var i = 0; i < _gacPaths.Count; i++)
            {
                var gac_path = _gacPaths[i];
                var file = GetAssemblyFile(reference, string.Empty, gac_path);
                if (File.Exists(file))
                {
                    return GetAssembly(file, parameters);
                }
            }

            return null;
        }

        private AssemblyDefinition GetAssemblyInNetGac(AssemblyNameReference reference, ReaderParameters parameters)
        {
            var gacs = new[] { "GAC_MSIL", "GAC_32", "GAC_64", "GAC" };
            var prefixes = new[] { string.Empty, "v4.0_" };

            for (var i = 0; i < _gacPaths.Count; i++)
            {
                for (var j = 0; j < gacs.Length; j++)
                {
                    var gac = Path.Combine(_gacPaths[i], gacs[j]);
                    var file = GetAssemblyFile(reference, prefixes[i], gac);
                    if (Directory.Exists(gac) && File.Exists(file))
                    {
                        return GetAssembly(file, parameters);
                    }
                }
            }

            return null;
        }

        private static string GetAssemblyFile(AssemblyNameReference reference, string prefix, string gac)
        {
            var gacFolder = new StringBuilder()
                .Append(prefix)
                .Append(reference.Version)
                .Append("__");

            for (var i = 0; i < reference.PublicKeyToken.Length; i++)
            {
                _ = gacFolder.Append(reference.PublicKeyToken[i].ToString("x2"));
            }

            return Path.Combine(
                Path.Combine(
                    Path.Combine(gac, reference.Name), gacFolder.ToString()),
                reference.Name + ".dll");
        }


        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
        }
    }
}
