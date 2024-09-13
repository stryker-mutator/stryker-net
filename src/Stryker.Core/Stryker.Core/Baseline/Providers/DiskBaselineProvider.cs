using System.IO;
using System.IO.Abstractions;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Stryker.Abstractions.Baseline;
using Stryker.Abstractions.Logging;
using Stryker.Abstractions.Options;
using Stryker.Abstractions.Reporting;
using Stryker.Core.Reporters.Json;
using Stryker.Utilities;

namespace Stryker.Core.Baseline.Providers
{
    public class DiskBaselineProvider : IBaselineProvider
    {
        private readonly IStrykerOptions _options;
        private readonly IFileSystem _fileSystem;
        private readonly ILogger<DiskBaselineProvider> _logger;
        private const string _outputPath = "StrykerOutput";

        public DiskBaselineProvider(IStrykerOptions options, IFileSystem fileSystem = null)
        {
            _options = options;
            _fileSystem = fileSystem ?? new FileSystem();
            _logger = ApplicationLogging.LoggerFactory.CreateLogger<DiskBaselineProvider>();
        }


        public async Task<IJsonReport> Load(string version)
        {
            var reportPath = FilePathUtils.NormalizePathSeparators(
                Path.Combine(_options.ProjectPath, _outputPath, version, "stryker-report.json"));

            if (_fileSystem.File.Exists(reportPath))
            {
                await using var reportStream = _fileSystem.File.OpenRead(reportPath);

                return await reportStream.DeserializeJsonReportAsync();
            }

            _logger.LogDebug("No baseline was found at {ReportPath}", reportPath);
            return null;
        }

        public async Task Save(IJsonReport report, string version)
        {
            var reportDirectory = FilePathUtils.NormalizePathSeparators(
                Path.Combine(_options.ProjectPath, _outputPath, version));

            _fileSystem.Directory.CreateDirectory(reportDirectory);

            var reportPath = Path.Combine(reportDirectory, "stryker-report.json");
            await using var reportStream = _fileSystem.File.Create(reportPath);
            await report.SerializeAsync(reportStream);

            _logger.LogDebug("Baseline report has been saved to {ReportPath}", reportPath);
        }
    }
}
