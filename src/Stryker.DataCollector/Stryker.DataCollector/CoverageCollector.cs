using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Xml;
using Microsoft.VisualStudio.TestPlatform.ObjectModel;
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
        private int _activeMutation = -1;
        private Action<string> _logger;
        private readonly IDictionary<string, int> _mutantTestedBy = new Dictionary<string, int>();
        private readonly ICollection<TestCoverageInfo> _testsToMutantCoverage = new List<TestCoverageInfo>();

        private string _controlClassName;
        private Type _controller;
        private FieldInfo _activeMutantField;
        private string _coverageReportFile;

        private MethodInfo _getCoverageData;
        private IList<int> _mutationCoveredOutsideTests;
        private TestCase _currentTestCase;
        private TestCase _previousTest;

        private const string AnyId = "*";
        private const string TemplateForConfiguration =
            @"<InProcDataCollectionRunSettings><InProcDataCollectors><InProcDataCollector {0}>
<Configuration>{1}</Configuration></InProcDataCollector></InProcDataCollectors></InProcDataCollectionRunSettings>";

        public const string PropertyName = "Stryker.Coverage";
        public const string OutOfTestsPropertyName = "Stryker.Coverage.OutOfTests";

        public string MutantList => string.Join(",", _mutantTestedBy.Values.Distinct());

        public static string GetVsTestSettings(bool needCoverage,
            IEnumerable<(int mutant, IEnumerable<Guid> coveringTests)> mutantTestsMap,
            string helperNameSpace,
            string coverageCaptureFile = null)
        {
            var codeBase = typeof(CoverageCollector).GetTypeInfo().Assembly.Location;
            var qualifiedName = typeof(CoverageCollector).AssemblyQualifiedName;
            var friendlyName = typeof(CoverageCollector).ExtractAttribute<DataCollectorFriendlyNameAttribute>()
                .FriendlyName;
            // ReSharper disable once PossibleNullReferenceException
            var uri = (typeof(CoverageCollector).GetTypeInfo()
                    .GetCustomAttributes(typeof(DataCollectorTypeUriAttribute), false).First() as
                DataCollectorTypeUriAttribute).TypeUri;
            var line =
                $"friendlyName=\"{friendlyName}\" uri=\"{uri}\" codebase=\"{codeBase}\" assemblyQualifiedName=\"{qualifiedName}\"";
            var configuration = new StringBuilder();
            configuration.Append("<Parameters>");
            if (needCoverage)
            {
                configuration.Append($"<Coverage file='{coverageCaptureFile}'/>");
            }

            if (mutantTestsMap != null)
            {
                foreach (var (mutant, coveringTests) in mutantTestsMap)
                {
                    configuration.AppendFormat("<Mutant id='{0}' tests='{1}'/>", mutant,
                        coveringTests == null ? "" : string.Join(",", coveringTests));
                }
            }

            configuration.Append($"<MutantControl name='{helperNameSpace}.MutantControl'/>");
            configuration.Append("</Parameters>");

            return string.Format(TemplateForConfiguration, line, configuration);
        }

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

        private void OnAssemblyLoaded(object sender, AssemblyLoadEventArgs args)
        {
            var assembly = args.LoadedAssembly;
            FindControlType(assembly);
        }

        private void FindControlType(Assembly assembly)
        {
            if (_controller != null)
            {
                return;
            }

            _controller = assembly.ExportedTypes.FirstOrDefault(t => t.FullName == _controlClassName);
            if (_controller == null)
            {
                return;
            }

            _activeMutantField = _controller.GetField("ActiveMutant");
            var coverageControlField = _controller.GetField("CaptureCoverage");
            _getCoverageData = _controller.GetMethod("GetCoverageData");

            if (_coverageOn)
            {
                coverageControlField.SetValue(null, true);
            }

            _activeMutantField.SetValue(null, _activeMutation);
        }

        private void SetActiveMutation(string id)
        {
            _activeMutation = GetActiveMutantForThisTest(id);
            if (_activeMutantField != null)
            {
                _activeMutantField.SetValue(null, _activeMutation);
            }
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
                if (coverage.Attributes != null)
                {
                    _coverageReportFile = coverage.Attributes["file"].Value;
                }
            }
        }

        private int GetActiveMutantForThisTest(string testId)
        {
            if (_mutantTestedBy.ContainsKey(testId))
            {
                return _mutantTestedBy[testId];
            }

            return !_mutantTestedBy.ContainsKey(AnyId) ? -1 : _mutantTestedBy[AnyId];
        }

        private void ParseTestMapping(XmlNodeList testMapping)
        {
            var mutations = new HashSet<int>();
            for (var i = 0; i < testMapping.Count; i++)
            {
                var current = testMapping[i];
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
            _previousTest = _currentTestCase;
            _currentTestCase = testCaseStartArgs.TestCase;
            if (_coverageOn)
            {
                // see if any mutation was executed outside a test
                // except for first (assumed to be covered by the first test)
                    CaptureCoverageOutsideTests();

                return;
            }

            // we need to set the proper mutant
            SetActiveMutation(testCaseStartArgs.TestCase.Id.ToString());

            Log($"Test {_currentTestCase.FullyQualifiedName} starts against mutant {_activeMutation} (var).");
        }

        public void TestCaseEnd(TestCaseEndArgs testCaseEndArgs)
        {
            Log($"Test {testCaseEndArgs.DataCollectionContext.TestCase.FullyQualifiedName} ends.");
            if (!_coverageOn)
            {
                return;
            }

            PublishCoverageData(testCaseEndArgs.DataCollectionContext);
        }

        private void PublishCoverageData(DataCollectionContext dataCollectionContext)
        {
            var covered = RetrieveCoverData();
            var testCoverageInfo = new TestCoverageInfo(_currentTestCase,
                _previousTest,
                covered[0],
                covered[1],
                _mutationCoveredOutsideTests);
            _testsToMutantCoverage.Add(testCoverageInfo);

            var coverData = testCoverageInfo.GetCoverageAsString();
            if (coverData == string.Empty)
            {
                // test covers no mutant, but empty string is not a valid value
                coverData = " ";
            }

            _dataSink.SendData(dataCollectionContext, PropertyName, coverData);
            if (!testCoverageInfo.HasLeakedMutations) { return; }

            // report any mutations covered before this test was executed
            _dataSink.SendData(dataCollectionContext, OutOfTestsPropertyName,
                testCoverageInfo.GetLeakedMutationsAsString());
            _mutationCoveredOutsideTests.Clear();
        }

        public IList<int>[] RetrieveCoverData()
            => (IList<int>[])_getCoverageData?.Invoke(null, new object[] { });

        private void CaptureCoverageOutsideTests()
        {
            var covered = RetrieveCoverData();
            if (covered?[0] != null)
            {
                _mutationCoveredOutsideTests =
                    covered[1] != null ? covered[0].Union(covered[1]).ToList() : covered[0].ToList();
            }
            else if (covered?[1] != null)
            {
                _mutationCoveredOutsideTests = covered[1].ToList();
            }
        }

        public void TestSessionEnd(TestSessionEndArgs testSessionEndArgs)
        {
            Log($"TestSession ends.");
            // report any mutations covered after the last test
            CaptureCoverageOutsideTests();
            if ((_mutationCoveredOutsideTests?.Count ?? 0) > 0)
            {
                _testsToMutantCoverage.Add(new TestCoverageInfo(null,
                    _currentTestCase,
                    null,
                    null,
                    _mutationCoveredOutsideTests));
            }
            SaveReport();
        }

        private void SaveReport()
        {
            if (string.IsNullOrEmpty(_coverageReportFile) || _testsToMutantCoverage.Count == 0)
            {
                return;
            }

            var reportBuilder = new StringBuilder(TestCoverageInfo.JSonSizeEstimate*_testsToMutantCoverage.Count).
                Append('[').
                Append(string.Join(",", _testsToMutantCoverage)).
                Append(']');
            Directory.CreateDirectory(Path.GetDirectoryName(_coverageReportFile));
            File.WriteAllText(_coverageReportFile, reportBuilder.ToString());
        }

        private readonly struct TestCoverageInfo
        {
            public const int JSonSizeEstimate = 100;
            private readonly TestCase _test;
            private readonly TestCase _previousTest;
            private readonly IList<int> _coveredMutations;
            private readonly IList<int> _coveredStaticMutations;
            private readonly IList<int> _leakedMutationsFromPreviousTest;

            public TestCoverageInfo(TestCase test, TestCase previousTest, IList<int> coveredMutations,
                IList<int> coveredStaticMutations, IList<int> leakedMutationsFromPreviousTest)
            {
                _test = test;
                _previousTest = previousTest;
                _coveredMutations = coveredMutations;
                _coveredStaticMutations = coveredStaticMutations;
                _leakedMutationsFromPreviousTest = leakedMutationsFromPreviousTest;
            }

            public string GetCoverageAsString() => string.Join(",", _coveredMutations) + ";" + string.Join(",", _coveredStaticMutations);

            public bool HasLeakedMutations => (_leakedMutationsFromPreviousTest?.Count ?? 0) > 1;

            public string GetLeakedMutationsAsString() =>
                HasLeakedMutations ? string.Join(",", _coveredStaticMutations) : string.Empty;

            public override string ToString()
            {
                var reportBuilder = new StringBuilder(JSonSizeEstimate);
                reportBuilder.Append('{');
                reportBuilder.Append(_test != null ? $"\"testCaseId\":\"{_test.Id}\"" : "\"testCaseId\":\"NoTest\"");
                reportBuilder.Append(_test != null ? $",\"testCaseName\":\"{_test.FullyQualifiedName}\"" : "");
                if (_coveredMutations != null)
                {
                    reportBuilder.Append($",\"mutationsCovered\":[{string.Join(",", _coveredMutations.Select(i => i.ToString()))}]");
                }

                if (_coveredStaticMutations != null)
                {
                    reportBuilder.Append($",\"staticMutationsCovered\":[{string.Join(",", _coveredStaticMutations.Select(i => i.ToString()))}]");
                }

                if (_leakedMutationsFromPreviousTest != null)
                {
                    reportBuilder.Append(
                        $",\"mutationsCoveredBeforeStart\":[{string.Join(",", _leakedMutationsFromPreviousTest.Select(i => i.ToString()))}]");
                }

                if (_previousTest != null)
                {
                    reportBuilder.Append($",\"testCaseBeforeId\":\"{_previousTest.Id}\"");
                }
                reportBuilder.AppendLine("}");
                return reportBuilder.ToString();
            }
        }
    }
}
