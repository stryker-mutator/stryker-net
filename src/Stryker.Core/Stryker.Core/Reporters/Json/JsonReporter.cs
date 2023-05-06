using System.Collections.Generic;
using System.IO;
using System.IO.Abstractions;
using Spectre.Console;
using Stryker.Core.Mutants;
using Stryker.Core.Options;
using Stryker.Core.ProjectComponents;
using Stryker.Core.ProjectComponents.TestProjects;

namespace Stryker.Core.Reporters.Json;

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

    public void OnAllMutantsTested(IReadOnlyProjectComponent reportComponent, TestProjectsInfo testProjectsInfo)
    {
        var mutationReport = JsonReport.Build(_options, reportComponent, testProjectsInfo);
        var filename = _options.ReportFileName + ".json";
        var reportPath = Path.Combine(_options.ReportPath, filename);
        var reportUri = "file://" + reportPath.Replace("\\", "/");

        WriteReportToJsonFile(reportPath, mutationReport);

        var green = new Style(Color.Green);
        _console.WriteLine();
        _console.WriteLine("Your json report has been generated at:", green);

        if (_console.Profile.Capabilities.Links)
        {
            // We must print the report path as the link text because on some terminals links might be supported but not actually clickable: https://github.com/spectreconsole/spectre.console/issues/764
            _console.WriteLine(reportPath, green.Combine(new Style(link: reportUri)));
        }
        else
        {
            _console.WriteLine(reportUri, green);
        }
    }

    private void WriteReportToJsonFile(string filePath, JsonReport mutationReport)
    {
        _fileSystem.Directory.CreateDirectory(Path.GetDirectoryName(filePath));
        using var file = _fileSystem.File.Create(filePath);
        mutationReport.Serialize(file);
    }

    public void OnMutantsCreated(IReadOnlyProjectComponent reportComponent, TestProjectsInfo testProjectsInfo)
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
