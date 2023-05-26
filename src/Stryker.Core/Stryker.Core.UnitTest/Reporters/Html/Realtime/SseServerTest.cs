using System;
using System.Threading;
using System.Threading.Tasks;
using LaunchDarkly.EventSource;
using Stryker.Core.Reporters.Html.Realtime;
using Stryker.Core.Reporters.Html.Realtime.Events;
using Xunit;

namespace Stryker.Core.UnitTest.Reporters.Html.Realtime;

public class SseServerTest : TestBase
{
    private readonly ISseServer _sut;

    public SseServerTest() => _sut = new SseServer();

    [Fact]
    public void ShouldSendFinishedEventCorrectly()
    {
        _sut.OpenSseEndpoint();

        var @event = "";
        var data = "";
        var eventReceived = new ManualResetEvent(false);
        var sseClient = new EventSource(new Uri($"http://localhost:{_sut.Port}/"));
        sseClient.MessageReceived += (_, e) =>
        {
            @event = e.EventName;
            data = e.Message.Data;
            eventReceived.Set();
        };

        Task.Run(() => sseClient.StartAsync());
        Thread.Sleep(100);

        _sut.SendEvent(new SseEvent<string> { Event = SseEventType.Finished, Data = "" });
        eventReceived.WaitOne();

        @event.ShouldBeSemantically("finished");
        data.ShouldBeSemantically("\"\"");
    }

    [Fact]
    public void ShouldSendMutantTestedEventCorrectly()
    {
        _sut.OpenSseEndpoint();

        var @event = "";
        var data = "";
        var @object = new { Id = "1", Status = "Survived" };
        var eventReceived = new ManualResetEvent(false);
        var sseClient = new EventSource(new Uri($"http://localhost:{_sut.Port}/"));
        sseClient.MessageReceived += (_, e) =>
        {
            @event = e.EventName;
            data = e.Message.Data;
            eventReceived.Set();
        };

        Task.Run(() => sseClient.StartAsync());
        Thread.Sleep(100);

        _sut.SendEvent(new SseEvent<object>
        {
            Event = SseEventType.MutantTested,
            Data = @object
        });
        eventReceived.WaitOne();

        @event.ShouldBeSemantically("mutant-tested");
        data.ShouldBeSemantically("{\"id\":\"1\",\"status\":\"Survived\"}");
    }
}
