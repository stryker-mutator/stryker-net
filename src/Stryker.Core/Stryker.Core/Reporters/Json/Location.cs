using Microsoft.CodeAnalysis;

namespace Stryker.Core.Reporters.Json;

public class Location
{
    public Position Start { get; init; }
    public Position End { get; init; }

    public Location()
    {
    }

    public Location(FileLinePositionSpan location)
    {
        Start = new Position
        {
            Line = location.StartLinePosition.Line + 1,
            Column = location.StartLinePosition.Character + 1
        };
        End = new Position
        {
            Line = location.EndLinePosition.Line + 1,
            Column = location.EndLinePosition.Character + 1
        };
    }
}
