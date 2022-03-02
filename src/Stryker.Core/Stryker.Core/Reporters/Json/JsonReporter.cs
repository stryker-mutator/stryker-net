using System.Collections.Generic;
using System.IO;
using System.IO.Abstractions;
using Spectre.Console;
using Stryker.Core.Mutants;
using Stryker.Core.Options;
using Stryker.Core.ProjectComponents;

namespace Stryker.Core.Reporters.Json
{
    public partial class JsonReporter : IReporter
    {
        private readonly StrykerOptions _options;
        private readonly IFileSystem _fileSystem;
        private readonly IAnsiConsole _console;

        public JsonReporter(StrykerOptions options, IFileSystem fileSystem = null, IAnsiConsole console = null)
        {
            _options = options;
            _fileSystem = fileSystem ?? new FileSystem();
            _console = console ?? AnsiConsole.Console;
        }

        public void OnAllMutantsTested(IReadOnlyProjectComponent reportComponent)
        {
            var mutationReport = JsonReport.Build(_options, reportComponent);
            var filename = _options.ReportFileName + ".json";
            var reportPath = Path.Combine(_options.OutputPath, "reports", filename);

            WriteReportToJsonFile(reportPath, mutationReport);

            var clickablePath = reportPath.Replace("\\", "/");
            clickablePath = clickablePath.StartsWith("/") ? clickablePath : $"/{clickablePath}";

            _console.Markup($"[Green]\nYour json report has been generated at: \n file://{clickablePath} \n[/]");
        }

        private void WriteReportToJsonFile(string filePath, JsonReport mutationReport)
        {
            _fileSystem.Directory.CreateDirectory(Path.GetDirectoryName(filePath));
            using var file = _fileSystem.File.Create(filePath);
            mutationReport.Serialize(file);
        }

        public void OnMutantsCreated(IReadOnlyProjectComponent reportComponent)
        {
            // This reporter does not currently report when mutants are created
        }

        public void OnMutantTested(IReadOnlyMutant result)
        {
            // This reporter does not currently report when mutants are tested
        }

        public void OnStartMutantTestRun(IEnumerable<IReadOnlyMutant> mutantsToBeTested)
        {
            // This reporter does not currently report when the mutation testrun starts
        }
    }
}
