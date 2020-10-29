using Stryker.Core.Mutants;
using System.Collections.Generic;

namespace Stryker.Core.ProjectComponents
{
    public interface IProjectComponent
    {
        string Name { get; set; }

        string FullPath { get; set; }
        string RelativePath { get; set; }
        string RelativePathToProjectFile { get; set; }

        IParentComponent Parent { get; set; }

        IEnumerable<Mutant> Mutants { get; set; }

        IReadOnlyProjectComponent ToReadOnlyInputComponent();

        IEnumerable<IProjectComponent> GetAllFiles();
    }
}
