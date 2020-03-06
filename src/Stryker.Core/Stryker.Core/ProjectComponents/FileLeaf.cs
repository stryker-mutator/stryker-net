using Microsoft.CodeAnalysis;
using Stryker.Core.Mutants;
using System;
using System.Collections.Generic;

namespace Stryker.Core.ProjectComponents
{
    public class FileLeaf : ProjectComponent
    {
        public string SourceCode { get; set; }
        public SyntaxTree SyntaxTree { get; set; }
        public SyntaxTree MutatedSyntaxTree { get; set; }

        public override IEnumerable<Mutant> Mutants { get; set; }

        public override IEnumerable<SyntaxTree> CompilationSyntaxTrees => new List<SyntaxTree> { SyntaxTree };
        public override IEnumerable<SyntaxTree> MutationSyntaxTrees => new List<SyntaxTree> { MutatedSyntaxTree };

        public override void Display(int depth)
        {
            DisplayFile(depth, this);
        }

        public override void Add(ProjectComponent component)
        {
            // no children can be added to a file instance
            throw new NotImplementedException();
        }

        public override IEnumerable<FileLeaf> GetAllFiles()
        {
            yield return this;
        }
    }
}
