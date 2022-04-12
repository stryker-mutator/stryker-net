using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Loader;
using Microsoft.Build.Locator;
using Microsoft.Extensions.Logging;
using Stryker.Core.Logging;

namespace Stryker.Core.Initialisation.ProjectAnalyzer
{
    public class BuildLocator
    {
        private readonly ILogger _logger = ApplicationLogging.LoggerFactory.CreateLogger<BuildLocator>();

        public virtual void Initialize()
        {
            IEnumerable<VisualStudioInstance> instances = MSBuildLocator.QueryVisualStudioInstances();
            foreach (VisualStudioInstance vsi in instances)
            {
                _logger.LogDebug($"Found version {vsi.Version} of SDK to process SLN and csproj files.");
            }
            VisualStudioInstance instance = instances/*.Where(x => x.Version.ToString() == "3.1.302")*/.First();
            _logger.LogInformation($"Using version {instance.Version} of SDK to process SLN and csproj files.");
            MSBuildLocator.RegisterInstance(instance);
        }
    }
}
