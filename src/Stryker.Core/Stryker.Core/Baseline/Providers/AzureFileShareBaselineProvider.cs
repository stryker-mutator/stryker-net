using System;
using System.IO;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Azure;
using Azure.Storage.Files.Shares;
using Microsoft.Extensions.Logging;
using Stryker.Core.Logging;
using Stryker.Core.Options;
using Stryker.Core.Reporters.Json;

namespace Stryker.Core.Baseline.Providers
{
    public class AzureFileShareBaselineProvider : IBaselineProvider
    {
        private const string DefaultOutputDirectoryName = "StrykerOutput";
        private const string StrykerReportName = "stryker-report.json";

        private readonly ShareClient _fileShareClient;
        private readonly ILogger<AzureFileShareBaselineProvider> _logger;
        private readonly string _outputPath;

        public AzureFileShareBaselineProvider(StrykerOptions options, ShareClient shareClient = null)
        {
            _logger = ApplicationLogging.LoggerFactory.CreateLogger<AzureFileShareBaselineProvider>();

            _outputPath = string.IsNullOrWhiteSpace(options.ProjectName) ? DefaultOutputDirectoryName : $"{DefaultOutputDirectoryName}/{options.ProjectName}";
            _fileShareClient = shareClient ?? new ShareClient(new Uri(options.AzureFileStorageUrl), new AzureSasCredential(options.AzureFileStorageSas));
        }

        public async Task<JsonReport> Load(string version)
        {
            var (uri, root, subdirectoryNames, fileName) = BuildFileUriComponents(version);
            var directoryClient = _fileShareClient.GetDirectoryClient(root);

            if (TryEnumerateDirectories(directoryClient, subdirectoryNames, createIfNotExists: false, out directoryClient))
            {
                var fileClient = directoryClient.GetFileClient(fileName);
                if (await fileClient.ExistsAsync())
                {
                    return await fileClient.Download().Value.Content.DeserializeJsonReportAsync();
                }
            }

            _logger.LogDebug("No baseline was found at {reportUri}", uri);
            return null;
        }

        public async Task Save(JsonReport report, string version)
        {
            var (uri, root, subdirectoryNames, fileName) = BuildFileUriComponents(version);
            var directoryClient = _fileShareClient.GetDirectoryClient(root);

            if (TryEnumerateDirectories(directoryClient, subdirectoryNames, createIfNotExists: true, out directoryClient))
            {
                var fileClient = directoryClient.GetFileClient(fileName);
                using var fileContentStream = new MemoryStream(Encoding.UTF8.GetBytes(report.ToJson()));

                if (await fileClient.CreateAsync(fileContentStream.Length) is var fileCreateResult && fileCreateResult.GetRawResponse().Status == ((int)HttpStatusCode.Created))
                {
                    _logger.LogDebug("Allocated azure file share file at {reportUri}", uri);

                    var blockSize = 1 * 4194304; // 4MB max block size
                    long offset = 0;
                    var reader = new BinaryReader(fileContentStream);

                    while (true)
                    {
                        var buffer = reader.ReadBytes(blockSize);
                        if (buffer.Length == 0)
                        {
                            break;
                        }

                        using var uploadChunk = new MemoryStream();
                        uploadChunk.Write(buffer, 0, buffer.Length);
                        uploadChunk.Position = 0;

                        var httpRange = new HttpRange(offset, buffer.Length);
                        offset += buffer.Length; // Shift the offset by number of bytes already written

                        if (await fileClient.UploadRangeAsync(httpRange, uploadChunk) is var fileChunkUploadResult && fileChunkUploadResult.GetRawResponse().Status == ((int)HttpStatusCode.Created))
                        {
                            _logger.LogDebug("Uploaded report chunk {bytesUploaded}/{bytesTotal} to azure file share", Math.Min(offset + blockSize, fileContentStream.Length), fileContentStream.Length);
                        }
                        else
                        {
                            _logger.LogError("Failed to upload report chunk {bytesUploaded}/{bytesTotal} to azure file share with statusCode {statusCode}", Math.Min(offset + blockSize, fileContentStream.Length), fileContentStream.Length, fileChunkUploadResult.GetRawResponse().Status);
                        }
                    }
                }
                else
                {
                    _logger.LogError("Failed to allocated file in azure file share at {reportUri} with statusCode {statusCode}", uri, fileCreateResult.GetRawResponse().Status);
                }
            }
        }

        private (string, string, string[], string) BuildFileUriComponents(string version)
        {
            var uri = $"{_outputPath}/{version}/{StrykerReportName}";
            var uriComponents = uri.Split('/', StringSplitOptions.RemoveEmptyEntries);
            return (uri, uriComponents[0], uriComponents[1..^1], uriComponents[^1]);
        }

        private bool TryEnumerateDirectories(ShareDirectoryClient directoryClient, string[] subdirectories, bool createIfNotExists, out ShareDirectoryClient shareDirectoryClient)
        {
            shareDirectoryClient = directoryClient;

            // root
            if (createIfNotExists)
            {
                shareDirectoryClient.CreateIfNotExists();
            }
            if (!shareDirectoryClient.Exists())
            {
                shareDirectoryClient = null;
                return false;
            }

            // subdirectories
            foreach (var subdirectory in subdirectories)
            {
                shareDirectoryClient = shareDirectoryClient.GetSubdirectoryClient(subdirectory);
                if (createIfNotExists)
                {
                    shareDirectoryClient.CreateIfNotExists();
                }
                if (!shareDirectoryClient.Exists())
                {
                    shareDirectoryClient = null;
                    return false;
                }
            }

            return true;
        }
    }
}
