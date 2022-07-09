using System;
using System.Collections.Generic;
using System.IO.Abstractions;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.Extensions.Logging;
using Microsoft.FSharp.Collections;
using Stryker.Core.CoverageAnalysis;
using Stryker.Core.Exceptions;
using Stryker.Core.Initialisation;
using Stryker.Core.Initialisation.Buildalyzer;
using Stryker.Core.Logging;
using Stryker.Core.MutantFilters;
using Stryker.Core.Mutants;
using Stryker.Core.Options;
using Stryker.Core.ProjectComponents;
using Stryker.Core.Reporters;
using static FSharp.Compiler.SyntaxTree;

namespace Stryker.Core.MutationTest
{
    public class MutationTestProcess : IMutationTestProcess
    {
        public MutationTestInput Input { get; }
        private readonly IProjectComponent _projectContents;
        private readonly ILogger _logger;
        private readonly IMutationTestExecutor _mutationTestExecutor;
        private readonly IFileSystem _fileSystem;
        private readonly BaseMutantOrchestrator _orchestrator;
        private readonly IReporter _reporter;
        private readonly ICoverageAnalyser _coverageAnalyser;
        private readonly StrykerOptions _options;
        private readonly Language _language;
        private IMutationProcess _mutationProcess;

        public MutationTestProcess(MutationTestInput mutationTestInput,
            IReporter reporter,
            IMutationTestExecutor mutationTestExecutor,
            BaseMutantOrchestrator<SyntaxNode> cSharpOrchestrator = null,
            BaseMutantOrchestrator<FSharpList<SynModuleOrNamespace>> fSharpOrchestrator = null,
            IFileSystem fileSystem = null,
            IMutantFilter mutantFilter = null,
            ICoverageAnalyser coverageAnalyser = null,
            StrykerOptions options = null)
        {
            Input = mutationTestInput;
            _projectContents = mutationTestInput.ProjectInfo.ProjectContents;
            _reporter = reporter;
            _options = options;
            _mutationTestExecutor = mutationTestExecutor;
            _fileSystem = fileSystem ?? new FileSystem();
            _logger = ApplicationLogging.LoggerFactory.CreateLogger<MutationTestProcess>();
            _coverageAnalyser = coverageAnalyser ?? new CoverageAnalyser(_options);
            _language = Input.ProjectInfo.ProjectUnderTestAnalyzerResult.GetLanguage();
            _orchestrator = cSharpOrchestrator ?? fSharpOrchestrator ?? ChooseOrchestrator(_options);

            SetupMutationTestProcess(mutantFilter);
        }

        private BaseMutantOrchestrator ChooseOrchestrator(StrykerOptions options)
        {
            if (_language == Language.Fsharp)
            {
                return new FsharpMutantOrchestrator(options: options);
            }

            return new CsharpMutantOrchestrator(options: options);
        }

        private void SetupMutationTestProcess(IMutantFilter mutantFilter) =>
            _mutationProcess = _language switch
            {
                Language.Csharp => new CsharpMutationProcess(Input, _fileSystem, _options, mutantFilter,
                    (BaseMutantOrchestrator<SyntaxNode>)_orchestrator),
                Language.Fsharp => new FsharpMutationProcess(Input,
                    (BaseMutantOrchestrator<FSharpList<SynModuleOrNamespace>>)_orchestrator, _fileSystem, _options),
                _ => throw new GeneralStrykerException(
                    "no valid language detected || no valid csproj or fsproj was given")
            };

        public void Mutate()
        {
            Input.ProjectInfo.BackupOriginalAssembly();
            _mutationProcess.Mutate();
        }

        public void FilterMutants() => _mutationProcess.FilterMutants();

        public StrykerRunResult Test(IEnumerable<Mutant> mutantsToTest)
        {
            if (!MutantsToTest(mutantsToTest))
            {
                return new StrykerRunResult(_options, double.NaN);
            }

            var mutantCount = mutantsToTest.Count();
            var buildMutantGroupsForTest = BuildMutantGroupsForTest(mutantsToTest.ToList()).ToList();
            _logger.LogInformation(
                $"Mutations will be tested in {buildMutantGroupsForTest.Count} test runs" +
                (mutantCount > buildMutantGroupsForTest.Count ? $", instead of {mutantCount}." : "."));

            TestMutants(buildMutantGroupsForTest);

            return new StrykerRunResult(_options, _projectContents.GetMutationScore());
        }

        public IEnumerable<string> GetTestNames(ITestGuids testList) => _mutationTestExecutor.TestRunner.DiscoverTests().Extract(testList.GetGuids()).Select(t => t.Name);

        public MutantDiagnostic DiagnoseMutant(IEnumerable<Mutant> mutants, int mutantToDiagnose)
        {
            var monitoredMutant = Input.ProjectInfo.ProjectContents.Mutants.First(m => m.Id == mutantToDiagnose);
            _logger.LogWarning($"Diagnosing mutant {mutantToDiagnose}.");
            var monitoredMutantCoveringTests = monitoredMutant.CoveringTests;

            if (monitoredMutant.ResultStatus is MutantStatus.CompileError or MutantStatus.Ignored)
            {
                _logger.LogWarning("Stryker does not offer diagnosis for {0} mutants.", monitoredMutant.ResultStatus);
                return null;
            }

            var mutantGroup = monitoredMutant.ResultStatus == MutantStatus.NoCoverage ?  new List<Mutant>() : BuildMutantGroupsForTest(mutants.ToList()).First(l => l.Contains(monitoredMutant));
            var result = new MutantDiagnostic(monitoredMutant, GetTestNames(monitoredMutantCoveringTests), mutantGroup.Select(m => m.Id));
            if (monitoredMutant.AssessingTests.IsEveryTest)
            {
                var testNames = GetTestNames(monitoredMutant.KillingTests);
                _logger.LogInformation("Mutant is tested against all tests, no need for supplemental test runs.");
                RetestMutantGroup(new List<Mutant>{monitoredMutant});
                // all results assumed as identical
                result.DeclareResult(monitoredMutant.ResultStatus, testNames);
                result.DeclareResult(monitoredMutant.ResultStatus, testNames);
                result.DeclareResult(monitoredMutant.ResultStatus, testNames);
            }
            else
            {
                if (monitoredMutant.ResultStatus != MutantStatus.NoCoverage)
                {
                    _logger.LogInformation("Mutant is covered by the following tests: ");
                    _logger.LogInformation(string.Join(',', result.CoveringTests));

                    _logger.LogInformation("*** Step 1 normal run ***");
                    RetestMutantGroup(mutantGroup);
                    _logger.LogInformation($"Mutant {monitoredMutant.Id} is {monitoredMutant.ResultStatus}.");
                    result.DeclareResult(monitoredMutant.ResultStatus, GetTestNames(monitoredMutant.KillingTests));
                    _logger.LogInformation("*** Step 2 solo run ***");
                    RetestMutantGroup(new List<Mutant> { monitoredMutant });
                    _logger.LogInformation($"Mutant {monitoredMutant.Id} is {monitoredMutant.ResultStatus}.");
                    result.DeclareResult(monitoredMutant.ResultStatus, GetTestNames(monitoredMutant.KillingTests));
                }
                else
                {
                    _logger.LogInformation("Mutant appears as being not covered by any tests.");
                    result.DeclareResult(MutantStatus.NoCoverage, Enumerable.Empty<string>());
                    result.DeclareResult(MutantStatus.NoCoverage, Enumerable.Empty<string>());
                }

                _logger.LogInformation("*** Step 3 run against all tests ***");
                // we mark the mutant as needing all tests.
                monitoredMutant.AssessingTests = TestGuidsList.EveryTest();
                RetestMutantGroup(new List<Mutant> { monitoredMutant });
                monitoredMutant.AssessingTests = monitoredMutantCoveringTests;
                _logger.LogInformation($"Mutant {monitoredMutant.Id} is {monitoredMutant.ResultStatus}.");
                result.DeclareResult(monitoredMutant.ResultStatus, GetTestNames(monitoredMutant.KillingTests));
            }
            if (result.RunResults[0].status != result.RunResults[1].status)
            {
                var referenceStatus = result.RunResults[0].status;
                _logger.LogWarning("Inconsistent coverage based tests. There is some unwanted side effect. Using binary search to find problematic mutant.");
                var firstIndex = FindConflictingMutant(mutantGroup, monitoredMutant, referenceStatus);
                //
                var conflictingMutant = mutantGroup[firstIndex];
                _logger.LogInformation("Conflicting mutant is {0}", conflictingMutant.Id);
                result.ConflictingMutant = conflictingMutant;
                _logger.LogInformation("Using binary search to find a conflicting test.");
                // find a conflicting test
                var testName = FindConflictingTest(referenceStatus, monitoredMutant, conflictingMutant);
                _logger.LogInformation("Conflicting test is {0}.", testName);
            }
            return result;
        }

        private string FindConflictingTest(MutantStatus referenceStatus, Mutant monitoredMutant, Mutant conflictingMutant)
        {
            var originalList = conflictingMutant.CoveringTests.GetGuids().ToArray();
            var lower = 0;
            var upper = originalList.Length;
            var alternativeMutant = new Mutant
            {
                Id = conflictingMutant.Id,
                CoveringTests = conflictingMutant.CoveringTests,
                ResultStatus = MutantStatus.NotRun,
                Mutation = conflictingMutant.Mutation,
                IsStaticValue = conflictingMutant.IsStaticValue
            };
            var testGroup = new[] { monitoredMutant, alternativeMutant };
            while (upper-lower>1)
            {
                var pivot = (lower + upper) / 2;
                monitoredMutant.ResultStatus = MutantStatus.NotRun;
                alternativeMutant.CoveringTests = new TestGuidsList(originalList[lower..pivot]);
                alternativeMutant.ResultStatus = MutantStatus.NotRun;
                RetestMutantGroup(testGroup);
                if (monitoredMutant.ResultStatus == referenceStatus)
                {
                    // this group contains a conflicting test
                    upper = pivot;
                }
                else
                {
                    lower = pivot;
                }
            }

            return GetTestNames(alternativeMutant.CoveringTests).First();
        }

        private int FindConflictingMutant(List<Mutant> group, Mutant monitoredMutant, MutantStatus referenceStatus)
        {
            var mutantToDiagnose = monitoredMutant.Id;
            group.Remove(monitoredMutant);
            var firstIndex = 0;
            var lastIndex = group.Count - 1;
            var firstRun = true;
            while (lastIndex - firstIndex > 0)
            {
                var pivot = (lastIndex + firstIndex) / 2;
                RetestMutantGroup(group.GetRange(firstIndex, pivot - firstIndex+1).Append(monitoredMutant));
                if (monitoredMutant.ResultStatus == referenceStatus)
                {
                    if (firstRun)
                    {
                        // this group contains the problematic mutant, we test the other half to be sure the bad mutant is not the one we diagnose
                        RetestMutantGroup(group.GetRange(pivot+1, lastIndex - pivot).Append(monitoredMutant)
                            .ToList());
                        if (monitoredMutant.ResultStatus == referenceStatus)
                        {
                            // the diagnose mutant is the problematic one.
                            return mutantToDiagnose;
                        }
                    }

                    lastIndex = pivot;
                }
                else
                {
                    // the other half must contain a problematic mutant
                    firstIndex = pivot+1;
                }

                firstRun = false;
            }

            return firstIndex;
        }

        private void RetestMutantGroup(IEnumerable<Mutant> mutants)
        {
            var toTest = mutants.ToList();
            foreach (var mutant in toTest)
            {
                mutant.ResultStatus = MutantStatus.NotRun;
            }

            _logger.LogInformation("Testing a group of {0} mutants against {1} tests.", toTest.Count, toTest.Any(t => t.CoveringTests.IsEveryTest) ? "all" : toTest.Sum(t => t.CoveringTests.Count));
            TestMutants(new[] { toTest });
        }

        public void Restore() => Input.ProjectInfo.RestoreOriginalAssembly();

        private void TestMutants(IEnumerable<List<Mutant>> mutantGroups)
        {
            var parallelOptions = new ParallelOptions { MaxDegreeOfParallelism = _options.Concurrency };

            Parallel.ForEach(mutantGroups, parallelOptions, mutants =>
            {
                var reportedMutants = new HashSet<Mutant>();
                
                _mutationTestExecutor.Test(mutants,
                    Input.InitialTestRun.TimeoutValueCalculator,
                    (testedMutants, tests, ranTests, outTests) => TestUpdateHandler(testedMutants, tests, ranTests, outTests, reportedMutants));

                OnMutantsTested(mutants, reportedMutants);
            });
        }

        private bool TestUpdateHandler(IEnumerable<Mutant> testedMutants, ITestGuids failedTests, ITestGuids ranTests,
            ITestGuids timedOutTest, ISet<Mutant> reportedMutants)
        {
            var testsFailingInitially = Input.InitialTestRun.Result.FailingTests.GetGuids().ToHashSet();
            var continueTestRun = _options.OptimizationMode.HasFlag(OptimizationModes.DisableBail);
            if (testsFailingInitially.Count > 0 && failedTests.GetGuids().Any(id => testsFailingInitially.Contains(id)))
            {
                // some of the failing tests where failing without any mutation
                // we discard those tests
                failedTests = new TestGuidsList(
                    failedTests.GetGuids().Where(t => !testsFailingInitially.Contains(t)));
            }
            foreach (var mutant in testedMutants)
            {
                mutant.AnalyzeTestRun(failedTests, ranTests, timedOutTest);

                if (mutant.ResultStatus == MutantStatus.NotRun)
                {
                    continueTestRun = true; // Not all mutants in this group were tested so we continue
                }

                OnMutantTested(mutant, reportedMutants); // Report on mutant that has been tested
            }

            return continueTestRun;
        }

        private void OnMutantsTested(IEnumerable<Mutant> mutants, ISet<Mutant> reportedMutants)
        {
            foreach (var mutant in mutants)
            {
                if (mutant.ResultStatus == MutantStatus.NotRun)
                {
                    _logger.LogWarning($"Mutation {mutant.Id} was not fully tested.");
                }

                OnMutantTested(mutant, reportedMutants);
            }
        }

        private void OnMutantTested(Mutant mutant, ISet<Mutant> reportedMutants)
        {
            if (mutant.ResultStatus == MutantStatus.NotRun || !reportedMutants.Add(mutant))
            {
                // skip duplicates or useless notifications
                return;
            }
            _reporter?.OnMutantTested(mutant);
        }

        private static bool MutantsToTest(IEnumerable<Mutant> mutantsToTest)
        {
            if (!mutantsToTest.Any())
            {
                return false;
            }
            if (mutantsToTest.Any(x => x.ResultStatus != MutantStatus.NotRun))
            {
                throw new GeneralStrykerException("Only mutants to run should be passed to the mutation test process. If you see this message please report an issue.");
            }

            return true;
        }

        private IEnumerable<List<T>> BuildMutantGroupsForTest<T>(IReadOnlyCollection<T> mutantsNotRun) where T: IReadOnlyMutant
        {
            if (_options.OptimizationMode.HasFlag(OptimizationModes.DisableMixMutants) || !_options.OptimizationMode.HasFlag(OptimizationModes.CoverageBasedTest))
            {
                return mutantsNotRun.Select(x => new List<T> { x });
            }

            var blocks = new List<List<T>>(mutantsNotRun.Count);
            var mutantsToGroup = mutantsNotRun.ToList();
            // deal with mutants that must be run alone, either it is requested or because they run against all tests
            blocks.AddRange(mutantsToGroup.Where(m => m.AssessingTests.IsEveryTest).Select(m => new List<T> { m }));
            mutantsToGroup.RemoveAll(m => m.AssessingTests.IsEveryTest);

            mutantsToGroup = mutantsToGroup.Where(m => m.ResultStatus == MutantStatus.NotRun).ToList();
            
            var testsCount = Input.InitialTestRun.Result.RanTests.Count;
            mutantsToGroup = mutantsToGroup.OrderBy(m => m.AssessingTests.Count).ToList();
            while (mutantsToGroup.Count>0)
            {
                // we pick the first mutant
                var usedTests = mutantsToGroup[0].AssessingTests;
                var nextBlock = new List<T> { mutantsToGroup[0] };
                mutantsToGroup.RemoveAt(0);
                for (var j = 0; j < mutantsToGroup.Count; j++)

                {
                    var currentMutant = mutantsToGroup[j];
                    var nextSet = currentMutant.AssessingTests;
                    // quick optimization here: if we need more (distinct) tests that there is, no need to check for overlap
                    // if not, check if this mutant is not covered by any of the selected mutants
                    if (nextSet.Count + usedTests.Count > testsCount ||
                        nextSet.ContainsAny(usedTests))
                    {
                        continue;
                    }

                    // add this mutant to the block
                    nextBlock.Add(currentMutant);
                    // remove the mutant from the list of mutants to group
                    mutantsToGroup.RemoveAt(j--);
                    // add this mutant's tests
                    usedTests = usedTests.Merge(nextSet);
                }

                blocks.Add(nextBlock);
            }
            
            return blocks;
        }

        public void GetCoverage() => _coverageAnalyser.DetermineTestCoverage(_mutationTestExecutor.TestRunner, _projectContents.Mutants, Input.InitialTestRun.Result.FailingTests);
    }
}
