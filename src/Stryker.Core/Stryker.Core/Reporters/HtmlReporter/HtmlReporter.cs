using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Abstractions;
using System.Linq;
using Crayon;
using Stryker.Core.Mutants;
using Stryker.Core.Options;
using Stryker.Core.ProjectComponents;
using Stryker.Core.Reporters.HtmlReporter.ProcessWrapper;
using Stryker.Core.Reporters.Json;

namespace Stryker.Core.Reporters.Html.reporter
{
    public class HtmlReporter : IReporter
    {
        private readonly StrykerOptions _options;
        private readonly IFileSystem _fileSystem;
        private readonly TextWriter _consoleWriter;
        private readonly IProcessWrapper _processWrapper;

        public HtmlReporter(StrykerOptions options, IFileSystem fileSystem = null,
            TextWriter consoleWriter = null, IProcessWrapper processWrapper = null)
        {
            _options = options;
            _fileSystem = fileSystem ?? new FileSystem();
            _consoleWriter = consoleWriter ?? Console.Out;
            _processWrapper = processWrapper ?? new ProcessWrapper();
        }

        public void OnAllMutantsTested(IReadOnlyProjectComponent reportComponent)
        {
            var mutationReport = JsonReport.Build(_options, reportComponent);
            var filename = _options.ReportFileName + ".html";
            var reportPath = Path.Combine(_options.OutputPath, "reports", filename);

            WriteHtmlReport(reportPath, mutationReport.ToJsonHtmlSafe());

            var clickablePath = FilePathUtils.NormalizePathSeparators(reportPath);
            clickablePath = clickablePath.StartsWith(Path.DirectorySeparatorChar) ? clickablePath : Path.DirectorySeparatorChar + clickablePath;

            _consoleWriter.Write(Output.Green($"\nYour html report has been generated at: \n " +
                $"file://{clickablePath} \n" +
                $"You can open it in your browser of choice. \n"));

            if (_options.ReportTypeToOpen == Options.Inputs.ReportType.Html)
            {
                clickablePath = clickablePath.Remove(0, 1);

                _processWrapper.Start("file://" + clickablePath);
            }
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
                        fileContent = fileContent.Replace("##REPORT_TITLE##", "Stryker.NET Report");
                        fileContent = fileContent.Replace("##REPORT_JSON##", mutationReport);

                        file.WriteLine(fileContent);
                    }
                }
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
