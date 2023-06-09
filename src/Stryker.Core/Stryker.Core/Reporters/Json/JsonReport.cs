using System.Collections.Generic;
using System.Linq;
using Stryker.Core.Options;
using Stryker.Core.ProjectComponents;
using Stryker.Core.ProjectComponents.TestProjects;
using Stryker.Core.Reporters.Json.SourceFiles;
using Stryker.Core.Reporters.Json.TestFiles;

namespace Stryker.Core.Reporters.Json
{
    public class JsonReport
    {
        public string SchemaVersion { get; init; } = "2";
        public IDictionary<string, int> Thresholds { get; init; } = new Dictionary<string, int>();
        public string ProjectRoot { get; init; }
        public IDictionary<string, SourceFile> Files { get; init; } = new Dictionary<string, SourceFile>();
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

        private IDictionary<string, SourceFile> GenerateReportComponents(IReadOnlyProjectComponent component)
        {
            var files = new Dictionary<string, SourceFile>();
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

        private IDictionary<string, SourceFile> GenerateFolderReportComponents(IReadOnlyFolderComposite folderComponent)
        {
            var files = new Dictionary<string, SourceFile>();
            foreach (var child in folderComponent.Children)
            {
                Merge(files, GenerateReportComponents(child));
            }

            return files;
        }

        private static IDictionary<string, SourceFile> GenerateFileReportComponents(IReadOnlyFileLeaf fileComponent) => new Dictionary<string, SourceFile> { { fileComponent.FullPath, new SourceFile(fileComponent) } };

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
