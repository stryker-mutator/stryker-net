using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using Shouldly;
using Stryker.TestRunner.MicrosoftTestPlatform.Models;
using Stryker.TestRunner.Tests;

namespace Stryker.TestRunner.MicrosoftTestPlatform.UnitTest;

[TestClass]
public class DefaultRunnerFactoryTests
{
    [TestMethod]
    public void CreateRunner_ShouldReturnSingleMicrosoftTestPlatformRunner()
    {
        var factory = new DefaultRunnerFactory();
        var testsByAssembly = new Dictionary<string, List<TestNode>>();
        var testDescriptions = new Dictionary<string, MtpTestDescription>();
        var testSet = new TestSet();
        var discoveryLock = new object();
        var logger = NullLogger.Instance;

        var runner = factory.CreateRunner(1, testsByAssembly, testDescriptions, testSet, discoveryLock, logger);

        runner.ShouldNotBeNull();
        runner.ShouldBeOfType<SingleMicrosoftTestPlatformRunner>();
    }

    [TestMethod]
    public void CreateRunner_ShouldPassAllParametersToRunner()
    {
        var factory = new DefaultRunnerFactory();
        var testNode = new TestNode("test1", "testMethod1", "test", "pending");
        var testsByAssembly = new Dictionary<string, List<TestNode>>
        {
            ["test.dll"] = [testNode]
        };
        var testDescriptions = new Dictionary<string, MtpTestDescription>
        {
            ["test1"] = new MtpTestDescription(testNode)
        };
        var testSet = new TestSet();
        testSet.RegisterTest(testDescriptions["test1"].Description);
        var discoveryLock = new object();
        var logger = NullLogger.Instance;
        const int expectedId = 42;

        var runner = factory.CreateRunner(expectedId, testsByAssembly, testDescriptions, testSet, discoveryLock, logger);

        runner.ShouldNotBeNull();
    }

    [TestMethod]
    public void CreateRunner_ShouldCreateMultipleIndependentRunners()
    {
        var factory = new DefaultRunnerFactory();
        var testsByAssembly = new Dictionary<string, List<TestNode>>();
        var testDescriptions = new Dictionary<string, MtpTestDescription>();
        var testSet = new TestSet();
        var discoveryLock = new object();
        var logger = NullLogger.Instance;

        var runner1 = factory.CreateRunner(1, testsByAssembly, testDescriptions, testSet, discoveryLock, logger);
        var runner2 = factory.CreateRunner(2, testsByAssembly, testDescriptions, testSet, discoveryLock, logger);

        runner1.ShouldNotBeNull();
        runner2.ShouldNotBeNull();
        runner1.ShouldNotBeSameAs(runner2);
    }

    [TestMethod]
    public void CreateRunner_ShouldHandleEmptyCollections()
    {
        var factory = new DefaultRunnerFactory();
        var testsByAssembly = new Dictionary<string, List<TestNode>>();
        var testDescriptions = new Dictionary<string, MtpTestDescription>();
        var testSet = new TestSet();
        var discoveryLock = new object();
        var logger = NullLogger.Instance;

        var runner = factory.CreateRunner(0, testsByAssembly, testDescriptions, testSet, discoveryLock, logger);

        runner.ShouldNotBeNull();
    }

    [TestMethod]
    public void CreateRunner_ShouldAcceptDifferentLoggerInstances()
    {
        var factory = new DefaultRunnerFactory();
        var testsByAssembly = new Dictionary<string, List<TestNode>>();
        var testDescriptions = new Dictionary<string, MtpTestDescription>();
        var testSet = new TestSet();
        var discoveryLock = new object();
        
        var mockLogger = new Mock<ILogger>();
        var runner1 = factory.CreateRunner(1, testsByAssembly, testDescriptions, testSet, discoveryLock, mockLogger.Object);
        var runner2 = factory.CreateRunner(2, testsByAssembly, testDescriptions, testSet, discoveryLock, NullLogger.Instance);

        runner1.ShouldNotBeNull();
        runner2.ShouldNotBeNull();
    }
}
