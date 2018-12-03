using Stryker.Core.Mutants;
using System.Collections.Generic;
using System.Linq;

namespace Stryker.Core.Initialisation.ProjectComponent
{
    public abstract class ProjectComponent : IReadOnlyInputComponent
    {
        public string Name { get; set; }
        public abstract IEnumerable<Mutant> Mutants { get; set; }
        public IEnumerable<IReadOnlyMutant> ReadOnlyMutants => Mutants.Cast<IReadOnlyMutant>();
        public IEnumerable<IReadOnlyMutant> TotalMutants => ReadOnlyMutants.Where(m => m.ResultStatus != MutantStatus.BuildError && m.ResultStatus != MutantStatus.Skipped);
        public IEnumerable<IReadOnlyMutant> DetectedMutants => ReadOnlyMutants.Where(m => 
        m.ResultStatus == MutantStatus.Killed ||
        m.ResultStatus == MutantStatus.Timeout);
        public IReadOnlyCollection<FileLeaf> ExcludedFiles => GetAllFiles().Where(f => f.IsExcluded).ToList();

        // These delegates will get invoked while walking the tree during Display();
        public Display DisplayFile { get; set; }
        public Display DisplayFolder { get; set; }


        public abstract void Add(ProjectComponent component);
        public abstract void Display(int depth);
        public abstract IEnumerable<FileLeaf> GetAllFiles();

        /// <summary>
        /// Returns the mutation score for this folder / file
        /// </summary>
        /// <returns>decimal between 0 and 1 or null when no score could be calculated</returns>
        public decimal? GetMutationScore()
        {
            var totalCount = TotalMutants.Count();
            var killedCount = DetectedMutants.Count();
            if(totalCount > 0)
            {
                return killedCount / (decimal)totalCount;
            } else
            {
                return null;
            }
        }
    }
}
