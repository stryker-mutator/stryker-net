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
        private const string Injector = "Injector";

        private static readonly StaticInstrumentationEngine StaticEngine;
        private static readonly IfInstrumentationEngine IfEngine;
        private static readonly ConditionalInstrumentationEngine ConditionalEngine;
        private static readonly ExpressionMethodToBodyEngine expressionMethodEngine;
        private static readonly AccessorExpressionToBodyEngine accessorExpressionToBodyEngine;
        private static readonly PropertyExpressionToBodyEngine propertyExpressionToBodyEngine;
        private static readonly EndingReturnEngine endingReturnEngine;
        private static readonly DefaultInitializationEngine defaultInitializationEngine;
        private static ExpressionSyntax _binaryExpression;
        private static SyntaxNode _placeHolderNode;
        
        private static readonly IDictionary<string, IInstrumentCode> InstrumentEngines = new Dictionary<string, IInstrumentCode>();

        public static IEnumerable<string> MutationMarkers => new[] { MutationMarker, Injector};

        static MutantPlacer()
        {
            StaticEngine = new StaticInstrumentationEngine(Injector);
            RegisterEngine(StaticEngine);
            IfEngine = new IfInstrumentationEngine(Injector);
            RegisterEngine(IfEngine);
            ConditionalEngine = new ConditionalInstrumentationEngine(Injector);
            RegisterEngine(ConditionalEngine);
            expressionMethodEngine = new ExpressionMethodToBodyEngine(Injector);
            RegisterEngine(expressionMethodEngine);
            accessorExpressionToBodyEngine = new AccessorExpressionToBodyEngine(Injector);
            RegisterEngine(accessorExpressionToBodyEngine);
            propertyExpressionToBodyEngine = new PropertyExpressionToBodyEngine(Injector);
            RegisterEngine(propertyExpressionToBodyEngine);
            endingReturnEngine = new EndingReturnEngine(Injector);
            RegisterEngine(endingReturnEngine);
            defaultInitializationEngine = new DefaultInitializationEngine(Injector);
            RegisterEngine(defaultInitializationEngine);
        }

        /// <summary>
        ///  register an instrumentation engine
        /// </summary>
        /// <param name="engine"></param>
        public static void RegisterEngine(IInstrumentCode engine) => InstrumentEngines.Add(engine.InstrumentEngineID, engine);

        public static T ConvertExpressionToBody<T>(T method) where T: BaseMethodDeclarationSyntax =>
            expressionMethodEngine.ConvertToBody(method);

        public static AccessorDeclarationSyntax ConvertExpressionToBody(AccessorDeclarationSyntax method) =>
            accessorExpressionToBodyEngine.ConvertExpressionToBody(method);

        public static PropertyDeclarationSyntax ConvertPropertyExpressionToBodyAccessor(PropertyDeclarationSyntax property) =>
            propertyExpressionToBodyEngine.ConvertExpressionToBody(property);

        public static BaseMethodDeclarationSyntax AddEndingReturn(BaseMethodDeclarationSyntax node) => endingReturnEngine.InjectReturn(node);

        public static BlockSyntax PlaceStaticContextMarker(BlockSyntax block) => 
            StaticEngine.PlaceStaticContextMarker(block);

        public static BaseMethodDeclarationSyntax AddDefaultInitialization(BaseMethodDeclarationSyntax node, SyntaxToken outParameterParameterName, TypeSyntax outParameterParameterType)
        {
            return defaultInitializationEngine.AddDefaultInitializer(node, outParameterParameterName,
                outParameterParameterType);
        }

        public static StatementSyntax PlaceStatementControlledMutations(StatementSyntax original,
            IEnumerable<(int mutantId, StatementSyntax mutated)> mutations)
        {
            return mutations.Aggregate(original, (syntaxNode, mutation) => 
                IfEngine.InjectIf(GetBinaryExpression(mutation.mutantId), syntaxNode, mutation.mutated)
                // Mark this node as a MutationIf node. Store the MutantId in the annotation to retrace the mutant later
                    .WithAdditionalAnnotations(new SyntaxAnnotation(MutationMarker, mutation.mutantId.ToString())));
        }

        public static ExpressionSyntax PlaceExpressionControlledMutations( 
            ExpressionSyntax modified, 
            IEnumerable<(int id, ExpressionSyntax mutation)> mutations)
        {
            return mutations.Aggregate(modified, (current, mutation) => 
                ConditionalEngine.PlaceWithConditionalExpression(GetBinaryExpression(mutation.id), current, mutation.mutation)
                    // Mark this node as a MutationConditional node. Store the MutantId in the annotation to retrace the mutant later
                    .WithAdditionalAnnotations(new SyntaxAnnotation(MutationMarker, mutation.id.ToString())));
        }
        
        public static SyntaxNode RemoveMutant(SyntaxNode nodeToRemove)
        {
            var engine = nodeToRemove.GetAnnotatedNodes(Injector).FirstOrDefault()?.GetAnnotations(Injector).First().Data;
            if (!string.IsNullOrEmpty(engine))
            {
                return InstrumentEngines[engine].RemoveInstrumentation(nodeToRemove);
            }

            throw new InvalidOperationException($"Unable to find an engine to remove injection from this node: '{nodeToRemove}' ");
        }

        public static (string engine, int id) FindEngine(SyntaxNode node)
        {
            string engine = null;
            var id = -1;
            var first = node.GetAnnotations(MutantPlacer.MutationMarkers);
            foreach (var annotation in first)
            {
                if (annotation.Kind == MutationMarker)
                {
                    id = int.Parse(annotation.Data);
                }
                else if (annotation.Kind == Injector)
                {
                    engine = annotation.Data;
                }
            }

            return (engine, id);
        }

        /// <summary>
        /// Builds a syntax for the expression to check if a mutation is active
        /// Example for mutationId 1: Stryker.Helper.ActiveMutation == 1
        /// </summary>
        /// <param name="mutantId"></param>
        /// <returns></returns>
        private static ExpressionSyntax GetBinaryExpression(int mutantId)
        {
            if (_binaryExpression == null)
            {
                _binaryExpression = SyntaxFactory.ParseExpression(CodeInjection.SelectorExpression);
                _placeHolderNode = _binaryExpression.DescendantNodes().First(n => n is IdentifierNameSyntax identifier && identifier.Identifier.Text == "ID");
            }

            return _binaryExpression.ReplaceNode(_placeHolderNode,
                SyntaxFactory.LiteralExpression(SyntaxKind.NumericLiteralExpression, SyntaxFactory.Literal(mutantId)));
        }

    }
}
