using FSharp.Compiler.Text;
using Stryker.Abstractions;

namespace Stryker.Fsharp.Mutants;

internal static class FsharpLocations
{
    /// <summary>
    /// Converts an FCS range (lines 1-based, columns 0-based) to a <see cref="MutationLocation"/> (columns 1-based).
    /// This is the only place the +1 on columns is applied.
    /// </summary>
    public static MutationLocation ToLocation(FSharp.Compiler.Text.Range range, string sourceText)
    {
        var lineStarts = LineStarts(sourceText);
        return new MutationLocation(
            range.FileName,
            range.StartLine,
            range.StartColumn + 1,
            range.EndLine,
            range.EndColumn + 1,
            lineStarts[range.StartLine - 1] + range.StartColumn,
            lineStarts[range.EndLine - 1] + range.EndColumn);
    }

    /// <summary>Character offset of the start of each line; handles \n, \r\n and lone \r terminators.</summary>
    internal static List<int> LineStarts(string text)
    {
        var starts = new List<int> { 0 };
        for (var i = 0; i < text.Length; i++)
        {
            if (text[i] == '\r' && i + 1 < text.Length && text[i + 1] == '\n')
            {
                i++;
                starts.Add(i + 1);
            }
            else if (text[i] is '\n' or '\r')
            {
                starts.Add(i + 1);
            }
        }

        return starts;
    }
}
