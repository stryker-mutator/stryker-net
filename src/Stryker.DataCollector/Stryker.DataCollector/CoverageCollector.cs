using System;
using System.Collections.Generic;
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
        private bool _usePipe;
        private readonly object _lck = new object();
        private string _lastMessage;
        private bool _coverageOn;

        public const string ModeEnvironmentVariable = "CaptureCoverage";
        public const string EnvMode = "env";
        public const string PipeMode = "pipe";
        private const string EnvName = "CoveredMutants";

        private const string TemplateForConfiguration = 
            @"<InProcDataCollectionRunSettings><InProcDataCollectors><InProcDataCollector {0} >
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
            Init(true);
        }

        public void Init(bool usePipe)
        {
            if (usePipe)
            {
                _server = new CommunicationServer("CoverageCollector");
                _server.RaiseNewClientEvent += ConnectionEstablished;
                _server.Listen();
                _usePipe = true;
            }
        }

        public IDictionary<string, string> GetEnvironmentVariables()
        {
            var result = new Dictionary<string, string>();
            if (_usePipe)
            {
                result["Coverage"]= $"{PipeMode}:{_server.PipeName}";
                result[ModeEnvironmentVariable] = PipeMode;
            }
            else
            {
                result["Coverage"]= $"{EnvMode}:{EnvName}";
                result[ModeEnvironmentVariable] = EnvMode;
            }

            return result;
        }

        public void TestSessionStart(TestSessionStartArgs testSessionStartArgs)
        {
            var coverageString = Environment.GetEnvironmentVariable(ModeEnvironmentVariable);
            _coverageOn = coverageString != null;
            _usePipe = (coverageString == PipeMode);

            foreach (var environmentVariable in GetEnvironmentVariables())
            {
                Environment.SetEnvironmentVariable(environmentVariable.Key, environmentVariable.Value);
            }
        }

        private void ConnectionEstablished(object s, ConnectionEventArgs e)
        {
            lock (_lck)
            {
                _client = e.Client;
                _client.RaiseReceivedMessage += ReceivedMutant;
                Monitor.Pulse(_lck);
            }
        }

        private void ReceivedMutant(object sender, string args)
        {
            lock (_lck)
            {
                _lastMessage = args;
                Monitor.Pulse(_lck);
            }
        }

        public void TestCaseStart(TestCaseStartArgs testCaseStartArgs)
        {
        }

        public void TestCaseEnd(TestCaseEndArgs testCaseEndArgs)
        {
            if (!_coverageOn) return;
            var testCaseDisplayName = testCaseEndArgs.DataCollectionContext.TestCase.DisplayName;
            if (_usePipe)
            {
                if (!WaitOnLck(_lck, () => _client != null, 500))
                {
                    throw new InvalidOperationException("connection");
                }
                _client.SendText($"DUMP {testCaseDisplayName}");
            }

            var coverData = RetrieveCoverData(testCaseDisplayName);
            _dataSink.SendData(testCaseEndArgs.DataCollectionContext, "Stryker.Coverage", coverData);
        }

        public string RetrieveCoverData(string testCase)
        {
            string coverData;
            if (_usePipe)
            {
                if (!WaitOnLck(_lck, () => _lastMessage != null, 500))
                {
                    // Failed to retrieve coverage data for {testCase
                    return string.Empty;
                }

                coverData = _lastMessage;

                _lastMessage = null;
            }
            else
            {
                // get coverage 
                coverData = Environment.GetEnvironmentVariable(EnvName);
                Environment.SetEnvironmentVariable(EnvName, null);
            }

            return coverData;
        }

        public void TestSessionEnd(TestSessionEndArgs testSessionEndArgs)
        {
            _client.Dispose();
            _client = null;
            _server.RaiseNewClientEvent -= ConnectionEstablished;
        }
    }
}
