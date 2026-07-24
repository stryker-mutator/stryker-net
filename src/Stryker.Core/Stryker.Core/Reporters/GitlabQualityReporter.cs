using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Abstractions;
using System.Text.Json;
using System.Text.Json.Serialization;
using Spectre.Console;
using Stryker.Abstractions;
using Stryker.Abstractions.Options;
using Stryker.Abstractions.ProjectComponents;
using Stryker.Abstractions.Reporting;
using Stryker.Core.Reporters.Json;

namespace Stryker.Core.Reporters;

public class GitlabQualityReporter : IReporter
{
    private readonly IStrykerOptions _options;
    private readonly IFileSystem _fileSystem;
    private readonly IAnsiConsole _console;

    public GitlabQualityReporter(
        IStrykerOptions options,
        IFileSystem fileSystem = null,
        IAnsiConsole console = null)
    {
        _options = options;
        _fileSystem = fileSystem ?? new FileSystem();
        _console = console ?? AnsiConsole.Console;
    }

    public void OnAllMutantsTested(IReadOnlyProjectComponent reportComponent, ITestProjectsInfo testProjectsInfo)
    {
        var jsonReport = JsonReport.Build(_options, reportComponent, testProjectsInfo);
        var filename = "gitlab_quality.json";
        var reportPath = Path.Combine(_options.ReportPath, filename);
        var reportUri = "file://" + reportPath.Replace("\\", "/");

        IEnumerable<GitlabJsonItem> mutationReport = ConvertJsonReportToGitlabReport(jsonReport);
        WriteReportToJsonFile(reportPath, mutationReport);

        var green = new Style(Color.Green);
        _console.WriteLine();
        _console.WriteLine("Your Gitlab Quality Report has been generated at:", green);

        if (_console.Profile.Capabilities.Links)
        {
            // We must print the report path as the link text because on some terminals links might be supported but not actually clickable: https://github.com/spectreconsole/spectre.console/issues/764
            _console.Write(new Paragraph(reportPath, green, new Link(reportUri)));
            _console.WriteLine();
        }
        else
        {
            _console.WriteLine(reportUri, green);
        }
    }

    private void WriteReportToJsonFile(string reportPath, IEnumerable<GitlabJsonItem> mutationReport)
    {
        _fileSystem.Directory.CreateDirectory(Path.GetDirectoryName(reportPath));
        using var file = _fileSystem.File.Create(reportPath);
        using var writer = new Utf8JsonWriter(file);
        JsonSerializer.Serialize(writer, mutationReport, JsonReportSerialization.Options);
    }

    private static List<GitlabJsonItem> ConvertJsonReportToGitlabReport(IJsonReport jsonReport)
    {
        var items = new List<GitlabJsonItem>();
        foreach (var file in jsonReport.Files)
        {
            var fileName = file.Key.Replace(jsonReport.ProjectRoot + "\\", "").Replace("\\", "/");
            var fileContent = file.Value;
            foreach (var mutant in fileContent.Mutants)
            {
                var mutantStatus = Enum.Parse<MutantStatus>(mutant.Status, true);
                var severity = mutantStatus switch
                {
                    MutantStatus.Killed => "info",
                    MutantStatus.Survived => "major",
                    _ => "minor"
                };
                //var fingerprint = SHA1.HashData();
                items.Add(new GitlabJsonItem
                {
                    Description = !string.IsNullOrEmpty(mutant.Description) ? mutant.Description : mutant.MutatorName,
                    CheckName = mutant.MutatorName,
                    Fingerprint = mutant.Id,
                    Location = new GitlabJsonLocation
                    {
                        Path = fileName,
                        Lines = new GitlabJsonLine
                        {
                            Begin = mutant.Location.Start.Line
                        }
                    },
                    Severity = severity
                });
            }
        }
        return items;
    }

    public void OnMutantsCreated(IReadOnlyProjectComponent reportComponent, ITestProjectsInfo testProjectsInfo)
    {
        // This reporter does not report during the testrun
    }

    public void OnMutantTested(IReadOnlyMutant result)
    {
        // This reporter does not report during the testrun
    }

    public void OnStartMutantTestRun(IEnumerable<IReadOnlyMutant> mutantsToBeTested)
    {
        // This reporter does not report during the testrun
    }

    public void OnCoverageAnalysisStarted(int totalTests)
    {
        // This reporter does not report coverage analysis progress
    }

    public void OnCoverageAnalysisProgress(int testsCompleted, int totalTests)
    {
        // This reporter does not report coverage analysis progress
    }

    public void OnCoverageAnalysisCompleted()
    {
        // This reporter does not report coverage analysis progress
    }

    protected record GitlabJsonItem
    {
        public string Description { get; set; }
        [JsonPropertyName("check_name")]
        public string CheckName { get; set; }
        public string Fingerprint { get; set; }
        public GitlabJsonLocation Location { get; set; }
        public string Severity { get; set; }
    }

    protected record GitlabJsonLocation
    {
        public string Path { get; set; }
        public GitlabJsonLine Lines { get; set; }
    }

    protected record GitlabJsonLine
    {
        public int Begin { get; set; }
    }
}
