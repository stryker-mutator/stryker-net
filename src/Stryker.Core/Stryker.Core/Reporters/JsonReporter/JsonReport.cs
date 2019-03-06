using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Stryker.Core.Initialisation.ProjectComponent;
using Stryker.Core.Options;
using System.Collections.Generic;
using System.Linq;

namespace Stryker.Core.Reporters.Json
{
    public class MutationReport
    {
        public string SchemaVersion { get; } = "0.0.7";
        public IDictionary<string, int> Thresholds { get; } = new Dictionary<string, int>();
        public IDictionary<string, JsonReportFileComponent> Files { get; } = new Dictionary<string, JsonReportFileComponent>();

        [JsonIgnore]
        private readonly StrykerOptions _options;
        [JsonIgnore]
        private static MutationReport _report = null;

        private MutationReport(StrykerOptions options, IReadOnlyInputComponent mutationReport)
        {
            _options = options;

            Thresholds.Add("high", _options.ThresholdOptions.ThresholdHigh);
            Thresholds.Add("low", _options.ThresholdOptions.ThresholdLow);

            Merge(Files, GenerateReportComponents(mutationReport));
        }

        public static MutationReport Build(StrykerOptions options, IReadOnlyInputComponent mutationReport)
        {
            _report = _report ?? new MutationReport(options, mutationReport);

            return _report;
        }

        public string ToJson()
        {
            var json = JsonConvert.SerializeObject(_report, new JsonSerializerSettings
            {
                ContractResolver = new DefaultContractResolver
                {
                    NamingStrategy = new CamelCaseNamingStrategy()
                },
                Formatting = Formatting.Indented,
                NullValueHandling = NullValueHandling.Ignore
            });

            return json;
        }

        private IDictionary<string, JsonReportFileComponent> GenerateReportComponents(IReadOnlyInputComponent component)
        {
            Dictionary<string, JsonReportFileComponent> files = new Dictionary<string, JsonReportFileComponent>();
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

        private IDictionary<string, JsonReportFileComponent> GenerateFolderReportComponents(FolderComposite folderComponent)
        {
            Dictionary<string, JsonReportFileComponent> files = new Dictionary<string, JsonReportFileComponent>();
            foreach (var child in folderComponent.Children)
            {
                Merge(files, GenerateReportComponents(child));
            }

            return files;
        }

        private IDictionary<string, JsonReportFileComponent> GenerateFileReportComponents(FileLeaf fileComponent)
        {
            var reportComponent = new JsonReportFileComponent(fileComponent)
            {
                Health = CheckHealth(fileComponent)
            };

            return new Dictionary<string, JsonReportFileComponent> { { fileComponent.RelativePath, reportComponent } };
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
