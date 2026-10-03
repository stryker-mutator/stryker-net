using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.CodeAnalysis;
using Stryker.Abstractions;
using Stryker.Abstractions.ProjectComponents;

namespace Stryker.CLI.MutationServer;

internal sealed class MutationServerScope
{
    private readonly string _basePath;
    private readonly IReadOnlyCollection<TargetRange> _targetRanges;
    private readonly IReadOnlyDictionary<string, HashSet<string>> _targetMutants;
    private readonly bool _targetsMutants;
    private readonly bool _selectAll;

    private MutationServerScope(
        string basePath,
        IReadOnlyCollection<TargetRange> targetRanges,
        IReadOnlyDictionary<string, HashSet<string>> targetMutants,
        bool targetsMutants,
        bool selectAll)
    {
        _basePath = basePath;
        _targetRanges = targetRanges;
        _targetMutants = targetMutants;
        _targetsMutants = targetsMutants;
        _selectAll = selectAll;
    }

    public static MutationServerScope ForDiscovery(string basePath, DiscoverParams parameters)
        => new(
            basePath,
            CreateTargetRanges(basePath, parameters?.Files),
            new Dictionary<string, HashSet<string>>(PathComparer),
            targetsMutants: false,
            selectAll: parameters?.Files is null);

    public static MutationServerScope ForMutationTest(string basePath, MutationTestParams parameters)
    {
        if (parameters?.Mutants is not null)
        {
            return new MutationServerScope(
                basePath,
                [],
                CreateTargetMutants(basePath, parameters.Mutants),
                targetsMutants: true,
                selectAll: false);
        }

        return new MutationServerScope(
            basePath,
            CreateTargetRanges(basePath, parameters?.Files),
            new Dictionary<string, HashSet<string>>(PathComparer),
            targetsMutants: false,
            selectAll: parameters?.Files is null);
    }

    public bool Includes(IReadOnlyFileLeaf file, IReadOnlyMutant mutant)
    {
        if (_targetsMutants)
        {
            var filePath = NormalizePath(_basePath, file.FullPath);
            return _targetMutants.TryGetValue(filePath, out var targetIds) &&
                   targetIds.Contains(MutationServerMutantIdentity.GetId(_basePath, file, mutant));
        }

        if (_selectAll)
        {
            return true;
        }

        if (_targetRanges.Count == 0)
        {
            return false;
        }

        var normalizedFilePath = NormalizePath(_basePath, file.FullPath);
        return _targetRanges.Any(target => target.Includes(normalizedFilePath, mutant));
    }

    public bool IncludesFile(IReadOnlyFileLeaf file)
    {
        var filePath = NormalizePath(_basePath, file.FullPath);
        if (_targetsMutants)
        {
            return _targetMutants.ContainsKey(filePath);
        }

        return _selectAll || _targetRanges.Any(target => target.MatchesPath(filePath));
    }

    private static IReadOnlyCollection<TargetRange> CreateTargetRanges(
        string basePath,
        IReadOnlyCollection<FileRange> files)
        => files?
            .Where(file => !string.IsNullOrWhiteSpace(file?.Path))
            .Select(file => new TargetRange(
                NormalizePath(basePath, file.Path),
                file.Path.EndsWith('/') || file.Path.EndsWith('\\'),
                file.Range))
            .ToArray() ?? [];

    private static IReadOnlyDictionary<string, HashSet<string>> CreateTargetMutants(
        string basePath,
        IDictionary<string, DiscoveredFile> files)
    {
        var targets = new Dictionary<string, HashSet<string>>(PathComparer);
        foreach (var file in files.Where(file =>
                     !string.IsNullOrWhiteSpace(file.Key) &&
                     file.Value?.Mutants is not null))
        {
            var ids = file.Value.Mutants
                .Where(mutant => !string.IsNullOrWhiteSpace(mutant?.Id))
                .Select(mutant => mutant.Id)
                .ToHashSet(StringComparer.Ordinal);
            targets[NormalizePath(basePath, file.Key)] = ids;
        }

        return targets;
    }

    private static string NormalizePath(string basePath, string path)
        => Path.GetFullPath(path.Replace('/', Path.DirectorySeparatorChar), basePath)
            .TrimEnd(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar);

    private static StringComparer PathComparer
        => OperatingSystem.IsWindows() ? StringComparer.OrdinalIgnoreCase : StringComparer.Ordinal;

    private sealed class TargetRange
    {
        private readonly string _path;
        private readonly bool _isDirectory;
        private readonly MutationServerLocation _range;

        public TargetRange(string path, bool isDirectory, MutationServerLocation range)
        {
            _path = path;
            _isDirectory = isDirectory;
            _range = range;
            ValidateRange(range);
        }

        public bool Includes(string filePath, IReadOnlyMutant mutant)
        {
            if (!MatchesPath(filePath))
            {
                return false;
            }

            if (_range is null)
            {
                return true;
            }

            var location = mutant.Mutation.OriginalNode.GetLocation().GetMappedLineSpan();
            var start = location.StartLinePosition;
            var end = location.EndLinePosition;

            return Compare(start.Line + 1, start.Character + 1, _range.Start) >= 0 &&
                   Compare(end.Line + 1, end.Character + 1, _range.End) <= 0;
        }

        public bool MatchesPath(string filePath)
        {
            if (!_isDirectory)
            {
                return PathComparer.Equals(_path, filePath);
            }

            var pathPrefix = _path + Path.DirectorySeparatorChar;
            return filePath.StartsWith(
                pathPrefix,
                OperatingSystem.IsWindows() ? StringComparison.OrdinalIgnoreCase : StringComparison.Ordinal);
        }

        private static int Compare(int line, int column, MutationServerPosition position)
        {
            var lineComparison = line.CompareTo(position.Line);
            return lineComparison != 0 ? lineComparison : column.CompareTo(position.Column);
        }

        private static void ValidateRange(MutationServerLocation range)
        {
            if (range is null)
            {
                return;
            }

            if (range.Start is null || range.End is null ||
                range.Start.Line < 1 || range.Start.Column < 1 ||
                range.End.Line < 1 || range.End.Column < 1 ||
                Compare(range.Start.Line, range.Start.Column, range.End) > 0)
            {
                throw new ArgumentException("Mutation server ranges must be positive, ordered, and 1-based.");
            }
        }
    }
}
