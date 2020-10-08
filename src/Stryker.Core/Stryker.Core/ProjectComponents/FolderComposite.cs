using Microsoft.CodeAnalysis;
using Stryker.Core.Mutants;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace Stryker.Core.ProjectComponents
{
    public class FolderComposite : ProjectComponent
    {
        private readonly IList<SyntaxTree> _compilationSyntaxTrees = new List<SyntaxTree>();
        public ICollection<ProjectComponent> Children { get; set; } = new Collection<ProjectComponent>();

        /// <summary>
        /// Add a syntax tree to this folder that is needed in compilation but should not be mutated
        /// </summary>
        /// <param name="syntaxTree"></param>
        public void AddCompilationSyntaxTree(SyntaxTree syntaxTree) => _compilationSyntaxTrees.Add(syntaxTree);

        public override IEnumerable<SyntaxTree> CompilationSyntaxTrees => _compilationSyntaxTrees.Union(Children.SelectMany(c => c.CompilationSyntaxTrees));
        public override IEnumerable<SyntaxTree> MutatedSyntaxTrees => Children.SelectMany(c => c.MutatedSyntaxTrees);

        public override IEnumerable<Mutant> Mutants
        {
            get => Children.SelectMany(x => x.Mutants);
            set => throw new NotImplementedException();
        }

        public override IEnumerable<FileLeaf> GetAllFiles()
        {
            return Children.SelectMany(x => x.GetAllFiles());
        }

        public override void Add(ProjectComponent component)
        {
            component.Parent = this;
            Children.Add(component);
        }

        public override void Display(int depth)
        {
            // only walk this branch of the tree if there are MutatedSyntaxTrees, otherwise we have nothing to display.
            if (MutatedSyntaxTrees.Any())
            {
                // do not display root node
                if (!string.IsNullOrEmpty(Name))
                {
                    DisplayFolder(depth, this);
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
