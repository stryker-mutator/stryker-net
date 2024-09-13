using System.Collections.Generic;
using Microsoft.Extensions.Logging;
using Stryker.Abstractions.Logging;
using Stryker.Abstractions.ProjectComponents;
using Stryker.Abstractions.Reporting;

namespace Stryker.Core.Reporters.Json.SourceFiles
{
    public class SourceFile : ISourceFile
    {
        public string Language { get; init; }
        public string Source { get; init; }
        public ISet<IJsonMutant> Mutants { get; init; }

        public SourceFile() { }

        public SourceFile(IReadOnlyFileLeaf file, ILogger logger = null)
        {
            logger ??= ApplicationLogging.LoggerFactory.CreateLogger<SourceFile>();

            Source = file.SourceCode;
            Language = "cs";
            Mutants = new HashSet<IJsonMutant>(new UniqueJsonMutantComparer());

            foreach (var mutant in file.Mutants)
            {
                if (!Mutants.Add(new JsonMutant(mutant)))
                {
                    logger.LogWarning(
                        "Mutant {Id} was generated twice in file {RelativePath}. \n" +
                        "This should not have happened. Please create an issue at https://github.com/stryker-mutator/stryker-net/issues",
                        mutant.Id, file.RelativePath);
                }
            }
        }

        private sealed class UniqueJsonMutantComparer : EqualityComparer<IJsonMutant>
        {
            public override bool Equals(IJsonMutant left, IJsonMutant right) => left.Id == right.Id;

            public override int GetHashCode(IJsonMutant jsonMutant) => jsonMutant.Id.GetHashCode();
        }
    }
}
