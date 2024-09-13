using System.Collections.Generic;
using DotNet.Globbing;
using Microsoft.CodeAnalysis.Text;

namespace Stryker.Abstractions.ProjectComponents;

public interface IFilePattern
{
    Glob Glob { get; }
    bool IsExclude { get; }
    IReadOnlyCollection<TextSpan> TextSpans { get; }

    bool Equals(IFilePattern other);
    bool Equals(object obj);
    int GetHashCode();
    bool IsMatch(string filePath, TextSpan textSpan);
}
