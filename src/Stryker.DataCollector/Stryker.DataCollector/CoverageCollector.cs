using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Xml;
using Microsoft.VisualStudio.TestPlatform.ObjectModel.DataCollection;
using Microsoft.VisualStudio.TestPlatform.ObjectModel.DataCollector.InProcDataCollector;
using Microsoft.VisualStudio.TestPlatform.ObjectModel.InProcDataCollector;

namespace Stryker.DataCollector
{
    [DataCollectorFriendlyName("StrykerCoverage")]
    [DataCollectorTypeUri("https://stryker-mutator.io/")]
    public class CoverageCollector : InProcDataCollection
    {
        private IDataCollectionSink _dataSink;
        private bool _coverageOn;
        private bool _traceOn;
        private int _activeMutation = -1;
        private Action<string> _logger;
        private readonly IDictionary<string, int> _mutantTestedBy = new Dictionary<string, int>();

        // fields and methods from MutantControl for interaction during tests
        private string _controlClassName;
        private readonly object _synchro = new object();
        private Type _controller;
        private MethodInfo _setActiveMutant;
        private MethodInfo _activeMutantSeen;
        private MethodInfo _getCoverageData;
        private MethodInfo _getTraceData;

        private ISet<int> _mutationCoveredOutsideTests;

        private const string AnyId = "*";
        private const string TemplateForConfiguration =
            @"<InProcDataCollectionRunSettings><InProcDataCollectors><InProcDataCollector {0}>
<Configuration>{1}</Configuration></InProcDataCollector></InProcDataCollectors></InProcDataCollectionRunSettings>";

        public const string PropertyName = "Stryker.Coverage";
        public const string OutOfTestsPropertyName = "Stryker.Coverage.OutOfTests";
        public const string ActiveMutationSeen = "Stryker.Control.MutantSeen";
        public const string MutationHitTrace = "Stryker.Control.Trace";

        public string MutantList => string.Join(",", _mutantTestedBy.Values.Distinct());

        public void Initialize(IDataCollectionSink dataCollectionSink)
        {
            _dataSink = dataCollectionSink;
            SetLogger(Console.WriteLine);
        }

        public void SetLogger(Action<string> logger) => _logger = logger;

        public void Log(string message) => _logger?.Invoke(message);

        // called before any test is run
        public void TestSessionStart(TestSessionStartArgs testSessionStartArgs)
        {
            var configuration = testSessionStartArgs.Configuration;
            ReadConfiguration(configuration);
            // scan loaded assembly, just in case the test assembly is already loaded
            var assemblies = AppDomain.CurrentDomain.GetAssemblies().Where(assembly => !assembly.IsDynamic);

            foreach (var assembly in assemblies)
            {
                FindControlType(assembly);
            }

            AppDomain.CurrentDomain.AssemblyLoad += OnAssemblyLoaded;

            Log($"Test Session start with conf {configuration}.");
        }

        private void OnAssemblyLoaded(object sender, AssemblyLoadEventArgs args) => FindControlType(args.LoadedAssembly);

        private void FindControlType(Assembly assembly)
        {
            lock(_synchro)
            {
                if (_controller != null)
                {
                    return;
                }

                var scan =  assembly.ExportedTypes.FirstOrDefault(t => t.FullName == _controlClassName); 
                if (scan == null)
                {
                    return;
                }

                _controller = scan;
            }

            _setActiveMutant = _controller.GetMethod("SetActiveMutant");
            _activeMutantSeen = _controller.GetMethod("ActiveMutantSeen");
            _getCoverageData = _controller.GetMethod("GetCoverageData");
            _getTraceData = _controller.GetMethod("GetTrace");

            _controller.GetMethod("CaptureCoverage")?.Invoke(null, new object[] {_coverageOn});
            _controller.GetMethod("CaptureTrace")?.Invoke(null, new object[] {_traceOn});

            _setActiveMutant?.Invoke(null, new object[] {_activeMutation});
        }

        private void SetActiveMutationForTest(string id = null)
        {
            _activeMutation = id == null ? -1 : GetActiveMutantForThisTest(id);

            _setActiveMutant?.Invoke(null, new object[] {_activeMutation});
        }

        public static string GetVsTestSettings(bool needCoverage,
            IEnumerable<(int mutant, IEnumerable<Guid> coveringTests)> mutantTestsMap,
            string helperNameSpace,
            bool withTrace)
        {
            var codeBase = typeof(CoverageCollector).GetTypeInfo().Assembly.Location;
            var qualifiedName = typeof(CoverageCollector).AssemblyQualifiedName;
            var friendlyName = typeof(CoverageCollector).ExtractAttribute<DataCollectorFriendlyNameAttribute>()
                .FriendlyName;
            var uri = typeof(CoverageCollector).GetTypeInfo()
                .GetCustomAttributes(typeof(DataCollectorTypeUriAttribute), false).Cast<DataCollectorTypeUriAttribute>().First().TypeUri;
            var line =
                $"friendlyName=\"{friendlyName}\" uri=\"{uri}\" codebase=\"{codeBase}\" assemblyQualifiedName=\"{qualifiedName}\"";
            var configuration = new StringBuilder();
            configuration.Append("<Parameters>");

            if (needCoverage)
            {
                configuration.Append("<Coverage/>");
            }

            if (withTrace)
            {
                configuration.Append("<Trace/>");
            }

            if (mutantTestsMap != null)
            {
                foreach (var (mutant, coveringTests) in mutantTestsMap)
                {
                    configuration.AppendFormat("<Mutant id='{0}' tests='{1}'/>", mutant,
                        coveringTests == null ? "" : string.Join(",", coveringTests));
                }
            }

            configuration.Append($"<MutantControl name='{helperNameSpace}.MutantControl'/>").
                Append("</Parameters>");

            return string.Format(TemplateForConfiguration, line, configuration);
        }

        private void ReadConfiguration(string configuration)
        {
            var node = new XmlDocument();
            node.LoadXml(configuration);

            var testMapping = node.SelectNodes("//Parameters/Mutant");
            if (testMapping != null)
            {
                ParseTestMapping(testMapping);
            }

            var nameSpaceNode = node.SelectSingleNode("//Parameters/MutantControl");
            if (nameSpaceNode?.Attributes != null)
            {
                _controlClassName = nameSpaceNode.Attributes["name"].Value;
            }

            var coverage = node.SelectSingleNode("//Parameters/Coverage");
            if (coverage != null)
            {
                _coverageOn = true;
            }

            var trace = node.SelectSingleNode("//Parameters/Trace");
            if (trace != null)
            {
                _traceOn = true;
            }

            SetActiveMutationForTest(AnyId);
        }

        private int GetActiveMutantForThisTest(string testId)
        {
            if (_mutantTestedBy.ContainsKey(testId))
            {
                return _mutantTestedBy[testId];
            }

            return _mutantTestedBy.ContainsKey(AnyId) ? _mutantTestedBy[AnyId] : -1;
        }

        private void ParseTestMapping(XmlNodeList testMapping)
        {
            var mutations = new HashSet<int>();
            for (var i = 0; i < testMapping.Count; i++)
            {
                var current = testMapping[i];
                if (current?.Attributes == null)
                {
                    continue;
                }
                var id = int.Parse(current.Attributes["id"].Value);
                var tests = current.Attributes["tests"].Value;
                mutations.Add(id);
                if (string.IsNullOrEmpty(tests))
                {
                    _mutantTestedBy[AnyId] = id;
                }
                else
                {
                    foreach (var test in tests.Split(','))
                    {
                        _mutantTestedBy[test] = id;
                    }
                }
            }

            // special case if we test only one mutant
            if (mutations.Count == 1)
            {
                _mutantTestedBy[AnyId] = mutations.First();
            }
        }

        public void TestCaseStart(TestCaseStartArgs testCaseStartArgs)
        {
            if (_coverageOn)
            {
                // see if any mutation was executed outside a test
                // except for first (assumed to be covered by the first test)
                CaptureCoverageOutsideTests();
                return;
            }

            // we need to set the proper mutant
            var testCase = testCaseStartArgs.TestCase;
            SetActiveMutationForTest(testCase.Id.ToString());

            Log($"Test {testCase.FullyQualifiedName} starts against mutant {_activeMutation} (var).");
        }

        public void TestCaseEnd(TestCaseEndArgs testCaseEndArgs)
        {
            Log($"Test {testCaseEndArgs.DataCollectionContext.TestCase.FullyQualifiedName} ends.");
            if (!_coverageOn)
            {
                if ((bool) _activeMutantSeen.Invoke(null, null))
                {
                    _dataSink.SendData(testCaseEndArgs.DataCollectionContext, ActiveMutationSeen, _activeMutation.ToString());
                }

                if (_traceOn)
                {
                    var traceData = string.Join(",", (List<int>) _getTraceData.Invoke(null, null));
                    _dataSink.SendData(testCaseEndArgs.DataCollectionContext, MutationHitTrace, traceData);
                }
                return;
            }
            PublishCoverageData(testCaseEndArgs.DataCollectionContext);
            SetActiveMutationForTest();
        }

        private void PublishCoverageData(DataCollectionContext dataCollectionContext)
        {
            var covered = RetrieveCoverData();
            var testCoverageInfo = new TestCoverageInfo(
                covered[0],
                covered[1],
                _mutationCoveredOutsideTests);

            var coverData = testCoverageInfo.GetCoverageAsString();

            _dataSink.SendData(dataCollectionContext, PropertyName, coverData);
            if (!testCoverageInfo.HasLeakedMutations) { return; }

            // report any mutations covered before this test was executed
            _dataSink.SendData(dataCollectionContext, OutOfTestsPropertyName,
                testCoverageInfo.GetLeakedMutationsAsString());
            _mutationCoveredOutsideTests.Clear();
        }
        
        public ISet<int>[] RetrieveCoverData()
            => (ISet<int>[])_getCoverageData?.Invoke(null, new object[] { });

        private void CaptureCoverageOutsideTests()
        {
            var covered = RetrieveCoverData();
            if (covered?[0] != null)
            {
                _mutationCoveredOutsideTests = new HashSet<int>(
                    covered[1] != null ? covered[0].Union(covered[1]) : covered[0]);
            }
        }

        public void TestSessionEnd(TestSessionEndArgs testSessionEndArgs) => Log("TestSession ends.");

        private readonly struct TestCoverageInfo
        {
            private readonly ISet<int> _coveredMutations;
            private readonly ISet<int> _coveredStaticMutations;
            private readonly ISet<int> _leakedMutationsFromPreviousTest;

            public TestCoverageInfo(ISet<int> coveredMutations,
                ISet<int> coveredStaticMutations, ISet<int> leakedMutationsFromPreviousTest)
            {
                _coveredMutations = coveredMutations;
                _coveredStaticMutations = coveredStaticMutations;
                _leakedMutationsFromPreviousTest = leakedMutationsFromPreviousTest;
            }

            public string GetCoverageAsString() => string.Join(",", _coveredMutations) + ";" + string.Join(",", _coveredStaticMutations);

            public bool HasLeakedMutations => (_leakedMutationsFromPreviousTest?.Count ?? 0) > 0;

            public string GetLeakedMutationsAsString() =>
                HasLeakedMutations ? string.Join(",", _leakedMutationsFromPreviousTest) : string.Empty;
        }
    }
}
