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
    public interface IEnvironmentVariablesHandler
    {
        void SetVariable(string name, string value);
    }

    internal class EnvironmentVariablesHandler : IEnvironmentVariablesHandler
    {
        public void SetVariable(string name, string value)
        {
            Environment.SetEnvironmentVariable(name, value);
        }
    }

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
        private Action<string> _logger;
        private readonly IEnvironmentVariablesHandler _environmentVariablesHandler;
        private readonly IDictionary<string, int> _mutantTestedBy = new Dictionary<string, int>();
        private readonly IList<int> _mutantsAllWaysTested = new List<int>();

        public const string ModeEnvironmentVariable = "CaptureCoverage";
        public const string EnvMode = "env";
        public const string PipeMode = "pipe";
        private const string EnvName = "CoveredMutants";

        private readonly IDictionary<string, string> _options = new Dictionary<string, string>();
        private const string TemplateForConfiguration = 
            @"<InProcDataCollectionRunSettings><InProcDataCollectors><InProcDataCollector {0}>
<Configuration>{1}</Configuration></InProcDataCollector></InProcDataCollectors></InProcDataCollectionRunSettings>";

        private FieldInfo _activeMutantField;
        private const string ActiveMutantVariable = "ActiveMutation";

        public CoverageCollector() : this(new EnvironmentVariablesHandler())
        {}

        public CoverageCollector(IEnvironmentVariablesHandler environmentVariablesHandler)
        {
            _environmentVariablesHandler = environmentVariablesHandler;
        }

        public string MutantList => string.Join(",", _mutantTestedBy.Values.Union(_mutantsAllWaysTested));

        public static string GetVsTestSettings(bool needCoverage, bool usePipe,
            Dictionary<int, IList<string>> mutantTestsMap)
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
                configuration.AppendFormat("<Environment name=\"{0}\" value=\"{1}\" />", ModeEnvironmentVariable,
                usePipe ? PipeMode : EnvMode);
            }
            if (mutantTestsMap != null)
            {
                foreach ( var entry in mutantTestsMap)
                {
                    configuration.AppendFormat("<Mutant id='{0}' tests='{1}'/>", entry.Key,  entry.Value == null ? "" : string.Join(",", entry.Value));
                }
            }
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
           // Init(true);
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
            else
            {
                result["Coverage"]= $"{EnvMode}:{EnvName}";
                result[ModeEnvironmentVariable] = EnvMode;
            }

            return result;
        }

        public void TestSessionStart(TestSessionStartArgs testSessionStartArgs)
        {            
            var configuration = testSessionStartArgs.Configuration;
            ReadConfiguration(configuration);

            _options.TryGetValue(ModeEnvironmentVariable, out var coverageString);
            _coverageOn = coverageString != null;
            if (_coverageOn)
            {
                Init(coverageString == PipeMode);

                foreach (var environmentVariable in GetEnvironmentVariables())
                {
                    _environmentVariablesHandler.SetVariable(environmentVariable.Key, environmentVariable.Value);
                }

                Log($"Mode: {(_usePipe ? "pipe" : "env")}");
            }

            Log($"Test Session start with conf {configuration}.");
        }

        private void ReadConfiguration(string configuration)
        {
            var node = new XmlDocument();
            node.LoadXml(configuration);
            var parameters = node.SelectNodes("//Parameters/Environment");
            for (var i = 0; i < parameters.Count; i++)
            {
                var current = parameters[i];
                if (current.Attributes == null)
                {
                    Log("Invalid environment entry in configuration.");
                    continue;
                }
                var name = current.Attributes["name"].Value;
                var value = current.Attributes["value"].Value;
                _options.Add(name, value);
            }

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
            if (_coverageOn)
            {
                return;
            }
            // we need to set the proper mutant
            var  testId = testCaseStartArgs.TestCase.Id.ToString();
            int mutantId;
            if (_mutantTestedBy.ContainsKey(testId))
            {
                mutantId = _mutantTestedBy[testId];
            }
            else
            {
                mutantId =  _mutantsAllWaysTested[0];
            }

            if (_activeMutantField == null)
            {
                var assemblies = AppDomain.CurrentDomain.GetAssemblies().Where(assembly => !assembly.IsDynamic)
                    .Where( assembly => assembly.FullName.Contains("NFluent") && !assembly.FullName.Contains("Tests"));
                var types = assemblies
                    .SelectMany(x => x.ExportedTypes);
                Type controller;

                controller = types.FirstOrDefault(t => t.Name == "MutantControl");
                _activeMutantField = controller.GetField("ActiveMutant");
            }
            
            if (_activeMutantField == null)
            {
                _environmentVariablesHandler.SetVariable(ActiveMutantVariable, mutantId.ToString());
                Log($"Test {testCaseStartArgs.TestCase.FullyQualifiedName} starts against mutant {mutantId} (env).");
            }
            else
            {
                _activeMutantField.SetValue(null, mutantId);
                Log($"Test {testCaseStartArgs.TestCase.FullyQualifiedName} starts against mutant {mutantId} (var).");
            }
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
                Log(
                    $"Failed to retrieve coverage data for {testCaseEndArgs.DataCollectionContext.TestCase.FullyQualifiedName}");
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
            Log($"TestSession ends.");
            if (!_usePipe) return;
            _client?.Dispose();
            _client = null;
            _server.RaiseNewClientEvent -= ConnectionEstablished;
        }
    }
}
