using System.Collections.Generic;
using System.Linq;
using Microsoft.FSharp.Collections;
using static FSharp.Compiler.SyntaxTree;
using static FSharp.Compiler.SyntaxTree.SynConst;
using static FSharp.Compiler.SyntaxTree.SynPat;

namespace Stryker.Core.Mutants.FsharpOrchestrator
{
    public class IfThenElseOrchestrator : IFsharpTypehandle<SynExpr>
    {
        public SynExpr Mutate(SynExpr input, FsharpCoreOrchestrator iterator)
        {
            var castinput = input as SynExpr.IfThenElse;

            //Console.WriteLine("Conditional:");
            return SynExpr.NewIfThenElse(iterator.Mutate(castinput.ifExpr), iterator.Mutate(castinput.thenExpr), iterator.Mutate(castinput.elseExpr.Value), castinput.spIfToThen, castinput.isFromErrorRecovery, castinput.ifToThenRange, castinput.range);
        }
    }

    public class LetOrUseOrchestrator : IFsharpTypehandle<SynExpr>
    {
        public SynExpr Mutate(SynExpr input, FsharpCoreOrchestrator iterator)
        {
            var castinput = input as SynExpr.LetOrUse;

            //Console.WriteLine("LetOrUse with the following bindings:");
            var childlist = new List<SynBinding>();
            foreach (var binding in castinput.bindings)
            {
                //VisitPattern(binding.headPat);
                childlist.Add(SynBinding.NewBinding(binding.accessibility, binding.kind, binding.mustInline, binding.isMutable, binding.attributes, binding.xmlDoc, binding.valData, binding.headPat, binding.returnInfo, iterator.Mutate(binding.expr), binding.range, binding.seqPoint));
            }
            //Console.WriteLine("And the following body:");
            return SynExpr.NewLetOrUse(castinput.isRecursive, castinput.isUse, ListModule.OfSeq(childlist), iterator.Mutate(castinput.body), castinput.range);
        }
    }

    public class MatchOrchestrator : IFsharpTypehandle<SynExpr>
    {
        public SynExpr Mutate(SynExpr input, FsharpCoreOrchestrator iterator)
        {
            var castinput = input as SynExpr.Match;

            var list = castinput.clauses.ToList();
            foreach (var clause in castinput.clauses)
            {
                if (clause.pat.IsConst && ((Const)clause.pat).constant.IsBool)
                {
                    list[castinput.clauses.ToList().FindIndex(x => x.Equals(clause))] = SynMatchClause.NewClause(NewConst(NewBool(!((Bool)((Const)clause.pat).constant).Item), ((Const)clause.pat).Range), clause.whenExpr, clause.resultExpr, clause.range, clause.spInfo);
                }
            }
            return SynExpr.NewMatch(castinput.matchSeqPoint, castinput.expr, ListModule.OfSeq(list), castinput.range);
        }
    }
}
