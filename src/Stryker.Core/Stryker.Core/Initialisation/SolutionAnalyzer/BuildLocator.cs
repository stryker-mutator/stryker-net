using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Loader;
using Microsoft.Build.Locator;

namespace Stryker.Core.Initialisation.ProjectAnalyzer
{
    public class BuildLocator
    {
        public void Initialize()
        {
            IEnumerable<VisualStudioInstance> instances = MSBuildLocator.QueryVisualStudioInstances();
            //            bool registered = false;
            foreach (VisualStudioInstance vsi in instances)
            {
                Console.WriteLine($"Found version {vsi.Version} of SDK to process SLN and csproj files.");

                /*                            if (!registered)
                                            {
                                                registered = true;
                                                MSBuildLocator.RegisterInstance(instance);
                                            }
                  */
            }
            //VisualStudioInstance instance = instances.First();

            VisualStudioInstance instance = instances/*.Where(x => x.Version.ToString() == "3.1.302")*/.First();
            Console.WriteLine($"Using version {instance.Version} of SDK to process SLN and csproj files.");
            MSBuildLocator.RegisterInstance(instance);

            // We're using the installed version of the binaries to avoid a dependency between
            // the .NET Core SDK version and NuGet. This is a workaround due to the issue below:
            // https://github.com/microsoft/MSBuildLocator/issues/86
            AssemblyLoadContext.Default.Resolving += (assemblyLoadContext, assemblyName) =>
            {
                string path = Path.Combine(instance.MSBuildPath, assemblyName.Name + ".dll");
                if (File.Exists(path))
                {
                    try
                    {
                        Assembly assembly = assemblyLoadContext.LoadFromAssemblyPath(path);
                        return assembly;
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex);
                    }
                }
                return null;
            };
        }
    }
}
