namespace Stryker.Abstractions.ProjectComponents;

public interface IReadOnlyFolderComposite : IReadOnlyProjectComponent
{
    IEnumerable<IReadOnlyProjectComponent> Children { get; }
    void Add(IReadOnlyProjectComponent child);
    void AddRange(IEnumerable<IReadOnlyProjectComponent> children);
}
