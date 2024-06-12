using Microsoft.Testing.Platform.Extensions.Messages;
using Microsoft.Testing.Platform.Extensions.TestHost;

namespace Stryker.TestRunner.MSTest.Testing.Consumers;
internal class TestConsumer : IDataConsumer
{
    public Type[] DataTypesConsumed => [typeof(TestNodeUpdateMessage)];

    public string Uid => nameof(TestConsumer);

    public string Version => "1.0.0";

    public string DisplayName => "Test consumer";

    public string Description => "Test consumer";

    public Task ConsumeAsync(IDataProducer dataProducer, IData value, CancellationToken cancellationToken)
    {
        var update = value as TestNodeUpdateMessage;
        var state = update!.TestNode.Properties.Single<TestNodeStateProperty>();

        if (state is InProgressTestNodeStateProperty)
        {
            return Task.CompletedTask;
        }

        



        return Task.CompletedTask;
    }
    public Task<bool> IsEnabledAsync() => Task.FromResult(true);
}
