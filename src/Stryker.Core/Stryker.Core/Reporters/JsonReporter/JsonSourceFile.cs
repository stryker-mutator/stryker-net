using Microsoft.Extensions.Logging;
using Stryker.Core.Logging;
using Stryker.Core.ProjectComponents;
using System.Collections.Generic;
using System.Linq;

namespace Stryker.Core.Reporters.Json
{
    public class JsonSourceFile
    {
        public string Language { get; init; }
        public string Source { get; init; }
        public ISet<JsonMutant> Mutants { get; init; }

        public JsonSourceFile()
        {
        }

        public JsonSourceFile(ReadOnlyFileLeaf file, ILogger logger = null)
        {
            logger ??= ApplicationLogging.LoggerFactory.CreateLogger<JsonSourceFile>();

            Source = file.SourceCode;
            Language = "cs";
            Mutants = new HashSet<JsonMutant>(new UniqueJsonMutantComparer());

            foreach (var mutant in file.Mutants)
            {
                var jsonMutant = new JsonMutant
                {
                    Id = mutant.Id.ToString(),
                    MutatorName = mutant.Mutation.DisplayName ?? "",
                    Replacement = mutant.Mutation.ReplacementNode.ToFullString(),
                    Location = new SourceLocation(mutant.Mutation.OriginalNode.GetLocation().GetMappedLineSpan()),
                    Status = mutant.ResultStatus.ToString(),
                    Description = mutant.Mutation.Description ?? "",
                    Static = mutant.IsStaticValue,
                    CoveredBy = mutant.CoveringTests?.GetGuids()?.Select(t => t.ToString())
                };

                if (!Mutants.Add(jsonMutant))
                {
                    logger.LogWarning(
                        $"Mutant {mutant.Id} was generated twice in file {file.RelativePath}. \n" +
                        $"This should not have happened. Please create an issue at https://github.com/stryker-mutator/stryker-net/issues");
                }
            }
        }

        private class UniqueJsonMutantComparer : EqualityComparer<JsonMutant>
        {
            public override bool Equals(JsonMutant left, JsonMutant right) => left.Id == right.Id;

            public override int GetHashCode(JsonMutant jsonMutant) => jsonMutant.Id.GetHashCode();
        }
    }
}
