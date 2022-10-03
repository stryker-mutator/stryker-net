using static FSharp.Compiler.SyntaxTree;

namespace Stryker.Core.Mutants.FsharpOrchestrators
{
    public class IfThenElseOrchestrator : IFsharpTypeHandler<SynExpr>
    {
        public SynExpr Mutate(SynExpr input, FsharpMutantOrchestrator iterator)
        {
            var castinput = input as SynExpr.IfThenElse;

            return SynExpr.NewIfThenElse(iterator.Mutate(castinput.ifExpr), iterator.Mutate(castinput.thenExpr), iterator.Mutate(castinput.elseExpr.Value), castinput.spIfToThen, castinput.isFromErrorRecovery, castinput.ifToThenRange, castinput.range);
        }
    }
}
