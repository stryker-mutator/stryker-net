using Microsoft.CodeAnalysis;

namespace Stryker.Core.Reporters.Json
{
    public class JsonMutantLocation
    {
        public JsonMutantPosition Start { get; init; }
        public JsonMutantPosition End { get; init; }

        public JsonMutantLocation()
        {
        }

        public JsonMutantLocation(FileLinePositionSpan location)
        {
            Start = new JsonMutantPosition
            {
                Line = location.StartLinePosition.Line + 1,
                Column = location.StartLinePosition.Character + 1
            };
            End = new JsonMutantPosition
            {
                Line = location.EndLinePosition.Line + 1,
                Column = location.EndLinePosition.Character + 1
            };
        }
    }
}
