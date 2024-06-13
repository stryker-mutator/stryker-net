using Microsoft.Testing.Platform.Extensions.Messages;
using Microsoft.Testing.Platform.Extensions.TestHost;
using Stryker.TestRunner.MSTest.Testing.Results;
using Stryker.TestRunner.MSTest.Testing.Tests;

namespace Stryker.TestRunner.MSTest.Testing.Consumers;
internal class DiscoveryConsumer : IDataConsumer
{
    private readonly DiscoveryResult _discoveryResult;
    private readonly Uri _executor;
    private readonly string _source;

    public DiscoveryConsumer(string source, Uri executor, DiscoveryResult discoveryResult)
    {
        _source = source;
        _executor = executor;
        _discoveryResult = discoveryResult;
    }

    public Type[] DataTypesConsumed => [typeof(TestNodeUpdateMessage)];

    public string Uid => nameof(DiscoveryConsumer);

    public string Version => "1.0.0";

    public string DisplayName => "Discovery consumer";

    public string Description => "Discovery consumer";

    public Task ConsumeAsync(IDataProducer dataProducer, IData value, CancellationToken cancellationToken)
    {
        var update = value as TestNodeUpdateMessage;

        var state = update!.TestNode.Properties.Single<TestNodeStateProperty>();

        if(state is InProgressTestNodeStateProperty)
        {
            return Task.CompletedTask;
        }

        var testCase = new TestCase(_source, _executor, update.TestNode);

        _discoveryResult.AddTestSource(_source, update.TestNode.Uid);
        _discoveryResult.AddTestDescription(update.TestNode.Uid, testCase);
        _discoveryResult.AddTest(new TestDescription(testCase.Id.ToString(), testCase.Name, testCase.CodeFilePath));

        return Task.CompletedTask;
    }

    public Task<bool> IsEnabledAsync() => Task.FromResult(true);
}
