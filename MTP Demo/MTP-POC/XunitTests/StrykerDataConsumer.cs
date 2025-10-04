using System.Text.Json;
using Microsoft.Testing.Platform.OutputDevice;

namespace TestProject1;

// Stryker.Mtp.Agent
using Microsoft.Testing.Platform.Extensions.Messages;
using Microsoft.Testing.Platform.Extensions.TestHost;
using Microsoft.Testing.Platform.Services;

public sealed class StrykerDataConsumer : IDataConsumer
{
    private readonly IOutputDevice _out;

    public StrykerDataConsumer(IOutputDevice outDev)
    {
        _out = outDev;
    }

    public Task<bool> IsEnabledAsync() => Task.FromResult(true);

    public string Uid => "Stryker.DataConsumer";
    public string Version => "1.0.0";
    public string DisplayName => "Stryker Coverage Consumer";
    public string Description => "Consumes test execution messages to report coverage for Stryker";

    public ValueTask<bool> IsEnabledAsync(IServiceProvider sp, CancellationToken ct) => ValueTask.FromResult(true);

    // public async ValueTask OnTestNodeUpdateAsync(TestNodeUpdateMessage msg, CancellationToken ct)
    // {
    //     var node = msg.TestNode;
    //     msg.
    //     if (node is null) return;
    //     
    //     node.Properties.


    // Pseudocode: adjust to actual node phases and types from the API.
    // switch (node.State)
    // {
    //     case TestNodeState.Started when node.Kind == TestNodeKind.TestCase:
    //         MutantControl.SetCurrentTest(node.Id);
    //         break;
    //
    //     case TestNodeState.Completed when node.Kind == TestNodeKind.TestCase:
    //         MutantControl.SetCurrentTest(null);
    //         var hits = MutantControl.ConsumeHitsFor(node.Id);
    //         // Emit structured line that the orchestrator will parse from client/log
    //         var json = JsonSerializer.Serialize(new { testId = node.Id, hits });
    //         await _out.WriteLineAsync($"STRYKER_COV:{json}");
    //         break;
    // }
    // }

    public Task ConsumeAsync(IDataProducer dataProducer, IData value, CancellationToken cancellationToken)
    {
        if (value is TestNodeUpdateMessage msg)
        {
            var test = msg.TestNode.Properties;

            // return OnTestNodeUpdateAsync(msg, cancellationToken).AsTask();
        }
        return Task.CompletedTask;
    }

    public Type[] DataTypesConsumed => [typeof(TestNodeUpdateMessage)];
}