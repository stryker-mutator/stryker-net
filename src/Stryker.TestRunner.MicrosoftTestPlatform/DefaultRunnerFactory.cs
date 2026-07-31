using Microsoft.Extensions.Logging;
using Stryker.Abstractions.Options;
using Stryker.TestRunner.MicrosoftTestPlatform.Models;
using Stryker.TestRunner.Tests;

namespace Stryker.TestRunner.MicrosoftTestPlatform;

/// <summary>
/// A factory to create SingleMicrosoftTestPlatformRunner instances. Useful for dependency injection and mocking in tests.
/// </summary>
public interface ISingleRunnerFactory
{
    SingleMicrosoftTestPlatformRunner CreateRunner(
        int id,
        Dictionary<string, List<TestNode>> testsByAssembly,
        Dictionary<string, MtpTestDescription> testDescriptions,
        TestSet testSet,
        object discoveryLock,
        ILogger logger,
        IStrykerOptions? options = null);
}

/// <summary>
/// The default implementation of ISingleRunnerFactory that creates SingleMicrosoftTestPlatformRunner instances.
/// </summary>
public class DefaultRunnerFactory : ISingleRunnerFactory
{
    public SingleMicrosoftTestPlatformRunner CreateRunner(
        int id,
        Dictionary<string, List<TestNode>> testsByAssembly,
        Dictionary<string, MtpTestDescription> testDescriptions,
        TestSet testSet,
        object discoveryLock,
        ILogger logger,
        IStrykerOptions? options = null) =>
        new(id, testsByAssembly, testDescriptions, testSet, discoveryLock, logger, options);
}

