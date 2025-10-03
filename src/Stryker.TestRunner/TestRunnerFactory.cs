using System;
using System.IO.Abstractions;
using Stryker.Abstractions.Options;
using Stryker.Abstractions.Testing;

namespace Stryker.TestRunner;

/// <summary>
/// Factory for creating test runners based on configuration
/// </summary>
public static class TestRunnerFactory
{
    /// <summary>
    /// Creates a test runner instance based on the provided options
    /// </summary>
    /// <param name="options">Stryker options containing test runner configuration</param>
    /// <param name="fileSystem">Optional file system abstraction for testing</param>
    /// <returns>An ITestRunner instance</returns>
    public static ITestRunner Create(IStrykerOptions options, IFileSystem fileSystem = null)
    {
        return options.TestRunner switch
        {
            Abstractions.Testing.TestRunner.VsTest => CreateVsTestRunner(options, fileSystem),
            Abstractions.Testing.TestRunner.MicrosoftTestingPlatform => CreateMtpRunner(options),
            _ => throw new ArgumentException($"Unknown test runner type: {options.TestRunner}", nameof(options))
        };
    }

    private static ITestRunner CreateVsTestRunner(IStrykerOptions options, IFileSystem fileSystem)
    {
        // VsTestRunnerPool is in a different project, so we use reflection to create it
        var vsTestRunnerPoolType = Type.GetType("Stryker.TestRunner.VsTest.VsTestRunnerPool, Stryker.TestRunner.VsTest");
        
        if (vsTestRunnerPoolType == null)
        {
            throw new InvalidOperationException("VsTest runner not found. Ensure Stryker.TestRunner.VsTest is referenced.");
        }

        // Create instance using constructor: VsTestRunnerPool(IStrykerOptions options, IFileSystem fileSystem = null)
        return (ITestRunner)Activator.CreateInstance(vsTestRunnerPoolType, options, fileSystem);
    }

    private static ITestRunner CreateMtpRunner(IStrykerOptions options)
    {
        // MTP runner is in a different project, so we use reflection to create it
        var mtpRunnerType = Type.GetType("Stryker.TestRunner.MTP.MicrosoftTestingPlatformRunner, Stryker.TestRunner.MTP");
        
        if (mtpRunnerType == null)
        {
            throw new InvalidOperationException("Microsoft Testing Platform runner not found. Ensure Stryker.TestRunner.MTP is referenced.");
        }

        // Create instance using constructor: MicrosoftTestingPlatformRunner(IStrykerOptions options)
        return (ITestRunner)Activator.CreateInstance(mtpRunnerType, options);
    }
}
