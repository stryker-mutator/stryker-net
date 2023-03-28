using System.Collections.Generic;
using System.IO;
using System.IO.Abstractions;
using System.Linq;
using Spectre.Console;
using Stryker.Core.Mutants;
using Stryker.Core.Options;
using Stryker.Core.ProjectComponents;
using Stryker.Core.Reporters.HtmlReporter;
using Stryker.Core.ProjectComponents.TestProjects;
using Stryker.Core.Reporters.HtmlReporter.ProcessWrapper;
using Stryker.Core.Reporters.Json;

namespace Stryker.Core.Reporters.HtmlReporter;

public class HtmlReporter : IReporter
{
    private readonly StrykerOptions _options;
    private readonly IFileSystem _fileSystem;
    private readonly IAnsiConsole _console;
    private readonly IWebbrowserOpener _processWrapper;
    private readonly SseServer _sseServer;

    public HtmlReporter(StrykerOptions options, IFileSystem fileSystem = null,
        IAnsiConsole console = null, IWebbrowserOpener processWrapper = null)
    {
        _options = options;
        _fileSystem = fileSystem ?? new FileSystem();
        _console = console ?? AnsiConsole.Console;
        _processWrapper = processWrapper ?? new WebbrowserOpener();
        _sseServer = new SseServer(_console);
    }

    public void OnAllMutantsTested(IReadOnlyProjectComponent reportComponent, TestProjectsInfo testProjectsInfo)
    {
        var mutationReport = JsonReport.Build(_options, reportComponent, testProjectsInfo);
        var filename = _options.ReportFileName + ".html";
        var reportPath = Path.Combine(_options.ReportPath, filename);

        reportPath = FilePathUtils.NormalizePathSeparators(reportPath);

        WriteHtmlReport(reportPath, mutationReport.ToJsonHtmlSafe());

        if (_options.ReportTypeToOpen == Options.Inputs.ReportType.Html)
        {
            _processWrapper.Open("file://" + reportPath.Replace("\\", "/"));
        }
        else
        {
            var aqua = new Style(Color.Aqua);
            _console.WriteLine("Hint: by passing \"--open-report or -o\" the report will open automatically once Stryker is done.", aqua);
        }

        var reportUri = "file://" + reportPath.Replace("\\", "/").Replace(" ", "%20");

        var green = new Style(Color.Green);
        _console.WriteLine();
        _console.WriteLine("Your html report has been generated at:", green);

        if (_console.Profile.Capabilities.Links)
        {
            // We must print the report path as the link text because on some terminals links might be supported but not actually clickable: https://github.com/spectreconsole/spectre.console/issues/764
            _console.WriteLine(reportPath, green.Combine(new Style(link: reportUri)));
        }
        else
        {
            _console.WriteLine(reportUri, green);
        }

        _console.WriteLine("You can open it in your browser of choice.", green);
    }

    private void WriteHtmlReport(string filePath, string mutationReport)
    {
        _fileSystem.Directory.CreateDirectory(Path.GetDirectoryName(filePath));

        using var htmlStream = typeof(HtmlReporter).Assembly
            .GetManifestResourceStream(typeof(HtmlReporter)
                .Assembly.GetManifestResourceNames()
                .Single(m => m.Contains("mutation-report.html")));

        using var jsStream = typeof(HtmlReporter).Assembly
            .GetManifestResourceStream(typeof(HtmlReporter)
                .Assembly.GetManifestResourceNames()
                .Single(m => m.Contains("mutation-test-elements.js")));

        using var htmlReader = new StreamReader(htmlStream);
        using var jsReader = new StreamReader(jsStream);

        using var file = _fileSystem.File.CreateText(filePath);
        var fileContent = htmlReader.ReadToEnd();

        fileContent = fileContent.Replace("##REPORT_JS##", jsReader.ReadToEnd());
        fileContent = fileContent.Replace("##REPORT_TITLE##", "Stryker.NET Report");
        fileContent = fileContent.Replace("##REPORT_JSON##", mutationReport);

        file.WriteLine(fileContent);
    }

    public void OnMutantsCreated(IReadOnlyProjectComponent reportComponent)
    {
        // This reporter does not currently report when mutants are created
    }

    public void OnMutantTested(IReadOnlyMutant result) => _sseServer.PublishNewMutantData(result);

    public void OnStartMutantTestRun(IEnumerable<IReadOnlyMutant> mutantsToBeTested) => _sseServer.Start();
}
