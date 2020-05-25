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
            var url = new Uri($"{_options.AzureFileStorageUrl}/{subdir}/{filename}?sv={_options.AzureSAS}");

            var requestMessage = new HttpRequestMessage(HttpMethod.Get, url);

            requestMessage.Headers.Add("x-ms-date", "now");

            var response = await _httpClient.SendAsync(requestMessage);

            if (response.StatusCode == HttpStatusCode.OK)
            {
                var content = await response.Content.ReadAsStringAsync();

                return JsonConvert.DeserializeObject<JsonReport>(content);
            }
            return null;
        }

        public async Task Save(JsonReport report, string version)
        {
            var existingReport = await Load(version);

            var reportJson = report.ToJson();

            int byteSize;
            if (existingReport == null)
            {
                byteSize = Encoding.UTF8.GetByteCount(report.ToJson());

                var succesfullyCreatedDirectory = await CreateDirectory(version);

                if (!succesfullyCreatedDirectory)
                {
                    return;
                }

                var successfullyAllocated = await AllocateFileLocation(byteSize, version);

                if (!successfullyAllocated)
                {
                    return;
                }
            }
            else
            {
                byteSize = Encoding.UTF8.GetByteCount(existingReport.ToJson());
            }


            await UploadFile(reportJson, byteSize, version);
        }

        public async Task<bool> CreateDirectory(string version)
        {
            var subdir = $"strykerOutput/baselines/{version}";
            _logger.LogInformation("Creating directory {0}", subdir);

            var url = new Uri($"{_options.AzureFileStorageUrl}/{subdir}?restype=directory&sv={_options.AzureSAS}");

            var requestMessage = new HttpRequestMessage(HttpMethod.Put, url);

            SetWritingHeaders(requestMessage);

            var response = await _httpClient.SendAsync(requestMessage);

            if (response.StatusCode == HttpStatusCode.Created)
            {
                _logger.LogDebug("Succesfully created directory {0} on fileshare {1}", subdir, _options.AzureFileStorageUrl);
                return true;
            }
            else if (response.StatusCode == HttpStatusCode.Conflict)
            {
                _logger.LogDebug("Directory {0} already excists on fileshare {1}", subdir, _options.AzureFileStorageUrl);
                return true;
            }

            _logger.LogError("Creating directory failed with status {0} and message {1}", response.StatusCode.ToString(), response.RequestMessage);
            return false;

        }

        private async Task<bool> AllocateFileLocation(int byteSize, string version)
        {
            _logger.LogInformation("Allocating storage for file");

            var subdir = $"strykerOutput/baselines/{version}";
            var filename = "stryker-report.json";
            var url = new Uri($"{_options.AzureFileStorageUrl}/{subdir}/{filename}?sv={_options.AzureSAS}");

            var requestMessage = new HttpRequestMessage(HttpMethod.Put, url);

            SetWritingHeaders(requestMessage);

            requestMessage.Headers.Add("x-ms-type", "file");
            requestMessage.Headers.Add("x-ms-content-length", byteSize.ToString());


            var response = await _httpClient.SendAsync(requestMessage);

            if (response.StatusCode == HttpStatusCode.Created)
            {
                _logger.LogDebug("Succesfully allocated storage on fileshare {0}", _options.AzureFileStorageUrl);
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
            _logger.LogInformation("Uploading file to azure file storage");

            var subdir = $"strykerOutput/baselines/{version}";
            var filename = "stryker-report.json";

            var url = new Uri($"{_options.AzureFileStorageUrl}/{subdir}/{filename}?comp=range&sv={_options.AzureSAS}");

            var requestMessage = new HttpRequestMessage(HttpMethod.Put, url)
            {
                Content = new StringContent(report, Encoding.UTF8, "application/json")
            };

            SetWritingHeaders(requestMessage);

            requestMessage.Headers.Add("x-ms-range", $"bytes=0-{byteSize - 1}");
            requestMessage.Headers.Add("x-ms-write", "update");

            var response = await _httpClient.SendAsync(requestMessage);

            if (response.StatusCode != HttpStatusCode.Created)
            {
                _logger.LogError("Azure File Storage upload failed with statuscode {0} and message: {1}", response.StatusCode.ToString(), await response.Content.ReadAsStringAsync());
            }
            else
            {
                _logger.LogInformation("Report uploaded");
            }
        }

        private void SetWritingHeaders(HttpRequestMessage requestMessage)
        {
            requestMessage.Headers.Add("x-ms-file-permission", "inherit");
            requestMessage.Headers.Add("x-ms-file-attributes", "None");
            requestMessage.Headers.Add("x-ms-file-creation-time", "now");
            requestMessage.Headers.Add("x-ms-file-last-write-time", "now");
        }
    }
}
