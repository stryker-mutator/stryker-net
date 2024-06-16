using Microsoft.Testing.Platform.Extensions.Messages;
using Microsoft.Testing.Platform.Extensions.TestHost;
using Stryker.TestRunner.MSTest.Testing.Tests;

namespace Stryker.TestRunner.MSTest.Testing.Consumers;
internal class CoverageConsumer : IDataConsumer
{
    private readonly CoverageCollector _coverageCollector;

    private CoverageConsumer(CoverageCollector coverageCollector)
    {
        _coverageCollector = coverageCollector;
    }

    public static CoverageConsumer Create(CoverageCollector coverageCollector) => new(coverageCollector);

    public Type[] DataTypesConsumed => [typeof(TestNodeUpdateMessage)];

    public string Uid => nameof(CoverageConsumer);

    public string Version => "1.0.0";

    public string DisplayName => "Stryker.CoverageConsumer";

    public string Description => "Used to gather coverage";

    public Task<bool> IsEnabledAsync() => Task.FromResult(true);

    public Task ConsumeAsync(IDataProducer dataProducer, IData value, CancellationToken cancellationToken)
    {
        var update = value as TestNodeUpdateMessage;
        var state = update!.TestNode.Properties.Single<TestNodeStateProperty>();

        if (state is InProgressTestNodeStateProperty)
        {
            return Task.CompletedTask;
        }

        _coverageCollector.PublishCoverageData(update.TestNode);

        return Task.CompletedTask;
    }
}
