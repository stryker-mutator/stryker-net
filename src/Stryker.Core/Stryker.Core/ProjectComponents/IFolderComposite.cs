using System.Collections.Generic;

namespace Stryker.Core.ProjectComponents
{
    public interface IFolderComposite : IProjectComponent
    {
        IEnumerable<IProjectComponent> Children { get; }

        ReadOnlyFolderComposite ToReadOnly();
    }
}
