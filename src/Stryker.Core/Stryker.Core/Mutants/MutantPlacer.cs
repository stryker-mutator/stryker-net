using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Stryker.Core.Helpers;
using Stryker.Core.InjectedHelpers;
using Stryker.Core.Instrumentation;

namespace Stryker.Core.Mutants;

/// <summary>
/// Implements multiple (reversible) patterns for injecting code in the mutated assembly
/// Each pattern is implemented in a dedicated class.
/// </summary>
public class MutantPlacer
{
    private const string MutationIdMarker = "MutationId";
    private const string MutationTypeMarker = "MutationType";
    public static readonly string Injector = "Injector";

    private static readonly StaticInstrumentationEngine StaticEngine = new();
    private static readonly StaticInitializerMarkerEngine StaticInitializerEngine = new();
    private static readonly IfInstrumentationEngine IfEngine = new();
    private static readonly ConditionalInstrumentationEngine ConditionalEngine = new();
    private static readonly ExpressionMethodToBodyEngine ExpressionMethodEngine = new();
    private static readonly LocalFunctionExpressionToBodyEngine LocalFunctionExpressionToBodyEngine = new();
    private static readonly AccessorExpressionToBodyEngine AccessorExpressionToBodyEngine = new();
    private static readonly PropertyExpressionToBodyEngine PropertyExpressionToBodyEngine = new();
    private static readonly AnonymousFunctionExpressionToBodyEngine AnonymousFunctionExpressionToBodyEngine = new();
    private static readonly LambdaExpressionToBodyEngine LambdaExpressionToBodyEngine = new();
    private static readonly EndingReturnEngine EndingReturnEngine = new();
    private static readonly DefaultInitializationEngine DefaultInitializationEngine = new();

    private static readonly IDictionary<string, IInstrumentCode> InstrumentEngines = new Dictionary<string, IInstrumentCode>();

    private readonly CodeInjection _injection;
    private ExpressionSyntax _binaryExpression;
    private SyntaxNode _placeHolderNode;
    public static IEnumerable<string> MutationMarkers => new[] { MutationIdMarker, MutationTypeMarker, Injector };

    static MutantPlacer()
    {
        RegisterEngine(StaticEngine);
        RegisterEngine(IfEngine);
        RegisterEngine(ConditionalEngine);
        RegisterEngine(ExpressionMethodEngine);
        RegisterEngine(AccessorExpressionToBodyEngine);
        RegisterEngine(PropertyExpressionToBodyEngine);
        RegisterEngine(AnonymousFunctionExpressionToBodyEngine);
        RegisterEngine(EndingReturnEngine);
        RegisterEngine(DefaultInitializationEngine);
        RegisterEngine(StaticInitializerEngine);
        RegisterEngine(LocalFunctionExpressionToBodyEngine);
        RegisterEngine(LambdaExpressionToBodyEngine);
    }

    public MutantPlacer(CodeInjection injection) => _injection = injection;

    /// <summary>
    ///  register an instrumentation engine
    /// </summary>
    /// <param name="engine"></param>
    public static void RegisterEngine(IInstrumentCode engine) => InstrumentEngines.Add(engine.InstrumentEngineId, engine);

    public static T ConvertExpressionToBody<T>(T method) where T : BaseMethodDeclarationSyntax =>
        ExpressionMethodEngine.ConvertToBody(method);

    public static AccessorDeclarationSyntax ConvertExpressionToBody(AccessorDeclarationSyntax method) =>
        AccessorExpressionToBodyEngine.ConvertExpressionToBody(method);

    public static LocalFunctionStatementSyntax ConvertExpressionToBody(LocalFunctionStatementSyntax method) =>
        LocalFunctionExpressionToBodyEngine.ConvertToBody(method);

    public static LambdaExpressionSyntax ConvertExpressionToBody(LambdaExpressionSyntax lambdaExpression) =>
        LambdaExpressionToBodyEngine.ConvertToBody(lambdaExpression);

    public static AnonymousFunctionExpressionSyntax ConvertExpressionToBody(AnonymousFunctionExpressionSyntax anonymousFunction) =>
        AnonymousFunctionExpressionToBodyEngine.ConvertToBody(anonymousFunction);

    public static PropertyDeclarationSyntax ConvertPropertyExpressionToBodyAccessor(PropertyDeclarationSyntax property) =>
        PropertyExpressionToBodyEngine.ConvertExpressionToBody(property);

    public static BaseMethodDeclarationSyntax AddEndingReturn(BaseMethodDeclarationSyntax method) =>
        method.WithBody(EndingReturnEngine.InjectReturn(method.Body, method.ReturnType(), method.Modifiers));
    public static AccessorDeclarationSyntax AddEndingReturn(AccessorDeclarationSyntax method, TypeSyntax propertyType) =>
        method.WithBody(EndingReturnEngine.InjectReturn(method.Body, propertyType, method.Modifiers));
    public static LocalFunctionStatementSyntax AddEndingReturn(LocalFunctionStatementSyntax function) =>
        function.WithBody(EndingReturnEngine.InjectReturn(function.Body, function.ReturnType, function.Modifiers));
    public static AnonymousFunctionExpressionSyntax AddEndingReturn(AnonymousFunctionExpressionSyntax function) =>
        function.WithBlock(EndingReturnEngine.InjectReturn(function.Block));
    public static LambdaExpressionSyntax AddEndingReturn(LambdaExpressionSyntax lambda) =>
        lambda.WithBlock(EndingReturnEngine.InjectReturn(lambda.Block));

    public BlockSyntax PlaceStaticContextMarker(BlockSyntax block) =>
        StaticEngine.PlaceStaticContextMarker(block, _injection);

    public ExpressionSyntax PlaceStaticContextMarker(ExpressionSyntax expression) =>
        StaticInitializerEngine.PlaceValueMarker(expression, _injection);

    public static BlockSyntax AddDefaultInitializers(BlockSyntax block, IEnumerable<ParameterSyntax> parameters) =>
        DefaultInitializationEngine.AddDefaultInitializers(block, parameters);

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
            ConditionalEngine.PlaceWithConditionalExpression(GetBinaryExpression(mutationInfo.mutant.Id), current, mutationInfo.mutation)
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

    /// <summary>
    /// Returns true if the node contains a mutation requiring all child mutations to be removed when it has to be removed
    /// </summary>
    /// <param name="node"></param>
    /// <returns></returns>
    public static bool RequiresRemovingChildMutations(SyntaxNode node)
    {
        var annotations = node.GetAnnotations(MutationMarkers).ToList();
        if (annotations.TrueForAll(a => a.Kind != Injector))
        {
            throw new InvalidOperationException("No mutation in this node!");
        }
        return annotations.Exists(a => a.Data == ExpressionMethodEngine.InstrumentEngineId);
    }

    /// <summary>
    /// Gets mutant related annotations from a syntaxnode
    /// </summary>
    /// <param name="node"></param>
    /// <returns></returns>
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
