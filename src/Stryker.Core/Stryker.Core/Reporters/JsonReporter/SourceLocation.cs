using Microsoft.CodeAnalysis;

namespace Stryker.Core.Reporters.Json
{
    public class SourceLocation
    {
        public SourcePosition Start { get; init; }
        public SourcePosition End { get; init; }

        public SourceLocation()
        {
        }

        public SourceLocation(FileLinePositionSpan location)
        {
            Start = new SourcePosition
            {
                Line = location.StartLinePosition.Line + 1,
                Column = location.StartLinePosition.Character + 1
            };
            End = new SourcePosition
            {
                Line = location.EndLinePosition.Line + 1,
                Column = location.EndLinePosition.Character + 1
            };
        }
    }
}
