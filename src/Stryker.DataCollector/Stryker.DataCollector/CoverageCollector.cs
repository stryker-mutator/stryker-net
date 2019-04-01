using System;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using Microsoft.VisualStudio.TestPlatform.ObjectModel.DataCollection;
using Microsoft.VisualStudio.TestPlatform.ObjectModel.DataCollector.InProcDataCollector;
using Microsoft.VisualStudio.TestPlatform.ObjectModel.InProcDataCollector;

namespace Stryker.DataCollector
{

    [DataCollectorFriendlyName("StrykerCoverage")]
    [DataCollectorTypeUri("https://stryker-mutator.io/")]
    public class CoverageCollector: InProcDataCollection
    {
        private IDataCollectionSink _dataSink;
        private CommunicationServer _server;
        private CommunicationChannel _client;
        private readonly object lck = new object();
        private string _lastMessage;
        private bool _coverageOn;

        private const string TemplateForConfiguration = @"<InProcDataCollectionRunSettings><InProcDataCollectors><InProcDataCollector {0} >
        <Configuration>
        </Configuration>
        </InProcDataCollector>
        </InProcDataCollectors>
        </InProcDataCollectionRunSettings>";

        public static string GetVsTestSettings()
        {
            var codeBase = typeof(CoverageCollector).Assembly.Location;
            var qualifiedName = typeof(CoverageCollector).AssemblyQualifiedName;
            var friendlyName = typeof(CoverageCollector).ExtractAttribute<DataCollectorFriendlyNameAttribute>().FriendlyName;
            var uri = (typeof(CoverageCollector).GetCustomAttributes(typeof(DataCollectorTypeUriAttribute), false).First() as
                DataCollectorTypeUriAttribute).TypeUri;
            var line= $"friendlyName=\"{friendlyName}\" uri=\"{uri}\" codebase=\"{codeBase}\" assemblyQualifiedName=\"{qualifiedName}\"";
            return string.Format(TemplateForConfiguration, line);
        }

        static bool WaitOnLck(object lck, Func<bool> predicate, int timeout)
        {
            var watch = new Stopwatch();
            watch.Start();
            lock (lck)
            {
                while (!predicate() && watch.ElapsedMilliseconds < timeout)
                {
                    Monitor.Wait(lck, Math.Max(1, (int)(timeout - watch.ElapsedMilliseconds)));
                }
            }

            return predicate();
        }

        public void Initialize(IDataCollectionSink dataCollectionSink)
        {
            this._dataSink = dataCollectionSink;
            _server = new CommunicationServer(Process.GetCurrentProcess().Id.ToString());
            Environment.SetEnvironmentVariable("Coverage", _server.PipeName);
            _server.Listen();
        }

        public void TestSessionStart(TestSessionStartArgs testSessionStartArgs)
        {
            var coverageString = Environment.GetEnvironmentVariable("CaptureCoverage");
            _coverageOn = coverageString != null;
            _server.RaiseNewClientEvent += ConnectionEstablished;
        }

        private void ConnectionEstablished(object s, ConnectionEventArgs e)
        {
            lock (lck)
            {
                _client = e.Client;
                _client.RaiseReceivedMessage += ReceivedMutant;
                Monitor.Pulse(lck);
            }
        }

        private void ReceivedMutant(object sender, string args)
        {
            lock (lck)
            {
                _lastMessage = args;
                Monitor.Pulse(lck);
            }
        }

        public void TestCaseStart(TestCaseStartArgs testCaseStartArgs)
        {
        }

        public void TestCaseEnd(TestCaseEndArgs testCaseEndArgs)
        {
            if (_coverageOn)
            {
                if (!WaitOnLck(lck, () => _client != null, 500))
                {
                    throw new InvalidOperationException("connection");
                }

                _client.SendText($"DUMP {testCaseEndArgs.DataCollectionContext.TestCase.DisplayName}");
                if (!WaitOnLck(lck, () => _lastMessage != null, 5000))
                {
                    throw new InvalidOperationException(testCaseEndArgs.DataCollectionContext.TestCase.DisplayName);
                }

                if (_lastMessage != null)
                {
                    _dataSink.SendData(testCaseEndArgs.DataCollectionContext, "Stryker.Coverage", _lastMessage);
                }
                _lastMessage = null;
            }
        }

        public void TestSessionEnd(TestSessionEndArgs testSessionEndArgs)
        {
            _client.Dispose();
            _client = null;
            _server.RaiseNewClientEvent -= ConnectionEstablished;
        }
    }
}
