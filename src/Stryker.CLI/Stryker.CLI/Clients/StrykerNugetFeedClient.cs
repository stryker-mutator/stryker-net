using System;
using System.Linq;
using NuGet.Versioning;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using NuGet.Common;
using NuGet.Configuration;
using NuGet.Protocol;
using NuGet.Protocol.Core.Types;
using Stryker.CLI.Logging;

namespace Stryker.CLI.Clients;

public interface IStrykerNugetFeedClient
{
    Task<SemanticVersion> GetLatestVersionAsync();
    Task<SemanticVersion> GetPreviewVersionAsync();
}

public class StrykerNugetFeedClient : IStrykerNugetFeedClient
{
    private readonly Lazy<NuGet.Common.ILogger> _logger;
    private readonly SourceRepository _sourceRepository;
    private readonly SourceCacheContext _sourceCacheContext;

    public StrykerNugetFeedClient()
    {
        _logger = new Lazy<NuGet.Common.ILogger>(() => new NuGetLogger(ApplicationLogging.LoggerFactory.CreateLogger("NuGet")));
        _sourceRepository = Repository.Factory.GetCoreV3(new PackageSource(NuGetConstants.V3FeedUrl));
        _sourceCacheContext = new SourceCacheContext();
    }

    public async Task<SemanticVersion> GetLatestVersionAsync() => await GetVersionAsync(prerelease: false);

    public async Task<SemanticVersion> GetPreviewVersionAsync() => await GetVersionAsync(prerelease: true);

    private async Task<SemanticVersion> GetVersionAsync(bool prerelease)
    {
        try
        {
            var metadataResource = await _sourceRepository.GetResourceAsync<MetadataResource>();
            var versions = await metadataResource.GetVersions("dotnet-stryker", includePrerelease: true, includeUnlisted: false, _sourceCacheContext, _logger.Value, CancellationToken.None);
            return versions.OrderBy(x => x).Last(x => prerelease ? x.IsPrerelease : !x.IsPrerelease);
        }
        catch
        {
            return new SemanticVersion(0, 0, 0);
        }
    }

    private class NuGetLogger : LoggerBase
    {
        private readonly Microsoft.Extensions.Logging.ILogger _logger;

        public NuGetLogger(Microsoft.Extensions.Logging.ILogger logger) => _logger = logger;

        public override void Log(ILogMessage message) => _logger.LogTrace("{Message}", message.Message);

        public override Task LogAsync(ILogMessage message)
        {
            Log(message);
            return Task.CompletedTask;
        }
    }
}
