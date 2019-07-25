using Stryker.Core.Mutants;
using Stryker.Core.Options;
using Stryker.Core.ProjectComponents;
using Stryker.Core.Testing;
using System.Collections.Generic;
using System.IO;
using System.IO.Abstractions;

namespace Stryker.Core.Reporters.Json
{
    public partial class JsonReporter : IReporter
    {
        private readonly StrykerOptions _options;
        private readonly IFileSystem _fileSystem;
        private readonly IChalk _chalk;

        public JsonReporter(StrykerOptions options, IFileSystem fileSystem = null, IChalk chalk = null)
        {
            _options = options;
            _fileSystem = fileSystem ?? new FileSystem();
            _chalk = chalk ?? new Chalk();
        }

        public void OnAllMutantsTested(IReadOnlyInputComponent mutationTree)
        {
            var mutationReport = JsonReport.Build(_options, mutationTree);

            var reportPath = Path.Combine(_options.OutputPath, "reports", "mutation-report.json");
            WriteReportToJsonFile(reportPath, mutationReport.ToJson());

            _chalk.Green($"\nYour json report has been generated at: \n " +
                $"{reportPath} \n");
        }

        private void WriteReportToJsonFile(string filePath, string mutationReport)
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

        public void OnStartMutantTestRun(IEnumerable<IReadOnlyMutant> mutantsToBeTested, IEnumerable<TestDescription> testDescriptions)
        {
        }
    }
}
