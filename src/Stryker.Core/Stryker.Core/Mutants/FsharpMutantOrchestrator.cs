using System;
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
using static FSharp.Compiler.SyntaxTree.SynConst;
using static FSharp.Compiler.SyntaxTree.SynPat;

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
            var mutation = /*treeroot*/Mutate(treeroot, mutationContext);

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
            return mutation;
        }

        private FSharpList<SynModuleOrNamespace> Mutate(FSharpList<SynModuleOrNamespace> treeroot, MutationContext context)
        {
            var mutated = VisitModulesAndNamespaces(treeroot);
            return mutated;
        }

        private FSharpList<SynModuleOrNamespace> VisitModulesAndNamespaces(FSharpList<SynModuleOrNamespace> modules)
        {
            var list = new List<SynModuleOrNamespace>();
            foreach (SynModuleOrNamespace module in modules)
            {
                //Console.WriteLine("Namespace or module: " + module.longId);
                list.Add(SynModuleOrNamespace.NewSynModuleOrNamespace(module.longId, module.isRecursive, module.kind, VisitDeclarations(module.decls), module.xmlDoc, module.attribs, module.accessibility, module.range));
            }
            return ListModule.OfSeq(list);
        }

        private FSharpList<SynModuleDecl> VisitDeclarations(FSharpList<SynModuleDecl> decls)
        {
            var list = new List<SynModuleDecl>();
            foreach (SynModuleDecl declaration in decls)
            {
                if (declaration.IsLet)
                {
                    var childlist = new List<SynBinding>();
                    foreach (var binding in ((SynModuleDecl.Let)declaration).bindings)
                    {
                        //VisitPattern(binding.headPat);
                        childlist.Add(SynBinding.NewBinding(binding.accessibility, binding.kind, binding.mustInline, binding.isMutable, binding.attributes, binding.xmlDoc, binding.valData, binding.headPat, binding.returnInfo, VisitExpression(binding.expr), binding.range, binding.seqPoint));
                    }
                    list.Add(SynModuleDecl.NewLet(((SynModuleDecl.Let)declaration).isRecursive, ListModule.OfSeq(childlist), ((SynModuleDecl.Let)declaration).range));
                }
                else if (declaration.IsNestedModule)
                {
                    var decla = (SynModuleDecl.NestedModule)declaration;
                    var visitedDeclarations = VisitDeclarations(decla.decls);
                    list.Add(SynModuleDecl.NewNestedModule(decla.moduleInfo, decla.isRecursive, visitedDeclarations, decla.isContinuing, decla.range));
                }
                else
                {
                    list.Add(declaration);
                    //Console.WriteLine(" - not supported declaration: " + declaration);
                }
            }
            return ListModule.OfSeq(list);
        }

        private SynExpr VisitExpression(SynExpr expr)
        {
            if (expr.IsIfThenElse)
            {
                var expression = (SynExpr.IfThenElse)expr;
                //Console.WriteLine("Conditional:");
                expr = SynExpr.NewIfThenElse(VisitExpression(expression.ifExpr), VisitExpression(expression.thenExpr), VisitExpression(expression.elseExpr.Value), expression.spIfToThen, expression.isFromErrorRecovery, expression.ifToThenRange, expression.range);
            }
            else if (expr.IsLetOrUse)
            {
                var expression = (SynExpr.LetOrUse)expr;
                //Console.WriteLine("LetOrUse with the following bindings:");
                var childlist = new List<SynBinding>();
                foreach (var binding in expression.bindings)
                {
                    //VisitPattern(binding.headPat);
                    childlist.Add(SynBinding.NewBinding(binding.accessibility, binding.kind, binding.mustInline, binding.isMutable, binding.attributes, binding.xmlDoc, binding.valData, binding.headPat, binding.returnInfo, VisitExpression(binding.expr), binding.range, binding.seqPoint));
                }
                //Console.WriteLine("And the following body:");
                expr = SynExpr.NewLetOrUse(expression.isRecursive, expression.isUse, ListModule.OfSeq(childlist), VisitExpression(expression.body), expression.range);
            }
            else if (expr.IsMatch)
            {
                var list = ((SynExpr.Match)expr).clauses.ToList();
                foreach (var clause in ((SynExpr.Match)expr).clauses)
                {
                    if (clause.pat.IsConst && ((Const)clause.pat).constant.IsBool)
                    {
                        list[((SynExpr.Match)expr).clauses.ToList().FindIndex(x => x.Equals(clause))] = SynMatchClause.NewClause(NewConst(NewBool(!((Bool)((Const)clause.pat).constant).Item), ((Const)clause.pat).Range), clause.whenExpr, clause.resultExpr, clause.range, clause.spInfo);
                    }
                }
                expr = SynExpr.NewMatch(((SynExpr.Match)expr).matchSeqPoint, ((SynExpr.Match)expr).expr, ListModule.OfSeq(list), ((SynExpr.Match)expr).range);
                //Console.WriteLine(expr);
            }

            else
            {
                //Console.WriteLine(" - not supported expression: " + expr);
            }
            return expr;
        }

        private void VisitPattern(SynPat headPat)
        {
            if (headPat.IsWild)
            {
                //Console.WriteLine("  .. underscore pattern");
            }
            else if (headPat.IsNamed)
            {
                //VisitPattern(((SynPat.Named)headPat).pat);
                //Console.WriteLine("  .. named as " + ((SynPat.Named)headPat).ident.idText);
            }
            else if (headPat.IsLongIdent)
            {
                //var longIndent = (SynPat.LongIdent)headPat;
                //string names = "";
                //foreach (var i in longIndent.longDotId.id)
                //{
                //    names = string.Concat(".", i.idText);
                //}
                //Console.WriteLine("  .. identifier: " + names);
            }
            else
            {
                //Console.WriteLine("  .. other pattern: " + headPat);
            }
        }
    }
}
