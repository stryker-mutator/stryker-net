using Stryker.Core.Initialisation.ProjectComponent;
using Stryker.Core.Options;

namespace Stryker.Core {
    public class StrykerRunResult {
        private StrykerOptions _options { get; }
        public decimal? mutationScore { get; }

        public StrykerRunResult(StrykerOptions options, decimal? mutationScore) 
        {
            this._options = options;
            this.mutationScore = mutationScore;
        }

        public bool isScoreAboveThresholdBreak() {
            decimal thresholdBreak = (decimal) _options.ThresholdOptions.ThresholdBreak/100;
            // Check if the mutation score is not below the threshold break
            return this.mutationScore > thresholdBreak;
        }
    } 
}
