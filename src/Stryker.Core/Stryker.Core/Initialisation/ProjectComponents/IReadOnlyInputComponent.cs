using Stryker.Core.Mutants;
using System.Collections.Generic;

namespace Stryker.Core.Initialisation.ProjectComponent
{
    /// <summary>
    /// This interface should only contain readonly properties to ensure that others than the mutation test process cannot modify components.
    /// </summary>
    public interface IReadOnlyInputComponent
    {
        string Name { get; }
        IEnumerable<IReadOnlyMutant> ReadOnlyMutants { get; }
        IEnumerable<IReadOnlyMutant> TotalMutants { get; }
        IEnumerable<IReadOnlyMutant> DetectedMutants { get; }
        IReadOnlyCollection<FileLeaf> ExcludedFiles { get; }

        /// <summary>
        /// The display handlers are an exception to the readonly rule
        /// </summary>
        Display DisplayFile { get; set; }
        Display DisplayFolder { get; set; }

        void Display(int depth);

        decimal? GetMutationScore();
    }

    public delegate void Display(int depth, IReadOnlyInputComponent current);
}
