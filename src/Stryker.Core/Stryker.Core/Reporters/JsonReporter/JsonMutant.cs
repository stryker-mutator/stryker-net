
namespace Stryker.Core.Reporters.Json
{
    public class JsonMutant
    {
        public int Id { get; set; }
        public string MutatorName { get; set; }
        public string Replacement { get; set; }
        public JsonMutantLocation Location { get; set; }
        public string Status { get; set; }
    }
}