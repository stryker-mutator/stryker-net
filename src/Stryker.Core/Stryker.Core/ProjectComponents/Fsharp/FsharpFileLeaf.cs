using Stryker.Core.Mutants;
using System;
using System.Collections.Generic;
using static FSharp.Compiler.SyntaxTree;

namespace Stryker.Core.ProjectComponents
{
    public class FsharpFileLeaf : ProjectComponent<ParsedInput>, IFileLeaf<ParsedInput>
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

        public ReadOnlyFileLeaf ToReadOnly()
        {
            return new ReadOnlyFileLeaf(this);
        }

        public override IReadOnlyProjectComponent ToReadOnlyInputComponent()
        {
            return ToReadOnly();
        }

        public override IEnumerable<IProjectComponent> GetAllFiles()
        {
            yield return this;
        }
    }
}
