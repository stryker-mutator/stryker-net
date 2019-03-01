using Stryker.Core.Initialisation.ProjectComponent;
using Stryker.Core.Mutants;
using Stryker.Core.Options;
using Stryker.Core.Reporters.Json;
using System.Collections.Generic;
using System.IO;
using System.IO.Abstractions;

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

            // First generate json using json reporter
            WriteReportToJsonFile(mutationReport.ToJson(), Path.Combine(_options.OutputPath, "reports", "mutation-report.json"));

            foreach (var fileName in new Dictionary<string, string>
            {
                { "index.html", "Stryker.Core.Reporters.HtmlReporter.index.html" },
                { "mutation-test-elements.js", "Stryker.Core.Reporters.HtmlReporter.mutation-test-elements.js" }
            })
            {
                using (var stream = typeof(HtmlReporter).Assembly.GetManifestResourceStream(fileName.Value))
                {
                    using (var reader = new StreamReader(stream))
                    {
                        var filePath = Path.Combine(_options.OutputPath, "reports", fileName.Key);
                        using (var file = _fileSystem.File.CreateText(filePath))
                        {
                            file.WriteLine(reader.ReadToEnd());
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