using System;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Stryker.Core.InjectedHelpers;
using System.Collections.Generic;
using System.Linq;
using Stryker.Core.Helpers;
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
        public readonly static string Injector = "Injector";

        private static readonly StaticInstrumentationEngine staticEngine = new ();
        private static readonly BlockInstrumentationEngine blockEngine = new ();
        private static readonly StaticInitializerMarkerEngine staticInitializerEngine = new ();
        private static readonly IfInstrumentationEngine IfEngine = new ();
        private static readonly ConditionalInstrumentationEngine conditionalEngine = new ();
        private static readonly ExpressionMethodToBodyEngine expressionMethodEngine = new();
        private static readonly LocalFunctionExpressionToBodyEngine localFunctionExpressionToBodyEngine = new();
        private static readonly AccessorExpressionToBodyEngine accessorExpressionToBodyEngine = new();
        private static readonly PropertyExpressionToBodyEngine propertyExpressionToBodyEngine = new();
        private static readonly EndingReturnEngine endingReturnEngine = new();
        private static readonly DefaultInitializationEngine defaultInitializationEngine = new();
        private static ExpressionSyntax _binaryExpression;
        private static SyntaxNode _placeHolderNode;
        
        private static readonly IDictionary<string, IInstrumentCode> InstrumentEngines = new Dictionary<string, IInstrumentCode>();

        public static IEnumerable<string> MutationMarkers => new[] { MutationMarker, Injector};

        static MutantPlacer()
        {
            RegisterEngine(staticEngine);
            RegisterEngine(blockEngine);
            RegisterEngine(IfEngine);
            RegisterEngine(conditionalEngine);
            RegisterEngine(expressionMethodEngine);
            RegisterEngine(accessorExpressionToBodyEngine);
            RegisterEngine(propertyExpressionToBodyEngine);
            RegisterEngine(endingReturnEngine);
            RegisterEngine(defaultInitializationEngine);
            RegisterEngine(staticInitializerEngine);
            RegisterEngine(localFunctionExpressionToBodyEngine);
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

        public static LocalFunctionStatementSyntax ConvertExpressionToBody(LocalFunctionStatementSyntax method) =>
            localFunctionExpressionToBodyEngine.ConvertToBody(method);

        public static PropertyDeclarationSyntax ConvertPropertyExpressionToBodyAccessor(PropertyDeclarationSyntax property) =>
            propertyExpressionToBodyEngine.ConvertExpressionToBody(property);

        public static BaseMethodDeclarationSyntax AddEndingReturn(BaseMethodDeclarationSyntax method) => method.WithBody(endingReturnEngine.InjectReturn(method.Body, method.ReturnType(), method.Modifiers));
        public static LocalFunctionStatementSyntax AddEndingReturn(LocalFunctionStatementSyntax function) => function.WithBody(endingReturnEngine.InjectReturn(function.Body, function.ReturnType, function.Modifiers));

        public static BlockSyntax PlaceStaticContextMarker(BlockSyntax block) => 
            staticEngine.PlaceStaticContextMarker(block);

        public static ExpressionSyntax PlaceStaticContextMarker(ExpressionSyntax expression) =>
            staticInitializerEngine.PlaceValueMarker(expression);

        public static BlockSyntax AddDefaultInitializers(BlockSyntax block, IEnumerable<ParameterSyntax> parameters) =>
            defaultInitializationEngine.AddDefaultInitializers(block, parameters);

        public static StatementSyntax PlaceStatementControlledMutations(StatementSyntax original,
            IEnumerable<(int mutantId, StatementSyntax mutated)> mutations) =>
            mutations.Aggregate(original, (syntaxNode, mutation) => 
                IfEngine.InjectIf(GetBinaryExpression(mutation.mutantId), syntaxNode, mutation.mutated)
                    // Mark this node as a MutationIf node. Store the MutantId in the annotation to retrace the mutant later
                    .WithAdditionalAnnotations(new SyntaxAnnotation(MutationMarker, mutation.mutantId.ToString())));

        public static ExpressionSyntax PlaceExpressionControlledMutations( 
            ExpressionSyntax modified, 
            IEnumerable<(int id, ExpressionSyntax mutation)> mutations) =>
            mutations.Aggregate(modified, (current, mutation) => 
                conditionalEngine.PlaceWithConditionalExpression(GetBinaryExpression(mutation.id), current, mutation.mutation)
                    // Mark this node as a MutationConditional node. Store the MutantId in the annotation to retrace the mutant later
                    .WithAdditionalAnnotations(new SyntaxAnnotation(MutationMarker, mutation.id.ToString())));

        public static SyntaxNode RemoveMutant(SyntaxNode nodeToRemove)
        {
            var engine = nodeToRemove.GetAnnotatedNodes(Injector).FirstOrDefault()?.GetAnnotations(Injector).First().Data;
            if (!string.IsNullOrEmpty(engine))
            {
                return InstrumentEngines[engine].RemoveInstrumentation(nodeToRemove);
            }

            throw new InvalidOperationException($"Unable to find an engine to remove injection from this node: '{nodeToRemove}'");
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
