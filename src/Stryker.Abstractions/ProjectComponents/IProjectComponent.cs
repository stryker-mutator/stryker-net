using System.Collections.Generic;
using Stryker.Abstractions.Mutants;
using Stryker.Abstractions.Options;

namespace Stryker.Abstractions.ProjectComponents;

public interface IReadOnlyProjectComponent
{
    string FullPath { get; }
    IEnumerable<IMutant> Mutants { get; }
    IFolderComposite Parent { get; }
    string RelativePath { get; set; }
    public Display DisplayFile { get; set; }
    public Display DisplayFolder { get; set; }
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
}
