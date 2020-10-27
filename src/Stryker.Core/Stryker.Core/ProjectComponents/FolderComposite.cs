using Microsoft.CodeAnalysis;
using Stryker.Core.Mutants;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace Stryker.Core.ProjectComponents
{
    public interface IParentComponent
    {
        ICollection<IProjectComponent> Children { get; set; }
    }

    public class FolderComposite : ProjectComponent<SyntaxTree>, IParentComponent
    {
        private readonly IList<SyntaxTree> _compilationSyntaxTrees = new List<SyntaxTree>();
        public ICollection<ProjectComponent<SyntaxTree>> ChildrenTyped
        {
            get
            {
                var returnable = new Collection<ProjectComponent<SyntaxTree>>();
                foreach (var child in Children)
                { returnable.Add((ProjectComponent<SyntaxTree>)child); }
                return returnable;
            }
            set
            {
                var returnable = new Collection<IProjectComponent>();
                foreach (var child in value)
                { returnable.Add(child); }
                Children = returnable;
            }
        }
        public ICollection<IProjectComponent> Children { get; set; } = new Collection<IProjectComponent>();

        /// <summary>
        /// Add a syntax tree to this folder that is needed in compilation but should not be mutated
        /// </summary>
        /// <param name="syntaxTree"></param>
        public void AddCompilationSyntaxTree(SyntaxTree syntaxTree) => _compilationSyntaxTrees.Add(syntaxTree);

        public override IEnumerable<SyntaxTree> CompilationSyntaxTrees => _compilationSyntaxTrees.Union(ChildrenTyped.SelectMany(c => c.CompilationSyntaxTrees));
        public override IEnumerable<SyntaxTree> MutatedSyntaxTrees => ChildrenTyped.SelectMany(c => c.MutatedSyntaxTrees);

        public override IEnumerable<Mutant> Mutants
        {
            get => Children.SelectMany(x => x.Mutants);
            set => throw new NotImplementedException();
        }

        public override void Add(ProjectComponent<SyntaxTree> component)
        {
            component.Parent = this;
            Children.Add(component);
        }

        public ReadOnlyFolderComposite ToReadOnly()
        {
            return new ReadOnlyFolderComposite(this, MutatedSyntaxTrees.Any(), Children);
        }

        public override IReadOnlyInputComponent ToReadOnlyBase()
        {
            return ToReadOnly();
        }

        public override IEnumerable<IProjectComponent> GetAllFiles()
        {
            return Children.SelectMany(x => x.GetAllFiles());
        }
    }
}
