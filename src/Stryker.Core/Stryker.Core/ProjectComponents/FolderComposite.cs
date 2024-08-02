using System;
using System.Collections.Generic;
using System.Linq;
using Stryker.Configuration.Mutants;

namespace Stryker.Configuration.ProjectComponents
{
    public interface IReadOnlyFolderComposite : IProjectComponent
    {
        IEnumerable<IProjectComponent> Children { get; }
        void Add(IProjectComponent child);
        void AddRange(IEnumerable<IProjectComponent> children);
    }

    public class FolderComposite<T> : ProjectComponent<T>, IReadOnlyFolderComposite
    {
        private readonly List<IProjectComponent> _children = [];
        public readonly List<T> _compilationSyntaxTrees = [];

        public IEnumerable<IProjectComponent> Children => _children;

        public override IEnumerable<Mutant> Mutants
        {
            get => Children.SelectMany(x => x.Mutants);
            set => throw new NotSupportedException("Folders do not contain mutants.");
        }

        /// <summary>
        /// Add a syntax tree to this folder that is needed in compilation but should not be mutated
        /// </summary>
        public void AddCompilationSyntaxTree(T syntaxTree) => _compilationSyntaxTrees.Add(syntaxTree);
        public override IEnumerable<T> CompilationSyntaxTrees => _compilationSyntaxTrees.Union(ChildCompilationSyntaxTree);
        private IEnumerable<T> ChildCompilationSyntaxTree => Children.Cast<ProjectComponent<T>>().SelectMany(c => c.CompilationSyntaxTrees);
        public override IEnumerable<T> MutatedSyntaxTrees => Children.Cast<ProjectComponent<T>>().SelectMany(c => c.MutatedSyntaxTrees);

        public void Add(IProjectComponent child)
        {
            child.Parent = this;
            _children.Add(child);
        }

        public void AddRange(IEnumerable<IProjectComponent> children)
        {
            foreach (var child in children)
            {
                Add(child);
            }
        }

        public override IEnumerable<IFileLeaf> GetAllFiles() => Children.SelectMany(x => x.GetAllFiles());
        public override void Display()
        {
            // only walk this branch of the tree if it belongs to the source project, otherwise we have nothing to display.
            if (MutatedSyntaxTrees.Any())
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
