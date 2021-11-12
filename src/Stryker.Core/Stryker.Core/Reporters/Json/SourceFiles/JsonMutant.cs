
namespace Stryker.Core.Reporters.Json.SourceFiles
{
    public class JsonMutant
    {
        public string Id { get; set; }
        public string MutatorName { get; set; }
        public string Replacement { get; set; }
        public JsonMutantLocation Location { get; set; }
        public string Status { get; set; }
        public string Description { get; set; }
    }
}
