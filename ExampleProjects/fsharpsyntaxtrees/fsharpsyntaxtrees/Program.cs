using FSharp.Compiler;
using FSharp.Compiler.SourceCodeServices;
using FSharp.Compiler.Text;
using Microsoft.FSharp.Collections;
using Microsoft.FSharp.Control;
using Microsoft.FSharp.Core;
using System;
using System.IO;
using System.Linq;
using static FSharp.Compiler.SourceCodeServices.AstTraversal.TraverseStep;
using static FSharp.Compiler.SyntaxTree;
using static FSharp.Compiler.SyntaxTree.ParsedInput;
using static FSharp.Compiler.SyntaxTree.SynConst;
using static FSharp.Compiler.SyntaxTree.SynPat;

namespace fsharpsyntaxtrees
{
    class Program
    {
        static void Main(string[] args)
        {
            FSharpChecker fSharpChecker = FSharpChecker.Create(null, null, null, null, null, null, null, null);

            var fullPath = Path.GetFullPath("..\\..\\..\\..\\..\\boolMutateTestApp\\boolMutateTestApp\\Program.fs");
            var sourceCode = File.ReadAllText(fullPath);

            Tuple<FSharpProjectOptions, FSharpList<FSharpErrorInfo>>  fsharpoptions = FSharpAsync.RunSynchronously(fSharpChecker.GetProjectOptionsFromScript(fullPath, SourceText.ofString(sourceCode), null, null, null, null, null, null, null, null, null), null, null);
            FSharpParseFileResults result = FSharpAsync.RunSynchronously(fSharpChecker.ParseFile(fullPath, SourceText.ofString(sourceCode), fSharpChecker.GetParsingOptionsFromProjectOptions(fsharpoptions.Item1).Item1, null), null, null);
            var syntaxTree = result.ParseTree;

            if (syntaxTree.Value.IsImplFile) {
                var test = ((ImplFile)syntaxTree.Value).Item;
                VisitModulesAndNamespaces(test.modules);
            }
            else
            {
                Console.WriteLine("F# Interface file (*.fsi) not supported.");
            }
            //fSharpChecker.CompileToDynamicAssembly()
            //    FSharpAsync<Tuple<FSharpErrorInfo[], int>> Compile(FSharpList<SyntaxTree.ParsedInput> ast, string assemblyName, string outFile, FSharpList<string> dependencies, [OptionalArgument] FSharpOption<string> pdbFile, [OptionalArgument] FSharpOption<bool> executable, [OptionalArgument] FSharpOption<bool> noframework, [OptionalArgument] FSharpOption<string> userOpName);

        }

        private static void VisitModulesAndNamespaces(FSharpList<SynModuleOrNamespace> modules)
        {
            foreach(SynModuleOrNamespace module in modules)
            {
                Console.WriteLine("Namespace or module: " + module.longId);
                VisitDeclarations(module.decls);
            }
        }

        private static void VisitDeclarations(FSharpList<SynModuleDecl> decls)
        {
            foreach(SynModuleDecl declaration in decls)
            {
                if (declaration.IsLet)
                {
                    foreach(var binding in ((SynModuleDecl.Let)declaration).bindings)
                    {
                        VisitPattern(binding.headPat);
                        VisitExpression(binding.expr);
                    }
                }
                else
                {
                    Console.WriteLine(" - not supported declaration: " + declaration);
                }
            }
        }

        private static void VisitExpression(SynExpr expr)
        {
            if (expr.IsIfThenElse)
            {
                SynExpr.IfThenElse expression = (SynExpr.IfThenElse)expr;
                Console.WriteLine("Conditional:");
                VisitExpression(expression.ifExpr);
                VisitExpression(expression.thenExpr);
                VisitExpression(expression.elseExpr.Value);
            }
            else if (expr.IsLetOrUse)
            {
                SynExpr.LetOrUse expression = (SynExpr.LetOrUse)expr;
                Console.WriteLine("LetOrUse with the following bindings:");
                foreach (var binding in expression.bindings)
                {
                    VisitPattern(binding.headPat);
                    VisitExpression(binding.expr);
                }
                Console.WriteLine("And the following body:");
                VisitExpression(expression.body);
            }
            else if (expr.IsMatch)
            {
                SynMatchClause toReplace = null;
                foreach(var clause in ((SynExpr.Match)expr).clauses)
                {
                    if (clause.pat.IsConst)
                    {
                        if (((Const)clause.pat).constant.IsBool && ((Bool)((Const)clause.pat).constant).Item == true)
                        {
                            toReplace = clause;
                        }
                    }
                }
                if (toReplace != null)
                {
                    var list = ((SynExpr.Match)expr).clauses.ToList();
                    list[((SynExpr.Match)expr).clauses.ToList().FindIndex(x => x.Equals(toReplace))] = SynMatchClause.NewClause(NewConst(NewBool(false), ((Const)toReplace.pat).Range), toReplace.whenExpr, toReplace.resultExpr, toReplace.range, toReplace.spInfo);
                    expr = SynExpr.NewMatch(((SynExpr.Match)expr).matchSeqPoint, ((SynExpr.Match)expr).expr, ListModule.OfSeq(list), ((SynExpr.Match)expr).range);                 
                }
                Console.WriteLine(expr);
            }

            else
            {
                Console.WriteLine(" - not supported expression: " + expr);
            }
        }

        private static void VisitPattern(SynPat headPat)
        {
            if (headPat.IsWild)
            {
                Console.WriteLine("  .. underscore pattern");
            }
            else if (headPat.IsNamed)
            {
                VisitPattern(((SynPat.Named)headPat).pat);
                Console.WriteLine("  .. named as " + ((SynPat.Named)headPat).ident.idText);
            }
            else if (headPat.IsLongIdent)
            {
                var longIndent = (SynPat.LongIdent)headPat;
                string names = "";
                foreach (var i in longIndent.longDotId.id)
                {
                   names = string.Concat(".", i.idText);
                }
                Console.WriteLine("  .. identifier: " + names);
            }
            else
            {
                Console.WriteLine("  .. other pattern: " + headPat);
            }
        }
    }
}


/* https://fsharp.github.io/FSharp.Compiler.Service/untypedtree.html */
