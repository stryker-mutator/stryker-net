using Microsoft.CodeAnalysis;

namespace Stryker.Core.Reporters.Json
{
    public class JsonMutant
    {
        public int Id { get; set; }
        public string MutatorName { get; set; }
        public string Replacement { get; set; }
        public JsonMutantLocation Location { get; set; }
        public string Status { get; set; }

        public class JsonMutantLocation
        {
            public JsonMutantLocation(FileLinePositionSpan location)
            {
                Start = new JsonMutantLocationPoint
                {
                    Line = location.StartLinePosition.Line + 1,
                    Column = location.StartLinePosition.Character + 1
                };
                End = new JsonMutantLocationPoint
                {
                    Line = location.EndLinePosition.Line + 1,
                    Column = location.EndLinePosition.Character + 1
                };
            }

            public JsonMutantLocationPoint Start { get; set; }
            public JsonMutantLocationPoint End { get; set; }
            public class JsonMutantLocationPoint
            {
                public int Line { get; set; }
                public int Column { get; set; }
            }
        }
    }
}