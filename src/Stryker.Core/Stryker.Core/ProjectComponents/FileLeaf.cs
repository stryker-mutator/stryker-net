using Microsoft.CodeAnalysis;
using Stryker.Core.Mutants;
using System;
using System.Collections.Generic;

namespace Stryker.Core.ProjectComponents
{
    public class FileLeaf : ProjectComponent<SyntaxTree>, IFileLeaf<SyntaxTree>
    {
        public string SourceCode { get; set; }

        /// <summary>
        /// The original unmutated syntaxtree
        /// </summary>
        public SyntaxTree SyntaxTree { get; set; }

        /// <summary>
        /// The mutated syntax tree
        /// </summary>
        public SyntaxTree MutatedSyntaxTree { get; set; }

        public override IEnumerable<Mutant> Mutants { get; set; }

        public override IEnumerable<SyntaxTree> CompilationSyntaxTrees => MutatedSyntaxTrees;

        public override IEnumerable<SyntaxTree> MutatedSyntaxTrees => new List<SyntaxTree> { MutatedSyntaxTree };

        public override void Add(ProjectComponent<SyntaxTree> component)
        {
            // no children can be added to a file instance
            throw new NotImplementedException();
        }

        public ReadOnlyFileLeaf ToReadOnly()
        {
            return new ReadOnlyFileLeaf(this);
        }

        public override IReadOnlyInputComponent ToReadOnlyInputComponent()
        {
            return ToReadOnly();
        }

        public override IEnumerable<IProjectComponent> GetAllFiles()
        {
            yield return this;
        }
    }
}
