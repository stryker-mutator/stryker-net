using System;
using System.Collections.Generic;

namespace Stryker.Core.ProjectComponents
{
    public class Solution : ProjectComponent
    {
        private readonly IList<IProjectComponent> _children = new List<IProjectComponent>();

        public IEnumerable<IProjectComponent> Children => _children;

        public void Add(IProjectComponent child)
        {
            _children.Add(child);
        }

        public void AddRange(IEnumerable<IProjectComponent> children)
        {
            foreach (var child in children)
            {
                Add(child);
            }
        }

        public override void Display()
        {
            foreach (var child in Children)
            {
                child.DisplayFile = DisplayFile;
                child.DisplayFolder = DisplayFolder;
                child.Display();
            }
        }

        public override IEnumerable<IFileLeaf> GetAllFiles() => throw new NotImplementedException();
    }
}
