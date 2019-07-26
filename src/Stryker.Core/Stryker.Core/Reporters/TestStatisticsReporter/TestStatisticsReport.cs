using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Stryker.Core.Reporters
{
    public class TestStatisticsReport
    {
        public TestStatisticsReport(IReadOnlyCollection<JsonTestedMutant> mutants, IReadOnlyCollection<JsonTest> tests)
        {
            Mutants = mutants;
            Tests = tests;
        }

        public IReadOnlyCollection<JsonTestedMutant> Mutants { get; }
        public IReadOnlyCollection<JsonTest> Tests { get; }

        public string ToJson()
        {
            var json = JsonConvert.SerializeObject(this, new JsonSerializerSettings
            {
                ContractResolver = new DefaultContractResolver
                {
                    NamingStrategy = new CamelCaseNamingStrategy()
                },
                Formatting = Formatting.Indented,
                NullValueHandling = NullValueHandling.Ignore
            });

            return json;
        }
    }
}