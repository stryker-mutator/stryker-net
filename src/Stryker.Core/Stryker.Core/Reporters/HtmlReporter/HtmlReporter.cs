using Stryker.Core.Mutants;
using Stryker.Core.Options;
using Stryker.Core.ProjectComponents;
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
            var mutationReport = JsonReport.Build(_options, mutationTree);

            WriteHtmlReport(Path.Combine(_options.OutputPath, "reports", "mutation-report.html"), mutationReport.ToJson());
        }

        private void WriteHtmlReport(string filePath, string mutationReport)
        {
            _fileSystem.Directory.CreateDirectory(Path.GetDirectoryName(filePath));

            using (var htmlStream = typeof(HtmlReporter).Assembly
                .GetManifestResourceStream(typeof(HtmlReporter)
                .Assembly.GetManifestResourceNames()
                .Single(m => m.Contains("mutation-report.html"))))
            using (var jsStream = typeof(HtmlReporter).Assembly
                .GetManifestResourceStream(typeof(HtmlReporter)
                .Assembly.GetManifestResourceNames()
                .Single(m => m.Contains("mutation-test-elements.js"))))

            {
                using (var htmlReader = new StreamReader(htmlStream))
                using (var jsReader = new StreamReader(jsStream))
                {
                    using (var file = _fileSystem.File.CreateText(filePath))
                    {
                        var fileContent = htmlReader.ReadToEnd();

                        fileContent = fileContent.Replace("##REPORT_JS##", jsReader.ReadToEnd());
                        fileContent = fileContent.Replace("##REPORT_TITLE##", "Stryker Dotnet Report");
                        fileContent = fileContent.Replace("##REPORT_JSON##", mutationReport);

                        file.WriteLine(fileContent);
                    }
                }
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