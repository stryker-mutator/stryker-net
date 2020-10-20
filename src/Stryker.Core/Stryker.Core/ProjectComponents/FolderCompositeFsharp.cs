using Stryker.Core.Mutants;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using static FSharp.Compiler.SyntaxTree;

namespace Stryker.Core.ProjectComponents
{
    public class FolderCompositeFsharp : ProjectComponent<ParsedInput>
    {
        private readonly IList<ParsedInput> _compilationSyntaxTrees = new List<ParsedInput>();
        public ICollection<ProjectComponent<ParsedInput>> Children { get; set; } = new Collection<ProjectComponent<ParsedInput>>();

        /// <summary>
        /// Add a syntax tree to this folder that is needed in compilation but should not be mutated
        /// </summary>
        /// <param name="syntaxTree"></param>
        public void AddCompilationSyntaxTree(ParsedInput syntaxTree) => _compilationSyntaxTrees.Add(syntaxTree);

        public override IEnumerable<ParsedInput> CompilationSyntaxTrees => _compilationSyntaxTrees.Union(Children.SelectMany(c => c.CompilationSyntaxTrees));
        public override IEnumerable<ParsedInput> MutatedSyntaxTrees => Children.SelectMany(c => c.MutatedSyntaxTrees);

        public override IEnumerable<Mutant> Mutants
        {
            get => Children.SelectMany(x => x.Mutants);
            set => throw new NotImplementedException();
        }

        public override IEnumerable<IFileLeaf<ParsedInput>> GetAllFiles()
        {
            return Children.SelectMany(x => x.GetAllFiles());
        }

        public override void Add(ProjectComponent<ParsedInput> component)
        {
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
}
