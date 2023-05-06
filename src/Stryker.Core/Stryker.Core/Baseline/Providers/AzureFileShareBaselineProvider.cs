using Microsoft.Extensions.Logging;
using Stryker.Core.Logging;
using Stryker.Core.Options;
using Stryker.Core.Reporters.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace Stryker.Core.Baseline.Providers;

public class AzureFileShareBaselineProvider : IBaselineProvider
{
    private const string DefaultOutputDirectoryName = "StrykerOutput";

    private readonly StrykerOptions _options;
    private readonly HttpClient _httpClient;
    private readonly ILogger<AzureFileShareBaselineProvider> _logger;
    private readonly string _outputPath;

    public AzureFileShareBaselineProvider(StrykerOptions options, HttpClient httpClient = null)
    {
        _options = options;
        _httpClient = httpClient ?? new HttpClient();
        _logger = ApplicationLogging.LoggerFactory.CreateLogger<AzureFileShareBaselineProvider>();

        _outputPath = string.IsNullOrWhiteSpace(options.ProjectName) ? DefaultOutputDirectoryName : $"{DefaultOutputDirectoryName}/{options.ProjectName}";
    }

    public async Task<JsonReport> Load(string version)
    {
        var fileUrl = $"{_options.AzureFileStorageUrl}/{_outputPath}/{version}/stryker-report.json";

        var url = GenerateUri(fileUrl, _options.AzureFileStorageSas).ToString();

        using var requestMessage = new HttpRequestMessage(HttpMethod.Get, url);

        requestMessage.Headers.Add("x-ms-date", "now");

        using var response = await _httpClient.SendAsync(requestMessage);

        if (response.StatusCode == HttpStatusCode.OK)
        {
            var stream = await response.Content.ReadAsStreamAsync();

            return await stream.DeserializeJsonReportAsync();
        }

        _logger.LogDebug("No baseline was found at {0}", fileUrl);
        return null;
    }

    public async Task Save(JsonReport report, string version)
    {
        var existingReport = await Load(version);

        var reportJson = report.ToJson();

        int byteSize = Encoding.UTF8.GetByteCount(report.ToJson());

        var fileUrl = $"{_options.AzureFileStorageUrl}/{_outputPath}/{version}/stryker-report.json";

        var url = GenerateUri(fileUrl, _options.AzureFileStorageSas, new Dictionary<string, string> { { "comp", "range" } }).Uri;

        if (existingReport == null)
        {
            var successfullyCreatedDirectories = await CreateDirectoriesAsync(fileUrl);

            if (!successfullyCreatedDirectories)
            {
                return;
            }
        }

        // If the file already exists, this replaces the existing file. Does not add content to the file
        var successfullyAllocated = await AllocateFileLocationAsync(byteSize, fileUrl);

        if (!successfullyAllocated)
        {
            return;
        }

        // This adds the content to the file
        await UploadFileAsync(reportJson, byteSize, url);

        _logger.LogDebug("Baseline report has been saved to {0}", fileUrl);
    }

    private async Task<bool> CreateDirectoriesAsync(string fileUrl)
    {
        _logger.LogDebug("Creating directories for file {0}", fileUrl);

        var uriParts = fileUrl.Split(_outputPath);
        var currentDirectory = new StringBuilder(uriParts[0]);

        var storagePathSegments = _outputPath.Split('/').Concat(uriParts[1].Split('/')).Where(s => !string.IsNullOrEmpty(s) && !s.Contains(".json"));

        foreach (var segment in storagePathSegments)
        {
            currentDirectory.Append($"{segment}/");

            if (!await CreateDirectoryAsync(currentDirectory.ToString()))
            {
                return false;
            }
        }

        return true;
    }

    private async Task<bool> CreateDirectoryAsync(string fileUrl)
    {
        _logger.LogDebug("Creating directory {0}", fileUrl);

        var url = GenerateUri(fileUrl, _options.AzureFileStorageSas, new Dictionary<string, string> { { "restype", "directory" } }).ToString();

        using var requestMessage = new HttpRequestMessage(HttpMethod.Put, url);

        using var response = await _httpClient.SendAsync(requestMessage);
        if (response.StatusCode == HttpStatusCode.Created)
        {
            _logger.LogDebug("Successfully created directory {0}", fileUrl);
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

    private async Task<bool> AllocateFileLocationAsync(int byteSize, string fileUrl)
    {
        _logger.LogDebug("Allocating storage for file {0}", fileUrl);
        var url = GenerateUri(fileUrl, _options.AzureFileStorageSas).ToString();
        
        using var requestMessage = new HttpRequestMessage(HttpMethod.Put, url);

        requestMessage.Headers.Add("x-ms-type", "file");
        requestMessage.Headers.Add("x-ms-content-length", byteSize.ToString());


        using var response = await _httpClient.SendAsync(requestMessage);

        if (response.StatusCode == HttpStatusCode.Created)
        {
            _logger.LogDebug("Successfully allocated storage");
            return true;
        }
        else
        {
            _logger.LogError("Azure File Storage allocation failed with statuscode {0} and message: {1}", response.StatusCode.ToString(), ToSafeResponseMessage(await response.Content.ReadAsStringAsync()));
            return false;
        }
    }

    private async Task UploadFileAsync(string report, int byteSize, Uri uploadUri)
    {
        _logger.LogDebug("Uploading file to azure file storage");

        using var requestMessage = new HttpRequestMessage(HttpMethod.Put, uploadUri)
        {
            Content = new StringContent(report, Encoding.UTF8, "application/json")
        };

        requestMessage.Headers.Add("x-ms-range", $"bytes=0-{byteSize - 1}");
        requestMessage.Headers.Add("x-ms-write", "update");

        using var response = await _httpClient.SendAsync(requestMessage);

        if (response.StatusCode != HttpStatusCode.Created)
        {
            _logger.LogError("Azure File Storage upload failed with statuscode {0} and message: {1}", response.StatusCode.ToString(), ToSafeResponseMessage(await response.Content.ReadAsStringAsync()));
        }
        else
        {
            _logger.LogDebug("Uploaded report to azure file share");
        }
    }

    private string ToSafeResponseMessage(string responseMessage) =>
        responseMessage.Replace(_options.AzureFileStorageUrl, "xxxxxxxxxx").Replace(_options.AzureFileStorageSas, "xxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx");

    private static UriBuilder GenerateUri(string fileUrl, string sasToken, Dictionary<string, string> queryParameters = default)
    {
        var uriBuilder = new UriBuilder(fileUrl);
        var query = HttpUtility.ParseQueryString(sasToken);
        if (queryParameters != default)
        {
            foreach (var para in queryParameters)
            {
                query.Add(para.Key, para.Value);
            }
        }

        uriBuilder.Query = query.ToString();
        return uriBuilder;
    }
}
