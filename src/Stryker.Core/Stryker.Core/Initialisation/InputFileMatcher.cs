using DotNet.Globbing;
using Microsoft.Extensions.Logging;
using Stryker.Core.Logging;
using Stryker.Core.Options;
using Stryker.Core.ProjectComponents;
using System.Collections.Generic;
using System.Linq;

namespace Stryker.Core.Initialisation
{
    public interface IInputFileMatcher
    {
        void MatchInputFiles(FolderComposite root, List<PathOption> mutate, string projectUnderTestPath);
    }

    public class InputFileMatcher : IInputFileMatcher
    {
        private ILogger _logger { get; set; }

        public InputFileMatcher()
        {
            _logger = ApplicationLogging.LoggerFactory.CreateLogger<IInputFileMatcher>();
            GlobOptions.Default.Evaluation.CaseInsensitive = true;
        }

        public void MatchInputFiles(FolderComposite root, List<PathOption> mutate, string projectUnderTestPath)
        {
            foreach (var child in root.Children)
            {
                if (mutate.Any(x => !x.Exclude))
                {
                    child.IsExcluded = true;
                }
                if (child is FolderComposite folder)
                {
                    // recursively scan further
                    MatchInputFiles(folder, mutate, projectUnderTestPath);
                }
                else
                {
                    if (mutate.Any(x => !x.Exclude && x.Matcher.IsMatch(child.RelativePath)))
                    {
                        child.IsExcluded = false;
                    }
                    else if (mutate.Any(x => x.Exclude && x.Matcher.IsMatch(child.RelativePath)))
                    {
                        _logger.LogInformation("Excluded {0}", child.FullPath);
                        child.IsExcluded = true;
                    }
                }
            }
        }
    }
}
