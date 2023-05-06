using FSharp.Compiler.Syntax;
using Stryker.Core.Mutants;
using System.Collections.Generic;

namespace Stryker.Core.ProjectComponents;

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

    public override IEnumerable<IFileLeaf<ParsedInput>> GetAllFiles()
    {
        yield return this;
    }

    public override void Display()
    {
        DisplayFile(this);
    }
}
