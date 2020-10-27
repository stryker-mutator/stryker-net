using Stryker.Core.Mutants;
using Stryker.Core.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;

namespace Stryker.Core.ProjectComponents
{
    public abstract class ReadOnlyProjectComponent : IReadOnlyInputComponent
    {
        private readonly IProjectComponent _projectComponent;

        public ReadOnlyProjectComponent(IProjectComponent projectComponent)
        {
            _projectComponent = projectComponent;
            Name = projectComponent.Name;
            FullPath = projectComponent.FullPath;
            RelativePath = projectComponent.RelativePath;
            RelativePathToProjectFile = projectComponent.RelativePathToProjectFile;
            Mutants = projectComponent.Mutants;
            Parent = projectComponent.Parent;
        }

        public string Name { get; }
        public string FullPath { get; }

        /// <summary>
        /// Gets or sets the path relative to the virtual project root.
        /// Includes the project folder.
        /// </summary>
        public string RelativePath { get; }

        /// <summary>
        /// Gets or sets the path relative to the .csproj file.
        /// </summary>
        public string RelativePathToProjectFile { get; }

        public IEnumerable<IReadOnlyMutant> Mutants { get; }
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

        public IParentComponent Parent { get; }

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