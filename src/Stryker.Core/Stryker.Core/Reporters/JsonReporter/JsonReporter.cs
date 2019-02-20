using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Stryker.Core.Initialisation.ProjectComponent;
using Stryker.Core.Mutants;
using Stryker.Core.Options;
using System.Collections.Generic;
using System.IO;
using System.IO.Abstractions;

namespace Stryker.Core.Reporters.Json
{
    public partial class JsonReporter : IReporter
    {
        private readonly StrykerOptions _options;
        private readonly IFileSystem _fileSystem;

        public JsonReporter(StrykerOptions options, IFileSystem fileSystem = null)
        {
            _options = options;
            _fileSystem = fileSystem ?? new FileSystem();
        }

        public void OnAllMutantsTested(IReadOnlyInputComponent reportComponent)
        {
            var jsonReport = JsonReportComponent.FromProjectComponent(reportComponent, _options);
            jsonReport.ThresholdHigh = _options.ThresholdOptions.ThresholdHigh;
            jsonReport.ThresholdLow = _options.ThresholdOptions.ThresholdLow;
            jsonReport.ThresholdBreak = _options.ThresholdOptions.ThresholdBreak;

            WriteReportToJsonFile(jsonReport, Path.Combine(_options.OutputPath, "reports", "mutation-report.json"));
        }

        private void WriteReportToJsonFile(JsonReportComponent jsonReport, string filePath)
        {
            var json = JsonConvert.SerializeObject(jsonReport, new JsonSerializerSettings
            {
                ContractResolver = new DefaultContractResolver
                {
                    NamingStrategy = new CamelCaseNamingStrategy()
                },
                Formatting = Formatting.Indented,
                NullValueHandling = NullValueHandling.Ignore
            });

            using (var file = _fileSystem.File.CreateText(filePath))
            {
                file.WriteLine(json);
            }
        }

        public void OnMutantsCreated(IReadOnlyInputComponent reportComponent)
        {
        }

        public void OnMutantTested(IReadOnlyMutant result)
        {
        }

        public void OnStartMutantTestRun(IEnumerable<Mutant> mutantsToBeTested)
        {
        }
    }
}
