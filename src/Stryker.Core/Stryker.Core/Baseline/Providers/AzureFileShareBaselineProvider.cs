using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Stryker.Core.Logging;
using Stryker.Core.Options;
using Stryker.Core.Reporters.Json;
using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Stryker.Core.Baseline.Providers
{
    public class AzureFileShareBaselineProvider : IBaselineProvider
    {
        private readonly IStrykerOptions _options;
        private readonly HttpClient _httpClient;
        private readonly ILogger<AzureFileShareBaselineProvider> _logger;
        private const string _outputPath = "StrykerOutput/Baselines";

        public AzureFileShareBaselineProvider(IStrykerOptions options, HttpClient httpClient = null)
        {
            _options = options;
            _httpClient = httpClient ?? new HttpClient();
            _logger = ApplicationLogging.LoggerFactory.CreateLogger<AzureFileShareBaselineProvider>();
        }

        public async Task<JsonReport> Load(string version)
        {
            var fileUrl = $"{_options.AzureFileStorageUrl}/{_outputPath}/{version}/stryker-report.json";
            var url = new Uri($"{fileUrl}?sv={_options.AzureSAS}");

            var requestMessage = new HttpRequestMessage(HttpMethod.Get, url);

            requestMessage.Headers.Add("x-ms-date", "now");

            var response = await _httpClient.SendAsync(requestMessage);

            if (response.StatusCode == HttpStatusCode.OK)
            {
                var content = await response.Content.ReadAsStringAsync();

                return JsonConvert.DeserializeObject<JsonReport>(content);
            }

            _logger.LogDebug("No baseline was found at {0}", fileUrl);
            return null;
        }

        public async Task Save(JsonReport report, string version)
        {
            var fileUrl = $"{_options.AzureFileStorageUrl}/{_outputPath}/{version}/stryker-report.json";
            var url = new Uri($"{fileUrl}?comp=range&sv={_options.AzureSAS}");

            var existingReport = await Load(version);

            var reportJson = report.ToJson();

            int byteSize = Encoding.UTF8.GetByteCount(report.ToJson());

            if (existingReport == null)
            {
                var succesfullyCreatedDirectories = await CreateDirectories(fileUrl);

                if (!succesfullyCreatedDirectories)
                {
                    return;
                }
            }

            // If the file already exists, this replaces the existing file. Does not add content to the file
            var successfullyAllocated = await AllocateFileLocation(byteSize, fileUrl);

            if (!successfullyAllocated)
            {
                return;
            }

            // This adds the content to the file
            await UploadFile(reportJson, byteSize, url);

            _logger.LogDebug("Baseline report has been saved to {0}", fileUrl);
        }

        private async Task<bool> CreateDirectories(string fileUrl)
        {
            _logger.LogDebug("Creating directories for file {0}", fileUrl);

            var uriParts = fileUrl.Split(_outputPath);
            var currentDirectory = new StringBuilder(uriParts[0]);

            var storagePathSegments = _outputPath.Split('/').Concat(uriParts[1].Split('/')).Where(s => !string.IsNullOrEmpty(s) && !s.Contains(".json"));

            foreach (var segment in storagePathSegments)
            {
                currentDirectory.Append($"{segment}/");
                
                if (!await CreateDirectory(currentDirectory.ToString()))
                {
                    return false;
                }
            }
            
            return true;
        }

        private async Task<bool> CreateDirectory(string fileUrl)
        {
            _logger.LogDebug("Creating directory {0}", fileUrl);

            var url = new Uri($"{fileUrl}?restype=directory&sv={_options.AzureSAS}");

            var requestMessage = new HttpRequestMessage(HttpMethod.Put, url);

            SetWritingHeaders(requestMessage);

            var response = await _httpClient.SendAsync(requestMessage);

            if (response.StatusCode == HttpStatusCode.Created)
            {
                _logger.LogDebug("Succesfully created directory {0}", fileUrl);
                return true;
            }
            else if (response.StatusCode == HttpStatusCode.Conflict)
            {
                _logger.LogDebug("Directory {0} already exists", fileUrl);
                return true;
            }

            _logger.LogError("Creating directory failed with status {0} and message {1}", response.StatusCode.ToString(), ToSafeResponseMessage(await response.Content.ReadAsStringAsync()));
            return false;
        }

        private async Task<bool> AllocateFileLocation(int byteSize, string fileUrl)
        {
            _logger.LogDebug("Allocating storage for file {0}", fileUrl);

            var url = new Uri($"{fileUrl}?sv={_options.AzureSAS}");

            var requestMessage = new HttpRequestMessage(HttpMethod.Put, url);

            SetWritingHeaders(requestMessage);

            requestMessage.Headers.Add("x-ms-type", "file");
            requestMessage.Headers.Add("x-ms-content-length", byteSize.ToString());


            var response = await _httpClient.SendAsync(requestMessage);

            if (response.StatusCode == HttpStatusCode.Created)
            {
                _logger.LogDebug("Succesfully allocated storage");
                return true;
            }
            else
            {
                _logger.LogError("Azure File Storage allocation failed with statuscode {0} and message: {1}", response.StatusCode.ToString(), ToSafeResponseMessage(await response.Content.ReadAsStringAsync()));
                return false;
            }
        }

        private async Task UploadFile(string report, int byteSize, Uri uploadUri)
        {
            _logger.LogDebug("Uploading file to azure file storage");

            var requestMessage = new HttpRequestMessage(HttpMethod.Put, uploadUri)
            {
                Content = new StringContent(report, Encoding.UTF8, "application/json")
            };

            SetWritingHeaders(requestMessage);

            requestMessage.Headers.Add("x-ms-range", $"bytes=0-{byteSize - 1}");
            requestMessage.Headers.Add("x-ms-write", "update");

            var response = await _httpClient.SendAsync(requestMessage);

            if (response.StatusCode != HttpStatusCode.Created)
            {
                _logger.LogError("Azure File Storage upload failed with statuscode {0} and message: {1}", response.StatusCode.ToString(), ToSafeResponseMessage(await response.Content.ReadAsStringAsync()));
            }
            else
            {
                _logger.LogDebug("Uploaded report to azure file share");
            }
        }

        private void SetWritingHeaders(HttpRequestMessage requestMessage)
        {
            requestMessage.Headers.Add("x-ms-file-permission", "inherit");
            requestMessage.Headers.Add("x-ms-file-attributes", "None");
            requestMessage.Headers.Add("x-ms-file-creation-time", "now");
            requestMessage.Headers.Add("x-ms-file-last-write-time", "now");
        }

        private string ToSafeResponseMessage(string responseMessage)
        {
            return responseMessage.Replace(_options.AzureFileStorageUrl, "xxxxxxxxxx").Replace(_options.AzureSAS, "xxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx");
        }
    }
}
