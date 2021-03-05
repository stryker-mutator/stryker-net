using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Stryker.Core.Logging;
using Stryker.Core.Options;
using Stryker.Core.Reporters.Json;
using System.IO;
using System.IO.Abstractions;
using System.Threading.Tasks;

namespace Stryker.Core.Baseline.Providers
{
    public class DiskBaselineProvider : IBaselineProvider
    {
        private readonly IStrykerOptions _options;
        private readonly IFileSystem _fileSystem;
        private readonly ILogger<DiskBaselineProvider> _logger;
        private const string _outputPath = "StrykerOutput/Baselines/";

        public DiskBaselineProvider(IStrykerOptions options, IFileSystem fileSystem = null)
        {
            _options = options;
            _fileSystem = fileSystem ?? new FileSystem();
            _logger = ApplicationLogging.LoggerFactory.CreateLogger<DiskBaselineProvider>();
        }


        public async Task<JsonReport> Load(string version)
        {
            var reportPath = FilePathUtils.NormalizePathSeparators(
                Path.Combine(_options.BasePath, _outputPath, version, "stryker-report.json"));

            if (_fileSystem.File.Exists(reportPath))
            {
                using StreamReader inputReader = _fileSystem.File.OpenText(reportPath);

                var reportJson = await inputReader.ReadToEndAsync();

                return JsonConvert.DeserializeObject<JsonReport>(reportJson);
            }

            _logger.LogDebug("No baseline was found at {0}", reportPath.ToString());
            return null;
        }

        public async Task Save(JsonReport report, string version)
        {
            var reportPath = FilePathUtils.NormalizePathSeparators(
                Path.Combine(_options.BasePath, _outputPath, version));

            var reportJson = report.ToJson();

            _fileSystem.Directory.CreateDirectory(reportPath);

            using StreamWriter outputWriter = _fileSystem.File.CreateText(Path.Combine(reportPath, $"stryker-report.json"));

            await outputWriter.WriteAsync(reportJson);

            _logger.LogDebug($"Baseline report has been saved to {Path.Combine(reportPath, $"stryker-report.json")}");
        }
    }
}
