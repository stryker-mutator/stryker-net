using Stryker.Core.Mutants;
using System.Collections.Generic;

namespace Stryker.Core.ProjectComponents
{
    public interface IProjectComponent
    {
        string FullPath { get; set; }
        /// <summary>
        /// Relative path to project file
        /// </summary>
        string RelativePath { get; set; }

        IParentComponent Parent { get; set; }

        IEnumerable<Mutant> Mutants { get; set; }

        IReadOnlyProjectComponent ToReadOnlyInputComponent();

        public abstract IEnumerable<IProjectComponent> GetAllFiles();
    }
}
