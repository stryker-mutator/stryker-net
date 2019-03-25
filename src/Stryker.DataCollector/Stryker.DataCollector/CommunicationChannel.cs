using System;
using System.IO;
using System.IO.Pipes;
using System.Text;

namespace Stryker.DataCollector
{
    public class CommunicationChannel : IDisposable
    {
        private readonly PipeStream pipeStream;
        private byte[] buffer;
        private int cursor;
        private bool processingHeader;

        public event MessageReceived RaiseReceivedMessage;

        public bool IsConnected => pipeStream.IsConnected;

        public CommunicationChannel(PipeStream stream)
        {
            pipeStream = stream;
        }

        public static CommunicationChannel Client(string pipename, int timeout = -1)
        {
            var pipe = new NamedPipeClientStream(".", pipename, PipeDirection.InOut,
                PipeOptions.Asynchronous);
            try
            {
                pipe.Connect(timeout);
            }
            catch (TimeoutException)
            {
                pipe.Dispose();
                throw;
            }
            return new CommunicationChannel(pipe);
        }

        public void Start()
        {
            Begin();
        }

        private void Begin(bool init = true)
        {
            if (init)
            {
                if (buffer != null && !processingHeader)
                {
                    RaiseReceivedMessage?.Invoke(this, Encoding.Unicode.GetString(buffer));
                }
                processingHeader = !processingHeader;
                buffer = new byte[processingHeader ? 4 : BitConverter.ToInt32(buffer,0)];
                cursor = 0;
            }

            try
            {
                pipeStream.BeginRead(buffer, cursor, buffer.Length-cursor, WhenReceived, null);
            }
            catch (ObjectDisposedException)
            {
            }
            catch (IOException)
            {
            }
        }

        private void WhenReceived(IAsyncResult ar)
        {
            try
            {
                var read = pipeStream.EndRead(ar);
                if (read == 0)
                {
                    return;
                }

                cursor += read;
                Begin(cursor == buffer.Length);
            }
            catch (ObjectDisposedException)
            {
            }
            catch (IOException)
            {
            }
        }

        public void SendText(string message)
        {
            var messageBytes = Encoding.Unicode.GetBytes(message);
            try
            {
                pipeStream.Write(BitConverter.GetBytes(messageBytes.Length), 0, 4);
                pipeStream.Write(messageBytes, 0 , messageBytes.Length);
            }
            catch (ObjectDisposedException)
            {
            }
            catch (IOException)
            {
            }
        }

        public void Dispose()
        {
            pipeStream.Dispose();
        }
    }

    public class ConnectionEventArgs : EventArgs
    {
        public CommunicationChannel Client;

        public ConnectionEventArgs(CommunicationChannel client)
        {
            this.Client = client;
        }
    }

    public delegate void MessageReceived(object sender, string args);
}