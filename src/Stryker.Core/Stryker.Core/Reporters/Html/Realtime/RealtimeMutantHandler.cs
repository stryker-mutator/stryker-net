using Stryker.Core.Mutants;
using Stryker.Core.Options;
using Stryker.Core.Reporters.Html.Realtime.Events;
using Stryker.Core.Reporters.Json.SourceFiles;

namespace Stryker.Core.Reporters.Html.Realtime;

public class RealtimeMutantHandler : IRealtimeMutantHandler
{
    private readonly ISseEventSender _eventSender;

    public RealtimeMutantHandler(StrykerOptions options, ISseEventSender eventSender = null)
        => _eventSender = eventSender ?? new SseEventSender(options);

    public void OpenSseEndpoint() => _eventSender.OpenSseEndpoint();

    public void CloseSseEndpoint()
    {
        _eventSender.SendEvent(new SseEvent<string> { Type = SseEventType.Finished, Data = "" });
        _eventSender.CloseSseEndpoint();
    }

    public void SendMutantResultEvent(IReadOnlyMutant testedMutant)
    {
        var jsonMutant = new JsonMutant(testedMutant);
        _eventSender.SendEvent(new SseEvent<JsonMutant> { Type = SseEventType.Mutation, Data = jsonMutant });
    }
}
