using System;
using System.IO;
using System.IO.Pipes;
using System.Text;
using System.Threading;

namespace Stryker.Core.InjectedHelpers.Coverage
{
    public class ConnectionEventArgs : EventArgs
    {
        public CommunicationChannel Client;

        public ConnectionEventArgs(CommunicationChannel client)
        {
            this.Client = client;
        }
    } 

    public delegate void MessageReceived(object sender, string args);

    public class CommunicationChannel : IDisposable
    {
        private readonly PipeStream _pipeStream;
        private byte[] _buffer;
        private int _cursor;
        private bool _processingHeader;
        private readonly object _lck = new object();

        public event MessageReceived RaiseReceivedMessage;

        public bool IsConnected => _pipeStream.IsConnected;

        public CommunicationChannel(PipeStream stream)
        {
            _pipeStream = stream;
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
                if (_buffer != null && !_processingHeader)
                {
                    RaiseReceivedMessage?.Invoke(this, Encoding.Unicode.GetString(_buffer));
                }
                _processingHeader = !_processingHeader;
                _buffer = new byte[_processingHeader ? 4 : BitConverter.ToInt32(_buffer,0)];
                _cursor = 0;
                if (!_processingHeader && _buffer.Length == 0)
                {
                    // we have NO DATA to read, notify of empty message and wait to read again.
                    Begin();
                    return;
                }
            }

            try
            {
                _pipeStream.BeginRead(_buffer, _cursor, _buffer.Length-_cursor, WhenReceived, null);
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
                var read = _pipeStream.EndRead(ar);
                if (read == 0)
                {
                    return;
                }

                _cursor += read;
                Begin(_cursor == _buffer.Length);
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
                lock (_lck)
                {
                    var finalBuffer = new byte[messageBytes.Length + 4];
                    Array.Copy(BitConverter.GetBytes(messageBytes.Length), finalBuffer, 4);
                    Array.Copy(messageBytes, 0, finalBuffer, 4, messageBytes.Length);
                    _pipeStream.Write(finalBuffer, 0 , finalBuffer.Length);                    
                }
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
            _pipeStream.Dispose();
        }
    }
}