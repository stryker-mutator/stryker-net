﻿using Crayon;
using Stryker.Core.Mutants;
using Stryker.Core.Options;
using Stryker.Core.ProjectComponents;
using Stryker.Core.Reporters.Json;
using System;
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
        private readonly TextWriter _consoleWriter;

        public HtmlReporter(StrykerOptions options, IFileSystem fileSystem = null, TextWriter consoleWriter = null)
        {
            _options = options;
            _fileSystem = fileSystem ?? new FileSystem();
            _consoleWriter = consoleWriter ?? Console.Out;
        }

        public void OnAllMutantsTested(IReadOnlyInputComponent mutationTree)
        {
            var mutationReport = JsonReport.Build(_options, mutationTree);

            var reportPath = Path.Combine(_options.OutputPath, "reports", "mutation-report.html");
            WriteHtmlReport(reportPath, mutationReport.ToJsonHtmlSafe());

            _consoleWriter.Write(Output.Green($"\nYour html report has been generated at: \n " +
                $"{reportPath} \n" +
                $"You can open it in your browser of choice. \n"));
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