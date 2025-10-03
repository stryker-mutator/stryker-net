using System;
using System.Collections.Generic;
using Microsoft.Extensions.Logging;
using Stryker.Abstractions;
using Stryker.Abstractions.Options;
using Stryker.Abstractions.Testing;
using Stryker.Utilities.Logging;

namespace Stryker.TestRunner.MTP;

/// <summary>
/// Microsoft Testing Platform runner implementation.
/// This is currently a placeholder/stub implementation as MTP support is under development.
/// </summary>
public class MicrosoftTestingPlatformRunner : ITestRunner
{
    private readonly ILogger _logger;
    private readonly IStrykerOptions _options;

    public MicrosoftTestingPlatformRunner(IStrykerOptions options)
    {
        _options = options ?? throw new ArgumentNullException(nameof(options));
        _logger = ApplicationLogging.LoggerFactory.CreateLogger<MicrosoftTestingPlatformRunner>();
        _logger.LogWarning("Microsoft Testing Platform runner is currently experimental and not fully supported. Falling back to VsTest is recommended.");
    }

    public bool DiscoverTests(string assembly)
    {
        _logger.LogWarning("MTP test discovery not yet implemented for assembly: {Assembly}", assembly);
        throw new NotImplementedException("Microsoft Testing Platform support is under development. Please use VsTest runner for now.");
    }

    public ITestSet GetTests(IProjectAndTests project)
    {
        _logger.LogWarning("MTP GetTests not yet implemented for project: {Project}", project);
        throw new NotImplementedException("Microsoft Testing Platform support is under development. Please use VsTest runner for now.");
    }

    public ITestRunResult InitialTest(IProjectAndTests project)
    {
        _logger.LogWarning("MTP InitialTest not yet implemented for project: {Project}", project);
        throw new NotImplementedException("Microsoft Testing Platform support is under development. Please use VsTest runner for now.");
    }

    public IEnumerable<ICoverageRunResult> CaptureCoverage(IProjectAndTests project)
    {
        _logger.LogWarning("MTP CaptureCoverage not yet implemented for project: {Project}", project);
        throw new NotImplementedException("Microsoft Testing Platform support is under development. Please use VsTest runner for now.");
    }

    public ITestRunResult TestMultipleMutants(IProjectAndTests project, ITimeoutValueCalculator timeoutCalc, IReadOnlyList<IMutant> mutants, ITestRunner.TestUpdateHandler update)
    {
        _logger.LogWarning("MTP TestMultipleMutants not yet implemented for project: {Project}", project);
        throw new NotImplementedException("Microsoft Testing Platform support is under development. Please use VsTest runner for now.");
    }

    public void Dispose()
    {
        _logger.LogDebug("Disposing MicrosoftTestingPlatformRunner");
        // No resources to dispose currently
    }
}
