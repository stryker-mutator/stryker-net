using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Stryker.Core.Helpers;
using Stryker.Core.InjectedHelpers;
using Stryker.Core.Instrumentation;

namespace Stryker.Core.Mutants
{
    /// <summary>
    /// Implements multiple (reversible) patterns for injecting code in the mutated assembly?
    /// Each pattern is implemented in a dedicated class.
    /// </summary>
    public class MutantPlacer
    {
        private const string MutationIdMarker = "MutationId";
        private const string MutationTypeMarker = "MutationType";
        public static readonly string Injector = "Injector";

        private static readonly StaticInstrumentationEngine staticEngine = new();
        private static readonly StaticInitializerMarkerEngine staticInitializerEngine = new();
        private static readonly IfInstrumentationEngine IfEngine = new();
        private static readonly ConditionalInstrumentationEngine conditionalEngine = new();
        private static readonly ExpressionMethodToBodyEngine expressionMethodEngine = new();
        private static readonly LocalFunctionExpressionToBodyEngine localFunctionExpressionToBodyEngine = new();
        private static readonly AccessorExpressionToBodyEngine accessorExpressionToBodyEngine = new();
        private static readonly PropertyExpressionToBodyEngine propertyExpressionToBodyEngine = new();
        private static readonly AnonymousFunctionExpressionToBodyEngine anonymousFunctionExpressionToBodyEngine = new();
        private static readonly EndingReturnEngine endingReturnEngine = new();
        private static readonly DefaultInitializationEngine defaultInitializationEngine = new();
        private static readonly IDictionary<string, IInstrumentCode> InstrumentEngines = new Dictionary<string, IInstrumentCode>();

        private readonly CodeInjection _injection;
        private ExpressionSyntax _binaryExpression;
        private SyntaxNode _placeHolderNode;
        public static IEnumerable<string> MutationMarkers => new[] { MutationIdMarker, MutationTypeMarker, Injector };

        static MutantPlacer()
        {
            RegisterEngine(staticEngine);
            RegisterEngine(IfEngine);
            RegisterEngine(conditionalEngine);
            RegisterEngine(expressionMethodEngine);
            RegisterEngine(accessorExpressionToBodyEngine);
            RegisterEngine(propertyExpressionToBodyEngine);
            RegisterEngine(anonymousFunctionExpressionToBodyEngine);
            RegisterEngine(endingReturnEngine);
            RegisterEngine(defaultInitializationEngine);
            RegisterEngine(staticInitializerEngine);
            RegisterEngine(localFunctionExpressionToBodyEngine);
        }

        public MutantPlacer(CodeInjection injection) => _injection = injection;

        /// <summary>
        ///  register an instrumentation engine
        /// </summary>
        /// <param name="engine"></param>
        public static void RegisterEngine(IInstrumentCode engine) => InstrumentEngines.Add(engine.InstrumentEngineID, engine);

        public static T ConvertExpressionToBody<T>(T method) where T : BaseMethodDeclarationSyntax =>
            expressionMethodEngine.ConvertToBody(method);

        public static AccessorDeclarationSyntax ConvertExpressionToBody(AccessorDeclarationSyntax method) =>
            accessorExpressionToBodyEngine.ConvertExpressionToBody(method);

        public static LocalFunctionStatementSyntax ConvertExpressionToBody(LocalFunctionStatementSyntax method) =>
            localFunctionExpressionToBodyEngine.ConvertToBody(method);

        public static AnonymousFunctionExpressionSyntax ConvertExpressionToBody(AnonymousFunctionExpressionSyntax property) =>
            anonymousFunctionExpressionToBodyEngine.ConvertToBody(property);

        public static PropertyDeclarationSyntax ConvertPropertyExpressionToBodyAccessor(PropertyDeclarationSyntax property) =>
            propertyExpressionToBodyEngine.ConvertExpressionToBody(property);

        public static BaseMethodDeclarationSyntax AddEndingReturn(BaseMethodDeclarationSyntax method) =>
            method.WithBody(endingReturnEngine.InjectReturn(method.Body, method.ReturnType(), method.Modifiers));
        public static AccessorDeclarationSyntax AddEndingReturn(AccessorDeclarationSyntax method, TypeSyntax propertyType) =>
            method.WithBody(endingReturnEngine.InjectReturn(method.Body, propertyType, method.Modifiers));
        public static LocalFunctionStatementSyntax AddEndingReturn(LocalFunctionStatementSyntax function) =>
            function.WithBody(endingReturnEngine.InjectReturn(function.Body, function.ReturnType, function.Modifiers));
        public static AnonymousFunctionExpressionSyntax AddEndingReturn(AnonymousFunctionExpressionSyntax function) =>
            function.WithBlock(endingReturnEngine.InjectReturn(function.Block));

        public BlockSyntax PlaceStaticContextMarker(BlockSyntax block) =>
            staticEngine.PlaceStaticContextMarker(block, _injection);

        public ExpressionSyntax PlaceStaticContextMarker(ExpressionSyntax expression) =>
            staticInitializerEngine.PlaceValueMarker(expression, _injection);

        public static BlockSyntax AddDefaultInitializers(BlockSyntax block, IEnumerable<ParameterSyntax> parameters) =>
            defaultInitializationEngine.AddDefaultInitializers(block, parameters);

        public StatementSyntax PlaceStatementControlledMutations(StatementSyntax original,
            IEnumerable<(Mutant mutant, StatementSyntax mutation)> mutants) =>
            mutants.Aggregate(original, (syntaxNode, mutationInfo) =>
                IfEngine.InjectIf(GetBinaryExpression(mutationInfo.mutant.Id), syntaxNode, mutationInfo.mutation)
                    // Mark this node as a MutationIf node. Store the MutantId in the annotation to retrace the mutant later
                    .WithAdditionalAnnotations(new SyntaxAnnotation(MutationIdMarker, mutationInfo.mutant.Id.ToString()))
                    .WithAdditionalAnnotations(new SyntaxAnnotation(MutationTypeMarker, mutationInfo.mutant.Mutation.Type.ToString())));

        public ExpressionSyntax PlaceExpressionControlledMutations(ExpressionSyntax original,
            IEnumerable<(Mutant mutant, ExpressionSyntax mutation)> mutants) =>
            mutants.Aggregate(original, (current, mutationInfo) =>
                conditionalEngine.PlaceWithConditionalExpression(GetBinaryExpression(mutationInfo.mutant.Id), current, mutationInfo.mutation)
                    // Mark this node as a MutationConditional node. Store the MutantId in the annotation to retrace the mutant later
                    .WithAdditionalAnnotations(new SyntaxAnnotation(MutationIdMarker, mutationInfo.mutant.Id.ToString()))
                    .WithAdditionalAnnotations(new SyntaxAnnotation(MutationTypeMarker, mutationInfo.mutant.Mutation.Type.ToString())));

        public static SyntaxNode RemoveMutant(SyntaxNode nodeToRemove)
        {
            var annotatedNode = nodeToRemove.GetAnnotatedNodes(Injector).FirstOrDefault();
            if (annotatedNode != null)
            {
                var engine = annotatedNode.GetAnnotations(Injector).First().Data;
                if (!string.IsNullOrEmpty(engine))
                {
                    var restoredNode = InstrumentEngines[engine].RemoveInstrumentation(annotatedNode);
                    return annotatedNode == nodeToRemove ? restoredNode : nodeToRemove.ReplaceNode(annotatedNode, restoredNode);
                }
            }
            throw new InvalidOperationException($"Unable to find an engine to remove injection from this node: '{nodeToRemove}'");
        }

        public static MutantInfo FindAnnotations(SyntaxNode node)
        {
            var id = -1;
            string engine = null;
            string type = null;
            var annotations = node.GetAnnotations(MutationMarkers);
            foreach (var annotation in annotations)
            {
                if (annotation.Kind == MutationIdMarker)
                {
                    id = int.Parse(annotation.Data!);
                }
                else if (annotation.Kind == Injector)
                {
                    engine = annotation.Data;
                }
                else if (annotation.Kind == MutationTypeMarker)
                {
                    type = annotation.Data;
                }
            }

            return new MutantInfo
            {
                Id = id,
                Engine = engine,
                Type = type,
                Node = node
            };
        }

        /// <summary>
        /// Builds a syntax for the expression to check if a mutation is active
        /// Example for mutationId 1: Stryker.Helper.ActiveMutation == 1
        /// </summary>
        /// <param name="mutantId"></param>
        /// <returns></returns>
        private  ExpressionSyntax GetBinaryExpression(int mutantId)
        {
            if (_binaryExpression == null)
            {
                _binaryExpression = SyntaxFactory.ParseExpression(_injection.SelectorExpression);
                _placeHolderNode = _binaryExpression.DescendantNodes().First(n => n is IdentifierNameSyntax
                {
                    Identifier.Text: "ID"
                });
            }

            return _binaryExpression.ReplaceNode(_placeHolderNode,
                SyntaxFactory.LiteralExpression(SyntaxKind.NumericLiteralExpression, SyntaxFactory.Literal(mutantId)));
        }

    }
}
