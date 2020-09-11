using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Stryker.Core.InjectedHelpers;
using System.Collections.Generic;
using System.Linq;
using Stryker.Core.Instrumentation;

namespace Stryker.Core.Mutants
{
    public static class MutantPlacer
    {
        private const string MutationConditional = "MutationConditional";
        private const string MutationIf = "MutationIf";
        private const string MutationHelper = "Helper";
        private const string Injector = "Injector";
        private const string HelperId = "-1";

        private static readonly StaticInstrumentationEngine StaticEngine;
        private static readonly IfInstrumentationEngine IfEngine;
        private static readonly IDictionary<string, IInstrumentCode> InstrumentEngines = new Dictionary<string, IInstrumentCode>();

        public static IEnumerable<string> MutationMarkers => new[] { MutationConditional, MutationIf, MutationHelper};


        static MutantPlacer()
        {
            StaticEngine = new StaticInstrumentationEngine(Injector);
            RegisterEngine(StaticEngine);
            IfEngine = new IfInstrumentationEngine(Injector);
            RegisterEngine(IfEngine);
        }

        private static void RegisterEngine(IInstrumentCode engine)
        {
            InstrumentEngines.Add(engine.IInstrumentEngineID, engine);
        }

        public static BlockSyntax PlaceStaticContextMarker(BlockSyntax block) => StaticEngine.PlaceStaticContextMarker(block).WithAdditionalAnnotations(new SyntaxAnnotation(Injector, StaticEngine.IInstrumentEngineID));

        public static IfStatementSyntax PlaceWithIfStatement(StatementSyntax original, StatementSyntax mutated, int mutantId) =>
            IfEngine.InjectIf(GetBinaryExpression(mutantId), original, mutated)
                // Mark this node as a MutationIf node. Store the MutantId in the annotation to retrace the mutant later
                .WithAdditionalAnnotations(new SyntaxAnnotation(MutationIf, mutantId.ToString()));

        public static SyntaxNode RemoveMutant(SyntaxNode nodeToRemove)
        {
            var engine = nodeToRemove.GetAnnotatedNodes(Injector).FirstOrDefault()?.GetAnnotations(Injector).First().Data;
            if (!string.IsNullOrEmpty(engine))
            {
                return InstrumentEngines[engine].RemoveInstrumentation(nodeToRemove);
            }

            return nodeToRemove switch
            {
                // remove the mutated node using its MutantPlacer remove method and update the tree
                ParenthesizedExpressionSyntax parenthesizedExpression => RemoveByConditionalExpression(
                    parenthesizedExpression),
                _ => nodeToRemove.GetAnnotatedNodes(StaticEngine.IInstrumentEngineID).Any()
                    ? SyntaxFactory.EmptyStatement()
                    : nodeToRemove
            };
        }

        public static ParenthesizedExpressionSyntax PlaceWithConditionalExpression(ExpressionSyntax original, ExpressionSyntax mutated, int mutantId) =>
            SyntaxFactory.ParenthesizedExpression(
                    SyntaxFactory.ConditionalExpression(
                        condition: GetBinaryExpression(mutantId),
                        whenTrue: mutated,
                        whenFalse: original))
                // Mark this node as a MutationConditional node. Store the MutantId in the annotation to retrace the mutant later
                .WithAdditionalAnnotations(new SyntaxAnnotation(MutationConditional, mutantId.ToString()));

        // us this method to annotate injected helper code. I.e. any injected code that is NOT a mutation but provides some infrastructure for the mutant to run
        public static T AnnotateHelper<T>(T node) where T:SyntaxNode => node.WithAdditionalAnnotations(new SyntaxAnnotation(MutationHelper, HelperId));

        private static SyntaxNode RemoveByConditionalExpression(ParenthesizedExpressionSyntax parenthesized) => 
            parenthesized.Expression is ConditionalExpressionSyntax conditional ? conditional.WhenFalse : null;

        /// <summary>
        /// Builds a syntax for the expression to check if a mutation is active
        /// Example for mutantId 1: Stryker.Helper.ActiveMutation == 1
        /// </summary>
        /// <param name="mutantId"></param>
        /// <returns></returns>
        private static ExpressionSyntax GetBinaryExpression(int mutantId) => 
            SyntaxFactory.ParseExpression(CodeInjection.SelectorExpression.Replace("ID", mutantId.ToString()));
    }
}
