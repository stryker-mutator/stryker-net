using System.Collections.Generic;
using System.Collections.ObjectModel;
using FSharp.Compiler.Syntax;
using Microsoft.Extensions.Logging;
using Microsoft.FSharp.Collections;
using Stryker.Core.Logging;
using Stryker.Core.Mutants.FsharpOrchestrators;
using Stryker.Core.Mutators;
using Stryker.Core.Options;

namespace Stryker.Core.Mutants
{
    /// <inheritdoc/>
    public class FsharpMutantOrchestrator : BaseMutantOrchestrator<FSharpList<SynModuleOrNamespace>, object>
    {
        private readonly OrchestratorFinder<SynModuleDecl> _fsharpMutationsSynModuleDecl;
        private readonly OrchestratorFinder<SynExpr> _fsharpMutationsSynExpr;

        public IEnumerable<IMutator> Mutators { get; }

        private ILogger Logger { get; }

        public FsharpMutantOrchestrator(IEnumerable<IMutator> mutators = null, StrykerOptions options = null) : base(options)
        {
            _fsharpMutationsSynModuleDecl = new OrchestratorFinder<SynModuleDecl>();
            _fsharpMutationsSynModuleDecl.Add(typeof(SynModuleDecl.Let), new LetOrchestrator());
            _fsharpMutationsSynModuleDecl.Add(typeof(SynModuleDecl.NestedModule), new NestedModuleOrchestrator());

            _fsharpMutationsSynExpr = new OrchestratorFinder<SynExpr>();
            _fsharpMutationsSynExpr.Add(typeof(SynExpr.Match), new MatchOrchestrator());
            _fsharpMutationsSynExpr.Add(typeof(SynExpr.LetOrUse), new LetOrUseOrchestrator());
            _fsharpMutationsSynExpr.Add(typeof(SynExpr.IfThenElse), new IfThenElseOrchestrator());

            Mutators = mutators ?? new List<IMutator>
            {
            };

            Mutants = new Collection<Mutant>();
            Logger = ApplicationLogging.LoggerFactory.CreateLogger<FsharpMutantOrchestrator>();
        }

        public override FSharpList<SynModuleOrNamespace> Mutate(
            FSharpList<SynModuleOrNamespace> input, object semanticModel)
        {
            var list = new List<SynModuleOrNamespace>();
            foreach (SynModuleOrNamespace module in input)
            {
                var mutation = Mutate(module.decls);
                list.Add(SynModuleOrNamespace.NewSynModuleOrNamespace(
                    module.longId,
                    module.isRecursive,
                    module.kind,
                    mutation,
                    module.xmlDoc,
                    module.attribs,
                    module.accessibility,
                    module.range,
                    module.trivia));
            }
            return ListModule.OfSeq(list);
        }

        public FSharpList<SynModuleDecl> Mutate(FSharpList<SynModuleDecl> decls)
        {
            var list = new List<SynModuleDecl>();
            foreach (SynModuleDecl declaration in decls)
            {
                var handler = _fsharpMutationsSynModuleDecl.FindHandler(declaration.GetType());
                list.Add(handler.Mutate(declaration, this));
            }
            return ListModule.OfSeq(list);
        }

        public SynExpr Mutate(SynExpr expr)
        {
            var handler = _fsharpMutationsSynExpr.FindHandler(expr.GetType());
            return handler.Mutate(expr, this);
        }
    }
}
