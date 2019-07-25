using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Stryker.Core.Reporters.Json;
using Stryker.Core.Reporters.TestStatisticsReporter;

namespace Stryker.Core.Reporters
{
    public class TestStatisticsReport
    {
        public TestStatisticsReport(IReadOnlyCollection<JsonMutant> mutants, IReadOnlyCollection<JsonTest> tests)
        {
            Mutants = mutants;
            Tests = tests;
        }

        public IReadOnlyCollection<JsonMutant> Mutants { get; }
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