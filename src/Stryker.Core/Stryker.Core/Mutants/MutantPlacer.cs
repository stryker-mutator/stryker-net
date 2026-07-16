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
public class MutantPlacer(CodeInjection injection)
{
    private const string MutationIdMarker = "MutationId";
    private const string MutationTypeMarker = "MutationType";
    private const string Injector = "Injector";

    private static readonly Dictionary<string, (IInstrumentCode engine, SyntaxAnnotation annotation)> InstrumentEngines = [];
    private static readonly HashSet<string> RequireRecursiveRemoval = [];

    private static readonly StaticInstrumentationEngine StaticEngine = new();
    private static readonly StaticInitializerMarkerEngine StaticInitializerEngine = new();
    private static readonly IfInstrumentationEngine IfEngine = new();
    private static readonly ConditionalInstrumentationEngine ConditionalEngine = new();
    private static readonly EndingReturnEngine EndingReturnEngine = new();
    private static readonly DefaultInitializationEngine DefaultInitializationEngine = new();

    private ExpressionSyntax _binaryExpression;
    private SyntaxNode _placeHolderNode;

    private static IEnumerable<string> MutationMarkers => [MutationIdMarker, MutationTypeMarker, Injector];

    /// <summary>
    /// Register an instrumentation engine
    /// </summary>
    /// <param name="engine">engine to register</param>
    /// <param name="requireRecursive">true if inner injections should be removed first.</param>
    public static SyntaxAnnotation RegisterEngine(IInstrumentCode engine, bool requireRecursive = false)
    {
        lock (InstrumentEngines)
        {
            if (InstrumentEngines.TryGetValue(engine.InstrumentEngineId, out var existing))
            {
                if (existing.engine!.GetType() != engine.GetType())
                {
                    throw new InvalidOperationException(
                        $"Cannot register {engine.GetType().Name} as name {engine.InstrumentEngineId} is already registered to {existing.engine.GetType().Name}.");
                }
                return existing.annotation;
            }

            var syntaxAnnotation = new SyntaxAnnotation(Injector, engine.InstrumentEngineId);
            InstrumentEngines[engine.InstrumentEngineId] = (engine, syntaxAnnotation);
            if (requireRecursive)
            {
                RequireRecursiveRemoval.Add(engine.InstrumentEngineId);
            }
            return syntaxAnnotation;
        }
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
        StaticEngine.PlaceStaticContextMarker(block, injection);

    /// <summary>
    /// Add a static marker so that Stryker can identify mutations used in static context
    /// </summary>
    /// <param name="expression">expression to augment with the marker</param>
    /// <returns><paramref name="expression"/> with the marker added via a using expression.</returns>
    public ExpressionSyntax PlaceStaticContextMarker(ExpressionSyntax expression) =>
        StaticInitializerEngine.PlaceValueMarker(expression, injection);

    /// <summary>
    /// Add initialization for all out parameters
    /// </summary>
    /// <param name="block">block to augment with an initialization block</param>
    /// <param name="parameters"></param>
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
            var id = annotatedNode.GetAnnotations(Injector).First().Data;
            if (!string.IsNullOrEmpty(id))
            {
                var restoredNode = InstrumentEngines[id].engine.RemoveInstrumentation(annotatedNode);
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
        return annotations.Any(a => a.Kind == Injector) ?
            annotations.Exists(a => RequireRecursiveRemoval.Contains(a.Data))
            : throw new InvalidOperationException("No mutation in this node!");
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
            switch (annotation.Kind)
            {
                case MutationIdMarker:
                    id = int.Parse(annotation.Data!);
                    break;
                case Injector:
                    engine = annotation.Data;
                    break;
                case MutationTypeMarker:
                    type = annotation.Data;
                    break;
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
    private ExpressionSyntax GetBinaryExpression(int mutantId)
    {
        if (_binaryExpression == null)
        {
            _binaryExpression = SyntaxFactory.ParseExpression(injection.SelectorExpression);
            _placeHolderNode = _binaryExpression.DescendantNodes().First(n => n is IdentifierNameSyntax { Identifier.Text: "ID" });
        }

        return _binaryExpression.ReplaceNode(_placeHolderNode,
            SyntaxFactory.LiteralExpression(SyntaxKind.NumericLiteralExpression, SyntaxFactory.Literal(mutantId)));
    }

    public static List<(SyntaxNode node, IInstrumentCode engine)> GetMutationsEngines(SyntaxNode node) => GetAllMutations(node).Select(entry => (entry.node, InstrumentEngines[entry.mutantInfo.Engine].engine)).ToList();

    public static List<(SyntaxNode node, MutantInfo mutantInfo)> GetAllMutations(SyntaxNode node)
    {
        var mutations = node.GetAnnotatedNodes(MutationIdMarker);
        return (from syntaxNode in mutations
            let mutationInfo = FindAnnotations(syntaxNode)
            select (syntaxNode, mutationInfo)).ToList();
    }

    /// <summary>
    /// Returns the syntax tree with every trace of Stryker instrumentation removed, restoring its
    /// original, un-instrumented form. This reverses not only the mutations themselves but also the
    /// structural rewrites Stryker applies to make room for them (for example converting an
    /// expression-bodied member to a block body), so the result is the genuine original source.
    /// Used to compile a mutation-free baseline so a mutation-induced compile failure can be told
    /// apart from a failure that exists independently of any mutation.
    /// </summary>
    internal static SyntaxTree RemoveAllMutations(SyntaxTree tree)
    {
        var root = tree.GetRoot();
        // Every instrumented node (a mutation or a structural rewrite) carries an Injector
        // annotation naming the engine that can revert it. Revert the innermost instrumentation
        // first: an outer rewrite (e.g. block-body -> expression-body) can only be undone once the
        // mutations nested inside it are already gone.
        while (true)
        {
            var innermost = root.GetAnnotatedNodes(Injector)
                .FirstOrDefault(node => !node.DescendantNodes().Any(descendant => descendant.HasAnnotations(Injector)));
            if (innermost == null)
            {
                break;
            }

            root = root.ReplaceNode(innermost, RemoveMutant(innermost));
        }

        // Rebuild from the original tree so its file path, encoding and parse options are preserved
        // exactly - the baseline must be the genuine original project, not a re-parsed approximation.
        return tree.WithRootAndOptions(root, tree.Options);
    }

    /// <summary>
    /// True if <paramref name="compilation"/> has an error-level diagnostic in <paramref name="tree"/>.
    /// The check is correlated to that specific tree by identity (via its semantic model), so an error
    /// in another tree - even one sharing an empty or duplicate file path - does not count.
    /// </summary>
    internal static bool HasCompileError(Compilation compilation, SyntaxTree tree) =>
        compilation.GetSemanticModel(tree).GetDiagnostics().Any(diagnostic => diagnostic.Severity == DiagnosticSeverity.Error);
}
