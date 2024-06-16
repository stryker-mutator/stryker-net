using Microsoft.Testing.Platform.Extensions.Messages;
using Microsoft.Testing.Platform.Extensions.TestHost;
using Stryker.TestRunner.MSTest.Testing.Results;

namespace Stryker.TestRunner.MSTest.Testing.Consumers;
internal class InitialTestRunConsumer : IDataConsumer
{
    private readonly DiscoveryResult _discoveryResult;
    private readonly List<TestNode> _executed;

    private InitialTestRunConsumer(DiscoveryResult discoveryResult, List<TestNode> executed)
    {
        _discoveryResult = discoveryResult;
        _executed = executed;
    }

    public static InitialTestRunConsumer Create(DiscoveryResult discoveryResult, List<TestNode> executed) =>
        new(discoveryResult, executed);

    public Type[] DataTypesConsumed => [typeof(TestNodeUpdateMessage)];

    public string Uid => nameof(InitialTestRunConsumer);

    public string Version => "1.0.0";

    public string DisplayName => "Initial Test Run consumer";

    public string Description => "Consumes tests during the initial run";

    public Task ConsumeAsync(IDataProducer dataProducer, IData value, CancellationToken cancellationToken)
    {
        var update = value as TestNodeUpdateMessage;

        var state = update!.TestNode.Properties.Single<TestNodeStateProperty>();

        if (state is InProgressTestNodeStateProperty)
        {
            return Task.CompletedTask;
        }

        var timing = update.TestNode.Properties.Single<TimingProperty>();
        var duration = timing.GlobalTiming.Duration;
        var testResult = new MsTestResult(duration);
        _discoveryResult.MsTests[update.TestNode.Uid].RegisterInitialTestResult(testResult);
        _executed.Add(update.TestNode);
 
        return Task.CompletedTask;
    }

    public Task<bool> IsEnabledAsync() => Task.FromResult(true);
}
