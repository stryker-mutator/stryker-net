using Microsoft.CodeAnalysis;
using Stryker.Core.Mutants;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Stryker.Core.ProjectComponents
{
    public class CsharpFolderComposite : ProjectComponent<SyntaxTree>, IFolderComposite
    {
        private readonly IList<SyntaxTree> _compilationSyntaxTrees = new List<SyntaxTree>();
        private readonly IList<IProjectComponent> _children = new List<IProjectComponent>();
        public IEnumerable<IProjectComponent> Children => _children;

        /// <summary>
        /// Add a syntax tree to this folder that is needed in compilation but should not be mutated
        /// </summary>
        /// <param name="syntaxTree"></param>
        public void AddCompilationSyntaxTree(SyntaxTree syntaxTree) => _compilationSyntaxTrees.Add(syntaxTree);

        public override IEnumerable<SyntaxTree> CompilationSyntaxTrees => _compilationSyntaxTrees.Union(
            Children.Cast<ProjectComponent<SyntaxTree>>().SelectMany(c => c.CompilationSyntaxTrees));
        public override IEnumerable<SyntaxTree> MutatedSyntaxTrees => Children.Cast<ProjectComponent<SyntaxTree>>().SelectMany(c => c.MutatedSyntaxTrees);

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

        public ReadOnlyFolderComposite ToReadOnly()
        {
            return new ReadOnlyFolderComposite(this, MutatedSyntaxTrees.Any());
        }

        public override IReadOnlyProjectComponent ToReadOnlyInputComponent()
        {
            return ToReadOnly();
        }

        public override IEnumerable<IProjectComponent> GetAllFiles()
        {
            return Children.SelectMany(x => x.GetAllFiles());
        }
    }
}
