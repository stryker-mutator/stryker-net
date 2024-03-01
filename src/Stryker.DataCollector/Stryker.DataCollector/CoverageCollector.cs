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
        private int _activeMutation = -1;
        private bool _reportFailure;

        private Action<string> _logger;
        private readonly IDictionary<string, int> _mutantTestedBy = new Dictionary<string, int>();

        private string _controlClassName;
        private Type _mutantControlType;
        private FieldInfo _activeMutantField;

        private MethodInfo _getCoverageData;
        private IList<int> _mutationCoveredOutsideTests;

        private const string AnyId = "*";
        private const string TemplateForConfiguration =
            @"<InProcDataCollectionRunSettings><InProcDataCollectors><InProcDataCollector {0}>
<Configuration>{1}</Configuration></InProcDataCollector></InProcDataCollectors></InProcDataCollectionRunSettings>";

        public const string PropertyName = "Stryker.Coverage";
        public const string OutOfTestsPropertyName = "Stryker.Coverage.OutOfTests";
        public const string Coveragelog = "CoverageLog";

        public string MutantList => string.Join(",", _mutantTestedBy.Values.Distinct());

        public static string GetVsTestSettings(bool needCoverage,
            IEnumerable<(int mutant, IEnumerable<Guid> coveringTests)> mutantTestsMap,
            string helperNameSpace)
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
                configuration.Append("<Coverage/>");
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

            // if assembly was not loaded yet, wait for assembly to load
            AppDomain.CurrentDomain.AssemblyLoad += OnAssemblyLoaded;

            // scan loaded assemblies, just in case the test assembly is already loaded
            var assemblies = AppDomain.CurrentDomain.GetAssemblies().Where(a => !a.IsDynamic);

            foreach (var assembly in assemblies)
            {
                FindControlType(assembly);
            }

            Log($"Test Session start with conf {configuration}.");
        }

        private void OnAssemblyLoaded(object sender, AssemblyLoadEventArgs args)
        {
            var assembly = args.LoadedAssembly;
            FindControlType(assembly);
        }

        private void FindControlType(Assembly assembly)
        {
            if (_mutantControlType != null)
            {
                return;
            }

            _mutantControlType = assembly.DefinedTypes?.FirstOrDefault(t => t.FullName == _controlClassName);
            if (_mutantControlType == null)
            {
                return;
            }
            _activeMutantField = _mutantControlType.GetField("ActiveMutant");
            var coverageControlField = _mutantControlType.GetField("CaptureCoverage");
            _getCoverageData = _mutantControlType.GetMethod("GetCoverageData");
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
            }

            SetActiveMutation(AnyId);
        }

        private int GetActiveMutantForThisTest(string testId)
        {
            if (_mutantTestedBy.TryGetValue(testId, out var test))
            {
                return test;
            }

            return _mutantTestedBy.TryGetValue(AnyId, out var value) ? value : -1;
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
            if (_coverageOn)
            {
                // see if any mutation was executed outside a test
                // except for first (assumed to be covered by the first test)
                CaptureCoverageOutsideTests();

                return;
            }

            // we need to set the proper mutant
            var testCase = testCaseStartArgs.TestCase;
            SetActiveMutation(testCase.Id.ToString());

            Log($"Test {testCase.FullyQualifiedName} starts against mutant {_activeMutation} (var).");
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
            if (covered == null)
            {
                // no test covered any mutations, so the controller was never properly initialized
                _dataSink.SendData(dataCollectionContext, PropertyName, ";");
                if (!_reportFailure)
                {
                    _dataSink.SendData(dataCollectionContext, Coveragelog, $"Did not find type {_controlClassName}. This indicates Stryker failed to copy the mutated assembly for test.");
                    _reportFailure = true;
                }
                return;
            }

            var testCoverageInfo = new TestCoverageInfo(
                covered[0],
                covered[1],
                _mutationCoveredOutsideTests);

            var coverData = testCoverageInfo.GetCoverageAsString();
            _dataSink.SendData(dataCollectionContext, PropertyName, coverData);
            if (!testCoverageInfo.HasLeakedMutations)
            {
                return;
            }

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

        public void TestSessionEnd(TestSessionEndArgs testSessionEndArgs) => Log($"TestSession ends.");

        private readonly struct TestCoverageInfo
        {
            private readonly IList<int> _coveredMutations;
            private readonly IList<int> _coveredStaticMutations;
            private readonly IList<int> _leakedMutationsFromPreviousTest;

            public TestCoverageInfo(IList<int> coveredMutations,
                IList<int> coveredStaticMutations, IList<int> leakedMutationsFromPreviousTest)
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
