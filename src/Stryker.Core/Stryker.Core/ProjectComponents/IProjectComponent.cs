using Stryker.Core.Mutants;
using System.Collections.Generic;

namespace Stryker.Core.ProjectComponents
{
    public interface IProjectComponent : IReadOnlyInputComponent
    {
        IEnumerable<Mutant> Mutants { get; set; }
    }
}