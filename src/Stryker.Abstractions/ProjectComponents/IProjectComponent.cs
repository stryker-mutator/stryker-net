using System.Collections.Generic;
using Stryker.Abstractions.Options;

namespace Stryker.Abstractions.ProjectComponents;

public interface IReadOnlyProjectComponent
{
    string FullPath { get; }
    IEnumerable<IMutant> Mutants { get; }
    IFolderComposite Parent { get; }
    string RelativePath { get; set; }

    /// <summary>
    /// Path relative to the root of the current analysis (the solution directory when running in
    /// solution mode with multiple projects; otherwise identical to <see cref="RelativePath"/>).
    /// Unlike <see cref="RelativePath"/> (which is always relative to this component's own project,
    /// and is what <c>mutate</c>/<c>ignore</c> pattern matching uses), this is guaranteed unique and
    /// portable across the whole report, per the mutation-testing-report-schema. Used for report file
    /// keys and baseline lookups.
    /// </summary>
    string RootRelativePath { get; set; }
    Display DisplayFile { get; set; }
    Display DisplayFolder { get; set; }
    IEnumerable<IReadOnlyMutant> TotalMutants();
    IEnumerable<IReadOnlyMutant> ValidMutants();
    IEnumerable<IReadOnlyMutant> UndetectedMutants();
    IEnumerable<IReadOnlyMutant> DetectedMutants();
    IEnumerable<IReadOnlyMutant> InvalidMutants();
    IEnumerable<IReadOnlyMutant> IgnoredMutants();
    IEnumerable<IReadOnlyMutant> NotRunMutants();

    Health CheckHealth(IThresholds threshold);
    IEnumerable<IFileLeaf> GetAllFiles();
    void Display();
    double GetMutationScore();
}

public delegate void Display(IReadOnlyProjectComponent current);

public interface IProjectComponent : IReadOnlyProjectComponent
{
    new string FullPath { get; set; }
    new IEnumerable<IMutant> Mutants { get; set; }
    new IFolderComposite Parent { get; set; }
    new string RelativePath { get; set; }
    new string RootRelativePath { get; set; }
}
