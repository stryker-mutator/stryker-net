using System.Collections.Generic;
using System.IO.Abstractions;
using System.Linq;
using System.Text.RegularExpressions;
using Microsoft.CodeAnalysis;
using Microsoft.Extensions.Logging;
using Stryker.Abstractions;
using Stryker.Abstractions.Options;
using Stryker.Abstractions.ProjectComponents;
using Stryker.Core.ProjectComponents;
using Stryker.Core.ProjectComponents.Csharp;
using Stryker.Utilities.Logging;

namespace Stryker.Core.Mutants;

/// <summary>
/// After mutation testing completes, injects Stryker disable comments for mutants that caused timeouts.
/// This allows subsequent runs to skip these slow mutants, significantly speeding up re-runs.
/// </summary>
public class DisableTimeoutsFeature
{
    private static readonly ILogger Logger = ApplicationLogging.LoggerFactory.CreateLogger<DisableTimeoutsFeature>();
    private static readonly Regex DisableOnceRegex = new(@"//\s*Stryker\s+disable\s+once\s+([\w,\s]+)", RegexOptions.Compiled);
    private readonly IFileSystem _fileSystem;

    public DisableTimeoutsFeature(IFileSystem fileSystem = null)
    {
        _fileSystem = fileSystem ?? new FileSystem();
    }

    public void Apply(IReadOnlyProjectComponent rootComponent, IStrykerOptions options)
    {
        if (!options.DisableTimeouts)
        {
            return;
        }

        var timeoutMutants = rootComponent.Mutants
            .Where(m => m.ResultStatus == MutantStatus.Timeout)
            .ToList();

        if (!timeoutMutants.Any())
        {
            Logger.LogDebug("No timed-out mutants found. Skipping disable timeouts feature.");
            return;
        }

        Logger.LogInformation("Found {Count} timed-out mutants. Adding disable comments to source files.", timeoutMutants.Count);

        var fileMutants = timeoutMutants
            .GroupBy(m => GetFilePath(m))
            .ToList();

        foreach (var fileGroup in fileMutants)
        {
            var mutants = fileGroup.ToList();
            ApplyToFile(fileGroup.Key, mutants);
        }
    }

    private static string GetFilePath(IMutant mutant)
    {
        var mutation = mutant.Mutation;
        var originalNode = mutation?.OriginalNode;
        if (originalNode == null)
        {
            return null;
        }

        var syntaxTree = originalNode.SyntaxTree;
        return syntaxTree.FilePath;
    }

    private void ApplyToFile(string filePath, List<IMutant> mutants)
    {
        if (string.IsNullOrEmpty(filePath) || !_fileSystem.File.Exists(filePath))
        {
            Logger.LogWarning("Source file not found at {FilePath}, skipping disable comment injection.", filePath);
            return;
        }

        var sourceCode = _fileSystem.File.ReadAllText(filePath);
        var updatedCode = ApplyCommentsToSource(sourceCode, mutants);

        if (updatedCode != sourceCode)
        {
            _fileSystem.File.WriteAllText(filePath, updatedCode);
            Logger.LogDebug("Updated {FilePath} with disable comment(s).", filePath);
        }
    }

    private static string ApplyCommentsToSource(string sourceCode, List<IMutant> mutants)
    {
        var lineMutants = mutants
            .Select(m => (Mutant: m, LineInfo: GetLineSpan(m)))
            .Where(x => x.LineInfo.HasValue)
            .ToList();

        if (!lineMutants.Any())
        {
            return sourceCode;
        }

        var lineGroups = lineMutants
            .GroupBy(x => x.LineInfo.Value.Line)
            .ToList();

        var result = sourceCode;

        foreach (var lineGroup in lineGroups)
        {
            var line = lineGroup.Key;
            var lineMutantList = lineGroup.ToList();

            var mutatorTypes = lineMutantList
                .Select(m => m.Mutant.Mutation?.Type.ToString())
                .Where(t => !string.IsNullOrEmpty(t))
                .Distinct()
                .OrderBy(t => t)
                .ToList();

            var lineStart = FindLineStart(sourceCode, line);
            var lineEnd = FindLineEnd(sourceCode, line);
            var existingLineContent = sourceCode.Substring(lineStart, lineEnd - lineStart);

            if (HasDisableOnceComment(existingLineContent) || HasDisableOnceCommentOnPreviousLine(sourceCode, line))
            {
                continue;
            }

            var indentation = GetIndentation(existingLineContent);
            var comment = $"{indentation}// Stryker disable once {string.Join(",", mutatorTypes)}: this mutation causes a timeout";
            var insertionPoint = lineStart;

            result = result.Insert(insertionPoint, $"{comment}\n");
        }

        return result;
    }

    private static bool HasDisableOnceComment(string lineContent)
    {
        return DisableOnceRegex.IsMatch(lineContent);
    }

    private static bool HasDisableOnceCommentOnPreviousLine(string sourceCode, int currentLine)
    {
        if (currentLine <= 1)
        {
            return false;
        }

        var previousLine = sourceCode.Split('\n')[currentLine - 2];
        return DisableOnceRegex.IsMatch(previousLine);
    }

    private static int FindLineStart(string sourceCode, int lineNumber)
    {
        var lines = sourceCode.Split('\n');
        var lineIndex = lineNumber - 1;
        if (lineIndex < 0 || lineIndex >= lines.Length)
        {
            return 0;
        }

        var offset = 0;
        for (var i = 0; i < lineIndex; i++)
        {
            offset += lines[i].Length + 1;
        }

        return offset;
    }

    private static int FindLineEnd(string sourceCode, int lineNumber)
    {
        var lines = sourceCode.Split('\n');
        var lineIndex = lineNumber - 1;
        if (lineIndex < 0 || lineIndex >= lines.Length)
        {
            return sourceCode.Length;
        }

        var offset = 0;
        for (var i = 0; i <= lineIndex; i++)
        {
            offset += lines[i].Length;
            if (i < lineIndex)
            {
                offset += 1;
            }
        }

        return offset;
    }

    private static string GetIndentation(string lineContent)
    {
        var indentation = string.Empty;

        foreach (var c in lineContent)
        {
            if (c == ' ' || c == '\t')
            {
                indentation += c;
            }
            else
            {
                break;
            }
        }

        return indentation;
    }

    private static (int Line, int Character, int InsertionPoint)? GetLineSpan(IMutant mutant)
    {
        var mutation = mutant.Mutation;
        var originalNode = mutation?.OriginalNode;
        if (originalNode == null)
        {
            return null;
        }

        var lineSpan = originalNode.GetLocation().GetLineSpan();
        var startPosition = lineSpan.StartLinePosition;

        var charOffset = originalNode.Span.Start;

        return (Line: startPosition.Line + 1, Character: startPosition.Character, InsertionPoint: charOffset);
    }
}
