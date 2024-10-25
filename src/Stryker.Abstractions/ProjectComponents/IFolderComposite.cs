using System.Collections.Generic;

namespace Stryker.Abstractions.ProjectComponents;

public interface IFolderComposite : IProjectComponent
{
    IEnumerable<IReadOnlyProjectComponent> Children { get; }
    void Add(IProjectComponent child);
    void AddRange(IEnumerable<IProjectComponent> children);
}
