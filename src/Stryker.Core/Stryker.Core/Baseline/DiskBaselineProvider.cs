using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Stryker.Core.Logging;
using Stryker.Core.Options;
using Stryker.Core.Reporters.Json;
using System.IO;
using System.IO.Abstractions;
using System.Threading.Tasks;

namespace Stryker.Core.Baseline
{
    public class DiskBaselineProvider : IBaselineProvider
    {
        private readonly StrykerOptions _options;
        private readonly IFileSystem _fileSystem;
        private readonly ILogger<DiskBaselineProvider> _logger;

        public DiskBaselineProvider(StrykerOptions options, IFileSystem fileSystem = null)
        {
            _options = options;
            _fileSystem = fileSystem ?? new FileSystem();
            _logger = ApplicationLogging.LoggerFactory.CreateLogger<DiskBaselineProvider>();
        }


        public async Task<JsonReport> Load(string version)
        {
            var reportPath = Path.Combine(_options.BaselineOutputPath, version, "stryker-report.json");

            using StreamReader inputReader = _fileSystem.File.OpenText(reportPath);

            var reportJson = await inputReader.ReadToEndAsync();

            return JsonConvert.DeserializeObject<JsonReport>(reportJson);
        }

        public async Task Save(JsonReport report, string version)
        {
            var reportPath = Path.Combine(_options.BaselineOutputPath, version);

            var reportJson = report.ToJson();

            _fileSystem.Directory.CreateDirectory(reportPath);

            using StreamWriter outputWriter = _fileSystem.File.CreateText(Path.Combine(reportPath, $"stryker-report.json"));

            await outputWriter.WriteAsync(reportJson);

            _logger.LogDebug($"Baseline report has been writter to {Path.Combine(reportPath, $"stryker-report.json")}");
        }
    }
}
