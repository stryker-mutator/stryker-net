using Stryker.Core.Initialisation.ProjectComponent;
using Stryker.Core.Mutants;
using Stryker.Core.Options;
using Stryker.Core.Reporters.Json;
using System.Collections.Generic;
using System.IO;
using System.IO.Abstractions;
using System.Linq;

namespace Stryker.Core.Reporters.Html
{
    public class HtmlReporter : IReporter
    {
        private readonly StrykerOptions _options;
        private readonly IFileSystem _fileSystem;

        public HtmlReporter(StrykerOptions options, IFileSystem fileSystem = null)
        {
            _options = options;
            _fileSystem = fileSystem ?? new FileSystem();
        }

        public void OnAllMutantsTested(IReadOnlyInputComponent mutationTree)
        {
            var mutationReport = MutationReport.Build(_options, mutationTree);

            var htmlFileName = "index.html";
            var jsFileName = "mutation-test-elements.js";

            var resourceFiles = new Dictionary<string, string>
            {
                {
                    htmlFileName,
                    "Stryker.Core.Reporters.HtmlReporter.index.html"
                },
                {
                    jsFileName,
                    typeof(HtmlReporter).Assembly.GetManifestResourceNames().Single(m => m.Contains(jsFileName))
                }
            };

            foreach (var fileName in resourceFiles)
            {
                using (var stream = typeof(HtmlReporter).Assembly.GetManifestResourceStream(fileName.Value))
                {
                    using (var reader = new StreamReader(stream))
                    {
                        var filePath = Path.Combine(_options.OutputPath, "reports", fileName.Key);
                        using (var file = _fileSystem.File.CreateText(filePath))
                        {
                            var fileContent = reader.ReadToEnd();

                            if (fileName.Key == htmlFileName)
                            {
                                fileContent = fileContent.Replace("##REPORT_TITLE##", "Stryker Dotnet Report");
                                fileContent = fileContent.Replace("##REPORT_JSON##", mutationReport.ToJson());
                            }
                            file.WriteLine(fileContent);
                        }
                    }
                }
            }
        }

        private void WriteReportToJsonFile(string mutationReport, string filePath)
        {
            _fileSystem.Directory.CreateDirectory(Path.GetDirectoryName(filePath));
            using (var file = _fileSystem.File.CreateText(filePath))
            {
                file.WriteLine(mutationReport);
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