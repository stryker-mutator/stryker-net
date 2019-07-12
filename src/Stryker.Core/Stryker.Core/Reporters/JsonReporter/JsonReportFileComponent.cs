﻿using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Stryker.Core.Logging;
using Stryker.Core.ProjectComponents;
using System.Collections.Generic;
using System.Linq;

namespace Stryker.Core.Reporters.Json
{
    public class JsonReportFileComponent
    {
        public string Language { get; }
        public string Source { get; }
        public ISet<JsonMutant> Mutants { get; }

        [JsonConstructor]
        private JsonReportFileComponent(string language, string source, ISet<JsonMutant> mutants)
        {
            Language = language;
            Source = source;
            Mutants = mutants;
        }

        public JsonReportFileComponent(FileLeaf file, ILogger logger = null)
        {
            var log = logger ?? ApplicationLogging.LoggerFactory.CreateLogger<JsonReportFileComponent>();

            Source = file.SourceCode;
            Language = "cs";
            Mutants = new HashSet<JsonMutant>(new UniqueJsonMutantComparer());

            foreach (var mutant in file.Mutants)
            {
                var jsonMutant = new JsonMutant
                {
                    Id = mutant.Id,
                    MutatorName = mutant.Mutation.DisplayName,
                    Replacement = mutant.Mutation.ReplacementNode.ToFullString(),
                    Location = new JsonMutantLocation(mutant.Mutation.OriginalNode.GetLocation().GetMappedLineSpan()),
                    Status = mutant.ResultStatus.ToString()
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
            public override bool Equals(JsonMutant left, JsonMutant right)
            {
                return left.Id == right.Id;
            }

            public override int GetHashCode(JsonMutant jsonMutant)
            {
                return jsonMutant.Id.GetHashCode();
            }
        }
    }
}