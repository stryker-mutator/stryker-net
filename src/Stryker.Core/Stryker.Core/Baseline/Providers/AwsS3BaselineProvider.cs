using System;
using System.IO;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Amazon;
using Amazon.S3;
using Amazon.S3.Model;
using Microsoft.Extensions.Logging;
using Stryker.Abstractions.Baseline;
using Stryker.Abstractions.Options;
using Stryker.Abstractions.Reporting;
using Stryker.Core.Reporters.Json;
using Stryker.Utilities.Logging;

namespace Stryker.Core.Baseline.Providers;

public class AwsS3BaselineProvider : IBaselineProvider
{
    private const string DefaultOutputDirectoryName = "StrykerOutput";
    private const string StrykerReportName = "stryker-report.json";

    private readonly IAmazonS3 _s3Client;
    private readonly ILogger<AwsS3BaselineProvider> _logger;
    private readonly string _bucketName;
    private readonly string _outputPath;

    public AwsS3BaselineProvider(IStrykerOptions options, IAmazonS3 s3Client = null, ILogger<AwsS3BaselineProvider> logger = null)
    {
        _logger = logger ?? ApplicationLogging.LoggerFactory.CreateLogger<AwsS3BaselineProvider>();
        _bucketName = options.AwsS3BucketName;
        _outputPath = string.IsNullOrWhiteSpace(options.ProjectName)
            ? DefaultOutputDirectoryName
            : $"{DefaultOutputDirectoryName}/{options.ProjectName}";

        _s3Client = s3Client ?? CreateS3Client(options.AwsS3Region);
    }

    public async Task<IJsonReport> Load(string version)
    {
        var key = BuildObjectKey(version);

        try
        {
            var response = await _s3Client.GetObjectAsync(_bucketName, key);
            return await response.ResponseStream.DeserializeJsonReportAsync();
        }
        catch (AmazonS3Exception ex) when (ex.StatusCode == HttpStatusCode.NotFound)
        {
            _logger.LogDebug("No baseline was found at s3://{BucketName}/{Key}", _bucketName, key);
            return null;
        }
        catch (AmazonS3Exception ex)
        {
            _logger.LogWarning("Failed to load baseline from S3: {ErrorCode} - {Message}", ex.ErrorCode, ex.Message);
            return null;
        }
    }

    public async Task Save(IJsonReport report, string version)
    {
        var key = BuildObjectKey(version);

        try
        {
            using var stream = new MemoryStream(Encoding.UTF8.GetBytes(report.ToJson()));

            var request = new PutObjectRequest
            {
                BucketName = _bucketName,
                Key = key,
                InputStream = stream,
                ContentType = "application/json"
            };

            await _s3Client.PutObjectAsync(request);
            _logger.LogDebug("Saved baseline report to s3://{BucketName}/{Key}", _bucketName, key);
        }
        catch (AmazonS3Exception ex)
        {
            _logger.LogError("Failed to save baseline to S3: {ErrorCode} - {Message}", ex.ErrorCode, ex.Message);
        }
    }

    private string BuildObjectKey(string version) => $"{_outputPath}/{version}/{StrykerReportName}";

    private static IAmazonS3 CreateS3Client(string region)
    {
        if (!string.IsNullOrWhiteSpace(region))
        {
            return new AmazonS3Client(RegionEndpoint.GetBySystemName(region));
        }

        return new AmazonS3Client();
    }
}
