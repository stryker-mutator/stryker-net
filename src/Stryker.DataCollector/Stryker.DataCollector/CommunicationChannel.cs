using System;
using System.IO;
using System.IO.Pipes;
using System.Text;

namespace Stryker.DataCollector
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
        private Action<string> _logger;
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
        public void SetLogger(Action<string> logger)
        {
            _logger = logger;
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
                var bufferLength = _buffer.Length-_cursor;
                Log($"Begin Read {bufferLength} bytes.");
                _pipeStream.BeginRead(_buffer, _cursor, bufferLength, WhenReceived, null);
            }
            catch (ObjectDisposedException)
            {
                Log($"Nothing to read, connection closed.");
            }
            catch (IOException e)
            {
                Log($"Begin Read {e} exception.");
            }
        }

        private void Log(string message)
        {
            var logLine = $"{message}({_pipeName}).";
            if (_logger == null)
            {
                Console.WriteLine($"[{DateTime.Now:HH:mm:ss.fff} DBG] {logLine}");
            }
            else
            {
                _logger(logLine);
            }
        }

        private void WhenReceived(IAsyncResult ar)
        {
            try
            {
                var read = _pipeStream.EndRead(ar);
                if (read == 0)
                {
                    Log($"Nothing to read, connection closed.");
                    return;
                }

                _cursor += read;
                Log($"Received {read} bytes.");
                Begin(_cursor == _buffer.Length);
            }
            catch (NullReferenceException)
            {
                Log($"Nothing to read, connection closed.");
            }
            catch (IOException e)
            {
                Log($"Begin Read {e} exception.");
            }
        }

        public void SendText(string message)
        {
            var messageBytes = Encoding.Unicode.GetBytes(message);
            try
            {
                lock (_lck)
                {
                    Log($"Send message header");
                    _pipeStream.Write(BitConverter.GetBytes(messageBytes.Length), 0, 4);
                    Log($"Send message data: {message} ({messageBytes.Length} bytes).");
                    _pipeStream.Write(messageBytes, 0, messageBytes.Length);
                }
            }
            catch (ObjectDisposedException)
            {
                Log($"Nothing to read, connection closed.");
            }
            catch (IOException e)
            {
                Log($"Begin Read {e} exception.");
            }
        }

        public void Dispose()
        {
            _pipeStream.Dispose();
        }
    }
}