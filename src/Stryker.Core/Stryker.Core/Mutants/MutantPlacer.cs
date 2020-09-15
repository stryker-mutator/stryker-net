using System;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Stryker.Core.InjectedHelpers;
using System.Collections.Generic;
using System.Linq;
using Stryker.Core.Instrumentation;

namespace Stryker.Core.Mutants
{
    /// <summary>
    /// Implements multiple (reversible) patterns for injecting code in the mutated assembly?
    /// Each pattern is implemented in a dedicated class.
    /// </summary>
    public static class MutantPlacer
    {
        private const string MutationMarker = "Mutation";
        private const string MutationHelper = "Helper";
        private const string Injector = "Injector";

        private static readonly StaticInstrumentationEngine StaticEngine;
        private static readonly IfInstrumentationEngine IfEngine;
        private static readonly ConditionalInstrumentationEngine ConditionalEngine;
        private static readonly ExpressionToBodyEngine ExpressionEngine;
        
        private static readonly IDictionary<string, IInstrumentCode> InstrumentEngines = new Dictionary<string, IInstrumentCode>();

        public static IEnumerable<string> MutationMarkers => new[] { MutationMarker, MutationHelper};

        static MutantPlacer()
        {
            StaticEngine = new StaticInstrumentationEngine(Injector);
            RegisterEngine(StaticEngine);
            IfEngine = new IfInstrumentationEngine(Injector);
            RegisterEngine(IfEngine);
            ConditionalEngine = new ConditionalInstrumentationEngine(Injector);
            RegisterEngine(ConditionalEngine);
            ExpressionEngine = new ExpressionToBodyEngine(Injector);
            RegisterEngine(ExpressionEngine);
        }

        /// <summary>
        ///  register an instrumentation engine
        /// </summary>
        /// <param name="engine"></param>
        public static void RegisterEngine(IInstrumentCode engine)
        {
            InstrumentEngines.Add(engine.InstrumentEngineID, engine);
        }

        public static T ConvertExpressionToBody<T>(T method) where T: BaseMethodDeclarationSyntax
        {
            return ExpressionEngine.ConvertToBody(method)
                .WithAdditionalAnnotations(new SyntaxAnnotation(MutationHelper));
        }

        public static BlockSyntax PlaceStaticContextMarker(BlockSyntax block) => StaticEngine.PlaceStaticContextMarker(block).WithAdditionalAnnotations(new SyntaxAnnotation(MutationHelper));

        public static IfStatementSyntax PlaceWithIfStatement(StatementSyntax original, StatementSyntax mutated, int mutantId) =>
            IfEngine.InjectIf(GetBinaryExpression(mutantId), original, mutated)
                // Mark this node as a MutationIf node. Store the MutantId in the annotation to retrace the mutant later
                .WithAdditionalAnnotations(new SyntaxAnnotation(MutationMarker, mutantId.ToString()));

        public static SyntaxNode RemoveMutant(SyntaxNode nodeToRemove)
        {
            var engine = nodeToRemove.GetAnnotatedNodes(Injector).FirstOrDefault()?.GetAnnotations(Injector).First().Data;
            if (!string.IsNullOrEmpty(engine))
            {
                return InstrumentEngines[engine].RemoveInstrumentation(nodeToRemove);
            }

            throw new InvalidOperationException($"Unable to remove any injection from this node: {nodeToRemove}");
        }


        public static ParenthesizedExpressionSyntax PlaceWithConditionalExpression(ExpressionSyntax original,
            ExpressionSyntax mutated, int mutantId) =>
            ConditionalEngine.PlaceWithConditionalExpression(GetBinaryExpression(mutantId), original, mutated)
                // Mark this node as a MutationConditional node. Store the MutantId in the annotation to retrace the mutant later
                .WithAdditionalAnnotations(new SyntaxAnnotation(MutationMarker, mutantId.ToString()));

        // us this method to annotate injected helper code. I.e. any injected code that is NOT a mutation but provides some infrastructure for the mutant to run
        public static T AnnotateHelper<T>(T node) where T:SyntaxNode => node.WithAdditionalAnnotations(new SyntaxAnnotation(MutationHelper));

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
