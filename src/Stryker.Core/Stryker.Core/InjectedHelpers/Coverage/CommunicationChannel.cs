using System;
using System.IO;
#if !STRYKER_NO_PIPE
using System.IO.Pipes;
#endif
using System.Text;

namespace Stryker.Core.InjectedHelpers.Coverage
{
#if !STRYKER_NO_PIPE
    public delegate void MessageReceived(object sender, string args);

    public class CommunicationChannel : IDisposable
    {
        private readonly PipeStream _pipeStream;
        private byte[] _buffer;
        private int _cursor;
        private readonly string _pipeName;
        private bool _processingHeader;
        private bool _started;
        private Action<string> _logger;
        private readonly object _lck = new object();

        public event MessageReceived RaiseReceivedMessage;

        public bool IsConnected
        {
            get
            {
                return _pipeStream.IsConnected;
            }
        }

        public CommunicationChannel(PipeStream stream, string name)
        {
            _pipeName = name;
            _pipeStream = stream;
        }

        public static CommunicationChannel Client(string pipeName)
        {
            return Client(pipeName, -1);
        }

        public static CommunicationChannel Client(string pipeName, int timeout)
        {
            NamedPipeClientStream pipe = new NamedPipeClientStream(".", pipeName, PipeDirection.InOut,
                PipeOptions.Asynchronous | PipeOptions.WriteThrough);

            try
            {
                pipe.Connect(timeout);
            }
            catch (TimeoutException)
            {
                pipe.Dispose();
                throw;
            }
            return new CommunicationChannel(pipe, "C[" + pipeName + "]");
        }

        public void SetLogger(Action<string> logger)
        {
            _logger = logger;
        }

        public void Start()
        {
            lock (_lck)
            {
                if (_started)
                {
                    return;
                }
                _started = true;
            }

            Begin(true);
        }

        private void Begin(bool init)
        {
            if (init)
            {
                if (_buffer != null && !_processingHeader)
                {
                    string message = Encoding.Unicode.GetString(_buffer);
                    Log("Received message: [" + message + "] (" + _buffer.Length + " bytes).");

                    if (RaiseReceivedMessage != null)
                    {
                        RaiseReceivedMessage.Invoke(this, message);
                    }
                }
                _processingHeader = !_processingHeader;
                int len = _processingHeader ? 4 : BitConverter.ToInt32(_buffer, 0);
                if (len < 0)
                {
                    Log("Got invalid length, synchro lost. Aborting!");
                    _pipeStream.Close();
                    return;
                }
                _buffer = new byte[len];
                _cursor = 0;
                if (!_processingHeader && _buffer.Length == 0)
                {
                    // we have NO DATA to read, notify of empty message and wait to read again.
                    Log("Empty message.");
                    Begin(true);
                    return;
                }
            }

            try
            {
                int bufferLength = _buffer.Length - _cursor;
                _pipeStream.BeginRead(_buffer, _cursor, bufferLength, WhenReceived, null);
                Log("Waiting to read (" + bufferLength + " bytes).");
            }
            catch (ObjectDisposedException)
            {
                Log("Nothing to read, connection closed.");
            }
            catch (IOException e)
            {
                Log("Begin Read error:" + e);
            }
        }

        private void Log(string message)
        {
            if (_logger != null)
            {
                string logLine = message + "(" + _pipeName + ").";
                _logger(logLine);
            }
        }

        private void WhenReceived(IAsyncResult ar)
        {
            try
            {
                int read = _pipeStream.EndRead(ar);
                if (read == 0)
                {
                    Log("Nothing to read, connection closed.");
                    return;
                }

                _cursor += read;
                Log("Received " + read + " bytes.");
                Begin(_cursor == _buffer.Length);
            }
            catch (NullReferenceException)
            {
                Log("Nothing to read, connection closed.");
            }
            catch (IOException e)
            {
                Log("End Read error: " + e);
            }
        }

        public void SendText(string message)
        {
            byte[] messageBytes = Encoding.Unicode.GetBytes(message);
            Log("Send message: [" + message + "].");
            byte[] convertedBytes = BitConverter.GetBytes(messageBytes.Length);
            byte[] buffer = new byte[convertedBytes.Length + messageBytes.Length];
            Array.Copy(convertedBytes, buffer, convertedBytes.Length);
            Array.Copy(messageBytes, 0, buffer, convertedBytes.Length, messageBytes.Length);
            try
            {
                lock (_lck)
                {
                    _pipeStream.Write(buffer, 0, buffer.Length);
                    Log("Sent message data: " + messageBytes.Length + " bytes.");
                }
            }
            catch (ObjectDisposedException)
            {
                Log("Can't send, connection closed.");
            }
            catch (IOException e)
            {
                Log("Can't send " + e.Message);
            }
        }

        public void Dispose()
        {
            _pipeStream.Dispose();
        }
    }
#endif
}