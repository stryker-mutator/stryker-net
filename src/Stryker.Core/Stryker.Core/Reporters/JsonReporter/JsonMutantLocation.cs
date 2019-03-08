using Microsoft.CodeAnalysis;

namespace Stryker.Core.Reporters.Json
{
    public class JsonMutantLocation
    {
        public JsonMutantPosition Start { get; }
        public JsonMutantPosition End { get; }

        public JsonMutantLocation(JsonMutantPosition start, JsonMutantPosition end)
        {
            Start = start;
            End = end;
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