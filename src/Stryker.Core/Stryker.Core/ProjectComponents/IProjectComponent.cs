using Stryker.Core.Mutants;
using System.Collections.Generic;

namespace Stryker.Core.ProjectComponents
{
    public interface IProjectComponent
    {
        public string Name { get; set; }
        IEnumerable<Mutant> Mutants { get; set; }
        IParentComponent Parent { get; set; }
        public string RelativePath { get; set; }
        public string RelativePathToProjectFile { get; set; }
        public string FullPath { get; set; }

        IReadOnlyInputComponent ToReadOnlyBase();

        public abstract IEnumerable<IProjectComponent> GetAllFiles();
    }
}