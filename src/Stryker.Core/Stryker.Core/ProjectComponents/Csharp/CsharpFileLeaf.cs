using Microsoft.CodeAnalysis;
using Stryker.Abstractions;
using Stryker.Abstractions.ProjectComponents;
using System.Collections.Generic;

namespace Stryker.Core.ProjectComponents.Csharp;

public class CsharpFileLeaf : ProjectComponent, IFileLeaf
{
    public string SourceCode { get; set; }

    /// <summary>
    /// The original unmutated syntax tree
    /// </summary>
    public SyntaxTree SyntaxTree { get; set; }

    /// <summary>
    /// The mutated syntax tree
    /// </summary>
    public SyntaxTree MutatedSyntaxTree { get; set; }

    public override IEnumerable<IMutant> Mutants { get; set; }

    public override IEnumerable<SyntaxTree> CompilationSyntaxTrees => MutatedSyntaxTrees;

    public override IEnumerable<SyntaxTree> MutatedSyntaxTrees => [MutatedSyntaxTree ?? SyntaxTree ];

    public override string ToString() => SourceCode;

    public override IEnumerable<IFileLeaf> GetAllFiles() => [this];

    public override void Display() => DisplayFile(this);
}
