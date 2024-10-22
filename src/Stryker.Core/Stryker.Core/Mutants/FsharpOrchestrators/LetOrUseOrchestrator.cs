using System.Collections.Generic;
using FSharp.Compiler.Syntax;
using Microsoft.FSharp.Collections;

namespace Stryker.Core.Mutants.FsharpOrchestrators;

public class LetOrUseOrchestrator : IFsharpTypeHandler<SynExpr>
{
    public SynExpr Mutate(SynExpr input, FsharpMutantOrchestrator iterator)
    {
        var castinput = input as SynExpr.LetOrUse;

        var childlist = new List<SynBinding>();
        foreach (var binding in castinput.bindings)
        {
            childlist.Add(SynBinding.NewSynBinding(
                binding.accessibility,
                binding.kind,
                binding.isInline,
                binding.isMutable,
                binding.attributes,
                binding.xmlDoc,
                binding.valData,
                binding.headPat,
                binding.returnInfo,
                iterator.Mutate(binding.expr),
                binding.range,
                binding.debugPoint,
                binding.trivia));
        }
        return SynExpr.NewLetOrUse(
            castinput.isRecursive,
            castinput.isUse,
            ListModule.OfSeq(childlist),
            iterator.Mutate(castinput.body),
            castinput.range,
            castinput.trivia);
    }
}
