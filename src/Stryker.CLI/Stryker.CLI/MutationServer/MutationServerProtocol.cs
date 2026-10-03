using System.Collections.Generic;

namespace Stryker.CLI.MutationServer;

internal static class MutationServerProtocol
{
    public const string Version = "0.4.0";
    public const string ConfigureMethod = "configure";
    public const string DiscoverMethod = "discover";
    public const string MutationTestMethod = "mutationTest";
    public const string ReportMutationTestProgressMethod = "reportMutationTestProgress";
}

internal sealed class ConfigureParams
{
    public string ConfigFilePath { get; init; }
}

internal sealed class ConfigureResult
{
    public string Version { get; init; }
}

internal sealed class DiscoverParams
{
    public IReadOnlyCollection<FileRange> Files { get; init; }
}

internal sealed class DiscoverResult
{
    public IDictionary<string, DiscoveredFile> Files { get; init; } = new Dictionary<string, DiscoveredFile>();
}

internal sealed class MutationTestParams
{
    public IReadOnlyCollection<FileRange> Files { get; init; }
    public IDictionary<string, DiscoveredFile> Mutants { get; init; }
}

internal sealed class MutationTestResult
{
    public IDictionary<string, MutantResultFile> Files { get; init; } = new Dictionary<string, MutantResultFile>();
}

internal sealed class FileRange
{
    public string Path { get; init; }
    public MutationServerLocation Range { get; init; }
}

internal sealed class DiscoveredFile
{
    public IReadOnlyCollection<DiscoveredMutant> Mutants { get; init; } = [];
}

internal class DiscoveredMutant
{
    public string Id { get; init; }
    public MutationServerLocation Location { get; init; }
    public string Description { get; init; }
    public string MutatorName { get; init; }
    public string Replacement { get; init; }
}

internal sealed class MutantResultFile
{
    public IReadOnlyCollection<MutantResult> Mutants { get; init; } = [];
}

internal sealed class MutantResult : DiscoveredMutant
{
    public IReadOnlyCollection<string> CoveredBy { get; init; }
    public int? Duration { get; init; }
    public IReadOnlyCollection<string> KilledBy { get; init; }
    public bool? Static { get; init; }
    public string Status { get; init; }
    public string StatusReason { get; init; }
    public int? TestsCompleted { get; init; }
}

internal sealed class MutationServerLocation
{
    public MutationServerPosition Start { get; init; }
    public MutationServerPosition End { get; init; }
}

internal sealed class MutationServerPosition
{
    public int Line { get; init; }
    public int Column { get; init; }
}
