using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
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

    private static readonly IDictionary<string, IInstrumentCode> InstrumentEngines = new Dictionary<string, IInstrumentCode>();
    private static readonly HashSet<string> RequireRecursiveRemoval = new();

    private static readonly StaticInstrumentationEngine StaticEngine = new();
    private static readonly StaticInitializerMarkerEngine StaticInitializerEngine = new();
    private static readonly IfInstrumentationEngine IfEngine = new();
    private static readonly ConditionalInstrumentationEngine ConditionalEngine = new();
    private static readonly EndingReturnEngine EndingReturnEngine = new();
    private static readonly DefaultInitializationEngine DefaultInitializationEngine = new();

    private readonly CodeInjection _injection;
    private ExpressionSyntax _binaryExpression;
    private SyntaxNode _placeHolderNode;
    public static IEnumerable<string> MutationMarkers => new[] { MutationIdMarker, MutationTypeMarker, Injector };

    public MutantPlacer(CodeInjection injection) => _injection = injection;

    /// <summary>
    /// Register an instrumentation engine
    /// </summary>
    /// <param name="engine">engine to register</param>
    /// <param name="requireRecursive">true if inner injections should be removed first.</param>
    public static SyntaxAnnotation RegisterEngine(IInstrumentCode engine, bool requireRecursive = false)
    {
        if (InstrumentEngines.TryGetValue(engine.InstrumentEngineId, out var existing) && existing!.GetType() != engine.GetType())
        {
            throw new InvalidOperationException($"Cannot register {engine.GetType().Name} as name {engine.InstrumentEngineId} is already registered to {existing.GetType().Name}.");
        }
        InstrumentEngines[engine.InstrumentEngineId] = engine;
        if (requireRecursive)
        {
            RequireRecursiveRemoval.Add(engine.InstrumentEngineId);
        }
        return new SyntaxAnnotation(Injector, engine.InstrumentEngineId);
    }

    /// <summary>
    /// Add a return at the end of the syntax block, assuming it appears to be useful and returns the new block.
    /// </summary>
    /// <param name="block">block to complete with an ending return</param>
    /// <param name="propertyType">type to return. if null, ends the statement block with a non-typed 'return default'.</param>
    /// <returns><paramref name="block"/> with an extra return or not</returns>
    /// <remarks>The engine verifies if a return may be useful and does not inject it otherwise. For example, it does nothing if the block is empty, return type is void or if the block already ends with a return or a throw statement.</remarks>
    public static BlockSyntax AddEndingReturn(BlockSyntax block, TypeSyntax propertyType) =>
        EndingReturnEngine.InjectReturn(block, propertyType);

    /// <summary>
    /// Adds a static marker so that Stryker can identify mutations used in static context
    /// </summary>
    /// <param name="block">block to augment with the marker</param>
    /// <returns><paramref name="block"/> with the marker added via a using statement.</returns>
    public BlockSyntax PlaceStaticContextMarker(BlockSyntax block) =>
        StaticEngine.PlaceStaticContextMarker(block, _injection);

    /// <summary>
    /// Add a static marker so that Stryker can identify mutations used in static context
    /// </summary>
    /// <param name="expression">expression to augment with the marker</param>
    /// <returns><paramref name="expression"/> with the marker added via a using expression.</returns>
    public ExpressionSyntax PlaceStaticContextMarker(ExpressionSyntax expression) =>
        StaticInitializerEngine.PlaceValueMarker(expression, _injection);

    /// <summary>
    /// Add initialization for all out parameters
    /// </summary>
    /// <param name="block">block to augment with an initialization block</param>
    /// <returns><paramref name="block"/> with assignment statements in a block.</returns>
    /// <remarks>return <paramref name="block"/> if there is no 'out' parameter.</remarks>
    public static BlockSyntax InjectOutParametersInitialization(BlockSyntax block, IEnumerable<ParameterSyntax> parameters) =>
        DefaultInitializationEngine.InjectOutParametersInitialization(block, parameters);

    /// <summary>
    /// Add one or more mutations controlled via one or more if statements
    /// </summary>
    /// <param name="original">original statement (will be used to generate mutations)</param>
    /// <param name="mutants">list of mutations to inject</param>
    /// <returns>an if statement (or a chain of if statements) containing the mutant(s) and the original node.</returns>
    public StatementSyntax PlaceStatementControlledMutations(StatementSyntax original,
        IEnumerable<(Mutant mutant, StatementSyntax mutation)> mutants) =>
        mutants.Aggregate(original, (syntaxNode, mutationInfo) =>
            IfEngine.InjectIf(GetBinaryExpression(mutationInfo.mutant.Id), syntaxNode, mutationInfo.mutation)
                // Mark this node as a MutationIf node. Store the MutantId in the annotation to retrace the mutant later
                .WithAdditionalAnnotations(new SyntaxAnnotation(MutationIdMarker, mutationInfo.mutant.Id.ToString()))
                .WithAdditionalAnnotations(new SyntaxAnnotation(MutationTypeMarker, mutationInfo.mutant.Mutation.Type.ToString())));

    /// <summary>
    /// Add one or more mutations controlled via one or more ternary operators
    /// </summary>
    /// <param name="original">original expression (will be used to generate mutations)</param>
    /// <param name="mutants">list of mutations to inject</param>
    /// <returns>a ternary expression (or a chain of ternary expression) containing the mutant(s) and the original node.</returns>
    public ExpressionSyntax PlaceExpressionControlledMutations(ExpressionSyntax original,
        IEnumerable<(Mutant mutant, ExpressionSyntax mutation)> mutants) =>
        mutants.Aggregate(original, (current, mutationInfo) =>
            ConditionalEngine.PlaceWithConditionalExpression(GetBinaryExpression(mutationInfo.mutant.Id), current, mutationInfo.mutation)
                // Mark this node as a MutationConditional node. Store the MutantId in the annotation to retrace the mutant later
                .WithAdditionalAnnotations(new SyntaxAnnotation(MutationIdMarker, mutationInfo.mutant.Id.ToString()))
                .WithAdditionalAnnotations(new SyntaxAnnotation(MutationTypeMarker, mutationInfo.mutant.Mutation.Type.ToString())));

    /// <summary>
    /// Removes the mutant (or injected code) from the syntax node
    /// </summary>
    /// <param name="nodeToRemove"></param>
    /// <returns>the node without any injection</returns>
    /// <remarks>only remove injection for <paramref name="nodeToRemove"/> and keep any child one.</remarks>
    /// <exception cref="InvalidOperationException">if there is no trace of a code injection</exception>
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
        return annotations.Exists(a => RequireRecursiveRemoval.Contains(a.Data));
    }

    /// <summary>
    /// Gets mutant related annotations from a syntax node
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
            _placeHolderNode = _binaryExpression.DescendantNodes().First(n => n is IdentifierNameSyntax { Identifier.Text: "ID" });
        }

        return _binaryExpression.ReplaceNode(_placeHolderNode,
            SyntaxFactory.LiteralExpression(SyntaxKind.NumericLiteralExpression, SyntaxFactory.Literal(mutantId)));
    }

}
