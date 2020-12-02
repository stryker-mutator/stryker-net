using System.Collections.Generic;
using Microsoft.FSharp.Collections;
using Stryker.Core.Mutants.FsharpOrchestrator;
using static FSharp.Compiler.SyntaxTree;

namespace Stryker.Core.Mutants
{
    public class FsharpBaseOrchestrator
    {
        private OrchestratorFinder<SynModuleDecl> _fsharpMutationsSynModuleDecl;
        private OrchestratorFinder<SynExpr> _fsharpMutationsSynExpr;

        public FsharpBaseOrchestrator()
        {
            _fsharpMutationsSynModuleDecl = new OrchestratorFinder<SynModuleDecl>();
            _fsharpMutationsSynModuleDecl.Add(typeof(SynModuleDecl.Let), new LetOrchestrator());
            _fsharpMutationsSynModuleDecl.Add(typeof(SynModuleDecl.NestedModule), new NestedModuleOrchestrator());

            _fsharpMutationsSynExpr = new OrchestratorFinder<SynExpr>();
            _fsharpMutationsSynExpr.Add(typeof(SynExpr.Match), new MatchOrchestrator());
            _fsharpMutationsSynExpr.Add(typeof(SynExpr.LetOrUse), new LetOrUseOrchestrator());
            _fsharpMutationsSynExpr.Add(typeof(SynExpr.IfThenElse), new IfThenElseOrchestrator());
        }

        public FSharpList<SynModuleOrNamespace> Mutate(FSharpList<SynModuleOrNamespace> modules)
        {
            var list = new List<SynModuleOrNamespace>();
            foreach (SynModuleOrNamespace module in modules)
            {
                //Console.WriteLine("Namespace or module: " + module.longId);
                var mutation = Mutate(module.decls);
                list.Add(SynModuleOrNamespace.NewSynModuleOrNamespace(module.longId, module.isRecursive, module.kind, mutation, module.xmlDoc, module.attribs, module.accessibility, module.range));
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
