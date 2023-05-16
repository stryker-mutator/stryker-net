using Microsoft.Extensions.Logging;
using Stryker.Core.Logging;
using Stryker.Core.Options;
using Stryker.Core.Reporters.Json;
using System;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace Stryker.Core.Clients
{
    public interface IDashboardClient
    {
        Task<string> PublishReport(JsonReport report, string version);
        Task<JsonReport> PullReport(string version);
    }

    public class DashboardClient : IDashboardClient
    {
        private readonly StrykerOptions _options;
        private readonly ILogger<DashboardClient> _logger;
        private readonly HttpClient _httpClient;

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

        public async Task<string> PublishReport(JsonReport report, string version)
        {
            var url = GetUrl(version);

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

        public async Task<JsonReport> PullReport(string version)
        {
            var url = GetUrl(version);

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

        private Uri GetUrl(string version)
        {
            var url = new Uri($"{_options.DashboardUrl}/api/reports/{_options.ProjectName}/{version}");

            if (_options.ModuleName != null)
            {
                url = new Uri(url, $"?module={_options.ModuleName}");
            }

            return url;
        }

        private sealed class DashboardResult
        {
            public string Href { get; init; } //NOSONAR: init accessor is used for json serialization
        }
    }
}
