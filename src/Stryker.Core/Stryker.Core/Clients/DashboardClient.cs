using Microsoft.Extensions.Logging;
using Stryker.Core.Logging;
using Stryker.Core.Options;
using Stryker.Core.Reporters.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Stryker.Core.Reporters.Json.SourceFiles;

namespace Stryker.Core.Clients
{
    public interface IDashboardClient
    {
        Task<string> PublishReport(JsonReport report, string version, bool realTime = false);
        Task<JsonReport> PullReport(string version);
        Task PublishMutantBatch(JsonMutant mutant);
        Task PublishFinished();
    }

    public class DashboardClient : IDashboardClient
    {
        private const int MutantBatchSize = 10;

        private readonly StrykerOptions _options;
        private readonly ILogger<DashboardClient> _logger;
        private readonly HttpClient _httpClient;
        private readonly List<JsonMutant> _batch = new();

        public DashboardClient(StrykerOptions options, HttpClient httpClient = null, ILogger<DashboardClient> logger = null)
        {
            _options = options;
            _logger = logger ?? ApplicationLogging.LoggerFactory.CreateLogger<DashboardClient>();
            if (httpClient != null)
            {
                _httpClient = httpClient;
            }
            else
            {
                _httpClient = new HttpClient();
                _httpClient.DefaultRequestHeaders.Add("X-Api-Key", _options.DashboardApiKey);
            }
        }

        public async Task<string> PublishReport(JsonReport report, string version, bool realTime = false)
        {
            var url = GetUrl(version, realTime);

            _logger.LogDebug("Sending PUT to {DashboardUrl}", url);

            try
            {
                using var response = await _httpClient.PutAsJsonAsync(url, report, JsonReportSerialization.Options);
                response.EnsureSuccessStatusCode();

                var result = await response.Content.ReadFromJsonAsync<DashboardResult>();
                return result?.Href;
            }
            catch(Exception exception)
            {
                _logger.LogError(exception, "Failed to upload report to the dashboard at {DashboardUrl}", url);
                return null;
            }
        }

        public async Task PublishMutantBatch(JsonMutant mutant)
        {
            _batch.Add(mutant);
            if (_batch.Count != MutantBatchSize)
            {
                return;
            }

            var url = GetUrl(_options.ProjectVersion, true);
            try
            {
                var response = await _httpClient.PostAsJsonAsync(url, _batch, JsonReportSerialization.Options);
                response.EnsureSuccessStatusCode();
                _batch.Clear();
            }
            catch(Exception exception)
            {
                _logger.LogError(exception, "Failed to upload mutant to the dashboard at {DashboardUrl}", url);
            }
        }

        public async Task PublishFinished()
        {
            var url = GetUrl(_options.ProjectVersion, true);

            try
            {
                if (_batch.Count != 0)
                {
                    var batchResponse = await _httpClient.PostAsJsonAsync(url, _batch, JsonReportSerialization.Options);
                    batchResponse.EnsureSuccessStatusCode();
                    _batch.Clear();
                }

                var deleteResponse = await _httpClient.DeleteAsync(url);
                deleteResponse.EnsureSuccessStatusCode();
            }
            catch(Exception exception)
            {
                _logger.LogError(exception, "Failed send finished event to the dashboard at {DashboardUrl}", url);
            }
        }

        public async Task<JsonReport> PullReport(string version)
        {
            var url = GetUrl(version, false);

            _logger.LogDebug("Sending GET to {DashboardUrl}", url);
            try
            {
                var report = await _httpClient.GetFromJsonAsync<JsonReport>(url, JsonReportSerialization.Options);
                return report;
            }
            catch (Exception exception)
            {
                _logger.LogError(exception, "Failed to retrieve the report at {DashboardUrl}", url);
                return null;
            }
        }

        private Uri GetUrl(string version, bool realTime)
        {
            var module = !string.IsNullOrEmpty(_options.ModuleName) ? $"?module={_options.ModuleName}" : "";
            var url = new Uri($"{_options.DashboardUrl}/api/reports/{_options.ProjectName}/{version}{module}");
            if (realTime)
            {
                url = new Uri($"{_options.DashboardUrl}/api/real-time/{_options.ProjectName}/{version}{module}");
            }

            return url;
        }

        private sealed class DashboardResult
        {
            public string Href { get; init; } //NOSONAR: init accessor is used for json serialization
        }
    }
}
