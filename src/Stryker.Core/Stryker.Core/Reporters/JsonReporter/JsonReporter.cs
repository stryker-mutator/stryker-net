using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Abstractions;
using Crayon;
using Stryker.Core.Mutants;
using Stryker.Core.Options;
using Stryker.Core.ProjectComponents;

namespace Stryker.Core.Reporters.Json
{
    public partial class JsonReporter : IReporter
    {
        private readonly StrykerOptions _options;
        private readonly IFileSystem _fileSystem;
        private readonly TextWriter _consoleWriter;

        public JsonReporter(StrykerOptions options, IFileSystem fileSystem = null, TextWriter consoleWriter = null)
        {
            _options = options;
            _fileSystem = fileSystem ?? new FileSystem();
            _consoleWriter = consoleWriter ?? Console.Out;
        }

        public void OnAllMutantsTested(IReadOnlyProjectComponent mutationTree)
        {
            var mutationReport = JsonReport.Build(_options, mutationTree);

            var reportPath = Path.Combine(_options.OutputPath, "reports", "mutation-report.json");

            WriteReportToJsonFile(reportPath, mutationReport.ToJson());

            var clickablePath = reportPath.Replace("\\", "/");
            clickablePath = clickablePath.StartsWith("/") ? clickablePath : $"/{clickablePath}";

            _consoleWriter.Write(Output.Green($"\nYour json report has been generated at: \n file://{clickablePath} \n"));
        }

        private void WriteReportToJsonFile(string filePath, string mutationReport)
        {
            _fileSystem.Directory.CreateDirectory(Path.GetDirectoryName(filePath));
            using (var file = _fileSystem.File.CreateText(filePath))
            {
                file.WriteLine(mutationReport);
            }
        }

        public void OnMutantsCreated(IReadOnlyProjectComponent reportComponent)
        {
        }

        public void OnMutantTested(IReadOnlyMutant result)
        {
        }

        public void OnStartMutantTestRun(IEnumerable<IReadOnlyMutant> mutantsToBeTested)
        {
        }
    }
}
