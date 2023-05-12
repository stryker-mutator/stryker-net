using System.Collections.Generic;
using System.IO;
using System.IO.Abstractions;
using System.Linq;
using Spectre.Console;
using Stryker.Core.Mutants;
using Stryker.Core.Options;
using Stryker.Core.Options.Inputs;
using Stryker.Core.ProjectComponents;
using Stryker.Core.ProjectComponents.TestProjects;
using Stryker.Core.Reporters.Html.Realtime;
using Stryker.Core.Reporters.Json;
using Stryker.Core.Reporters.WebBrowserOpener;

namespace Stryker.Core.Reporters.Html;

public class HtmlReporter : IReporter
{
    private readonly StrykerOptions _options;
    private readonly IFileSystem _fileSystem;
    private readonly IAnsiConsole _console;
    private readonly IWebbrowserOpener _browser;
    private readonly IRealtimeMutantHandler _mutantHandler;

    public HtmlReporter(
        StrykerOptions options,
        IFileSystem fileSystem = null,
        IAnsiConsole console = null,
        IWebbrowserOpener browser = null,
        IRealtimeMutantHandler mutantHandler = null)
    {
        _options = options;
        _fileSystem = fileSystem ?? new FileSystem();
        _console = console ?? AnsiConsole.Console;
        _browser = browser ?? new CrossPlatformBrowserOpener();
        _mutantHandler = mutantHandler ?? new RealtimeMutantHandler(_options);
    }

    public void OnAllMutantsTested(IReadOnlyProjectComponent reportComponent, TestProjectsInfo testProjectsInfo)
    {
        var reportPath = BuildReportPath();
        WriteHtmlReport(reportPath, reportComponent, testProjectsInfo);

        if (_options.ReportTypeToOpen == ReportType.Html)
        {
            _mutantHandler.CloseSseEndpoint();
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

    public void OnMutantsCreated(IReadOnlyProjectComponent reportComponent, TestProjectsInfo testProjectsInfo)
    {
        if (_options.ReportTypeToOpen == ReportType.Html)
        {
            _mutantHandler.OpenSseEndpoint();

            var reportPath = BuildReportPath();
            WriteHtmlReport(reportPath, reportComponent, testProjectsInfo);
            OpenReportInBrowser(reportPath);
            return;
        }

        var aqua = new Style(Color.Aqua);
        _console.WriteLine("Hint: by passing \"--open-report or -o\" the report will open automatically and update the report in realtime.", aqua);
    }

    public void OnMutantTested(IReadOnlyMutant result)
    {
        if (_options.ReportTypeToOpen != ReportType.Html)
        {
            return;
        }

        _mutantHandler.SendMutantTestedEvent(result);
    }

    public void OnStartMutantTestRun(IEnumerable<IReadOnlyMutant> mutantsToBeTested)
    {
        // This reporter does not currently report when the mutation test run starts
    }

    private void OpenReportInBrowser(string reportPath) => _browser.Open(reportPath);

    private string BuildReportPath()
    {
        var filename = _options.ReportFileName + ".html";
        var reportPath = Path.Combine(_options.ReportPath, filename);
        var reportPathNormalized = FilePathUtils.NormalizePathSeparators(reportPath);
        return reportPathNormalized;
    }

    private void WriteHtmlReport(string filePath, IReadOnlyProjectComponent reportComponent, TestProjectsInfo testProjectsInfo)
    {
        var mutationReport = JsonReport.Build(_options, reportComponent, testProjectsInfo).ToJsonHtmlSafe();

        _fileSystem.Directory.CreateDirectory(Path.GetDirectoryName(filePath));

        using var htmlStream = typeof(HtmlReporter).Assembly
            .GetManifestResourceStream(typeof(HtmlReporter)
                .Assembly.GetManifestResourceNames()
                .Single(m => m.Contains("mutation-report.html")));

        using var jsStream = typeof(HtmlReporter).Assembly
            .GetManifestResourceStream(typeof(HtmlReporter)
                .Assembly.GetManifestResourceNames()
                .Single(m => m.Contains("mutation-test-elements.js")));

        using var htmlReader = new StreamReader(htmlStream!);
        using var jsReader = new StreamReader(jsStream!);

        using var file = _fileSystem.File.CreateText(filePath);
        var fileContent = htmlReader.ReadToEnd();

        fileContent = fileContent.Replace("##REPORT_JS##", jsReader.ReadToEnd());
        fileContent = fileContent.Replace("##REPORT_TITLE##", "Stryker.NET Report");
        fileContent = fileContent.Replace("##REPORT_JSON##", mutationReport);
        fileContent = fileContent.Replace("##SSE_ENDPOINT##", $"http://localhost:{_mutantHandler.Port}/");

        file.WriteLine(fileContent);
    }
}
