namespace Stryker.Abstractions.Testing;

/// <summary>
/// Specifies the test runner to use for running tests
/// </summary>
public enum TestRunner
{
    /// <summary>
    /// Use Visual Studio Test Platform (VSTest). This is the default and currently the only fully supported option.
    /// </summary>
    VsTest = 0,

    /// <summary>
    /// Use Microsoft Testing Platform (MTP). This is currently under development and not yet fully supported.
    /// When specified, Stryker will attempt to use the new Microsoft Testing Platform for test execution.
    /// Note: This option is experimental and may not work with all test frameworks or scenarios.
    /// </summary>
    MicrosoftTestingPlatform = 1
}
