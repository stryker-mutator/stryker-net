using Stryker.Core.Initialisation.ProjectComponent;
using System.Collections.Generic;

namespace Stryker.Core.Reporters.Json
{
    public class JsonReportComponent
    {
        public string Language { get; }
        public string Health { get; set; }
        public string Source { get; }

        public IList<JsonMutant> Mutants { get; }

        public JsonReportComponent(IReadOnlyInputComponent component)
        {
            if (component is FileLeaf fileComponent)
            {
                Source = fileComponent.SourceCode;
                Language = "cs";
                Mutants = new List<JsonMutant>();

                foreach (var mutant in fileComponent.Mutants)
                {
                    var jsonMutant = new JsonMutant
                    {
                        Id = mutant.Id,
                        MutatorName = mutant.Mutation.DisplayName,
                        Replacement = mutant.Mutation.ReplacementNode.ToFullString(),
                        Location = new JsonMutant.JsonMutantLocation(mutant.Mutation.OriginalNode.SyntaxTree.GetLineSpan(mutant.Mutation.OriginalNode.FullSpan)),
                        Status = mutant.ResultStatus.ToString()
                    };

                    Mutants.Add(jsonMutant);
                }
            }
        }
    }
}