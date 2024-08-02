using System.Collections.Generic;
using System.Linq;
using Stryker.Configuration.Mutants;
using Stryker.Configuration.Options;
using Stryker.Core.Mutants;

namespace Stryker.Configuration.ProjectComponents
{
    public delegate void Display(IReadOnlyProjectComponent current);

    public interface IReadOnlyProjectComponent
    {
        string FullPath { get; }
        IEnumerable<Mutant> Mutants { get; }
        IReadOnlyFolderComposite Parent { get; }
        string RelativePath { get; set; }
        public Display DisplayFile { get; set; }
        public Display DisplayFolder { get; set; }
        IEnumerable<IReadOnlyMutant> TotalMutants();
        IEnumerable<IReadOnlyMutant> ValidMutants();
        IEnumerable<IReadOnlyMutant> UndetectedMutants();
        IEnumerable<IReadOnlyMutant> DetectedMutants();
        IEnumerable<IReadOnlyMutant> InvalidMutants();
        IEnumerable<IReadOnlyMutant> IgnoredMutants();
        IEnumerable<IReadOnlyMutant> NotRunMutants();

        Health CheckHealth(IThresholds threshold);
        IEnumerable<IFileLeaf> GetAllFiles();
        void Display();
        double GetMutationScore();
    }

    public interface IProjectComponent : IReadOnlyProjectComponent
    {
        new string FullPath { get; set; }
        new IEnumerable<Mutant> Mutants { get; set; }
        new IReadOnlyFolderComposite Parent { get; set; }
        new string RelativePath { get; set; }
    }

    public abstract class ProjectComponent : IProjectComponent
    {
        public string FullPath { get; set; }
        /// <summary>
        /// Relative path to project file
        /// </summary>
        public string RelativePath { get; set; }

        public IReadOnlyFolderComposite Parent { get; set; }

        public Display DisplayFile { get; set; }

        public Display DisplayFolder { get; set; }

        public virtual IEnumerable<Mutant> Mutants { get; set; }

        public abstract IEnumerable<IFileLeaf> GetAllFiles();

        public abstract void Display();

        public IEnumerable<IReadOnlyMutant> TotalMutants() => ValidMutants()
            .Union(InvalidMutants())
            .Union(IgnoredMutants());

        public IEnumerable<IReadOnlyMutant> ValidMutants() => UndetectedMutants()
            .Union(DetectedMutants());

        public IEnumerable<IReadOnlyMutant> InvalidMutants() => Mutants
            .Where(m => m.ResultStatus == MutantStatus.CompileError);

        public IEnumerable<IReadOnlyMutant> UndetectedMutants() => Mutants
            .Where(m =>
                m.ResultStatus is MutantStatus.Survived or MutantStatus.NoCoverage);

        public IEnumerable<IReadOnlyMutant> IgnoredMutants() => Mutants
            .Where(m => m.ResultStatus == MutantStatus.Ignored);

        public IEnumerable<IReadOnlyMutant> NotRunMutants() => Mutants
            .Where(m => m.ResultStatus == MutantStatus.Pending);

        public IEnumerable<IReadOnlyMutant> DetectedMutants() => Mutants
            .Where(m =>
                m.ResultStatus is MutantStatus.Killed or MutantStatus.Timeout);

        /// <summary>
        /// Returns the mutation score for this folder / file
        /// </summary>
        /// <returns>double between 0 and 1 or NaN when no score could be calculated</returns>
        public double GetMutationScore() => DetectedMutants().Count() / (double)ValidMutants().Count();

        public Health CheckHealth(IThresholds threshold)
        {
            var mutationScore = GetMutationScore();
            if (double.IsNaN(mutationScore))
            {
                // The mutation score is outside the bounds we can work with, so we don't have any health status
                return Health.None;
            }

            return (mutationScore*100) switch
            {
                var score when score >= threshold.High => Health.Good,
                var score when score < threshold.High && score >= threshold.Low => Health.Warning,
                _ => Health.Danger
            };
        }
    }

    public abstract class ProjectComponent<T> : ProjectComponent
    {
        /// <summary>
        /// All syntax trees that should be a part of the compilation
        /// </summary>
        public abstract IEnumerable<T> CompilationSyntaxTrees { get; }
        /// <summary>
        /// Only those syntax trees that were changed by the mutation process
        /// </summary>
        public abstract IEnumerable<T> MutatedSyntaxTrees { get; }
    }

}
