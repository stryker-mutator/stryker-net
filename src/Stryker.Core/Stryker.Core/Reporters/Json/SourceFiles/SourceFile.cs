using System.Collections.Generic;
using System.Text.Json.Serialization;
using System.Text.Json;
using Microsoft.Extensions.Logging;
using Stryker.Abstractions.Logging;
using Stryker.Abstractions.ProjectComponents;
using Stryker.Abstractions.Reporting;
using System;

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

    public class SourceFileConverter : JsonConverter<ISourceFile>
    {
        public override ISourceFile Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            // Deserialize the JSON into the concrete type
            var sourceFile = JsonSerializer.Deserialize<SourceFile>(ref reader, options);
            return sourceFile;
        }

        public override void Write(Utf8JsonWriter writer, ISourceFile value, JsonSerializerOptions options)
        {
            // Serialize the concrete type
            JsonSerializer.Serialize(writer, (SourceFile)value, options);
        }
    }
}
