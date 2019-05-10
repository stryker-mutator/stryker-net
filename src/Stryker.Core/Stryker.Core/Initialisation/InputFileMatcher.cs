using DotNet.Globbing;
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
        public InputFileMatcher()
        {
            GlobOptions.Default.Evaluation.CaseInsensitive = true;
        }

        public void MatchInputFiles(FolderComposite root, List<PathOption> mutate, string projectUnderTestPath)
        {
            foreach (var child in root.Children)
            {
                if (child is FolderComposite folder)
                {
                    if (mutate.Where(x => !x.Exclude).All(x => !x.Matcher.IsMatch(folder.RelativePath)))
                    {
                        foreach (var file in child.GetAllFiles())
                        {
                            file.IsExcluded = true;
                        }
                    }
                    // recursively scan further
                    MatchInputFiles(folder, mutate, projectUnderTestPath);
                }
                else
                {
                    if (mutate.Any(x => !x.Exclude && x.Matcher.IsMatch(child.RelativePath)))
                    {
                        //child.IsExcluded = false;
                    }
                    else if (mutate.Any(x => x.Exclude && x.Matcher.IsMatch(child.RelativePath)))
                    {
                        child.IsExcluded = true;
                    }
                }
            }
        }
    }
}
