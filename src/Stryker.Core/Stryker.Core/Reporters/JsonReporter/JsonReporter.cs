using Microsoft.CodeAnalysis;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Stryker.Core.Initialisation.ProjectComponent;
using Stryker.Core.Mutants;
using Stryker.Core.Options;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Abstractions;
using System.Linq;

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

            WriteReportToJsonFile(jsonReport, Path.Combine(_options.BasePath, "StrykerOutput", "reports", "mutation-report.json"));
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

            filePath = GenerateUniqueReportFilePath(filePath);

            using (var file = _fileSystem.File.CreateText(filePath))
            {
                file.WriteLine(json);
            }
        }

        private string GenerateUniqueReportFilePath(string requestedFilePath)
        {
            string date = DateTime.Today.ToShortDateString();

            string directory = Path.GetDirectoryName(requestedFilePath);
            string fileName = Path.GetFileNameWithoutExtension(requestedFilePath);
            string fileExtension = Path.GetExtension(requestedFilePath);
            string fileNameWithDate = fileName + "-" + date;
            string filePathWithDate = Path.Combine(directory, fileNameWithDate);

            string generatedFilePath = filePathWithDate;

            _fileSystem.Directory.CreateDirectory(directory);
            if (_fileSystem.File.Exists(filePathWithDate + fileExtension))
            {
                if (_fileSystem.Directory.EnumerateFiles(directory)
                    .Where(fn => fn.Contains(fileNameWithDate + "-")) is var matchingFiles
                    && matchingFiles.Any())
                {
                    var sequenceNumber = matchingFiles.Select(f =>
                    {
                        string fn = Path.GetFileNameWithoutExtension(f);
                        int index = fn.LastIndexOf("-") + 1;

                        return int.Parse(fn.Substring(index));
                    }).Max();

                    string generatedFileName = fileNameWithDate + "-0" + (sequenceNumber + 1);
                    generatedFilePath = Path.Combine(directory, generatedFileName);
                }
                else
                {
                    generatedFilePath = Path.Combine(directory, fileNameWithDate + "-01");
                }
            }

            return generatedFilePath + fileExtension;
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
