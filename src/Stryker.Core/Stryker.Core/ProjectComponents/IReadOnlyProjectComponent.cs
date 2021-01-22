using Stryker.Core.Mutants;
using Stryker.Core.Options;
using System;
using System.Collections.Generic;

namespace Stryker.Core.ProjectComponents
{
    /// <summary>
    /// This interface should only contain readonly properties to ensure that others than the mutation test process cannot modify components.
    /// </summary>
    public interface IReadOnlyProjectComponent : IEquatable<IReadOnlyProjectComponent>
    {
        string FullPath { get; }
        string RelativePath { get; }
        IParentComponent Parent { get; }

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

        Health CheckHealth(Threshold threshold);
    }

    public delegate void Display(IReadOnlyProjectComponent current);

}
