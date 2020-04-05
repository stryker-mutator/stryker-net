using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.Extensions.Logging;
using Stryker.Core.Logging;
using Stryker.Core.Mutators;
using Stryker.Core.Options;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace Stryker.Core.Mutants
{
    public interface IMutantOrchestrator
    {
        SyntaxNode Mutate(SyntaxNode rootNode);
        /// <summary>
        /// Gets the stored mutants and resets the mutant list to an empty collection
        /// </summary>
        /// <returns>Mutants</returns>
        IReadOnlyCollection<Mutant> GetLatestMutantBatch();
    }

    /// <summary>
    /// Mutates abstract syntax trees using mutators and places all mutations inside the abstract syntax tree.
    /// Orchestrator: to arrange or manipulate, especially by means of clever or thorough planning or maneuvering.
    /// </summary>
    public class MutantOrchestrator : IMutantOrchestrator
    {
        private readonly StrykerOptions _options;
        private ICollection<Mutant> Mutants { get; set; }
        private int MutantCount { get; set; }
        private IEnumerable<IMutator> Mutators { get; }
        private ILogger Logger { get; }

        private bool MustInjectCoverageLogic =>
            _options != null && _options.Optimizations.HasFlag(OptimizationFlags.CoverageBasedTest) &&
            !_options.Optimizations.HasFlag(OptimizationFlags.CaptureCoveragePerTest);

        /// <param name="mutators">The mutators that should be active during the mutation process</param>
        public MutantOrchestrator(IEnumerable<IMutator> mutators = null, StrykerOptions options = null)
        {
            _options = options;
            Mutators = mutators ?? new List<IMutator>()
                {
                    // the default list of mutators
                    new BinaryExpressionMutator(),
                    new BooleanMutator(),
                    new AssignmentExpressionMutator(),
                    new PrefixUnaryMutator(),
                    new PostfixUnaryMutator(),
                    new CheckedMutator(),
                    new LinqMutator(),
                    new StringMutator(),
                    new StringEmptyMutator(),
                    new InterpolatedStringMutator(),
                    new NegateConditionMutator(),
                };
            Mutants = new Collection<Mutant>();
            Logger = ApplicationLogging.LoggerFactory.CreateLogger<MutantOrchestrator>();
        }

        /// <summary>
        /// Gets the stored mutants and resets the mutant list to an empty collection
        /// </summary>
        /// <returns>Mutants</returns>
        public IReadOnlyCollection<Mutant> GetLatestMutantBatch()
        {
            var tempMutants = Mutants;
            Mutants = new Collection<Mutant>();
            return (IReadOnlyCollection<Mutant>)tempMutants;
        }

        /// <summary>
        /// Recursively mutates a single SyntaxNode
        /// </summary>
        /// <param name="currentNode">The current root node</param>
        /// <returns>Mutated node</returns>
        public SyntaxNode Mutate(SyntaxNode currentNode)
        {
            return Mutate(currentNode, new MutationContext());
        }

        private SyntaxNode Mutate(SyntaxNode currentNode, MutationContext context)
        {
            // don't mutate immutable nodes
            if (!SyntaxHelper.CanBeMutated(currentNode))
            {
                return currentNode;
            }

            // identify static related structure
            switch (currentNode)
            {
                // static fields
                case FieldDeclarationSyntax fieldDeclaration when fieldDeclaration.Modifiers.Any(x => x.Kind() == SyntaxKind.StaticKeyword):
                    context = new MutationContext { InStaticValue = true };
                    break;
                // static constructors
                case ConstructorDeclarationSyntax constructorDeclaration when constructorDeclaration.Modifiers.Any(x => x.Kind() == SyntaxKind.StaticKeyword):
                    context = new MutationContext { InStaticValue = true };
                    if (MustInjectCoverageLogic)
                    {
                        return MutateStaticConstructor(constructorDeclaration, context);
                    }
                    break;
                // static properties
                case PropertyDeclarationSyntax propertyDeclaration when propertyDeclaration.Modifiers.Any(x => x.Kind() == SyntaxKind.StaticKeyword) && propertyDeclaration.AccessorList != null:
                    context = new MutationContext { InStaticValue = true };
                    if (MustInjectCoverageLogic)
                    {
                        return MutateStaticAccessor(propertyDeclaration, context);
                    }
                    break;
            }

            switch (currentNode)
            {
                // apply statement specific strategies (where applicable)
                case ExpressionStatementSyntax tentativeAssignment when tentativeAssignment.Expression is AssignmentExpressionSyntax assign:
                    return MutateAssignment(tentativeAssignment, assign, context);
                case ExpressionStatementSyntax tentativeAssignment when tentativeAssignment.Expression is PostfixUnaryExpressionSyntax || tentativeAssignment.Expression is PrefixUnaryExpressionSyntax:
                    return MutateUnaryStatement(tentativeAssignment, context);
                case ExpressionStatementSyntax expressionStatement:
                    // we must skip the expression statement part
                    return currentNode.ReplaceNode(expressionStatement.Expression, Mutate(expressionStatement.Expression, context));
                case IfStatementSyntax ifStatement:
                    return MutateIfStatement(ifStatement, context);
                case ForStatementSyntax forStatement:
                    return MutateForStatement(forStatement, context);
            }

            var mutatedNode = MutateExpression(currentNode, context);
            return AddReturnDefault(mutatedNode);
        }

        private SyntaxNode MutateStaticConstructor(ConstructorDeclarationSyntax constructorDeclaration, MutationContext context)
        {
            var trackedConstructor = constructorDeclaration.TrackNodes((SyntaxNode) constructorDeclaration.Body ?? constructorDeclaration.ExpressionBody);
            if (constructorDeclaration.ExpressionBody != null)
            {
                var bodyBlock = SyntaxFactory.Block(SyntaxFactory.ExpressionStatement(constructorDeclaration.ExpressionBody.Expression));
                var markedBlock = MutantPlacer.PlaceStaticContextMarker((BlockSyntax) Mutate(bodyBlock, context));
                trackedConstructor = trackedConstructor.Update(
                    trackedConstructor.AttributeLists,
                    trackedConstructor.Modifiers,
                    trackedConstructor.Identifier,
                    trackedConstructor.ParameterList,
                    trackedConstructor.Initializer,
                    markedBlock,
                    null,
                    SyntaxFactory.Token(SyntaxKind.None));
            }
            else if (constructorDeclaration.Body != null)
            {
                var markedBlock = MutantPlacer.PlaceStaticContextMarker((BlockSyntax) Mutate(constructorDeclaration.Body, context));
                trackedConstructor = trackedConstructor.ReplaceNode(trackedConstructor.GetCurrentNode(constructorDeclaration.Body), markedBlock);
            }

            return trackedConstructor;
        }

        private SyntaxNode MutateStaticAccessor(PropertyDeclarationSyntax accessorDeclaration, MutationContext context)
        {
            var trackedNode = accessorDeclaration.TrackNodes(accessorDeclaration.AccessorList.Accessors.Select(x => (SyntaxNode)x.Body ?? x.ExpressionBody).Where(x => x != null));
            foreach (var accessor in accessorDeclaration.AccessorList.Accessors)
            {
                if (accessor.ExpressionBody != null)
                {
                    var markedBlock = Mutate(accessor.ExpressionBody, context);
                    trackedNode = trackedNode.ReplaceNode(trackedNode.GetCurrentNode(accessor.ExpressionBody), markedBlock);
                }
                else if (accessor.Body != null)
                {
                    var markedBlock = MutantPlacer.PlaceStaticContextMarker((BlockSyntax)Mutate(accessor.Body, context));
                    trackedNode = trackedNode.ReplaceNode(trackedNode.GetCurrentNode(accessor.Body), markedBlock);
                }
            }

            return trackedNode;
        }

        private SyntaxNode MutateAssignment(ExpressionStatementSyntax tentativeAssignment, AssignmentExpressionSyntax assign, MutationContext context)
        {
            var expressionCopy = tentativeAssignment.TrackNodes(tentativeAssignment, assign, assign.Right);
            // mutate +=, *=, ...
            var result = MutateSubExpressionWithIfStatements(tentativeAssignment, expressionCopy, assign, context);
            // mutate the part right to the equal sign
            return result.ReplaceNode(result.GetCurrentNode(assign.Right), Mutate(assign.Right, context));
        }

        private SyntaxNode MutateIfStatement(IfStatementSyntax ifStatement, MutationContext context)
        {
            var mutatedIf = ifStatement.Else != null
                ? ifStatement.TrackNodes(ifStatement.Condition, ifStatement.Statement, ifStatement.Else)
                : ifStatement.TrackNodes(ifStatement.Condition, ifStatement.Statement);

            var mutated = false;

            if (!ifStatement.Condition.ContainsDeclarations())
            {
                var currentCondition = mutatedIf.GetCurrentNode(ifStatement.Condition);
                var mutatedCondition = Mutate(ifStatement.Condition, context);
                if (mutatedCondition != currentCondition)
                {
                    mutatedIf = mutatedIf.ReplaceNode(currentCondition, mutatedCondition);
                    mutated = true;
                }
            }

            if (ifStatement.Else != null)
            {
                var currentElse = mutatedIf.GetCurrentNode(ifStatement.Else);
                var mutatedElse = Mutate(ifStatement.Else, context);
                if (mutatedElse != currentElse)
                {
                    mutatedIf = mutatedIf.ReplaceNode(currentElse, mutatedElse);
                    mutated = true;
                }
            }

            var currentStatement = mutatedIf.GetCurrentNode(ifStatement.Statement);
            var mutatedStatement = Mutate(ifStatement.Statement, context);
            if (currentStatement != mutatedStatement)
            {
                mutatedIf = mutatedIf.ReplaceNode(currentStatement, mutatedStatement);
                mutated = true;
            }
            return mutated ? mutatedIf : ifStatement;
        }

        private SyntaxNode MutateForStatement(ForStatementSyntax forStatement, MutationContext context)
        {
            // for needs special treatments for its incrementors
            StatementSyntax forWithMutantIncrementors = forStatement.TrackNodes(forStatement);

            foreach (var incrementor in forStatement.Incrementors)
            {
                forWithMutantIncrementors = MutateSubExpressionWithIfStatements(forStatement, forWithMutantIncrementors, incrementor, context);
            }

            var originalFor = forWithMutantIncrementors.GetCurrentNode(forStatement);

            // mutate condition, if any
            ForStatementSyntax mutatedFor;
            StatementSyntax statementPart;
            if (forStatement.Condition == null)
            {
                mutatedFor = forStatement;
                statementPart = forStatement.Statement;
            }
            else
            {
                mutatedFor = forStatement.TrackNodes(forStatement.Condition, forStatement.Statement);
                mutatedFor = mutatedFor.ReplaceNode(mutatedFor.GetCurrentNode(forStatement.Condition),
                    Mutate(forStatement.Condition, context));
                statementPart = mutatedFor.GetCurrentNode(forStatement.Statement);
            }

            // mutate the statement/block
            mutatedFor = mutatedFor.ReplaceNode(statementPart, Mutate(forStatement.Statement, context));
            // and now we replace it
            return forWithMutantIncrementors.ReplaceNode(originalFor, mutatedFor);
        }

        private SyntaxNode MutateUnaryStatement(ExpressionStatementSyntax expressionStatement, MutationContext context)
        {
            var expressionCopy = expressionStatement.TrackNodes(expressionStatement, expressionStatement.Expression);
            return MutateSubExpressionWithIfStatements(expressionStatement, expressionCopy, expressionStatement.Expression, context);
        }

        private SyntaxNode MutateExpression(SyntaxNode currentNode, MutationContext context)
        {
            // Nothing to mutate, dig further
            var childCopy = currentNode.TrackNodes(currentNode.ChildNodes().ToList().Append(currentNode));
            var mutated = false;
            foreach (var child in currentNode.ChildNodes().ToList())
            {
                var mutatedChild = Mutate(child, context);
                if (child != mutatedChild)
                {
                    var currentChild = childCopy.GetCurrentNode(child);
                    childCopy = childCopy.ReplaceNode(currentChild, mutatedChild);
                    mutated = true;
                }
            }

            if (currentNode is ExpressionSyntax expression && !expression.ContainsDeclarations())
            {
                childCopy = MutateSubExpressionWithConditional(expression, (ExpressionSyntax)childCopy, context);
                mutated = true;
            }

            return mutated ? childCopy : currentNode;
        }

        private IEnumerable<Mutant> FindMutants(SyntaxNode current, MutationContext context)
        {
            return Mutators.SelectMany(mutator => ApplyMutator(current, mutator, context));
        }

        private StatementSyntax MutateSubExpressionWithIfStatements(StatementSyntax originalNode, StatementSyntax nodeToReplace, SyntaxNode subExpression, MutationContext context)
        {
            var ast = nodeToReplace;
            // The mutations should be placed using an IfStatement
            foreach (var mutant in FindMutants(subExpression, context))
            {
                var mutatedNode = ApplyMutant(originalNode, mutant);
                ast = MutantPlacer.PlaceWithIfStatement(ast, mutatedNode, mutant.Id);
            }
            return ast;
        }

        private ExpressionSyntax MutateSubExpressionWithConditional(ExpressionSyntax originalNode, ExpressionSyntax currentNode, MutationContext context)
        {
            return FindMutants(originalNode, context).Aggregate(currentNode, (current, mutant) => MutantPlacer.PlaceWithConditionalExpression(current, ApplyMutant(originalNode, mutant), mutant.Id));
        }

        /// <summary>
        /// Mutates one single SyntaxNode using a mutator
        /// </summary>
        private IEnumerable<Mutant> ApplyMutator(SyntaxNode syntaxNode, IMutator mutator, MutationContext context)
        {
            var mutations = mutator.Mutate(syntaxNode, _options);
            foreach (var mutation in mutations)
            {
                Logger.LogDebug("Mutant {0} created {1} -> {2} using {3}", MutantCount, mutation.OriginalNode, mutation.ReplacementNode, mutator.GetType());
                yield return new Mutant()
                {
                    Id = MutantCount++,
                    Mutation = mutation,
                    ResultStatus = MutantStatus.NotRun,
                    IsStaticValue = context.InStaticValue
                };
            }
        }

        /// <summary>
        /// Add return default to the end of the method to prevent "not all code paths return a value" error as a result of mutations
        /// </summary>
        private SyntaxNode AddReturnDefault(SyntaxNode currentNode)
        {
            // If it's not a method or the method has no body skip the node
            if (!(currentNode is MethodDeclarationSyntax methodNode) || methodNode.Body == null)
            {
                return currentNode;
            }

            // If method return type is void skip the node
            if (methodNode.ReturnType is PredefinedTypeSyntax predefinedType && predefinedType.Keyword.IsKind(SyntaxKind.VoidKeyword))
            {
                return currentNode;
            }
            
            TypeSyntax returnType = methodNode.ReturnType;

            // the GenericNameSyntax node can be encapsulated by QualifiedNameSyntax nodes
            var genericReturn = returnType.DescendantNodesAndSelf().OfType<GenericNameSyntax>().FirstOrDefault();
            if (methodNode.Modifiers.Any(x => x.IsKind(SyntaxKind.AsyncKeyword)))
            {
                if (genericReturn != null)
                {
                    // if the method is async and returns a generic task, make the return default return the underlying type
                    returnType = genericReturn.TypeArgumentList.Arguments.First();
                } else
                {
                    // if the method is async but returns a non-generic task, don't add the return default
                    return currentNode;
                }
            }

            var newBody = methodNode.Body.AddStatements(MutantPlacer.AnnotateHelper(SyntaxFactory.ReturnStatement(SyntaxFactory.DefaultExpression(returnType))));
            currentNode = currentNode.ReplaceNode(methodNode.Body, newBody);

            return currentNode;
        }

        private T ApplyMutant<T>(T node, Mutant mutant) where T : SyntaxNode
        {
            Mutants.Add(mutant);
            return node.ReplaceNode(mutant.Mutation.OriginalNode, mutant.Mutation.ReplacementNode);
        }
    }
}
