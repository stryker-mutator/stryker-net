using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Stryker.Core.Mutants;
using Stryker.Core.Options;

namespace Stryker.Core.ProjectComponents
{
    public abstract class ReadOnlyProjectComponent : IReadOnlyProjectComponent
    {
        private readonly IProjectComponent _projectComponent;

        public string FullPath => _projectComponent.FullPath;
        public string RelativePath => _projectComponent.RelativePath;


        public IFolderComposite Parent => _projectComponent.Parent;

        public IEnumerable<IReadOnlyMutant> Mutants => _projectComponent.Mutants;
        public IEnumerable<IReadOnlyMutant> TotalMutants => Mutants.Where(m => m.ResultStatus != MutantStatus.CompileError && m.ResultStatus != MutantStatus.Ignored);
        public IEnumerable<IReadOnlyMutant> DetectedMutants => Mutants.Where(m => m.ResultStatus == MutantStatus.Killed || m.ResultStatus == MutantStatus.Timeout);

        protected ReadOnlyProjectComponent(IProjectComponent projectComponent)
        {
            _projectComponent = projectComponent;
        }

        // These delegates will get invoked while walking the tree during Display
        public Display DisplayFile { get; set; }

        public Display DisplayFolder { get; set; }

        public abstract void Display();

        /// <summary>
        /// Returns the mutation score for this folder / file
        /// </summary>
        /// <returns>decimal between 0 and 1 or null when no score could be calculated</returns>
        public double GetMutationScore()
        {
            double totalCount = TotalMutants.Count();
            double killedCount = DetectedMutants.Count();

            return killedCount / totalCount;
        }

        public Health CheckHealth(Thresholds threshold)
        {
            var mutationScore = GetMutationScore();
            if (double.IsNaN(mutationScore))
            {
                // The mutation score is outside of the bounds we can work with so we don't have a health
                return Health.None;
            }

            var mutationScorePercentage = mutationScore * 100;

            return mutationScorePercentage switch
            {
                var score when score >= threshold.High => Health.Good,
                var score when score < threshold.High && score >= threshold.Low => Health.Warning,
                _ => Health.Danger
            };
        }

        public virtual bool Equals([AllowNull] IReadOnlyProjectComponent other)
        {
            return other is { }
                && RelativePath.Equals(other.RelativePath)
                && FullPath.Equals(other.FullPath);
        }
    }
}
