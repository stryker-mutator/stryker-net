using FSharp.Compiler.Syntax;
using Microsoft.CodeAnalysis;
using Stryker.Abstractions;
using Stryker.Abstractions.ProjectComponents;
using Stryker.Core.ProjectComponents;

namespace Stryker.Fsharp.ProjectComponents;

/// <summary>An F# source file with its original text and FCS parse tree.</summary>
public class FsharpFileLeaf : ProjectComponent, IFileLeaf
{
    public string SourceCode { get; set; } = string.Empty;

    /// <summary>The unmutated FCS parse tree.</summary>
    public ParsedInput? ParseTree { get; set; }

    public override IEnumerable<IMutant> Mutants { get; set; } = [];

    // F# leaves carry no Roslyn trees; the Roslyn members required by IFileLeaf are always empty.
    SyntaxTree IFileLeaf.SyntaxTree { get => null!; set { } }
    SyntaxTree IFileLeaf.MutatedSyntaxTree { get => null!; set { } }

    public override IEnumerable<SyntaxTree> CompilationSyntaxTrees => [];
    public override IEnumerable<SyntaxTree> MutatedSyntaxTrees => [];
    public override IEnumerable<SyntaxTree> UnmutatedSyntaxTrees => [];

    public override IEnumerable<IFileLeaf> GetAllFiles() => [this];

    public override void Display() => DisplayFile(this);

    public override string ToString() => SourceCode;
}
