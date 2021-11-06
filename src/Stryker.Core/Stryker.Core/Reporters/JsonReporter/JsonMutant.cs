
namespace Stryker.Core.Reporters.Json
{
    public class JsonMutant
    {
        public string Id { get; set; }
        public string MutatorName { get; set; }
        public string Replacement { get; set; }
        public SourceLocation Location { get; set; }
        public string Status { get; set; }
        public string Description { get; set; }
    }
}