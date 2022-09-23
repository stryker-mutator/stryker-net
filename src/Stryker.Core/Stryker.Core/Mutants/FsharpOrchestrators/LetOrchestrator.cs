using System.Collections.Generic;
using Microsoft.FSharp.Collections;
using static FSharp.Compiler.SyntaxTree;

namespace Stryker.Core.Mutants.FsharpOrchestrators
{
    public class LetOrchestrator : IFsharpTypeHandler<SynModuleDecl>
    {
        public SynModuleDecl Mutate(SynModuleDecl input, FsharpMutantOrchestrator iterator)
        {
            var castinput = input as SynModuleDecl.Let;

            var childlist = new List<SynBinding>();
            foreach (var binding in castinput.bindings)
            {
                childlist.Add(SynBinding.NewBinding(binding.accessibility, binding.kind, binding.mustInline, binding.isMutable, binding.attributes, binding.xmlDoc, binding.valData, binding.headPat, binding.returnInfo, iterator.Mutate(binding.expr), binding.range, binding.seqPoint));
            }
            return SynModuleDecl.NewLet(castinput.isRecursive, ListModule.OfSeq(childlist), castinput.range);
        }
    }

}
