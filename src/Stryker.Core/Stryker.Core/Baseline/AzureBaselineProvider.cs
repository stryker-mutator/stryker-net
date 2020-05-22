using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Stryker.Core.Logging;
using Stryker.Core.Options;
using Stryker.Core.Reporters.Json;
using System;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Stryker.Core.Baseline
{
    public class AzureBaselineProvider : IBaselineProvider
    {
        private readonly StrykerOptions _options;
        private readonly HttpClient _httpClient;
        private readonly ILogger<AzureBaselineProvider> _logger;
        public AzureBaselineProvider(StrykerOptions options, HttpClient httpClient = null)
        {
            _options = options;
            _httpClient = httpClient ?? new HttpClient();
            _logger = ApplicationLogging.LoggerFactory.CreateLogger<AzureBaselineProvider>();
        }

        public async Task<JsonReport> Load(string version)
        {
            var subdir = $"strykerOutput/baselines/{version}";
            var filename = "stryker-report.json";
            var url = new Uri($"https://{_options.AzureStorageName}.file.core.windows.net/{_options.AzureFileShare}/{subdir}/{filename}?sv={_options.AzureSAS}");

            var requestMessage = new HttpRequestMessage(HttpMethod.Get, url);

            requestMessage.Headers.Add("x-ms-date", "now");

            var response = await _httpClient.SendAsync(requestMessage);

            var content = await response.Content.ReadAsStringAsync();

            return JsonConvert.DeserializeObject<JsonReport>(content);
        }

        public async Task Save(JsonReport report, string version)
        {
            var reportJson = report.ToJson();
            var byteSize = Encoding.UTF8.GetByteCount(report.ToJson());

            var successfullyAllocated = await AllocateFileLocation(byteSize, version);

            if (successfullyAllocated)
            {
                await UploadFile(reportJson, byteSize, version);
            }
        }

        private async Task<bool> AllocateFileLocation(int byteSize, string version)
        {
            var subdir = $"strykerOutput/baselines/{version}";
            var filename = "stryker-report.json";
            var url = new Uri($"https://{_options.AzureStorageName}.file.core.windows.net/{_options.AzureFileShare}/{subdir}/{filename}?sv={_options.AzureSAS}");

            var requestMessage = new HttpRequestMessage(HttpMethod.Put, url);

            requestMessage.Headers.Add("x-ms-type", "file");
            requestMessage.Headers.Add("x-ms-content-length", byteSize.ToString());
            requestMessage.Headers.Add("x-ms-file-permission", "inherit");
            requestMessage.Headers.Add("x-ms-file-attributes", "None");
            requestMessage.Headers.Add("x-ms-file-creation-time", "now");
            requestMessage.Headers.Add("x-ms-file-last-write-time", "now");



            var response = await _httpClient.SendAsync(requestMessage);

            if (response.StatusCode == HttpStatusCode.Created)
            {
                _logger.LogDebug("Succesfully allocated storage on fileshare {0}", _options.AzureFileShare);
                return true;
            }
            else
            {
                _logger.LogError("Azure File Storage upload failed with statuscode {0} and message: {1}", response.StatusCode.ToString(), await response.Content.ReadAsStringAsync());
                return false;
            }
        }

        private async Task UploadFile(string report, int byteSize, string version)
        {
            var subdir = $"strykerOutput/baselines/{version}";
            var filename = "stryker-report.json";

            var url = new Uri($"https://{_options.AzureStorageName}.file.core.windows.net/{_options.AzureFileShare}/{subdir}/{filename}?comp=range&sv={_options.AzureSAS}");

            var requestMessage = new HttpRequestMessage(HttpMethod.Put, url)
            {
                Content = new StringContent(report, Encoding.UTF8, "application/json")
            };

            requestMessage.Headers.Add("x-ms-write", "update");
            requestMessage.Headers.Add("x-ms-range", $"bytes=0-{byteSize - 1}");
            requestMessage.Headers.Add("x-ms-file-permission", "inherit");
            requestMessage.Headers.Add("x-ms-file-attributes", "None");
            requestMessage.Headers.Add("x-ms-file-last-write-time", "now");

            var response = await _httpClient.SendAsync(requestMessage);

            if (response.StatusCode != HttpStatusCode.Created)
            {
                _logger.LogError("Azure File Storage upload failed with statuscode {0} and message: {1}", response.StatusCode.ToString(), await response.Content.ReadAsStringAsync());
            }
        }
    }
}
