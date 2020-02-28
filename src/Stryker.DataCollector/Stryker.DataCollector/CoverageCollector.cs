using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Xml;
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
        private bool _useEnv;
        private bool _coverageOn;
        private bool _firstTestDone;
        private string _lastMessage;
        private Action<string> _logger;
        private readonly IDictionary<string, int> _mutantTestedBy = new Dictionary<string, int>();
        private readonly IList<int> _mutantsAllWaysTested = new List<int>();

        public const string EnvMode = "env";
        public const string PipeMode = "pipe";
        public const string VarMode = "var";
        public const string ModeEnvironmentVariable = "CaptureCoverage";
        private const string EnvName = "CoveredMutants";
        private FieldInfo _activeMutantField;
        private FieldInfo _coverageControlField;
        private string _controlClassName;
        private Type _controller;
        private readonly object _lck = new object();

        private MethodInfo _getCoverageData;

        private const string TemplateForConfiguration = 
            @"<InProcDataCollectionRunSettings><InProcDataCollectors><InProcDataCollector {0}>
<Configuration>{1}</Configuration></InProcDataCollector></InProcDataCollectors></InProcDataCollectionRunSettings>";

        public string MutantList => string.Join(",", _mutantTestedBy.Values.Union(_mutantsAllWaysTested));

        public static string GetVsTestSettings(bool needCoverage, bool useVar, Dictionary<int, IList<string>> mutantTestsMap, string helpNameSpace)
        {
            var codeBase = typeof(CoverageCollector).GetTypeInfo().Assembly.Location;
            var qualifiedName = typeof(CoverageCollector).AssemblyQualifiedName;
            var friendlyName = typeof(CoverageCollector).ExtractAttribute<DataCollectorFriendlyNameAttribute>().FriendlyName;
            // ReSharper disable once PossibleNullReferenceException
            var uri = (typeof(CoverageCollector).GetTypeInfo().GetCustomAttributes(typeof(DataCollectorTypeUriAttribute), false).First() as
                DataCollectorTypeUriAttribute).TypeUri;
            var line= $"friendlyName=\"{friendlyName}\" uri=\"{uri}\" codebase=\"{codeBase}\" assemblyQualifiedName=\"{qualifiedName}\"";
            var configuration = new StringBuilder();
            configuration.Append("<Parameters>");
            if (needCoverage)
            {
                configuration.Append($"<Coverage mode='{ (useVar ? VarMode : EnvMode)}' />");
            }
            if (mutantTestsMap != null)
            {
                foreach ( var entry in mutantTestsMap)
                {
                    configuration.AppendFormat("<Mutant id='{0}' tests='{1}'/>", entry.Key,  entry.Value == null ? "" : string.Join(",", entry.Value));
                }
            }

            configuration.Append($"<MutantControl  name='{helpNameSpace}.MutantControl'/>");
            configuration.Append("</Parameters>");
            
            return string.Format(TemplateForConfiguration, line, configuration);
        }

        private static bool WaitOnLck(object lck, Func<bool> predicate, int timeout)
        {
            var watch = new Stopwatch();
            watch.Start();
            lock (lck)
            {
                while (!predicate() && watch.ElapsedMilliseconds <= timeout)
                {
                    Monitor.Wait(lck, Math.Max(1, (int)(timeout - watch.ElapsedMilliseconds)));
                }
            }

            return predicate();
        }

        public void Initialize(IDataCollectionSink dataCollectionSink)
        {
            this._dataSink = dataCollectionSink;
        }

        public void Init(bool usePipe)
        {
            if (!usePipe)
            {
                return;
            }
            _server = new CommunicationServer("CoverageCollector");
            _server.SetLogger(_logger);
            _server.RaiseNewClientEvent += ConnectionEstablished;
            _server.Listen();
            _usePipe = true;
        }

        public void SetLogger(Action<string> logger)
        {
            _logger = logger;
            _server?.SetLogger(logger);
        }

        public void Log(string message)
        {
            Console.Error.WriteLine(message);
            _logger?.Invoke(message);
        }

        public IDictionary<string, string> GetEnvironmentVariables()
        {
            var result = new Dictionary<string, string>();
            if (_usePipe)
            {
                result["Coverage"]= $"{PipeMode}:{_server.PipeName}";
                result[ModeEnvironmentVariable] = PipeMode;
            }
            else if (_useEnv)
            {
                result["Coverage"]= $"{EnvMode}:{EnvName}";  
                result[ModeEnvironmentVariable] = EnvMode;
            }

            return result;
        }

        // called before any test is run
        public void TestSessionStart(TestSessionStartArgs testSessionStartArgs)
        {            
            var configuration = testSessionStartArgs.Configuration;
            ReadConfiguration(configuration);

            Log($"Test Session start with conf {configuration}.");
        }

        private void GetMutantControlMethods()
        {
            var assemblies = AppDomain.CurrentDomain.GetAssemblies().Where(assembly => !assembly.IsDynamic);
            var types = assemblies
                .SelectMany(x => x.ExportedTypes);

            _controller = types.FirstOrDefault(t => t.FullName == _controlClassName);
            if (_controller == null)
            {
                Log($"Failed to find type {_controlClassName}. Scanned these assemblies:");
                foreach (var assembly in assemblies)
                {
                    Log(assembly.FullName);
                }
                return;
            }
            _activeMutantField = _controller.GetField("ActiveMutant");
            _coverageControlField = _controller.GetField("CaptureCoverage");
            _getCoverageData = _controller.GetMethod("GetCoverageData");
        }

        private void ReadConfiguration(string configuration)
        {
            var node = new XmlDocument();
            node.LoadXml(configuration);

            var testMapping = node.SelectNodes("//Parameters/Mutant");
            if (testMapping !=null)
            {
                for (var i = 0; i < testMapping.Count; i++)
                {
                    var current = testMapping[i];
                    var id = int.Parse(current.Attributes["id"].Value);
                    var tests = current.Attributes["tests"].Value;
                    if (string.IsNullOrEmpty(tests))
                    {
                        _mutantsAllWaysTested.Add(id);
                    }
                    else
                    {
                        foreach (var test in tests.Split(','))
                        {
                            _mutantTestedBy[test] = id;
                        }
                    }
                }
            }

            var nameSpaceNode = node.SelectSingleNode("//Parameters/MutantControl");
            if (nameSpaceNode != null)
            {
                this._controlClassName = nameSpaceNode.Attributes["name"].Value;
            }

            var coverage = node.SelectSingleNode("//Parameters/Coverage");
            if (coverage != null)
            {
                _coverageOn = true;
                var val = coverage.Attributes["mode"].Value;
                if (val == PipeMode)
                {
                    _usePipe = true;
                }
                else if (val == EnvMode)
                {
                    _useEnv = true;
                }
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
            if (!_firstTestDone)
            {
                GetMutantControlMethods();

                if (_coverageOn)
                {
                    _coverageControlField.SetValue(null, true);
                    Init(_usePipe);
                    Log($"Mode: {(_usePipe ? "pipe" : "var")}");
                }

                _firstTestDone = true;
            }
            if (_coverageOn)
            {
                return;
            }
            // we need to set the proper mutant
            var  testId = testCaseStartArgs.TestCase.Id.ToString();
            var mutantId = _mutantTestedBy.ContainsKey(testId) ? _mutantTestedBy[testId] : _mutantsAllWaysTested[0];

            _activeMutantField.SetValue(null, mutantId);
            Log($"Test {testCaseStartArgs.TestCase.FullyQualifiedName} starts against mutant {mutantId} (var).");
        }

        public void TestCaseEnd(TestCaseEndArgs testCaseEndArgs)
        {
            Log($"Test {testCaseEndArgs.DataCollectionContext.TestCase.FullyQualifiedName} ends.");
            if (!_coverageOn)
            {
                return;
            }

            PublishCoverageData(testCaseEndArgs);
        }

        private void PublishCoverageData(TestCaseEndArgs testCaseEndArgs)
        {
            var testCaseDisplayName = testCaseEndArgs.DataCollectionContext.TestCase.DisplayName;
            if (_usePipe)
            {
                if (!WaitOnLck(_lck, () => _client != null, 500))
                {
                    throw new InvalidOperationException("connection");
                }

                _client.SendText($"DUMP {testCaseDisplayName}");
            }

            var coverData = RetrieveCoverData();
            // null means we failed to retrieve data
            if (coverData != null)
            {
                if (coverData == string.Empty)
                {
                    // test covers no mutant, but empty string is not a valid value
                    coverData = " ";
                }

                _dataSink.SendData(testCaseEndArgs.DataCollectionContext, "Stryker.Coverage", coverData);
            }
            else
            {
                Log($"Failed to retrieve coverage data for {testCaseEndArgs.DataCollectionContext.TestCase.FullyQualifiedName}");
            }
        }

        public string RetrieveCoverData()
        {
            string coverData;
            if (_usePipe)
            {
                if (!WaitOnLck(_lck, () => _lastMessage != null, 5000))
                {
                    // Failed to retrieve coverage data for testCase
                    return null;
                }

                coverData = _lastMessage;
                _lastMessage = null;
            }
            else if (_useEnv)
            {
                // get coverage 
                coverData = Environment.GetEnvironmentVariable(EnvName);
                Environment.SetEnvironmentVariable(EnvName, null);
            }
            else
            {
                var (covered, staticMutants) = ((IList<int>, IList<int>)) _getCoverageData.Invoke(null, new object[]{});
                coverData = string.Join(",", covered) + ";" + string.Join(",", staticMutants);
            }

            return coverData;
        }

        public void TestSessionEnd(TestSessionEndArgs testSessionEndArgs)
        {
            Log($"TestSession ends.");
            if (!_usePipe) return;
            _client?.Dispose();
            _client = null;
            _server.RaiseNewClientEvent -= ConnectionEstablished;
        }
    }
}
