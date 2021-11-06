using System.Collections.Generic;
using System.Linq;
using Stryker.Core.Options;
using Stryker.Core.ProjectComponents;

namespace Stryker.Core.Reporters.Json
{
    public class JsonReport
    {
        public string SchemaVersion { get; init; } = "1";
        public IDictionary<string, int> Thresholds { get; init; } = new Dictionary<string, int>();
        public string ProjectRoot { get; init; }
        public IDictionary<string, JsonSourceFile> Files { get; init; } = new Dictionary<string, JsonSourceFile>();

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

        public static JsonReport Build(StrykerOptions options, IReadOnlyProjectComponent mutationReport) => new(options, mutationReport);

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

        private IDictionary<string, JsonSourceFile> GenerateFileReportComponents(ReadOnlyFileLeaf fileComponent)
        {
            return new Dictionary<string, JsonSourceFile> { { fileComponent.RelativePath, new JsonSourceFile(fileComponent) } };
        }

        private void Merge<TTo, TFrom>(IDictionary<TTo, TFrom> to, IDictionary<TTo, TFrom> from)
        {
            from.ToList().ForEach(x => to[x.Key] = x.Value);
        }
    }
}
