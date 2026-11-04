using System.Collections.Generic;
using FSharp.Compiler.Syntax;
using Stryker.Abstractions;

namespace Stryker.Fsharp.Mutators;

/// <summary>Produces mutations for an untyped F# expression. Replacement text must compile when parenthesised.</summary>
internal interface IFsharpMutator
{
    MutationLevel MutationLevel { get; }

    IEnumerable<Mutation> ApplyMutations(SynExpr expr, string sourceText);
}
