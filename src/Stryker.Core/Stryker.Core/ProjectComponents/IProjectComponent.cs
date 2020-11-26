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

        IFolderComposite Parent { get; set; }

        IEnumerable<Mutant> Mutants { get; set; }

        IReadOnlyProjectComponent ToReadOnlyInputComponent();

        public abstract IEnumerable<IProjectComponent> GetAllFiles();
    }
}