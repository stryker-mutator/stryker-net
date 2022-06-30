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
    public class CoverageCollector : InProcDataCollection
    {
        private IDataCollectionSink _dataSink;
        private bool _coverageOn;
        private int _activeMutation = -1;
        private Action<string> _logger;
        private readonly IDictionary<string, int> _mutantTestedBy = new Dictionary<string, int>();
        private int? _singleMutant;

        private string _controlClassName;
        private Type _mutantControlType;
        private FieldInfo _activeMutantField;

        private MethodInfo _getCoverageData;
        private IList<int> _mutationCoveredOutsideTests;

        private const string TemplateForConfiguration =
            @"<InProcDataCollectionRunSettings><InProcDataCollectors><InProcDataCollector {0}>
<Configuration>{1}</Configuration></InProcDataCollector></InProcDataCollectors></InProcDataCollectionRunSettings>";

        public const string PropertyName = "Stryker.Coverage";
        public const string OutOfTestsPropertyName = "Stryker.Covrage.OutOfTests";

        public string MutantList => _singleMutant?.ToString() ?? string.Join(",", _mutantTestedBy.Values.Distinct());

        public static string GetVsTestSettings(bool needCoverage, IEnumerable<(int, IEnumerable<Guid>)> mutantTestsMap, string helpNameSpace)
        {
            var codeBase = typeof(CoverageCollector).GetTypeInfo().Assembly.Location;
            var qualifiedName = typeof(CoverageCollector).AssemblyQualifiedName;
            var friendlyName = typeof(CoverageCollector).ExtractAttribute<DataCollectorFriendlyNameAttribute>().FriendlyName;
            // ReSharper disable once PossibleNullReferenceException
            var uri = (typeof(CoverageCollector).GetTypeInfo().GetCustomAttributes(typeof(DataCollectorTypeUriAttribute), false).First() as
                DataCollectorTypeUriAttribute).TypeUri;
            var line = $"friendlyName=\"{friendlyName}\" uri=\"{uri}\" codebase=\"{codeBase}\" assemblyQualifiedName=\"{qualifiedName}\"";
            var configuration = new StringBuilder();
            configuration.Append("<Parameters>");
            if (needCoverage)
            {
                configuration.Append("<Coverage/>");
            }
            if (mutantTestsMap != null)
            {
                foreach (var entry in mutantTestsMap)
                {
                    configuration.AppendFormat("<Mutant id='{0}' tests='{1}'/>", entry.Item1, entry.Item2 == null ? "" : string.Join(",", entry.Item2));
                }
            }

            configuration.Append($"<MutantControl  name='{helpNameSpace}.MutantControl'/>");
            configuration.Append("</Parameters>");

            return string.Format(TemplateForConfiguration, line, configuration);
        }

        public void Initialize(IDataCollectionSink dataCollectionSink) => this._dataSink = dataCollectionSink;

        public void SetLogger(Action<string> logger) => _logger = logger;

        public void Log(string message) => _logger?.Invoke(message);

        // called before any test is run
        public void TestSessionStart(TestSessionStartArgs testSessionStartArgs)
        {
            //Debugger.Launch();
            var configuration = testSessionStartArgs.Configuration;
            ReadConfiguration(configuration);

            // if assembly was not loaded yet, wait for assembly to load
            AppDomain.CurrentDomain.AssemblyLoad += OnAssemblyLoaded;

            // scan loaded assemblies, just in case the test assembly is already loaded
            var assembly = AppDomain.CurrentDomain.GetAssemblies().FirstOrDefault(a => !a.IsDynamic && a.ExportedTypes.Any(t => t.FullName == _controlClassName));

            FindControlType(assembly);
            if (_singleMutant.HasValue)
            {
                SetActiveMutation(_singleMutant.Value);
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

            _mutantControlType = assembly.ExportedTypes.FirstOrDefault(t => t.FullName == _controlClassName);
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

        private void SetActiveMutation(int id)
        {
            _activeMutation = id;
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
                var mutations = new HashSet<int>();
                for (var i = 0; i < testMapping.Count; i++)
                {
                    var current = testMapping[i];
                    var id = int.Parse(current.Attributes["id"].Value);
                    var tests = current.Attributes["tests"].Value;
                    mutations.Add(id);
                    if (string.IsNullOrEmpty(tests))
                    {
                        _singleMutant = id;
                    }
                    else
                    {
                        foreach (var test in tests.Split(','))
                        {
                            _mutantTestedBy[test] = id;
                        }
                    }
                }
                if (mutations.Count == 1)
                {
                    _singleMutant = mutations.First();
                }
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
        }

        public void TestCaseStart(TestCaseStartArgs testCaseStartArgs)
        {
            //if (_coverageOn)
            //{
            //    // see if any mutation was executed outside a test
            //    var covered = RetrieveCoverData();
            //    if (covered[0] != null)
            //    {
            //        _mutationCoveredOutsideTests = covered[1] != null ? covered[0].Union(covered[1]).ToList() : covered[0].ToList();
            //    }
            //    else if (covered[1] != null)
            //    {
            //        _mutationCoveredOutsideTests = covered[1].ToList();
            //    }
            //    return;
            //}

            //// we need to set the proper mutant
            //var mutantId = _singleMutant ?? _mutantTestedBy[testCaseStartArgs.TestCase.Id.ToString()];

            //SetActiveMutation(mutantId);

            //Log($"Test {testCaseStartArgs.TestCase.FullyQualifiedName} starts against mutant {mutantId} (var).");
        }

        public void TestCaseEnd(TestCaseEndArgs testCaseEndArgs)
        {
            var assembly = AppDomain.CurrentDomain.GetAssemblies().FirstOrDefault(a => !a.IsDynamic && a.ExportedTypes.Any(t => t.FullName == _controlClassName));
            // The assembly should be found here. But it isn't. We use this assembly to call a method inside the tested assembly to create a map of covered mutation inside the assembly.
            // In a normal scenario we are able to find the assembly before the first test is run.
            Debugger.Launch();

            Log($"Test {testCaseEndArgs.DataCollectionContext.TestCase.FullyQualifiedName} ends.");
            if (!_coverageOn)
            {
                return;
            }

            PublishCoverageData(testCaseEndArgs);
            SetActiveMutation(_singleMutant ?? -2);
        }

        private void PublishCoverageData(TestCaseEndArgs testCaseEndArgs)
        {
            var covered = RetrieveCoverData();
            var coverData = string.Join(",", covered[0]) + ";" + string.Join(",", covered[1]);
            if (coverData == string.Empty)
            {
                // test covers no mutant, but empty string is not a valid value
                coverData = " ";
            }

            _dataSink.SendData(testCaseEndArgs.DataCollectionContext, PropertyName, coverData);
            if (_mutationCoveredOutsideTests.Count <= 0)
            { return; }
            // report any mutations covered before this test executed
            _dataSink.SendData(testCaseEndArgs.DataCollectionContext, OutOfTestsPropertyName, string.Join(",", _mutationCoveredOutsideTests));
            _mutationCoveredOutsideTests.Clear();
        }

        public IList<int>[] RetrieveCoverData()
            => (IList<int>[])_getCoverageData.Invoke(null, new object[] { });

        public void TestSessionEnd(TestSessionEndArgs testSessionEndArgs)
            => Log($"TestSession ends.");
    }
}
