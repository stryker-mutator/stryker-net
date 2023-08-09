namespace Stryker.Core.Reporters.Json
{
    public struct LocationDimensions
    {
        public int StartLine { get; set; }
        public int StartCharacter { get; set; }
        public int EndLine { get; set; }
        public int EndCharacter { get; set; }
    }

    public class Location
    {
        public Position Start { get; init; }
        public Position End { get; init; }

        // still needed for serialization purposes
        public Location()
        {
        }

        public Location(LocationDimensions dimensions)
        {
            Start = new Position
            {
                Line = dimensions.StartLine + 1,
                Column = dimensions.StartCharacter + 1
            };
            End = new Position
            {
                Line = dimensions.EndLine + 1,
                Column = dimensions.EndCharacter + 1
            };
        }
    }
}
