using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Stryker.Core.Options;
using Stryker.Core.ProjectComponents;
using System.Collections.Generic;
using System.Linq;

namespace Stryker.Core.Reporters.Json
{
    public class JsonReport
    {
        public string SchemaVersion { get; } = "1.1";
        public IDictionary<string, int> Thresholds { get; } = new Dictionary<string, int>();
        public IDictionary<string, JsonReportFileComponent> Files { get; } = new Dictionary<string, JsonReportFileComponent>();

        [JsonIgnore]
        private static StrykerOptions _options;
        [JsonIgnore]
        private static JsonReport _report = null;

        [JsonConstructor]
        private JsonReport()
        {

        }

        private JsonReport(StrykerOptions options, IReadOnlyInputComponent mutationReport)
        {
            _options = options;

            Thresholds.Add("high", _options.Thresholds.High);
            Thresholds.Add("low", _options.Thresholds.Low);

            Merge(Files, GenerateReportComponents(mutationReport));
        }

        public static JsonReport Build(StrykerOptions options, IReadOnlyInputComponent mutationReport)
        {
            // This should really only happen in unit tests.
            // We need this construct because in a unit test
            // we want to be able to generate different reports with different settings
            _report = _options == options ? _report : null;

            // If the report was already generated, return the existing report
            _report = _report ?? new JsonReport(options, mutationReport);

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

        public string ToJsonHtmlSafe()
        {
            return ToJson().Replace("<", "<\" + \"");
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
            return new Dictionary<string, JsonReportFileComponent> { { fileComponent.RelativePath, new JsonReportFileComponent(fileComponent) } };
        }

        private void Merge<T, Y>(IDictionary<T, Y> to, IDictionary<T, Y> from)
        {
            from.ToList().ForEach(x => to[x.Key] = x.Value);
        }
    }
}
