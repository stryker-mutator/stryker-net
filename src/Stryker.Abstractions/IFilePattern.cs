using DotNet.Globbing;
using Microsoft.CodeAnalysis.Text;

namespace Stryker.Configuration;
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
