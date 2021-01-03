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
        public string SchemaVersion { get; } = "1.3";
        public IDictionary<string, int> Thresholds { get; } = new Dictionary<string, int>();
        public IDictionary<string, JsonReportFileComponent> Files { get; private set; } = new Dictionary<string, JsonReportFileComponent>();

        [JsonIgnore]
        private static IStrykerOptions _options;
        [JsonIgnore]
        private static JsonReport _report = null;

        [JsonConstructor]
        private JsonReport()
        {

        }

        private JsonReport(IStrykerOptions options, IReadOnlyProjectComponent mutationReport)
        {
            _options = options;

            Thresholds.Add("high", _options.Thresholds.High);
            Thresholds.Add("low", _options.Thresholds.Low);

            Merge(Files, GenerateReportComponents(mutationReport));
        }

        protected JsonReport(string schemaVersion = null, IDictionary<string, int> thresholds = null, IDictionary<string, JsonReportFileComponent> files = null)
        {
            SchemaVersion = schemaVersion ?? SchemaVersion;
            Thresholds = thresholds ?? Thresholds;
            Files = files ?? Files;
        }

        public static JsonReport Build(IStrykerOptions options, IReadOnlyProjectComponent mutationReport)
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

        private IDictionary<string, JsonReportFileComponent> GenerateReportComponents(IReadOnlyProjectComponent component)
        {
            Dictionary<string, JsonReportFileComponent> files = new Dictionary<string, JsonReportFileComponent>();
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

        private IDictionary<string, JsonReportFileComponent> GenerateFolderReportComponents(ReadOnlyFolderComposite folderComponent)
        {
            Dictionary<string, JsonReportFileComponent> files = new Dictionary<string, JsonReportFileComponent>();
            foreach (var child in folderComponent.Children)
            {
                Merge(files, GenerateReportComponents(child));
            }

            return files;
        }

        private IDictionary<string, JsonReportFileComponent> GenerateFileReportComponents(ReadOnlyFileLeaf fileComponent)
        {
            return new Dictionary<string, JsonReportFileComponent> { { fileComponent.RelativePath, new JsonReportFileComponent(fileComponent) } };
        }

        private void Merge<T, Y>(IDictionary<T, Y> to, IDictionary<T, Y> from)
        {
            from.ToList().ForEach(x => to[x.Key] = x.Value);
        }
    }
}
