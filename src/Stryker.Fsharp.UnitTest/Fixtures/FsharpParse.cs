using FSharp.Compiler.CodeAnalysis;
using FSharp.Compiler.Syntax;
using FSharp.Compiler.Text;
using Microsoft.FSharp.Control;
using Stryker.Fsharp.ProjectComponents;

namespace Stryker.Fsharp.UnitTest.Fixtures;

internal static class FsharpParse
{
    private static readonly FSharpChecker Checker = FSharpChecker.Create(null, null, null, null, null, null, null, null, null, null, null, null, null, null);

    public static FsharpFileLeaf Leaf(string source, string path = "/proj/M.fs")
    {
        var (parsingOptions, _) = Checker.GetParsingOptionsFromCommandLineArgs([path], Microsoft.FSharp.Collections.FSharpList<string>.Empty, null, null);
        var results = FSharpAsync.RunSynchronously(Checker.ParseFile(path, SourceText.ofString(source), parsingOptions, null, null), null, null);
        return new FsharpFileLeaf { SourceCode = source, ParseTree = results.ParseTree, FullPath = path, RelativePath = Path.GetFileName(path) };
    }

    /// <summary>The expression of the first top-level binding of the first module.</summary>
    public static SynExpr FirstBindingExpression(FsharpFileLeaf leaf)
    {
        var impl = (ParsedInput.ImplFile)leaf.ParseTree!;
        var module = impl.Item.Contents.First();
        var let = (SynModuleDecl.Let)module.decls.First(d => d.IsLet);
        return let.bindings.First().expr;
    }
}
