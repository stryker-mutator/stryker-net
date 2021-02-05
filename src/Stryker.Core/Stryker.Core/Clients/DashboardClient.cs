using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Stryker.Core.Logging;
using Stryker.Core.Options;
using Stryker.Core.Reporters.Json;
using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Stryker.Core.Clients
{
    public interface IDashboardClient
    {
        Task<string> PublishReport(string json, string version);
        Task<JsonReport> PullReport(string version);
    }

    public class DashboardClient : IDashboardClient
    {
        private readonly IStrykerOptions _options;
        private readonly ILogger<DashboardClient> _logger;
        private readonly HttpClient _httpClient;

        public DashboardClient(IStrykerOptions options, HttpClient httpClient = null, ILogger<DashboardClient> logger = null)
        {
            _options = options;
            _logger = logger ?? ApplicationLogging.LoggerFactory.CreateLogger<DashboardClient>();
            _httpClient = httpClient ?? new HttpClient();
        }

        public async Task<string> PublishReport(string json, string version)
        {
            var url = new Uri($"{_options.DashboardUrl}/api/reports/{_options.ProjectName}/{version}");

            if (_options.ModuleName != null)
            {
                url = new Uri(url, $"?module={_options.ModuleName}");
            }


            var requestMessage = new HttpRequestMessage(HttpMethod.Put, url)
            {
                Content = new StringContent(json, Encoding.UTF8, "application/json")
            };

            requestMessage.Headers.Add("X-Api-Key", _options.DashboardApiKey);

            _logger.LogDebug("Sending POST to {0}", url);
            var response = await _httpClient.SendAsync(requestMessage);

            if (response.IsSuccessStatusCode)
            {
                var jsonResponse = await response.Content.ReadAsStringAsync();

                return JsonConvert.DeserializeAnonymousType(jsonResponse, new { Href = "" }).Href;
            }
            else
            {
                _logger.LogError("Dashboard upload failed with statuscode {0} and message: {1}", response.StatusCode.ToString(), await response.Content.ReadAsStringAsync());
                return null;
            }

        }

        public async Task<JsonReport> PullReport(string version)
        {
            var url = new Uri($"{_options.DashboardUrl}/api/reports/{_options.ProjectName}/{version}");

            if (_options.ModuleName != null)
            {
                url = new Uri(url, $"?module={_options.ModuleName}");
            }

            var requestMessage = new HttpRequestMessage(HttpMethod.Get, url);

            requestMessage.Headers.Add("X-Api-Key", _options.DashboardApiKey);

            _logger.LogDebug("Sending GET to {0}", url);
            var response = await _httpClient.SendAsync(requestMessage);

            if (response.IsSuccessStatusCode)
            {
                var jsonResponse = await response.Content.ReadAsStringAsync();

                return JsonConvert.DeserializeObject<JsonReport>(jsonResponse);
            }
            else
            {
                return null;
            }
        }
    }
}
