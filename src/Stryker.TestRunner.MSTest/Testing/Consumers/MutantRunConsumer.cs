using Microsoft.Testing.Platform.Extensions.Messages;
using Microsoft.Testing.Platform.Extensions.TestHost;
using Stryker.TestRunner.MSTest.Testing.Tests;

namespace Stryker.TestRunner.MSTest.Testing.Consumers;
internal class MutantRunConsumer : IDataConsumer
{
    private readonly MutantController _mutantController;
    private readonly List<TestNode> _executed;

    private MutantRunConsumer(MutantController mutantController, List<TestNode> executed)
    {
        _mutantController = mutantController;
        _executed = executed;
    }

    public static MutantRunConsumer Create(MutantController mutantController, List<TestNode> executed) =>
        new(mutantController, executed);

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
            OnTestStart(update.TestNode);
            return Task.CompletedTask;
        }

        _executed.Add(update.TestNode);
        return Task.CompletedTask;
    }

    private void OnTestStart(TestNode testNode)
    {
        var testId = testNode.Uid.Value;
        _mutantController.SetActiveMutation(testId);
    }

    public Task<bool> IsEnabledAsync() => Task.FromResult(true);
}
