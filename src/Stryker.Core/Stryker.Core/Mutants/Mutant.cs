using System.IO;
using System.Linq;
using Stryker.Core.MutationTest;

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
        ITestGuids AssessingTests { get; }
        int? Line { get; }
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
        public ITestGuids FalselyCoveringTests { get; private set;} = TestGuidsList.EveryTest();

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
        
        public void AnalyzeTestRun(ITestRunResults results)
        {
            if (AssessingTests.ContainsAny(results.FailedTests))
            {
                ResultStatus = MutantStatus.Killed;
                KillingTests = CoveringTests.Intersect(results.FailedTests);
                FalselyCoveringTests = AssessingTests.Intersect(results.NonCoveringTests);
            }
            else if (AssessingTests.ContainsAny(results.TimedOutTests) || results.SessionTimedOut)
            {
                ResultStatus = MutantStatus.Timeout;
                FalselyCoveringTests = AssessingTests.Intersect(results.NonCoveringTests);
            }
            else if (results.RanTests.IsEveryTest || AssessingTests.IsIncludedIn(results.RanTests))
            {
                ResultStatus = MutantStatus.Survived;
                FalselyCoveringTests = AssessingTests.Intersect(results.NonCoveringTests);
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
