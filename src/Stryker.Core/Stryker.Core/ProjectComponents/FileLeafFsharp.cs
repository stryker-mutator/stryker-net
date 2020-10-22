using Stryker.Core.Mutants;
using System;
using System.Collections.Generic;
using static FSharp.Compiler.SyntaxTree;

namespace Stryker.Core.ProjectComponents
{
    public class FileLeafFsharp : ProjectComponent<ParsedInput>, IFileLeaf<ParsedInput>
    {
        public string SourceCode { get; set; }

        /// <summary>
        /// The original unmutated syntaxtree
        /// </summary>
        public ParsedInput SyntaxTree { get; set; }

        /// <summary>
        /// The mutated syntax tree
        /// </summary>
        public ParsedInput MutatedSyntaxTree { get; set; }

        public override IEnumerable<Mutant> Mutants { get; set; }

        public override IEnumerable<ParsedInput> CompilationSyntaxTrees => MutatedSyntaxTrees;

        public override IEnumerable<ParsedInput> MutatedSyntaxTrees => new List<ParsedInput> { MutatedSyntaxTree };

        public override void Display(int depth)
        {
            DisplayFile(depth, this);
        }

        public override void Add(ProjectComponent<ParsedInput> component)
        {
            // no children can be added to a file instance
            throw new NotImplementedException();
        }

        public override IEnumerable<IFileLeaf<ParsedInput>> GetAllFiles()
        {
            yield return this;
        }
    }
}
