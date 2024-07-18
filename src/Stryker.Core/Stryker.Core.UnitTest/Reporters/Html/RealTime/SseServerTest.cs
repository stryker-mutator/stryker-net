using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using LaunchDarkly.EventSource;
using Shouldly;
using Stryker.Core.Reporters.Html.RealTime;
using Stryker.Core.Reporters.Html.RealTime.Events;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Stryker.Core.UnitTest.Reporters.Html.RealTime;

[TestClass]
public class SseServerTest : TestBase
{
    private readonly SseServer _sut;
    private readonly object _lock = new();
    private bool _connected;

    public SseServerTest()
    {
        _sut = new SseServer();
        _sut.ClientConnected += ClientConnected;
    }

    private void ClientConnected(object sender, EventArgs e)
    {
        lock (_lock)
        {
            _connected = true;
            Monitor.Pulse(_lock);
        }
    }

    private bool WaitForConnection(int timeout)
    {
        var watch = new Stopwatch();
        watch.Start();
        lock (_lock)
        {
            while (!_connected && watch.ElapsedMilliseconds < timeout)
            {
                Monitor.Wait(_lock, Math.Max(timeout - (int)watch.ElapsedMilliseconds, 1));
            }
        }

        return _connected;
    }

    private bool WaitForDisConnection(int timeout)
    {
        var watch = new Stopwatch();
        watch.Start();
        lock (_lock)
        {
            while (_sut.ConnectedClients>0 && watch.ElapsedMilliseconds < timeout)
            {
                Monitor.Wait(_lock,  Math.Max(Math.Min( timeout - (int)watch.ElapsedMilliseconds, 100), 1));
            }
        }

        return _sut.ConnectedClients==0;
    }

    [TestMethod]
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
        WaitForConnection(500).ShouldBeTrue();

        _sut.SendEvent(new SseEvent<string> { Event = SseEventType.Finished, Data = "" });
        eventReceived.WaitOne();

        @event.ShouldBeSemantically("finished");
        data.ShouldBeSemantically("\"\"");
        _sut.CloseSseEndpoint();
    }

    [TestMethod]
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
        WaitForConnection(500).ShouldBeTrue();
        
        _sut.SendEvent(new SseEvent<object>
        {
            Event = SseEventType.MutantTested,
            Data = @object
        });
        eventReceived.WaitOne();

        @event.ShouldBeSemantically("mutant-tested");
        data.ShouldBeSemantically("{\"id\":\"1\",\"status\":\"Survived\"}");
        _sut.CloseSseEndpoint();
    }

    // this test works reliably only on windows, probably due to difference in http listener implementation
    [TestMethod]
    public void ShouldHandleDroppedConnection()
    {
        _sut.OpenSseEndpoint();

        var @object = new { Id = "1", Status = "Survived" };
        var sseClient = new EventSource(new Uri($"http://localhost:{_sut.Port}/"));
        
        Task.Run(() => sseClient.StartAsync());
        WaitForConnection(500).ShouldBeTrue();
        Task.Run( ()=> {sseClient.Close(); sseClient.Dispose();}).Wait();

        _sut.SendEvent(new SseEvent<object>
        {
            Event = SseEventType.MutantTested,
            Data = @object
        });
        _sut.SendEvent(new SseEvent<object>
        {
            Event = SseEventType.MutantTested,
            Data = @object
        });
        WaitForDisConnection(500);
        sseClient.ReadyState.ShouldBe(ReadyState.Shutdown);
        _sut.CloseSseEndpoint();
    }

    [TestMethod]
    public void ShouldIndicateWhenAtLeastOneClientIsConnected()
    {
        _sut.OpenSseEndpoint();
        var sseClient = new EventSource(new Uri($"http://localhost:{_sut.Port}/"));

        Task.Run(() => sseClient.StartAsync());
        WaitForConnection(500).ShouldBeTrue();

        _sut.HasConnectedClients.ShouldBeTrue();
        _sut.CloseSseEndpoint();
    }
}
