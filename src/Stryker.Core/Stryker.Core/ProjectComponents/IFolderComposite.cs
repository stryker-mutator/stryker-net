using System.Collections.Generic;

namespace Stryker.Core.ProjectComponents
{
    public interface IFolderComposite : IProjectComponent
    {
        IEnumerable<IProjectComponent> Children { get; }

        void Add(IProjectComponent child);
        void AddRange(IEnumerable<IProjectComponent> children);
        ReadOnlyFolderComposite ToReadOnly();
    }
}
