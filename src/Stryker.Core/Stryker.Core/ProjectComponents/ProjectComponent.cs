using System.Collections.Generic;
using System.Linq;
using Stryker.Core.Mutants;
using Stryker.Core.Options;

namespace Stryker.Core.ProjectComponents
{
    public abstract class ProjectComponent : IReadOnlyInputComponent
    {
        public string FullPath { get; set; }

        /// <summary>
        /// Gets or sets the path relative to the virtual project root.
        /// Includes the project folder.
        /// </summary>
        public string RelativePath { get; set; }

        /// <summary>
        /// Gets or sets the path relative to the .csproj file.
        /// </summary>
        public string RelativePathToProjectFile { get; set; }

        public abstract IEnumerable<Mutant> Mutants { get; set; }
        public string Name { get; set; }

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

        /// <summary>
        /// Returns the mutation score for this folder / file
        /// </summary>
        /// <returns>decimal between 0 and 100 or null when no score could be calculated</returns>
        public decimal? GetMutationScore()
        {
            var totalCount = TotalMutants.Count();
            var killedCount = DetectedMutants.Count();

            if (totalCount > 0)
            {
                return killedCount / (decimal) totalCount * 100;
            }
            else
            {
                return null;
            }
        }

        public Health CheckHealth(Threshold threshold)
        {
            var mutationScore = GetMutationScore();

            switch (mutationScore)
            {
                case var score when score > threshold.High:
                    return Health.Good;
                case var score when score <= threshold.High && score > threshold.Low:
                    return Health.Warning;
                case var score when score <= threshold.Low && score > threshold.Break:
                    return Health.Danger;
                default:
                    return Health.Danger;
            }
        }

        public abstract void Add(ProjectComponent component);
        public abstract IEnumerable<FileLeaf> GetAllFiles();
    }
}