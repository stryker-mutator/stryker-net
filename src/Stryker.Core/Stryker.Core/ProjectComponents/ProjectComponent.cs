using Microsoft.CodeAnalysis;
using Stryker.Core.Mutants;
using Stryker.Core.Options;
using System.Collections.Generic;
using System.Linq;

namespace Stryker.Core.ProjectComponents
{
    public abstract class ProjectComponent : IReadOnlyInputComponent
    {
        public string Name { get; set; }
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
        public IEnumerable<IReadOnlyMutant> ReadOnlyMutants => Mutants;
        public IEnumerable<IReadOnlyMutant> TotalMutants =>
            ReadOnlyMutants.Where(m =>
                m.ResultStatus != MutantStatus.CompileError && m.ResultStatus != MutantStatus.Ignored);

        public IEnumerable<IReadOnlyMutant> DetectedMutants => ReadOnlyMutants.Where(
            m =>
                m.ResultStatus == MutantStatus.Killed ||
                m.ResultStatus == MutantStatus.Timeout);

        public abstract IEnumerable<SyntaxTree> CompilationSyntaxTrees { get; }
        public abstract IEnumerable<SyntaxTree> MutatedSyntaxTrees { get; }

        // These delegates will get invoked while walking the tree during Display();
        public Display DisplayFile { get; set; }

        public Display DisplayFolder { get; set; }

        public abstract void Display(int depth);

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
            if (GetMutationScore() is var mutationScore && !double.IsNaN(mutationScore))
            {
                var mutationScorePercentage = mutationScore * 100;

                switch (mutationScorePercentage)
                {
                    case var score when score > threshold.High:
                        return Health.Good;
                    case var score when score <= threshold.High && score > threshold.Low:
                        return Health.Warning;
                    case var score when score <= threshold.Low:
                        return Health.Danger;
                }
            }

            // The mutation score is outside of the bounds we can work with so we don't have a health
            return Health.None;
        }

        public abstract void Add(ProjectComponent component);
        public abstract IEnumerable<FileLeaf> GetAllFiles();
    }
}