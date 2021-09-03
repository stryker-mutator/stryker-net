using System.Collections.Generic;
using System.Linq;
using Stryker.Core.Mutants;
using Stryker.Core.Options;

namespace Stryker.Core.ProjectComponents
{
    public interface IProjectComponent
    {
        string FullPath { get; set; }
        IEnumerable<Mutant> Mutants { get; set; }
        IFolderComposite Parent { get; set; }
        string RelativePath { get; set; }

        IEnumerable<IProjectComponent> GetAllFiles();
        IReadOnlyProjectComponent ToReadOnlyInputComponent();
    }

    public abstract class ProjectComponent : IProjectComponent
    {
        public string FullPath { get; set; }
        /// <summary>
        /// Relative path to project file
        /// </summary>
        public string RelativePath { get; set; }

        public IFolderComposite Parent { get; set; }

        public virtual IEnumerable<Mutant> Mutants { get; set; }

        public abstract IReadOnlyProjectComponent ToReadOnlyInputComponent();

        public abstract IEnumerable<IProjectComponent> GetAllFiles();
    }

    public abstract class ProjectComponent<T> : ProjectComponent
    {
        public IEnumerable<IReadOnlyMutant> TotalMutants => Mutants.Where(m =>
                m.ResultStatus != MutantStatus.CompileError && m.ResultStatus != MutantStatus.Ignored);

        public IEnumerable<IReadOnlyMutant> DetectedMutants => Mutants.Where(m =>
                m.ResultStatus == MutantStatus.Killed ||
                m.ResultStatus == MutantStatus.Timeout);

        /// <summary>
        /// All syntax trees that should be a part of the compilation
        /// </summary>
        public abstract IEnumerable<T> CompilationSyntaxTrees { get; }
        /// <summary>
        /// Only those syntax trees that were changed by the mutation process
        /// </summary>
        public abstract IEnumerable<T> MutatedSyntaxTrees { get; }

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
    }
}
