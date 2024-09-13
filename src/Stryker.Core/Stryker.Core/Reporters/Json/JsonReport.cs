using System.Collections.Generic;
using System.Linq;
using Stryker.Abstractions.Options;
using Stryker.Abstractions.ProjectComponents;
using Stryker.Abstractions.Reporting;
using Stryker.Core.Reporters.Json.SourceFiles;
using Stryker.Core.Reporters.Json.TestFiles;

namespace Stryker.Core.Reporters.Json
{
    public class JsonReport : IJsonReport
    {
        public string SchemaVersion { get; init; } = "2";
        public IDictionary<string, int> Thresholds { get; init; } = new Dictionary<string, int>();
        public string ProjectRoot { get; init; }
        public IDictionary<string, ISourceFile> Files { get; init; } = new Dictionary<string, ISourceFile>();
        public IDictionary<string, IJsonTestFile> TestFiles { get; set; } = null;

        public JsonReport() { }

        private JsonReport(IStrykerOptions options, IReadOnlyProjectComponent mutationReport, ITestProjectsInfo testProjectsInfo)
        {
            Thresholds.Add("high", options.Thresholds.High);
            Thresholds.Add("low", options.Thresholds.Low);

            ProjectRoot = mutationReport.FullPath;

            Merge(Files, GenerateReportComponents(mutationReport));
            AddTestFiles(testProjectsInfo);
        }

        public static IJsonReport Build(IStrykerOptions options, IReadOnlyProjectComponent mutationReport, ITestProjectsInfo testProjectsInfo) => new JsonReport(options, mutationReport, testProjectsInfo);

        private IDictionary<string, ISourceFile> GenerateReportComponents(IReadOnlyProjectComponent component)
        {
            var files = new Dictionary<string, ISourceFile>();
            if (component is IFolderComposite folder)
            {
                Merge(files, GenerateFolderReportComponents(folder));
            }
            else if (component is IReadOnlyFileLeaf file)
            {
                Merge(files, GenerateFileReportComponents(file));
            }

            return files;
        }

        private IDictionary<string, ISourceFile> GenerateFolderReportComponents(IFolderComposite folderComponent)
        {
            var files = new Dictionary<string, ISourceFile>();
            foreach (var child in folderComponent.Children)
            {
                Merge(files, GenerateReportComponents(child));
            }

            return files;
        }

        private static IDictionary<string, ISourceFile> GenerateFileReportComponents(IReadOnlyFileLeaf fileComponent) => new Dictionary<string, ISourceFile> { { fileComponent.FullPath, new SourceFile(fileComponent) } };

        private void AddTestFiles(ITestProjectsInfo testProjectsInfo)
        {
            if (testProjectsInfo?.TestFiles is not null)
            {
                TestFiles = new Dictionary<string, IJsonTestFile>();
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
