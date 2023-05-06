using System.Collections.Generic;
using Microsoft.Extensions.Logging;
using Stryker.Core.Logging;
using Stryker.Core.ProjectComponents;

namespace Stryker.Core.Reporters.Json.SourceFiles;

public class SourceFile
{
    public string Language { get; init; }
    public string Source { get; init; }
    public ISet<JsonMutant> Mutants { get; init; }

    public SourceFile() { }

    public SourceFile(IReadOnlyFileLeaf file, ILogger logger = null)
    {
        logger ??= ApplicationLogging.LoggerFactory.CreateLogger<SourceFile>();

        Source = file.SourceCode;
        Language = "cs";
        Mutants = new HashSet<JsonMutant>(new UniqueJsonMutantComparer());

        foreach (var mutant in file.Mutants)
        {
            if (!Mutants.Add(new JsonMutant(mutant)))
            {
                logger.LogWarning(
                    $"Mutant {mutant.Id} was generated twice in file {file.RelativePath}. \n" +
                    $"This should not have happened. Please create an issue at https://github.com/stryker-mutator/stryker-net/issues");
            }
        }
    }

    private sealed class UniqueJsonMutantComparer : EqualityComparer<JsonMutant>
    {
        public override bool Equals(JsonMutant left, JsonMutant right) => left.Id == right.Id;

        public override int GetHashCode(JsonMutant jsonMutant) => jsonMutant.Id.GetHashCode();
    }
}
