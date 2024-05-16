using System.Diagnostics;
using Microsoft.Testing.Platform.Extensions.Messages;
using Microsoft.Testing.Platform.Extensions.TestHost;

namespace Stryker.TestRunners.MsTest;
internal class TestDataConsumer : IDataConsumer
{
    private TestDataConsumer() { }

    public static TestDataConsumer Create() => new();

    public Type[] DataTypesConsumed => new[] { typeof(TestNodeUpdateMessage) }; 

    public string Uid => nameof(TestDataConsumer);

    public string Version => "1.0.0";

    public string DisplayName => "TestDataConsumer";

    public string Description => "Consumes data produced by executing tests";

    public async Task ConsumeAsync(IDataProducer dataProducer, IData value, CancellationToken cancellationToken)
    {
        var data = (TestNodeUpdateMessage)value;
        var t = data.TestNode.Uid;
        var guid = NamedGuids.GetFromName(t);
        Debug.WriteLine(guid.ToString());
    }

    public Task<bool> IsEnabledAsync() => Task.FromResult(true);
}
