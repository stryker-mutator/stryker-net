using Stryker.Core.Mutants;
using Stryker.Core.Options;
using Stryker.Core.ProjectComponents;
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

        public void OnAllMutantsTested(IReadOnlyInputComponent mutationTree)
        {
            var mutationReport = JsonReport.Build(_options, mutationTree);

            WriteReportToJsonFile(mutationReport.ToJson(), Path.Combine(_options.OutputPath, "reports", "mutation-report.json"));
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
