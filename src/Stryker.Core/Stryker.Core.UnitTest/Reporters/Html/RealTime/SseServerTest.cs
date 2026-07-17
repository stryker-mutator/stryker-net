using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using LaunchDarkly.EventSource;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Shouldly;
using Stryker.Core.Reporters.Html.RealTime;
using Stryker.Core.Reporters.Html.RealTime.Events;

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
            while (_sut.HasConnectedClients && watch.ElapsedMilliseconds < timeout)
            {
                Monitor.Wait(_lock,  Math.Max(Math.Min( timeout - (int)watch.ElapsedMilliseconds, 100), 1));
            }
        }

        return !_sut.HasConnectedClients;
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
        using var sseClient = new EventSource(new Uri($"http://localhost:{_sut.Port}/"));

        Task.Run(() => sseClient.StartAsync());
        WaitForConnection(500).ShouldBeTrue();

        _sut.HasConnectedClients.ShouldBeTrue();
        _sut.CloseSseEndpoint();
        _sut.HasConnectedClients.ShouldBeFalse();
        _sut.CloseSseEndpoint();
    }

    [TestMethod]
    public void ShouldCloseEndpointFromClientConnectedHandler()
    {
        _sut.ClientConnected += (_, _) => _sut.CloseSseEndpoint();
        _sut.OpenSseEndpoint();
        using var sseClient = new EventSource(new Uri($"http://localhost:{_sut.Port}/"));

        Task.Run(() => sseClient.StartAsync());

        WaitForConnection(500).ShouldBeTrue();
        SpinWait.SpinUntil(() => !_sut.HasConnectedClients, 500).ShouldBeTrue();
    }

    [TestMethod]
    public void ShouldContinueDisposingWritersAfterAnIoFailure()
    {
        _sut.OpenSseEndpoint();
        var failingWriter = new StreamWriter(new FailingWriteStream());
        failingWriter.Write("buffered");
        var trackingStream = new TrackingStream();
        var trackingWriter = new StreamWriter(trackingStream);
        var writersField = typeof(SseServer).GetField("_writers", BindingFlags.Instance | BindingFlags.NonPublic)
            ?? throw new MissingFieldException(typeof(SseServer).FullName, "_writers");
        var writers = (List<StreamWriter>)(writersField.GetValue(_sut)
            ?? throw new InvalidOperationException("The writer collection was null."));
        writers.Add(failingWriter);
        writers.Add(trackingWriter);

        Should.NotThrow(_sut.CloseSseEndpoint);

        _sut.ConnectedClients.ShouldBe(0);
        trackingStream.IsDisposed.ShouldBeTrue();
    }

    [TestMethod]
    public void ShouldWaitForConcurrentDisposalToComplete()
    {
        _sut.OpenSseEndpoint();
        var blockingStream = new BlockingDisposeStream();
        var writersField = typeof(SseServer).GetField("_writers", BindingFlags.Instance | BindingFlags.NonPublic)
            ?? throw new MissingFieldException(typeof(SseServer).FullName, "_writers");
        var writers = (List<StreamWriter>)(writersField.GetValue(_sut)
            ?? throw new InvalidOperationException("The writer collection was null."));
        writers.Add(new StreamWriter(blockingStream));

        var firstClose = Task.Run(_sut.CloseSseEndpoint);
        blockingStream.DisposeStarted.Wait(500).ShouldBeTrue();
        try
        {
            var secondStarted = new ManualResetEventSlim();
            var secondClose = Task.Run(() =>
            {
                secondStarted.Set();
                _sut.CloseSseEndpoint();
            });
            secondStarted.Wait(500).ShouldBeTrue();

            secondClose.Wait(100).ShouldBeFalse();
            blockingStream.AllowDispose.Set();
            Task.WaitAll(firstClose, secondClose);
        }
        finally
        {
            blockingStream.AllowDispose.Set();
        }

        blockingStream.IsDisposed.ShouldBeTrue();
    }

    [TestMethod]
    public void ShouldCloseWhileClientConnectedHandlerIsBlocked()
    {
        using var handlerEntered = new ManualResetEventSlim();
        using var releaseHandler = new ManualResetEventSlim();
        _sut.ClientConnected += (_, _) =>
        {
            handlerEntered.Set();
            releaseHandler.Wait();
        };
        _sut.OpenSseEndpoint();
        using var sseClient = new EventSource(new Uri($"http://localhost:{_sut.Port}/"));
        Task.Run(() => sseClient.StartAsync());
        handlerEntered.Wait(500).ShouldBeTrue();

        var close = Task.Run(_sut.CloseSseEndpoint);
        try
        {
            close.Wait(500).ShouldBeTrue();
        }
        finally
        {
            releaseHandler.Set();
        }
    }

    private sealed class FailingWriteStream : MemoryStream
    {
        public override void Write(byte[] buffer, int offset, int count) =>
            throw new IOException("The client disconnected.");

        public override void Write(ReadOnlySpan<byte> buffer) =>
            throw new IOException("The client disconnected.");
    }

    private sealed class TrackingStream : MemoryStream
    {
        public bool IsDisposed { get; private set; }

        protected override void Dispose(bool disposing)
        {
            IsDisposed = disposing;
            base.Dispose(disposing);
        }
    }

    private sealed class BlockingDisposeStream : MemoryStream
    {
        public ManualResetEventSlim DisposeStarted { get; } = new();
        public ManualResetEventSlim AllowDispose { get; } = new();
        public bool IsDisposed { get; private set; }

        protected override void Dispose(bool disposing)
        {
            DisposeStarted.Set();
            AllowDispose.Wait();
            IsDisposed = disposing;
            base.Dispose(disposing);
        }
    }
}
