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
        private readonly IList<SyntaxTree> _syntaxTrees;
        public ICollection<ProjectComponent> Children { get; set; }
        public FolderComposite()
        {
            Children = new Collection<ProjectComponent>();
        }

        public void AddCompilationSyntaxTree(SyntaxTree syntaxTree) => _syntaxTrees.Add(syntaxTree);

        public override IEnumerable<SyntaxTree> CompilationSyntaxTrees => _syntaxTrees.Union(Children.SelectMany(c => c.MutationSyntaxTrees));
        public override IEnumerable<SyntaxTree> MutationSyntaxTrees => Children.SelectMany(c => c.MutationSyntaxTrees);

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
            Children.Add(component);
        }

        public override void Display(int depth)
        {
            // do not display root node
            if (!string.IsNullOrEmpty(Name))
            {
                DisplayFolder(depth, this);
                depth += 2;
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
