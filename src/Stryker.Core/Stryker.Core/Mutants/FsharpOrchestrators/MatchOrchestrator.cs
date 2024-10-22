using System.Linq;
using FSharp.Compiler.Syntax;
using Microsoft.FSharp.Collections;
using static FSharp.Compiler.Syntax.SynConst;
using static FSharp.Compiler.Syntax.SynPat;

namespace Stryker.Core.Mutants.FsharpOrchestrators;

//this orchestrtor also places a mutation. This is to proof the code works
//the mutation should come from a mutator, however mutators are currently not implemented
public class MatchOrchestrator : IFsharpTypeHandler<SynExpr>
{
    public SynExpr Mutate(SynExpr input, FsharpMutantOrchestrator iterator)
    {
        var castinput = input as SynExpr.Match;

        var list = castinput.clauses.ToList();
        foreach (var clause in castinput.clauses)
        {
            if (clause.pat.IsConst && ((Const)clause.pat).constant.IsBool)
            {
                //inverts boolean, true -> false and false -> true
                list[castinput.clauses.ToList().FindIndex(x => x.Equals(clause))] =
                    SynMatchClause.NewSynMatchClause(
                        NewConst(NewBool(!((Bool)((Const)clause.pat).constant).Item), ((Const)clause.pat).Range),
                        clause.whenExpr,
                        clause.resultExpr,
                        clause.range,
                        clause.debugPoint,
                        clause.trivia);
            }
        }
        return SynExpr.NewMatch(
            castinput.matchDebugPoint,
            castinput.expr,
            ListModule.OfSeq(list),
            castinput.range,
            castinput.trivia);
    }
}
