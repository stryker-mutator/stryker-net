using System;
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

    public delegate void MessageReceived(object sender, string args);

    public class CoverageChannel : IDisposable
    {
        private readonly PipeStream pipeStream;
        private byte[] buffer;
        private int cursor;
        private bool processingHeader;

        public event MessageReceived RaiseReceivedMessage;

        public CoverageChannel(PipeStream stream)
        {
            pipeStream = stream;
        }

        internal void Start()
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
                buffer = new byte[processingHeader ? 4 : BitConverter.ToInt32(buffer)];
                cursor = 0;
            }
            pipeStream.BeginRead(buffer, cursor, buffer.Length-cursor, WhenReceived, null);
        }

        private void WhenReceived(IAsyncResult ar)
        {
            try
            {
                var read = pipeStream.EndRead(ar);
                if (read == 0) return;
                cursor += read;
                Begin(cursor == buffer.Length);
            }
            catch (ObjectDisposedException)
            {
            }
        }

        public void SendText(string message)
        {
            var messageBytes = Encoding.Unicode.GetBytes(message);
            pipeStream.Write(BitConverter.GetBytes(messageBytes.Length));
            pipeStream.Write(messageBytes);
        }

        public void Dispose()
        {
            pipeStream.Dispose();
        }
    }
}