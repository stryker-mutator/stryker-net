using System.Collections.Generic;
using System.Linq;
using Stryker.Configuration.ProjectComponents;
using Stryker.Configuration.ProjectComponents.TestProjects;
using Stryker.Configuration.Reporters.Json.TestFiles;
using Stryker.Configuration.Reporting;
using Stryker.Configuration;
using Stryker.Reporters.Json.SourceFiles;

namespace Stryker.Configuration.Reporters.Json
{
    public class JsonReport : IJsonReport
    {
        public string SchemaVersion { get; init; } = "2";
        public IDictionary<string, int> Thresholds { get; init; } = new Dictionary<string, int>();
        public string ProjectRoot { get; init; }
        public IDictionary<string, ISourceFile> Files { get; init; } = new Dictionary<string, ISourceFile>();
        public IDictionary<string, JsonTestFile> TestFiles { get; set; } = null;

        public JsonReport() { }

        private JsonReport(StrykerOptions options, IReadOnlyProjectComponent mutationReport, TestProjectsInfo testProjectsInfo)
        {
            Thresholds.Add("high", options.Thresholds.High);
            Thresholds.Add("low", options.Thresholds.Low);

            ProjectRoot = mutationReport.FullPath;

            Merge(Files, GenerateReportComponents(mutationReport));
            AddTestFiles(testProjectsInfo);
        }

        public static JsonReport Build(StrykerOptions options, IReadOnlyProjectComponent mutationReport, TestProjectsInfo testProjectsInfo) => new(options, mutationReport, testProjectsInfo);

        private IDictionary<string, ISourceFile> GenerateReportComponents(IReadOnlyProjectComponent component)
        {
            var files = new Dictionary<string, ISourceFile>();
            if (component is IReadOnlyFolderComposite folder)
            {
                Merge(files, GenerateFolderReportComponents(folder));
            }
            else if (component is IReadOnlyFileLeaf file)
            {
                Merge(files, GenerateFileReportComponents(file));
            }

            return files;
        }

        private IDictionary<string, ISourceFile> GenerateFolderReportComponents(IReadOnlyFolderComposite folderComponent)
        {
            var files = new Dictionary<string, ISourceFile>();
            foreach (var child in folderComponent.Children)
            {
                Merge(files, GenerateReportComponents(child));
            }

            return files;
        }

        private static IDictionary<string, ISourceFile> GenerateFileReportComponents(IReadOnlyFileLeaf fileComponent) => new Dictionary<string, ISourceFile> { { fileComponent.FullPath, new SourceFile(fileComponent) } };

        private void AddTestFiles(TestProjectsInfo testProjectsInfo)
        {
            if (testProjectsInfo?.TestFiles is not null)
            {
                TestFiles = new Dictionary<string, JsonTestFile>();
                foreach (var testFile in testProjectsInfo.TestFiles)
                {
                    if (TestFiles.TryGetValue(testFile.FilePath, out var jsonFile))
                    {
                        jsonFile.AddTestFile(testFile);
                    }
                    else
                    {
                        TestFiles.Add(testFile.FilePath, new JsonTestFile(testFile));
                    }
                }
            }
        }

        private static void Merge<TTo, TFrom>(IDictionary<TTo, TFrom> to, IDictionary<TTo, TFrom> from) => from.ToList().ForEach(x => to[x.Key] = x.Value);
    }
}
