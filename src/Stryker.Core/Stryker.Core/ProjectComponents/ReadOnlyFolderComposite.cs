using Microsoft.CodeAnalysis;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace Stryker.Core.ProjectComponents
{
    public class ReadOnlyFolderComposite : ReadOnlyProjectComponent
    {

        private readonly FolderComposite _folderComposite;
        private bool _hasSyntaxTree;

        public ReadOnlyFolderComposite(FolderComposite folderComposite, bool hasSyntaxTree, ICollection<IProjectComponent> children) : base(folderComposite)
        {
            _folderComposite = folderComposite;
            _hasSyntaxTree = hasSyntaxTree;
            var convertedChildren = new Collection<IReadOnlyInputComponent>();
            foreach(var child in children)
            {
                convertedChildren.Add(child.ToReadOnlyBase());
            }
            Children = convertedChildren;
        }

        public ICollection<IReadOnlyInputComponent> Children { get; }


        public override void Display(int depth)
        {
            // only walk this branch of the tree if there are MutatedSyntaxTrees, otherwise we have nothing to display.
            if (_hasSyntaxTree)
            {
                DisplayFolder(depth, this);

                if (!string.IsNullOrEmpty(Name))
                {
                    depth++;
                }

                foreach (var child in Children)
                {
                    child.DisplayFile = DisplayFile;
                    child.DisplayFolder = DisplayFolder;
                    child.Display(depth);
                }
            }
        }

    }
}
