using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.Pipes;
using System.Threading;
using Microsoft.VisualStudio.TestPlatform.ObjectModel;

namespace Stryker.DataCollector
{
    public delegate void ConnectionEvent(object s, ConnectionEventArgs e);

    public sealed class CommunicationServer : IDisposable
    {
        private readonly object _lck = new object();
        private volatile bool _mustShutdown;
        private readonly IList<CommunicationChannel> _channels = new List<CommunicationChannel>();
        private NamedPipeServerStream _listener;
        public string PipeName { get; }

        public event ConnectionEvent RaiseNewClientEvent;
        public event MessageReceived RaiseReceivedMessage;


        private static int _instanceCounter = 0;
        public CommunicationServer(string name)
        {
            PipeName = $"StrykerPipe.{name}.{Process.GetCurrentProcess().Id}.{AppDomain.CurrentDomain.Id}:{Interlocked.Increment(ref _instanceCounter)}";
        }

        public void Listen()
        {
            lock (_lck)
            {
                Log("Listen for client.");
                if (_mustShutdown)
                {
                    return;
                }

                _listener = new NamedPipeServerStream(PipeName,
                    PipeDirection.InOut,
                    NamedPipeServerStream.MaxAllowedServerInstances,
                    PipeTransmissionMode.Byte, PipeOptions.Asynchronous);
                _listener.BeginWaitForConnection(OnConnect, null);
            }
        }

        private void OnConnect(IAsyncResult ar)
        {
            CommunicationChannel session = null;
            lock (_lck)
            {
                try
                {
                    _listener.EndWaitForConnection(ar);
                }
                catch (IOException)
                {
                    return;
                }

                if (_mustShutdown)
                    return;

                if (_listener.IsConnected)
                {
                    session = new CommunicationChannel(_listener, $"{PipeName}:S({_channels.Count})");
                    Log($"New connection.");
                    _channels.Add(session);
                    _listener = null;
                    session.RaiseReceivedMessage += Session_RaiseReceivedMessage;
                }
                Listen();
            }
            RaiseNewClientEvent?.Invoke(this, new ConnectionEventArgs(session));
            session.Start();
        }

        private void Log(string message)
        {
            // TODO: control this with logging options
            Console.WriteLine($"[{DateTime.Now:HH:mm:ss.fff} DBG] {message}({PipeName}).");
        }


        private void Session_RaiseReceivedMessage(object sender, string args)
        {
            //forward
            RaiseReceivedMessage?.Invoke(sender, args);
        }

        public void Dispose()
        {
            lock (_lck)
            {
                _mustShutdown = true;
                // connect to self to shut down last open pipe
                using (var client = new NamedPipeClientStream(PipeName))
                {
                    try
                    {
                        client.Connect(0);
                    }
                    catch (TimeoutException)
                    {
                    }
                }
                foreach (var channel in _channels)
                {
                    channel.Dispose();
                }
            }
        }
    }

}