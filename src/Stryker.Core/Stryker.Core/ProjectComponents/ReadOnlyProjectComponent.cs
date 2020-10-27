using Stryker.Core.Mutants;
using Stryker.Core.Options;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace Stryker.Core.ProjectComponents
{
    public abstract class ReadOnlyProjectComponent : IReadOnlyInputComponent
    {
        private readonly IProjectComponent _projectComponent;

        public ReadOnlyProjectComponent(IProjectComponent projectComponent)
        {
            _projectComponent = projectComponent;
        }

        public string Name => _projectComponent.Name;
        public string FullPath => _projectComponent.FullPath;

        /// <summary>
        /// Gets or sets the path relative to the virtual project root.
        /// Includes the project folder.
        /// </summary>
        public string RelativePath => _projectComponent.RelativePath;

        /// <summary>
        /// Gets or sets the path relative to the .csproj file.
        /// </summary>
        public string RelativePathToProjectFile => _projectComponent.RelativePathToProjectFile;

        public IEnumerable<IReadOnlyMutant> Mutants => _projectComponent.Mutants;
        public IEnumerable<IReadOnlyMutant> ReadOnlyMutants => Mutants;
        public IEnumerable<IReadOnlyMutant> TotalMutants =>
            ReadOnlyMutants.Where(m =>
                m.ResultStatus != MutantStatus.CompileError && m.ResultStatus != MutantStatus.Ignored);

        public IEnumerable<IReadOnlyMutant> DetectedMutants => ReadOnlyMutants.Where(
            m =>
                m.ResultStatus == MutantStatus.Killed ||
                m.ResultStatus == MutantStatus.Timeout);

        // These delegates will get invoked while walking the tree during Display();
        public Display DisplayFile { get; set; }

        public Display DisplayFolder { get; set; }

        public abstract void Display(int depth);

        public IParentComponent Parent => _projectComponent.Parent;

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

        public bool Equals([AllowNull] IReadOnlyInputComponent other)
        {
            if (other != null)
            {
                if (RelativePath.Equals(other.RelativePath)
                    && FullPath.Equals(other.FullPath)
                    && Name.Equals(other.Name))
                { return false; }
            }
            return true;
        }
    }
}