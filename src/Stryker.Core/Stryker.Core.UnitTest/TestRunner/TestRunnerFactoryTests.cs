using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Shouldly;
using Stryker.Abstractions;
using Stryker.Abstractions.Options;
using Stryker.Abstractions.Testing;
using Stryker.TestRunner;

namespace Stryker.Core.UnitTest.TestRunner;

[TestClass]
public class TestRunnerFactoryTests : TestBase
{
    [TestMethod]
    public void ShouldCreateVsTestRunnerByDefault()
    {
        var options = new StrykerOptions
        {
            ProjectPath = "/test",
            TestRunner = Abstractions.Testing.TestRunner.VsTest
        };

        var runner = TestRunnerFactory.Create(options);

        runner.ShouldNotBeNull();
        runner.GetType().Name.ShouldBe("VsTestRunnerPool");
    }

    [TestMethod]
    public void ShouldThrowExceptionForMtpRunner()
    {
        var options = new StrykerOptions
        {
            ProjectPath = "/test",
            TestRunner = Abstractions.Testing.TestRunner.MicrosoftTestingPlatform
        };

        var runner = TestRunnerFactory.Create(options);

        runner.ShouldNotBeNull();
        runner.GetType().Name.ShouldBe("MicrosoftTestingPlatformRunner");

        // Verify that calling a method throws NotImplementedException
        Should.Throw<NotImplementedException>(() => runner.DiscoverTests("test.dll"));
    }

    [TestMethod]
    public void ShouldDefaultToVsTestWhenOptionNotSpecified()
    {
        // When TestRunner is not explicitly set, it should default to VsTest
        var options = new StrykerOptions
        {
            ProjectPath = "/test"
            // TestRunner not set, should use default
        };

        var runner = TestRunnerFactory.Create(options);

        runner.ShouldNotBeNull();
        runner.GetType().Name.ShouldBe("VsTestRunnerPool");
    }
}
