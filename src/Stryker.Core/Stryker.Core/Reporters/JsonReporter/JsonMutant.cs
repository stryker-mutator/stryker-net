
namespace Stryker.Core.Reporters.Json
{
    public class JsonMutant
    {
        public string Id { get; set; }
        public string MutatorName { get; set; }
        public string Description { get; set; }

        public string Replacement { get; set; }
        public SourceLocation Location { get; set; }

        public string Status { get; set; }
        public bool IsStatic { get; set; }

        public string[] CoveredBy { get; set; }
        public string[] KilledBy { get; set; }
        public int TestsCompleted { get; set; }

        public int Duration { get; set; }
    }
}
