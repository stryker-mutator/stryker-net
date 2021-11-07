using System;
using System.Collections.Generic;
using System.Linq;
using Stryker.Core.Mutants;
using Stryker.Core.Options;
using Stryker.Core.ProjectComponents;
using Stryker.Core.ProjectComponents.TestProjects;

namespace Stryker.Core.Reporters.Json
{
    public class JsonReport
    {
        public string SchemaVersion { get; init; } = "1";
        public IDictionary<string, int> Thresholds { get; init; } = new Dictionary<string, int>();
        public string ProjectRoot { get; init; }
        public IDictionary<string, JsonSourceFile> Files { get; init; } = new Dictionary<string, JsonSourceFile>();
        public IDictionary<string, JsonTestFile> TestFiles { get; set; } = new Dictionary<string, JsonTestFile>();

        public JsonReport()
        {
        }

        private JsonReport(StrykerOptions options, IReadOnlyProjectComponent mutationReport, TestProjectsInfo testProjectsInfo)
        {
            Thresholds.Add("high", options.Thresholds.High);
            Thresholds.Add("low", options.Thresholds.Low);

            ProjectRoot = mutationReport.FullPath;

            Merge(Files, GenerateReportComponents(mutationReport));
            AddTestFiles(testProjectsInfo);
        }

        public static JsonReport Build(StrykerOptions options, IReadOnlyProjectComponent mutationReport, TestProjectsInfo testProjectsInfo = null) => new(options, mutationReport, testProjectsInfo);

        private void AddTestFiles(TestProjectsInfo testProjectsInfo)
        {
            if (testProjectsInfo is not null)
            {
                foreach (var testFile in testProjectsInfo?.TestFiles)
                {
                    TestFiles.Add(testFile.FilePath, new JsonTestFile(testFile));
                }
            }
        }

        private IDictionary<string, JsonSourceFile> GenerateReportComponents(IReadOnlyProjectComponent component)
        {
            var files = new Dictionary<string, JsonSourceFile>();
            if (component is ReadOnlyFolderComposite folder)
            {
                Merge(files, GenerateFolderReportComponents(folder));
            }
            else if (component is ReadOnlyFileLeaf file)
            {
                Merge(files, GenerateFileReportComponents(file));
            }

            return files;
        }

        private IDictionary<string, JsonSourceFile> GenerateFolderReportComponents(ReadOnlyFolderComposite folderComponent)
        {
            var files = new Dictionary<string, JsonSourceFile>();
            foreach (var child in folderComponent.Children)
            {
                Merge(files, GenerateReportComponents(child));
            }

            return files;
        }

        private static IDictionary<string, JsonSourceFile> GenerateFileReportComponents(ReadOnlyFileLeaf fileComponent)
        {
            return new Dictionary<string, JsonSourceFile> { { fileComponent.RelativePath, new JsonSourceFile(fileComponent) } };
        }

        private static void Merge<TTo, TFrom>(IDictionary<TTo, TFrom> to, IDictionary<TTo, TFrom> from)
        {
            from.ToList().ForEach(x => to[x.Key] = x.Value);
        }
    }
}
