using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Microsoft.Extensions.Logging;
using Microsoft.FSharp.Collections;
using Stryker.Core.Helpers;
using Stryker.Core.Logging;
using Stryker.Core.Mutants.NodeOrchestrators;
using Stryker.Core.Mutators;
using Stryker.Core.Options;
using static FSharp.Compiler.SyntaxTree;

namespace Stryker.Core.Mutants
{
    internal class FsharpMutantOrchestrator : BaseMutantOrchestrator
    {

        private readonly TypeBasedStrategy<SynExpr, IFsharpNodeMutator> _specificOrchestrator =
            new TypeBasedStrategy<SynExpr, IFsharpNodeMutator>();

        internal IEnumerable<IMutator> Mutators { get; }
        private ILogger Logger { get; }

        public FsharpMutantOrchestrator(IEnumerable<IMutator> mutators = null, StrykerOptions options = null) : base(options)
        {
            Mutators = mutators ?? new List<IMutator>
            {
            };
            Mutants = new Collection<Mutant>();
            Logger = ApplicationLogging.LoggerFactory.CreateLogger<MutantOrchestrator>();

            _specificOrchestrator.RegisterHandlers(new List<IFsharpNodeMutator>
            {
            });
        }

        public FSharpList<SynModuleOrNamespace> Mutate(FSharpList<SynModuleOrNamespace> treeroot)
        {
            var mutationContext = new MutationContext(this);
            //var mutation = Mutate(treeroot, mutationContext);
            if (mutationContext.HasStatementLevelMutant && _options?.DevMode == true)
            {
                // some mutants where not injected for some reason, they should be reviewed to understand why.
                Logger.LogError($"Several mutants were not injected in the project : {mutationContext.BlockLevelControlledMutations.Count + mutationContext.StatementLevelControlledMutations.Count}");
            }
            // mark remaining mutants as CompileError
            foreach (var mutant in mutationContext.StatementLevelControlledMutations.Union(mutationContext.BlockLevelControlledMutations))
            {
                mutant.ResultStatus = MutantStatus.CompileError;
                mutant.ResultStatusReason = "Stryker was not able to inject mutation in code.";
            }

            return /*mutation*/ treeroot;
        }

        //private FSharpList<SynModuleOrNamespace> Mutate(FSharpList<SynModuleOrNamespace> treeroot, MutationContext context)
        //{
        //    // don't mutate immutable nodes
        //    if (!SyntaxHelper.CanBeMutated(currentNode))
        //    {
        //        return currentNode;
        //    }

        //    // search for node specific handler
        //    var nodeHandler = _specificOrchestrator.FindHandler(currentNode);
        //    return nodeHandler.Mutate(currentNode, context);
        //}

        //internal IEnumerable<Mutant> GenerateMutationsForNodeFsharp(clause current, MutationContext context)
        //{
        //    var mutations = new List<Mutant>();
        //    foreach (var mutator in Mutators)
        //    {
        //        foreach (var mutation in mutator.Mutate(current, _options))
        //        {
        //            var id = MutantCount;
        //            Logger.LogDebug("Mutant {0} created {1} -> {2} using {3}", id, mutation.OriginalNode,
        //                mutation.ReplacementNode, mutator.GetType());
        //            var newMutant = new Mutant
        //            {
        //                Id = id,
        //                Mutation = mutation,
        //                ResultStatus = MutantStatus.NotRun,
        //                IsStaticValue = context.InStaticValue
        //            };
        //            var duplicate = false;
        //            // check if we have a duplicate
        //            foreach (var mutant in Mutants)
        //            {
        //                if (mutant.Mutation.OriginalNode != mutation.OriginalNode ||
        //                    !SyntaxFactory.AreEquivalent(mutant.Mutation.ReplacementNode, newMutant.Mutation.ReplacementNode))
        //                {
        //                    continue;
        //                }
        //                Logger.LogDebug($"Mutant {id} discarded as it is a duplicate of {mutant.Id}");
        //                duplicate = true;
        //                break;
        //            }

        //            if (duplicate)
        //            {
        //                continue;
        //            }

        //            Mutants.Add(newMutant);
        //            MutantCount++;
        //            mutations.Add(newMutant);
        //        }
        //    }

        //    return mutations;
        //}
    }
}
