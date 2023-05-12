namespace Stryker.Core.Mutants.FsharpOrchestrators;
using FSharp.Compiler.Syntax;

public class NestedModuleOrchestrator : IFsharpTypeHandler<SynModuleDecl>
{
    public SynModuleDecl Mutate(SynModuleDecl input, FsharpMutantOrchestrator iterator)
    {
        var castinput = input as SynModuleDecl.NestedModule;

        var visitedDeclarations = iterator.Mutate(castinput.decls);
        return SynModuleDecl.NewNestedModule(
            castinput.moduleInfo,
            castinput.isRecursive,
            visitedDeclarations,
            castinput.isContinuing,
            castinput.range,
            castinput.trivia);
    }
}
