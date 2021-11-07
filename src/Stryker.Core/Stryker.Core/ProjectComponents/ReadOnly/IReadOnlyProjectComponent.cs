using System;
using System.Collections.Generic;
using Stryker.Core.Mutants;
using Stryker.Core.Options;

namespace Stryker.Core.ProjectComponents
{
    /// <summary>
    /// This interface should only contain readonly properties to ensure that only the mutation test process modifies components.
    /// </summary>
    public interface IReadOnlyProjectComponent : IEquatable<IReadOnlyProjectComponent>
    {
        string FullPath { get; }
        string RelativePath { get; }

        IFolderComposite Parent { get; }

        IEnumerable<IReadOnlyMutant> Mutants { get; }
        IEnumerable<IReadOnlyMutant> TotalMutants { get; }
        IEnumerable<IReadOnlyMutant> DetectedMutants { get; }

        /// <summary>
        /// The display handlers are an exception to the readonly rule
        /// </summary>
        Display DisplayFile { get; set; }
        Display DisplayFolder { get; set; }
        public void Display();

        double GetMutationScore();

        Health CheckHealth(Thresholds threshold);
    }

    public delegate void Display(IReadOnlyProjectComponent current);
}
