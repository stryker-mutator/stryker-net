using System.Collections.Generic;
using System.Linq;

namespace Stryker.Core.ProjectComponents
{
    public class ReadOnlyFolderComposite : ReadOnlyProjectComponent
    {

        private readonly IFolderComposite _folderComposite;
        private readonly bool _belongsToProject;

        public IEnumerable<IReadOnlyProjectComponent> Children => _folderComposite.Children.Select(child => child.ToReadOnlyInputComponent());
        public ReadOnlyFolderComposite(IFolderComposite folderComposite, bool belongsToProjectUnderTest) : base(folderComposite)
        {
            _folderComposite = folderComposite;
            _belongsToProject = belongsToProjectUnderTest;
        }

        public override void Display()
        {
            // only walk this branch of the tree if it belongs to the source project, otherwise we have nothing to display.
            if (_belongsToProject)
            {
                DisplayFolder(this);

                foreach (var child in Children)
                {
                    child.DisplayFile = DisplayFile;
                    child.DisplayFolder = DisplayFolder;
                    child.Display();
                }
            }
        }

    }
}
