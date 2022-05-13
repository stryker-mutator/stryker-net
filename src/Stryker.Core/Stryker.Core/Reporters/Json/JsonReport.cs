using System.Collections.Generic;
using System.Linq;
using Stryker.Core.Options;
using Stryker.Core.ProjectComponents;
using Stryker.Core.Reporters.Json.SourceFiles;

namespace Stryker.Core.Reporters.Json
{
    public class JsonReport
    {
        public string SchemaVersion { get; init; } = "1";
        public IDictionary<string, int> Thresholds { get; init; } = new Dictionary<string, int>();
        public string ProjectRoot { get; init; }
        public IDictionary<string, SourceFile> Files { get; init; } = new Dictionary<string, SourceFile>();

        public JsonReport()
        {
        }

        private JsonReport(StrykerOptions options, IReadOnlyProjectComponent mutationReport)
        {
            Thresholds.Add("high", options.Thresholds.High);
            Thresholds.Add("low", options.Thresholds.Low);

            ProjectRoot = mutationReport.FullPath;

            Merge(Files, GenerateReportComponents(mutationReport));
        }

        public static JsonReport Build(StrykerOptions options, IReadOnlyProjectComponent mutationReport)
        {
            return new JsonReport(options, mutationReport);
        }

        private IDictionary<string, SourceFile> GenerateReportComponents(IReadOnlyProjectComponent component)
        {
            var files = new Dictionary<string, SourceFile>();
            if (component is Solution solution)
            {
                Merge(files, GenerateSolutionReportComponents(solution));
            }
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

        private IDictionary<string, SourceFile> GenerateSolutionReportComponents(IReadOnlyFolderComposite folderComponent)
        {
            var files = new Dictionary<string, SourceFile>();
            foreach (var child in folderComponent.Children)
            {
                Merge(files, GenerateReportComponents(child));
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

        private IDictionary<string, SourceFile> GenerateFileReportComponents(IReadOnlyFileLeaf fileComponent)
        {
            return new Dictionary<string, SourceFile> { { fileComponent.RelativePath, new SourceFile(fileComponent) } };
        }

        private void Merge<TTo, TFrom>(IDictionary<TTo, TFrom> to, IDictionary<TTo, TFrom> from)
        {
            from.ToList().ForEach(x => to[x.Key] = x.Value);
        }
    }
}
