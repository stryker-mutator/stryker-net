using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.Pipes;
using Microsoft.Extensions.Logging;
using Stryker.Core.InjectedHelpers.Coverage;
using Stryker.Core.Logging;

namespace Stryker.Core.Coverage
{

    public delegate void ConnectionEvent(object s, ConnectionEventArgs e);

    public sealed class CommunicationServer : IDisposable
    {
        private readonly object lck = new object();
        private volatile bool mustShutdown;
        private readonly IList<CommunicationChannel> channels = new List<CommunicationChannel>();
        private NamedPipeServerStream listener;
        private static readonly ILogger<CommunicationServer> Logger;

        public string PipeName { get; private set; }

        public event ConnectionEvent RaiseNewClientEvent;
        public event MessageReceived RaiseReceivedMessage;

        static CommunicationServer()
        {
            Logger = ApplicationLogging.LoggerFactory.CreateLogger<CommunicationServer>();
        }

        public CommunicationServer(string name)
        {
            PipeName = $"Stryker.{name}.Pipe.{Stopwatch.GetTimestamp()}";
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
                    PipeTransmissionMode.Byte, PipeOptions.Asynchronous);
                try
                {
                    listener.BeginWaitForConnection(OnConnect, null);
                }
                catch (Exception e)
                {
                    Logger.LogError("Exception on connect {0}", e);
                    throw;
                }
            }
        }

        private void OnConnect(IAsyncResult ar)
        {
            CommunicationChannel session = null;
            lock (lck)
            {
                try
                {
                    listener.EndWaitForConnection(ar);
                }
                catch (IOException)
                {
                    return;
                }
                catch (Exception e)
                {
                    Logger.LogError("Exception on connect {0}", e);
                    throw;
                }

                if (mustShutdown)
                    return;

                if (listener.IsConnected)
                {
                    session = new CommunicationChannel(listener);
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
}