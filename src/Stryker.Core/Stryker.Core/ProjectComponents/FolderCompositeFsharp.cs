using Stryker.Core.Mutants;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using static FSharp.Compiler.SyntaxTree;

namespace Stryker.Core.ProjectComponents
{
    public class FolderCompositeFsharp : ProjectComponent<ParsedInput>, IFolderComposite
    {
        private readonly IList<ParsedInput> _compilationSyntaxTrees = new List<ParsedInput>();
        private readonly IList<IProjectComponent> _children = new List<IProjectComponent>();
        public IEnumerable<IProjectComponent> Children => _children;

        /// <summary>
        /// Add a syntax tree to this folder that is needed in compilation but should not be mutated
        /// </summary>
        /// <param name="syntaxTree"></param>
        public void AddCompilationSyntaxTree(ParsedInput syntaxTree) => _compilationSyntaxTrees.Add(syntaxTree);

        public override IEnumerable<ParsedInput> CompilationSyntaxTrees => _compilationSyntaxTrees.Union(
            Children.Cast<ProjectComponent<ParsedInput>>().SelectMany(c => c.CompilationSyntaxTrees));
        public override IEnumerable<ParsedInput> MutatedSyntaxTrees => Children.Cast<ProjectComponent<ParsedInput>>().SelectMany(c => c.MutatedSyntaxTrees);

        public override IEnumerable<Mutant> Mutants
        {
            get => Children.SelectMany(x => x.Mutants);
            set => throw new NotSupportedException("Folders do not contain mutants.");
        }

        public void Add(IProjectComponent component)
        {
            component.Parent = this;
            _children.Add(component);
        }
        public void AddRange(IEnumerable<IProjectComponent> children)
        {
            foreach(var child in children)
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
