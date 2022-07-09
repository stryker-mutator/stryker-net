using System.IO;

namespace Stryker.Core.Mutants
{
    /// <summary>
    /// This interface should only contain readonly properties to ensure that others than the mutation test process cannot modify mutants.
    /// </summary>
    public interface IReadOnlyMutant
    {
        int Id { get; }
        Mutation Mutation { get; }
        MutantStatus ResultStatus { get; }
        string ResultStatusReason { get; }
        ITestGuids CoveringTests { get; }
        ITestGuids AssessingTests { get; }
        int? Line { get; }
        bool CountForStats { get; }
        bool IsStaticValue { get; }
        public bool MustBeTestedInIsolation { get; }
        public string Location { get; }
        string GetRelativeLocation(string relativeToThisPath);
    }

    /// <summary>
    /// Represents a single mutation on domain level
    /// </summary>
    public class Mutant : IReadOnlyMutant
    {
        public int Id { get; set; }

        public Mutation Mutation { get; set; }

        public MutantStatus ResultStatus { get; set; }

        public ITestGuids KillingTests { get; set; } = TestGuidsList.NoTest();
        public ITestGuids CoveringTests { get; set; } = TestGuidsList.NoTest();
        
        public ITestGuids AssessingTests { get; set; } = TestGuidsList.EveryTest();

        public int? Line
        {
            get
            {
                var location = Mutation?.OriginalNode?.GetLocation().GetMappedLineSpan();
                return location?.StartLinePosition.Line + 1;
            }
        }

        public string ResultStatusReason { get; set; }

        public bool CountForStats => ResultStatus != MutantStatus.CompileError && ResultStatus != MutantStatus.Ignored;
        public bool IsStaticValue { get; set; }

        public string DisplayName => $"{Id}: {Mutation?.DisplayName}";
        public string Location
        {
            get
            {
                var location = Mutation?.OriginalNode?.GetLocation().GetMappedLineSpan();
                return location == null ? "Unknown location" : $"{location.Value.Path} line {location.Value.StartLinePosition.Line}:{location.Value.StartLinePosition.Character}";
            }
        }

        public bool CannotDetermineCoverage { get; set; }
        public bool MustBeTestedInIsolation { get; set; }
        public void AnalyzeTestRun(ITestGuids failedTests, ITestGuids resultRanTests, ITestGuids timedOutTests)
        {
            if (AssessingTests.ContainsAny(failedTests))
            {
                ResultStatus = MutantStatus.Killed;
                KillingTests = CoveringTests.Intersect(failedTests);
            }
            else if (resultRanTests.IsEveryTest || (resultRanTests.IsEveryTest is not true && AssessingTests.IsIncludedIn(resultRanTests)))
            {
                ResultStatus = MutantStatus.Survived;
            }
            else if (AssessingTests.ContainsAny(timedOutTests))
            {
                ResultStatus = MutantStatus.Timeout;
            }
        }

        public string GetRelativeLocation(string relativeToThisPath)
        {
            var location = Mutation?.OriginalNode?.GetLocation().GetMappedLineSpan();
            if (!location.HasValue)
            {
                return "Unknown location (file may be generated)";
            }

            var filename = location.Value.Path;
            if (!string.IsNullOrEmpty(relativeToThisPath))
            {
                filename = Path.GetRelativePath(relativeToThisPath, filename);
            }
            return $"{filename} line {location.Value.StartLinePosition.Line}:{location.Value.StartLinePosition.Character}";
        }

    }
}
