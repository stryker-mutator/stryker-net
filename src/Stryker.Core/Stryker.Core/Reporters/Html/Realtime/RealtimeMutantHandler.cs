using Stryker.Core.Mutants;
using Stryker.Core.Options;
using Stryker.Core.Reporters.Html.Realtime.Events;
using Stryker.Core.Reporters.Json.SourceFiles;

namespace Stryker.Core.Reporters.Html.Realtime;

public class RealtimeMutantHandler : IRealtimeMutantHandler
{
    public int Port => _server.Port;

    private readonly ISseServer _server;

    public RealtimeMutantHandler(StrykerOptions options, ISseServer server = null)
        => _server = server ?? new SseServer();

    public void OpenSseEndpoint() => _server.OpenSseEndpoint();

    public void CloseSseEndpoint()
    {
        _server.SendEvent(new SseEvent<string> { Event = SseEventType.Finished, Data = "" });
        _server.CloseSseEndpoint();
    }

    public void SendMutantTestedEvent(IReadOnlyMutant testedMutant)
    {
        var jsonMutant = new JsonMutant(testedMutant);
        _server.SendEvent(new SseEvent<JsonMutant> { Event = SseEventType.MutantTested, Data = jsonMutant });
    }
}
