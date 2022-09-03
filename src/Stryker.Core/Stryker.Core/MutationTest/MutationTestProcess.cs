using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Stryker.Core.CoverageAnalysis;
using Stryker.Core.Exceptions;
using Stryker.Core.Initialisation;
using Stryker.Core.Initialisation.Buildalyzer;
using Stryker.Core.Logging;
using Stryker.Core.Mutants;
using Stryker.Core.Options;
using Stryker.Core.ProjectComponents;
using Stryker.Core.Reporters;
using Stryker.Core.TestRunners;

namespace Stryker.Core.MutationTest
{
    public class MutationTestProcess : IMutationTestProcess
    {
        private static readonly ILogger Logger = ApplicationLogging.LoggerFactory.CreateLogger<MutationTestProcess>();
        private readonly IProjectComponent _projectContents;
        private readonly IMutationTestExecutor _mutationTestExecutor;
        private readonly IReporter _reporter;
        private readonly ICoverageAnalyser _coverageAnalyser;
        private readonly StrykerOptions _options;
        private readonly IMutationProcess _mutationProcess;
        private static readonly Dictionary<Language, Func<MutationTestInput, StrykerOptions, IMutationProcess>> LanguageMap = new();

        static MutationTestProcess()
        {
            DeclareMutationProcessForLanguage<CsharpMutationProcess>(Language.Csharp);
            DeclareMutationProcessForLanguage<FsharpMutationProcess>(Language.Fsharp);
        }

        public static void DeclareMutationProcessForLanguage<T>(Language language) where T:IMutationProcess
        {
            var constructor = typeof(T).GetConstructor(new[]
                { typeof(MutationTestInput), typeof(StrykerOptions) });
            if (constructor == null)
            {
                throw new NotSupportedException(
                    $"Failed to find a constructor with the appropriate signature for type {typeof(T)}");
            }

            LanguageMap[language] = (x, y) => (IMutationProcess)constructor.Invoke(new object[] { x, y });
        }

        public MutationTestProcess(MutationTestInput input,
            StrykerOptions options,
            IReporter reporter,
            IMutationTestExecutor executor,
            IMutationProcess mutationProcess = null,
            ICoverageAnalyser coverageAnalyzer = null)
        {
            Input = input;
            _reporter = reporter;
            _options = options;
            _mutationTestExecutor = executor;
            _mutationProcess = mutationProcess ?? BuildMutationProcess();
            _coverageAnalyser = coverageAnalyzer ?? new CoverageAnalyser(_options);
            _projectContents = input.ProjectInfo.ProjectContents;
        }

        public MutationTestInput Input { get; }

        private IMutationProcess BuildMutationProcess()
        {
            if (!LanguageMap.ContainsKey(Input.ProjectInfo.ProjectUnderTestAnalyzerResult.GetLanguage()))
            {
                throw new GeneralStrykerException(
                    "no valid language detected || no valid csproj or fsproj was given.");
            }
            return LanguageMap[Input.ProjectInfo.ProjectUnderTestAnalyzerResult.GetLanguage()](Input, _options);
        }

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
            Logger.LogInformation(
                $"Mutations will be tested in {buildMutantGroupsForTest.Count} test runs" +
                (mutantCount > buildMutantGroupsForTest.Count ? $", instead of {mutantCount}." : "."));

            TestMutants(buildMutantGroupsForTest);

            return new StrykerRunResult(_options, _projectContents.GetMutationScore());
        }

        public IEnumerable<string> GetTestNames(ITestGuids testList) => _mutationTestExecutor.TestRunner.DiscoverTests().Extract(testList.GetGuids()).Select(t => t.Name);

        public MutantDiagnostic DiagnoseMutant(IEnumerable<Mutant> mutants, int mutantToDiagnose)
        {
            var monitoredMutant = Input.ProjectInfo.ProjectContents.Mutants.First(m => m.Id == mutantToDiagnose);
            Logger.LogWarning($"Diagnosing mutant {mutantToDiagnose}.");
            var monitoredMutantCoveringTests = monitoredMutant.CoveringTests;

            if (monitoredMutant.ResultStatus is MutantStatus.CompileError or MutantStatus.Ignored)
            {
                Logger.LogWarning("Stryker does not offer diagnosis for {0} mutants.", monitoredMutant.ResultStatus);
                return null;
            }

            var mutantGroup = monitoredMutant.ResultStatus == MutantStatus.NoCoverage ?  new List<Mutant>() : BuildMutantGroupsForTest(mutants.ToList()).First(l => l.Contains(monitoredMutant));
            var result = new MutantDiagnostic(monitoredMutant, GetTestNames(monitoredMutantCoveringTests), mutantGroup.Select(m => m.Id));
            if (monitoredMutant.AssessingTests.IsEveryTest)
            {
                var testNames = GetTestNames(monitoredMutant.KillingTests);
                Logger.LogInformation("Mutant is tested against all tests, no need for supplemental test runs.");
                RetestMutantGroup(new List<Mutant>{monitoredMutant});
                // all results assumed as identical
                result.DeclareResult(monitoredMutant.ResultStatus, testNames);
                result.DeclareResult(monitoredMutant.ResultStatus, testNames);
                result.DeclareResult(monitoredMutant.ResultStatus, testNames);
                // we do not know how to diagnose this
                return result;
            }

            if (monitoredMutant.ResultStatus == MutantStatus.NoCoverage)
            {
                Logger.LogInformation("Mutant appears as being not covered by any tests.");
                // first two test sessions will obviously result in NoCoverage
                result.DeclareResult(MutantStatus.NoCoverage, Enumerable.Empty<string>());
                result.DeclareResult(MutantStatus.NoCoverage, Enumerable.Empty<string>());
            }
            else
            {
                Logger.LogInformation("Mutant is covered by the following tests: ");
                Logger.LogInformation(string.Join(',', result.CoveringTests));

                Logger.LogInformation("*** Step 1 normal run ***");
                RetestMutantGroup(mutantGroup);
                Logger.LogInformation($"Mutant {monitoredMutant.Id} is {monitoredMutant.ResultStatus}.");
                result.DeclareResult(monitoredMutant.ResultStatus, GetTestNames(monitoredMutant.KillingTests));
                Logger.LogInformation("*** Step 2 solo run ***");
                RetestMutantGroup(new List<Mutant> { monitoredMutant });
                Logger.LogInformation($"Mutant {monitoredMutant.Id} is {monitoredMutant.ResultStatus}.");
                result.DeclareResult(monitoredMutant.ResultStatus, GetTestNames(monitoredMutant.KillingTests));
            }

            Logger.LogInformation("*** Step 3 run against all tests ***");
            // we mark the mutant as needing all tests.
            monitoredMutant.AssessingTests = TestGuidsList.EveryTest();
            RetestMutantGroup(new List<Mutant> { monitoredMutant });
            monitoredMutant.AssessingTests = monitoredMutantCoveringTests;
            Logger.LogInformation($"Mutant {monitoredMutant.Id} is {monitoredMutant.ResultStatus}.");
            result.DeclareResult(monitoredMutant.ResultStatus, GetTestNames(monitoredMutant.KillingTests));
            
            RefineDiagnosis(result, monitoredMutant, mutantGroup);
            return result;
        }

        private void RefineDiagnosis(MutantDiagnostic result, Mutant monitoredMutant, List<Mutant> mutantGroup)
        {
            var mutantToDiagnose = monitoredMutant.Id;
            if (result.RunResults[0].status == result.RunResults[1].status)
            {
                // no sign of a conflict, can't refine
                return;
            }
            
            var referenceStatus = result.RunResults[0].status;
            if (monitoredMutant.FalselyCoveringTests.Count > 0)
            {
                Logger.LogWarning(
                    monitoredMutant.FalselyCoveringTests.Count == monitoredMutant.AssessingTests.Count
                        ? "Mutant {mutant} as not been actually tested. It is likely another mutation altered normal test execution."
                        : "Some tests covering mutant {mutant} did not actually test it. It is likely another mutation altered normal test execution.",
                    mutantToDiagnose);
            }
            else
            {
                Logger.LogWarning(
                    "Inconsistent coverage based tests. There is some unwanted side effect. Using binary search to find problematic mutant.");
            }

            monitoredMutant.ResultStatus = referenceStatus;
            var firstIndex = FindConflictingMutant(mutantGroup, monitoredMutant, referenceStatus);
            
            var conflictingMutant = mutantGroup[firstIndex];
            Logger.LogInformation("Conflicting mutant is {0}", conflictingMutant.Id);
            result.ConflictingMutant = conflictingMutant;
            Logger.LogInformation("Using binary search to find a conflicting test.");
            // find a conflicting test
            var testName = FindConflictingTest(referenceStatus, monitoredMutant, conflictingMutant);
            Logger.LogInformation("Conflicting test is {0}.", testName);
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
                AssessingTests = conflictingMutant.AssessingTests,
                ResultStatus = MutantStatus.NotRun,
                Mutation = conflictingMutant.Mutation,
                IsStaticValue = conflictingMutant.IsStaticValue
            };
            var testGroup = new[] { monitoredMutant, alternativeMutant };
            var first = true;
            while (upper-lower>1)
            {
                var pivot = (lower + upper) / 2;
                monitoredMutant.ResultStatus = MutantStatus.NotRun;
                alternativeMutant.AssessingTests = new TestGuidsList(originalList[lower..pivot]);
                alternativeMutant.ResultStatus = MutantStatus.NotRun;
                RetestMutantGroup(testGroup);
                if (monitoredMutant.ResultStatus == referenceStatus)
                {
                    if (first)
                    {
                        // check that there is no conflict with the other half of tests
                        monitoredMutant.ResultStatus = MutantStatus.NotRun;
                        alternativeMutant.AssessingTests = new TestGuidsList(originalList[pivot..upper]);
                        alternativeMutant.ResultStatus = MutantStatus.NotRun;
                        RetestMutantGroup(testGroup);
                    }
                    // this group contains a conflicting test
                    upper = pivot;
                    first = false;
                }
                else
                {
                    lower = pivot;
                }
            }

            return GetTestNames(alternativeMutant.AssessingTests).First();
        }

        private int FindConflictingMutant(List<Mutant> group, Mutant monitoredMutant, MutantStatus referenceStatus)
        {
            var mutantToDiagnose = monitoredMutant.Id;
            // we remove the mutant of interest from the list
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
                        firstRun = false;
                    }

                    lastIndex = pivot;
                }
                else
                {
                    // the other half must contain a problematic mutant
                    firstIndex = pivot+1;
                }
            }
            // at this stage, we verify that the problematic mutant is not the one we diagnose.
            RetestMutantGroup(group.GetRange(lastIndex, 1).Append(monitoredMutant)
                .ToList());

            return firstIndex;
        }

        private void RetestMutantGroup(IEnumerable<Mutant> mutants)
        {
            var toTest = mutants.ToList();
            foreach (var mutant in toTest)
            {
                mutant.ResultStatus = MutantStatus.NotRun;
            }

            Logger.LogInformation("Testing a group of {0} mutants against {1} tests.", toTest.Count, toTest.Any(t => t.AssessingTests.IsEveryTest) ? "all" : toTest.Sum(t => t.AssessingTests.Count));
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
                    (testedMutants, testResults)
                        => TestUpdateHandler(testedMutants, testResults, reportedMutants));
                
                OnMutantsTested(mutants, reportedMutants);
            });
        }

        private bool TestUpdateHandler(IEnumerable<Mutant> testedMutants, ITestRunResults results, ISet<Mutant> reportedMutants)
        {
            var testsFailingInitially = Input.InitialTestRun.FailedTests;
            var continueTestRun = _options.OptimizationMode.HasFlag(OptimizationModes.DisableBail);
            
            if (testsFailingInitially.Count > 0 && results.FailedTests.ContainsAny(testsFailingInitially))
            {
                // some of the failing tests where already failed during initial test, we have to ignore those failures as they are not informative
                var filteredFailingTests = new TestGuidsList(results.FailedTests.GetGuids().Except(testsFailingInitially.GetGuids()));
                results = new TestRunResults(results.RanTests, filteredFailingTests, results.TimedOutTests, results.NonCoveringTests);
            }

            foreach (var mutant in testedMutants)
            {
                mutant.AnalyzeTestRun(results);

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
                    Logger.LogWarning($"Mutation {mutant.Id} was not fully tested.");
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
            var needOwnTestSession = mutantsToGroup.Where(m => m.AssessingTests.IsEveryTest || m.MustBeTestedInIsolation).ToHashSet();
            blocks.AddRange(needOwnTestSession.Select(m => new List<T> { m }));
            mutantsToGroup.RemoveAll(m => needOwnTestSession.Contains(m));

            mutantsToGroup = mutantsToGroup.Where(m => m.ResultStatus == MutantStatus.NotRun).ToList();
            
            var testsCount = Input.InitialTestRun.AllTests.Count;
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
            
            Logger.LogDebug(
                $"Mutations will be tested in {blocks.Count} test runs" +
                (mutantsNotRun.Count > blocks.Count ? $", instead of {mutantsNotRun.Count}." : "."));

            return blocks;
        }

        public void GetCoverage() => _coverageAnalyser.DetermineTestCoverage(_mutationTestExecutor.TestRunner, _projectContents.Mutants, Input.InitialTestRun.FailedTests);
    }
}
