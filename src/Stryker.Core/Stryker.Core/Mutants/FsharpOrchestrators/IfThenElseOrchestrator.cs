using FSharp.Compiler.Syntax;

namespace Stryker.Core.Mutants.FsharpOrchestrators
{
    public class IfThenElseOrchestrator : IFsharpTypeHandler<SynExpr>
    {
        public SynExpr Mutate(SynExpr input, FsharpMutantOrchestrator iterator)
        {
            var castinput = input as SynExpr.IfThenElse;

            return SynExpr.NewIfThenElse(
                castinput.ifKeyword,
                castinput.isElif,
                iterator.Mutate(castinput.ifExpr),
                castinput.thenKeyword,
                iterator.Mutate(castinput.thenExpr),
                castinput.elseKeyword,
                iterator.Mutate(castinput.elseExpr.Value),
                castinput.spIfToThen,
                castinput.isFromErrorRecovery,
                castinput.ifToThenRange,
                castinput.range);
        }
    }
}
