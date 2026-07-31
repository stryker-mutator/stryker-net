using Microsoft.CodeAnalysis;

namespace Stryker.Abstractions.ProjectComponents;

public interface IReadOnlyFileLeaf : IReadOnlyProjectComponent
{
    string SourceCode { get; }
}

public interface IFileLeaf : IReadOnlyFileLeaf
{
    new string SourceCode { get; set; }

    SyntaxTree SyntaxTree { get; set; }

    SyntaxTree MutatedSyntaxTree { get; set; }
}
