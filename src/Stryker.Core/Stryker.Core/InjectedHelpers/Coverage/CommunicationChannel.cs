using System;
using System.IO;
using System.IO.Pipes;
using System.Text;

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
        private string _pipeName;
        private bool _processingHeader;
        private bool _started;
        private readonly object _lck = new object();

        public event MessageReceived RaiseReceivedMessage;

        public bool IsConnected => _pipeStream.IsConnected;

        public CommunicationChannel(PipeStream stream, string name)
        {
            _pipeName = name;
            _pipeStream = stream;
        }

        public static CommunicationChannel Client(string pipeName, int timeout = -1)
        {
            var pipe = new NamedPipeClientStream(".", pipeName, PipeDirection.InOut,
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
            return new CommunicationChannel(pipe, $"{pipeName}:C");
        }

        public void Start()
        {
            lock(_lck)
            {
                if (_started)
                {
                    return;
                }
                _started = true;
            }

            Begin();
        }

        private void Begin(bool init = true)
        {
            if (init)
            {
                if (_buffer != null && !_processingHeader)
                {
                    var message = Encoding.Unicode.GetString(_buffer);
                    Log($"Received message: [{message}] ({_buffer.Length} bytes).");
                    RaiseReceivedMessage?.Invoke(this, message);
                }
                _processingHeader = !_processingHeader;
                var len = _processingHeader ? 4 : BitConverter.ToInt32(_buffer,0);
                if (len<0)
                {
                    Log($"Got invalid length, synchro lost. Aborting!");
                    _pipeStream.Close();
                    return;
                }
                _buffer = new byte[len];
                _cursor = 0;
                if (!_processingHeader && _buffer.Length == 0)
                {
                    // we have NO DATA to read, notify of empty message and wait to read again.
                    Log("Empty message.");
                    Begin();
                    return;
                }
            }

            try
            {
                Log("Begin Read");
                _pipeStream.BeginRead(_buffer, _cursor, _buffer.Length-_cursor, WhenReceived, null);
            }
            catch (ObjectDisposedException)
            {
            }
            catch (IOException)
            {
            }
        }

        private void Log(string message)
        {
            Console.WriteLine($"{message}({_pipeName}).");
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
                    Log($"Send message: [{message}] ({messageBytes.Length} bytes)");
                    _pipeStream.BeginWrite(BitConverter.GetBytes(messageBytes.Length), 0, 4, HeaderSent, messageBytes);
                }
            }
            catch (ObjectDisposedException)
            {
            }
            catch (IOException)
            {
            }
        }

        private void HeaderSent(IAsyncResult ar)
        {
            try
            {
                var data = (byte[]) ar.AsyncState;
                lock (_lck)
                {
                    _pipeStream.EndWrite(ar);
                    _pipeStream.BeginWrite(data, 0, data.Length, DataSent, data);
                }
            }
            catch (ObjectDisposedException)
            {
            }
            catch (IOException)
            {
            } 
        }

        private void DataSent(IAsyncResult ar)
        {
            try
            {
                lock (_lck)
                {
                    _pipeStream.EndWrite(ar);
                }
                Log("Send message sent.");
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