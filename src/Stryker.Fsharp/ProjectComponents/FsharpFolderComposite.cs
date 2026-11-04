using Stryker.Core.ProjectComponents;

namespace Stryker.Fsharp.ProjectComponents;

/// <summary>Folder in the F# component tree. F# has no Roslyn trees, so the inherited tree collections stay empty.</summary>
public class FsharpFolderComposite : FolderComposite;
