using System;
using System.Collections.Generic;
using System.Linq;
using Stryker.Core.Mutants;

namespace Stryker.Core.ProjectComponents
{
    public class FolderComposite<T> : ProjectComponent<T>, IFolderComposite
    {
        private readonly IList<IProjectComponent> _children = new List<IProjectComponent>();
        public IEnumerable<IProjectComponent> Children => _children;
        private readonly IList<T> _compilationSyntaxTrees = new List<T>();

        public override IEnumerable<Mutant> Mutants
        {
            get => Children.SelectMany(x => x.Mutants);
            set => throw new NotSupportedException("Folders do not contain mutants.");
        }

        /// <summary>
        /// Add a syntax tree to this folder that is needed in compilation but should not be mutated
        /// </summary>
        /// <param name="syntaxTree"></param>
        public void AddCompilationSyntaxTree(T syntaxTree) => _compilationSyntaxTrees.Add(syntaxTree);
        public override IEnumerable<T> CompilationSyntaxTrees => _compilationSyntaxTrees.Union(
            Children.Cast<ProjectComponent<T>>().SelectMany(c => c.CompilationSyntaxTrees));
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

        public override IEnumerable<IProjectComponent> GetAllFiles() => Children.SelectMany(x => x.GetAllFiles());
        public ReadOnlyFolderComposite ToReadOnly() => new ReadOnlyFolderComposite(this, MutatedSyntaxTrees.Any());
        public override IReadOnlyProjectComponent ToReadOnlyInputComponent() => ToReadOnly();
    }

    public class FolderComposite : ProjectComponent, IFolderComposite
    {
        private readonly IList<IProjectComponent> _children = new List<IProjectComponent>();
        public IEnumerable<IProjectComponent> Children => _children;

        public override IEnumerable<Mutant> Mutants
        {
            get => Children.SelectMany(x => x.Mutants);
            set => throw new NotSupportedException("Folders do not contain mutants.");
        }

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

        public override IEnumerable<IProjectComponent> GetAllFiles() => Children.SelectMany(x => x.GetAllFiles());
        public ReadOnlyFolderComposite ToReadOnly() => new ReadOnlyFolderComposite(this, false);
        public override IReadOnlyProjectComponent ToReadOnlyInputComponent() => ToReadOnly();
    }
}
