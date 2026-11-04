using System.IO.Abstractions;
using Stryker.Abstractions.ProjectComponents;
using Stryker.Core.Initialisation;

namespace Stryker.Fsharp;

/// <summary>Stub for the F# component tree builder; implemented in later tasks.</summary>
public class FsharpProjectComponentsBuilder(IFileSystem fileSystem) : ProjectComponentsBuilder(fileSystem)
{
    public override IReadOnlyProjectComponent Build() =>
        throw new NotImplementedException("F# component tree is not implemented yet.");

    public override void InjectHelpers(IReadOnlyProjectComponent inputFiles) =>
        throw new NotImplementedException("F# helper injection is not implemented yet.");

    public override Action PostBuildAction() =>
        throw new NotImplementedException("F# post-build action is not implemented yet.");
}
