using System.Collections.Generic;
using System.IO;
using System.IO.Abstractions;
using System.Linq;
using Stryker.Core.Mutants;
using Stryker.Core.Options;
using Stryker.Core.ProjectComponents;
using Stryker.Core.Reporters.Json;
using Stryker.Core.Testing;

namespace Stryker.Core.Reporters.TestStatisticsReporter
{
    public class TestStatisticsReporter: IReporter
    {
        private StrykerOptions _options;
        private IFileSystem _fileSystem;
        private IChalk _chalk;
        private IEnumerable<TestDescription> _tests;

        public TestStatisticsReporter(StrykerOptions options, IFileSystem fileSystem, IChalk chalk = null)
        {
            this._options = options;
            this._fileSystem = fileSystem;
            this._chalk = chalk ?? new Chalk();
        }

        public static TestStatisticsReport Build(StrykerOptions strykerOptions, IReadOnlyInputComponent folderComponent, IEnumerable<TestDescription> tests)
        {
            var mutants = folderComponent.ReadOnlyMutants.
                Select(mutant => new JsonMutant {Id = mutant.Id, 
                    Location = new JsonMutantLocation(mutant.Mutation.OriginalNode.GetLocation().GetMappedLineSpan()), 
                    Status = mutant.ResultStatus.ToString(), 
                    Replacement = mutant.Mutation.ReplacementNode.ToFullString()}).ToList();
            var jSonTests = tests.Select(test => new JsonTest {Guid = test.Guid, Name = test.Name}).ToList();
            return new TestStatisticsReport(mutants, jSonTests);
        }

        public void OnMutantsCreated(IReadOnlyInputComponent reportComponent)
        {
        }

        public void OnStartMutantTestRun(IEnumerable<IReadOnlyMutant> mutantsToBeTested, IEnumerable<TestDescription> testDescriptions)
        {
            _tests = testDescriptions;
        }

        public void OnMutantTested(IReadOnlyMutant result)
        {
        }

        public void OnAllMutantsTested(IReadOnlyInputComponent reportComponent)
        {
            var mutationReport = Build(_options, reportComponent, _tests);

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

    }
}