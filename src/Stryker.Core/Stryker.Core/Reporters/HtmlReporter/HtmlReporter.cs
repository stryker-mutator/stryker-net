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
        private readonly JsonReporter _jsonReporter;
        private readonly IFileSystem _fileSystem;

        public HtmlReporter(StrykerOptions options, JsonReporter jsonReporter, IFileSystem fileSystem = null)
        {
            _options = options;
            _jsonReporter = jsonReporter;
            _fileSystem = fileSystem ?? new FileSystem();
        }

        public void OnAllMutantsTested(IReadOnlyInputComponent reportComponent)
        {
            // First generate json using json reporter
            _jsonReporter.OnAllMutantsTested(reportComponent);

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