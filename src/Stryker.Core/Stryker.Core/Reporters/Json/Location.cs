using Microsoft.CodeAnalysis;
using Stryker.Abstractions.Reporting;

namespace Stryker.Abstractions.Reporters.Json
{
    public class Location : ILocation
    {
        public IPosition Start { get; init; }
        public IPosition End { get; init; }

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
}
