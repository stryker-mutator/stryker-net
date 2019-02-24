using Newtonsoft.Json;
using Stryker.Core.Initialisation.ProjectComponent;
using Stryker.Core.Options;
using System.Collections.Generic;
using System.Linq;

namespace Stryker.Core.Reporters.Json
{
    public class JsonReport
    {
        public string SchemaVersion { get; set; }
        public IDictionary<string, int> Thresholds { get; } = new Dictionary<string, int>();
        public IDictionary<string, JsonReportComponent> Files { get; } = new Dictionary<string, JsonReportComponent>();

        [JsonIgnore]
        private readonly StrykerOptions _options;
        public JsonReport(StrykerOptions options, IReadOnlyInputComponent mutationReport)
        {
            Merge(Files, GenerateReportComponents(mutationReport));
            _options = options;
        }

        private IDictionary<string, JsonReportComponent> GenerateReportComponents(IReadOnlyInputComponent component)
        {
            Dictionary<string, JsonReportComponent> files = new Dictionary<string, JsonReportComponent>();
            if (component is FolderComposite folder)
            {
                Merge(files, GenerateFolderReportComponents(folder));
            }
            else if (component is FileLeaf file)
            {
                Merge(files, GenerateFileReportComponents(file));
            }

            return files;
        }

        private IDictionary<string, JsonReportComponent> GenerateFolderReportComponents(FolderComposite folderComponent)
        {
            Dictionary<string, JsonReportComponent> files = new Dictionary<string, JsonReportComponent>();
            foreach (var child in folderComponent.Children)
            {
                Merge(files, GenerateReportComponents(child));
            }

            return files;
        }

        private IDictionary<string, JsonReportComponent> GenerateFileReportComponents(FileLeaf fileComponent)
        {
            var reportComponent = new JsonReportComponent(fileComponent)
            {
                Health = CheckHealth(fileComponent)
            };

            return new Dictionary<string, JsonReportComponent> { { fileComponent.Name, reportComponent } };
        }

        private string CheckHealth(FileLeaf file)
        {
            if (file.GetMutationScore() >= _options.ThresholdOptions.ThresholdHigh)
            {
                return "Good";
            }
            else if (file.GetMutationScore() <= _options.ThresholdOptions.ThresholdBreak)
            {
                return "Danger";
            }
            else
            {
                return "Warning";
            }
        }

        private void Merge<T, Y>(IDictionary<T, Y> to, IDictionary<T, Y> from)
        {
            from.ToList().ForEach(x => to[x.Key] = x.Value);
        }
    }
}
