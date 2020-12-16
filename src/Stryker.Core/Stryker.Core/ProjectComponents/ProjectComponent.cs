using Stryker.Core.Mutants;
using Stryker.Core.Options;
using System.Collections.Generic;
using System.Linq;

namespace Stryker.Core.ProjectComponents
{
    public abstract class ProjectComponent<T> : IProjectComponent
    {
        public string Name { get; set; }
        public string FullPath { get; set; }
        public string RelativePath { get; set; }
        public string RelativePathToProjectFile { get; set; }

        public IFolderComposite Parent { get; set; }

        public abstract IEnumerable<Mutant> Mutants { get; set; }
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

        public abstract IReadOnlyProjectComponent ToReadOnlyInputComponent();

        public abstract IEnumerable<IProjectComponent> GetAllFiles();

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

        public Health CheckHealth(Threshold threshold)
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
