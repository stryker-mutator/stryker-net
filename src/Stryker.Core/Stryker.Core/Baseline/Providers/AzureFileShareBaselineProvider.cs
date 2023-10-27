using System;
using System.IO;
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

        public AzureFileShareBaselineProvider(StrykerOptions options, ShareClient shareClient = null, ILogger<AzureFileShareBaselineProvider> logger = null)
        {
            _logger = logger ?? ApplicationLogging.LoggerFactory.CreateLogger<AzureFileShareBaselineProvider>();

            _outputPath = string.IsNullOrWhiteSpace(options.ProjectName) ? DefaultOutputDirectoryName : $"{DefaultOutputDirectoryName}/{options.ProjectName}";
            _fileShareClient = shareClient ?? new ShareClient(new Uri(options.AzureFileStorageUrl), new AzureSasCredential(options.AzureFileStorageSas));
        }

        public async Task<JsonReport> Load(string version)
        {
            var (uri, root, subdirectoryNames, fileName) = BuildFileUriComponents(version);

            if (TryGetAuthenticatedClient(root, out var directoryClient))
            {
                if (TryEnumerateDirectories(directoryClient, subdirectoryNames, createIfNotExists: false, out directoryClient))
                {
                    var fileClient = directoryClient.GetFileClient(fileName);
                    if (await fileClient.ExistsAsync())
                    {
                        return await fileClient.Download().Value.Content.DeserializeJsonReportAsync();
                    }
                }
            }
            else
            {
                _logger.LogWarning("Problem authenticating with azure file share. Make sure your SAS is valid.");
            }

            _logger.LogDebug("No baseline was found at {reportUri}", uri);
            return null;
        }

        public async Task Save(JsonReport report, string version)
        {
            var (uri, root, subdirectoryNames, fileName) = BuildFileUriComponents(version);

            if (TryGetAuthenticatedClient(root, out var directoryClient))
            {
                if (TryEnumerateDirectories(directoryClient, subdirectoryNames, createIfNotExists: true, out directoryClient))
                {
                    var fileClient = directoryClient.GetFileClient(fileName);
                    using var fileContentStream = new MemoryStream(Encoding.UTF8.GetBytes(report.ToJson()));

                    try
                    {
                        await fileClient.CreateAsync(fileContentStream.Length);
                        await UploadFileContent(uri, fileClient, fileContentStream);
                    }
                    catch (RequestFailedException ex)
                    {
                        _logger.LogError("Failed to allocated file in azure file share at {reportUri} with error code {errorCode}", uri, ex.ErrorCode);
                    }
                }
            }
            else
            {
                _logger.LogWarning("Problem authenticating with azure file share. Make sure your SAS is valid.");
            }
        }


        private (string uri, string root, string[] subdirectoryNames, string fileName) BuildFileUriComponents(string version)
        {
            var relativeUri = $"{_outputPath}/{version}/{StrykerReportName}";
            var uri = $"{_fileShareClient.Uri}/{relativeUri}";
            var uriComponents = relativeUri.Split('/', StringSplitOptions.RemoveEmptyEntries);

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

        private bool TryGetAuthenticatedClient(string root, out ShareDirectoryClient shareDirectoryClient)
        {
            try
            {
                _fileShareClient.Exists();

                shareDirectoryClient = _fileShareClient.GetDirectoryClient(root);
                return true;
            }
            catch (RequestFailedException)
            {
                shareDirectoryClient = null;
                return false;
            }

        }

        private async Task UploadFileContent(string uri, ShareFileClient fileClient, MemoryStream fileContentStream)
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

                try
                {
                    await fileClient.UploadRangeAsync(httpRange, uploadChunk);
                    _logger.LogDebug("Uploaded report chunk {bytesUploaded}/{bytesTotal} to azure file share", offset, fileContentStream.Length);
                }
                catch (RequestFailedException ex)
                {
                    _logger.LogError("Failed to upload report chunk {bytesUploaded}/{bytesTotal} to azure file share with error code {errorCode}", offset, fileContentStream.Length, ex.ErrorCode);
                }
            }
        }
    }
}
