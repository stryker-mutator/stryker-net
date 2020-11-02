using System.Collections.Generic;

namespace Stryker.Core.ProjectComponents
{
    public interface IParentComponent : IProjectComponent
    {
        IEnumerable<IProjectComponent> Children { get; }

        ReadOnlyFolderComposite ToReadOnly();
    }
}
