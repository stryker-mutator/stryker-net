using System.Diagnostics;
using Microsoft.Testing.Platform.Extensions.Messages;
using Microsoft.Testing.Platform.Extensions.TestHost;

namespace Stryker.TestRunner.MSTest.Testing.Consumers;
internal class MutantRunConsumer : IDataConsumer
{
    private readonly List<TestNode> _executed;

    private MutantRunConsumer(List<TestNode> executed)
    {
        _executed = executed;
    }

    public static MutantRunConsumer Create(List<TestNode> executed) =>
        new(executed);

    public Type[] DataTypesConsumed => [typeof(TestNodeUpdateMessage)];

    public string Uid => nameof(MutantRunConsumer);

    public string Version => "1.0.0";

    public string DisplayName => $"Stryker.{Uid}";

    public string Description => "Consumes tests during the mutation test run";
    
    public Task ConsumeAsync(IDataProducer dataProducer, IData value, CancellationToken cancellationToken)
    {
        var update = value as TestNodeUpdateMessage;
        var state = update!.TestNode.Properties.Single<TestNodeStateProperty>();

        if (state is InProgressTestNodeStateProperty)
        {
            return Task.CompletedTask;
        }

        _executed.Add(update.TestNode);
        return Task.CompletedTask;
    }

    public Task<bool> IsEnabledAsync() => Task.FromResult(true);
}
