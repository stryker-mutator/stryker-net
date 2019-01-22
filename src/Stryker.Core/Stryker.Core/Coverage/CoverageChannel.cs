using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Pipes;
using System.Text;

namespace Stryker.Core.Coverage
{
    public class ConnectionEventArgs : EventArgs
    {
        public CoverageChannel client;

        public ConnectionEventArgs(CoverageChannel client)
        {
            this.client = client;
        }
    }

    public delegate void ConnectionEvent(object s, ConnectionEventArgs e);

    public delegate void MessageReceived(object sender, string args);

    public sealed class CoverageServer : IDisposable
    {
        private readonly object lck = new object();
        private volatile bool mustShutdown;
        private readonly IList<CoverageChannel> channels = new List<CoverageChannel>();
        private NamedPipeServerStream listener;

        public string PipeName { get; private set; }

        public event ConnectionEvent RaiseNewClientEvent;
        public event MessageReceived RaiseReceivedMessage;

        public CoverageServer(string name)
        {
            PipeName = $"Stryker.{name}.Pipe";
        }

        public void Listen()
        {
            lock (lck)
            {
                if (mustShutdown)
                {
                    return;
                }

                listener = new NamedPipeServerStream(PipeName,
                    PipeDirection.InOut,
                    NamedPipeServerStream.MaxAllowedServerInstances,
                    PipeTransmissionMode.Byte);
                listener.BeginWaitForConnection(OnConnect, null);
            }
        }

        private void OnConnect(IAsyncResult ar)
        {
            CoverageChannel session = null;
            lock (lck)
            {
                try
                {
                    listener.EndWaitForConnection(ar);
                }
                catch (IOException)
                {
                }
                catch (ObjectDisposedException)
                {
                    // disposed
                    return;
                }

                if (listener.IsConnected)
                {
                    session = new CoverageChannel(listener);
                    channels.Add(session);
                    listener = null;
                    session.RaiseReceivedMessage += Session_RaiseReceivedMessage;
                    session.Start();
                }
                Listen();
            }

            RaiseNewClientEvent?.Invoke(this, new ConnectionEventArgs(session));
        }

        private void Session_RaiseReceivedMessage(object sender, string args)
        {
            //forward
            RaiseReceivedMessage?.Invoke(sender, args);
        }

        public void Dispose()
        {
            lock (lck)
            {
                mustShutdown = true;
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

                foreach (var channel in channels)
                {
                    channel.Dispose();
                }
            }
        }
    }

    public class CoverageChannel : IDisposable
    {
        private readonly NamedPipeServerStream namedPipeServerStream;
        private readonly byte[] buffer = new byte[1024];
        private readonly StringBuilder receivedText = new StringBuilder();
        private int cursor;

        public event MessageReceived RaiseReceivedMessage;

        internal CoverageChannel(NamedPipeServerStream stream)
        {
            namedPipeServerStream = stream;
        }

        internal void Start()
        {
            BeginRead();
        }

        private void BeginRead()
        {
            namedPipeServerStream.BeginRead(buffer, cursor, buffer.Length - cursor, OnReceived, null);
        }

        private void OnReceived(IAsyncResult ar)
        {
            try
            {
                var read = namedPipeServerStream.EndRead(ar);
                if (read != 0)
                {
                    receivedText.Append(Encoding.Unicode.GetString(buffer.AsSpan(cursor, read)));
                    RaiseReceivedMessage?.Invoke(this, receivedText.ToString());
                    receivedText.Clear();
                    cursor = 0;
                    BeginRead();
                }
            }
            catch (ObjectDisposedException e)
            {
            }

        }

        public void Dispose()
        {
            namedPipeServerStream.Disconnect();
            namedPipeServerStream.Dispose();
        }
    }
}