using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Stryker.Abstractions;

namespace Stryker.Core.Helpers;

internal static class RoslynHelper
{
    /// <summary>
    /// Check if an expression is an explicit string expression.
    /// </summary>
    /// <param name="node">Expression to check</param>
    /// <returns>true if it is a string</returns>
    public static bool IsAStringExpression(this ExpressionSyntax node) =>
        node.Kind() is SyntaxKind.StringLiteralExpression or
        SyntaxKind.InterpolatedStringExpression;

    /// <summary>
    /// Check if an expression is a string using the semantic model.
    /// </summary>
    /// <param name="node">Expression to check</param>
    /// <param name="model">Semantic model</param>
    /// <returns>true if it is a string</returns>
    public static bool IsAStringExpression(this ExpressionSyntax node, SemanticModel model) =>
        node.IsAStringExpression() || model.GetTypeInfo(node).Type?.SpecialType == SpecialType.System_String;

    /// <summary>
    /// Check if an expression contains a declaration
    /// </summary>
    /// <param name="node">expression to check</param>
    /// <returns>true if it contains a declaration</returns>
    public static bool ContainsDeclarations(this SyntaxNode node) =>
        node.ContainsNodeThatVerifies(x =>
            x.IsKind(SyntaxKind.DeclarationExpression) || x.IsKind(SyntaxKind.DeclarationPattern), true);

    /// <summary>
    /// Gets the return the type of the method (incl. constructor, destructor...)
    /// </summary>
    /// <param name="baseMethod">method to analyze</param>
    /// <returns>method returns type. Null if this not relevant</returns>
    public static TypeSyntax ReturnType(this BaseMethodDeclarationSyntax baseMethod) =>
        baseMethod switch
        {
            ConstructorDeclarationSyntax _ => VoidTypeSyntax(),
            ConversionOperatorDeclarationSyntax conversion => conversion.Type,
            DestructorDeclarationSyntax _ => VoidTypeSyntax(),
            MethodDeclarationSyntax method => method.ReturnType,
            OperatorDeclarationSyntax operatorSyntax => operatorSyntax.ReturnType,
            _ => null
        };

    /// <summary>
    /// Gets the return 'type' of a (get/set) accessor
    /// </summary>
    /// <param name="accessor">accessor</param>
    /// <returns>method returns type. Null if this not relevant</returns>
    public static TypeSyntax ReturnType(this AccessorDeclarationSyntax accessor)
    {
        if (accessor.IsGetter() && accessor.Parent is AccessorListSyntax accessorList)
        {
            return accessorList.Parent switch
            {
                PropertyDeclarationSyntax propertyDeclaration => propertyDeclaration.Type,
                IndexerDeclarationSyntax indexerDeclaration => indexerDeclaration.Type,
                _ => null
            };
        }

        return VoidTypeSyntax();
    }

    /// <summary>
    /// Returns 'void' type
    /// </summary>
    /// <returns>Returns 'void' type</returns>
    public static TypeSyntax VoidTypeSyntax() => SyntaxFactory.PredefinedType(SyntaxFactory.Token(SyntaxKind.VoidKeyword));

    /// <summary>
    /// Check if any modifier is the async keyword
    /// </summary>
    /// <param name="modifiers">modifier list</param>
    /// <returns>true if any modifiers is 'async' </returns>
    public static bool ContainsAsyncKeyword(this SyntaxTokenList modifiers) => modifiers.Any(x => x.IsKind(SyntaxKind.AsyncKeyword));

    /// <summary>
    /// True if the block does not contain any statement.
    /// </summary>
    /// <param name="blockSyntax">block to test</param>
    /// <returns>true when the block contains no statement.</returns>
    public static bool IsEmpty(this BlockSyntax blockSyntax) => blockSyntax.Statements.Count == 0;

    /// <summary>
    /// Checks if a member is static
    /// </summary>
    /// <param name="node">Member to analyze</param>
    /// <returns>true if it is a static method, properties, fields...</returns>
    public static bool IsStatic(this MemberDeclarationSyntax node) => node.Modifiers.Any(x => x.IsKind(SyntaxKind.StaticKeyword));

    /// <summary>
    /// Returns true if the given type is 'void'.
    /// </summary>
    /// <param name="type">type syntax</param>
    /// <returns>type == typeof(void)</returns>
    public static bool IsVoid(this TypeSyntax type) => type is PredefinedTypeSyntax predefinedType &&
                                                       predefinedType.Keyword.IsKind(SyntaxKind.VoidKeyword);

    /// <summary>
    /// Build a mutated version of a <see cref="SyntaxNode"/>.
    /// </summary>
    /// <typeparam name="T">Type of the node that hosts the mutation.</typeparam>
    /// <param name="sourceNode">Node of the original tree.</param>
    /// <param name="mutation">Mutation to apply to the sourceNode.</param>
    /// <returns>A copy of <see cref="sourceNode"/> with the mutation applied.</returns>
    /// <exception cref="InvalidOperationException">when mutation does not belong to this node.</exception>
    /// <remarks><paramref name="sourceNode"/> can be any node that includes the original, non mutated node described in the mutation.</remarks>
    public static T InjectMutation<T>(this T sourceNode, Mutation mutation) where T : SyntaxNode =>
        sourceNode.Contains(mutation.OriginalNode) ? sourceNode.ReplaceNode(mutation.OriginalNode, mutation.ReplacementNode)
            // if this happens, there is a probably a bug in some orchestrator
            : throw new InvalidOperationException($"Cannot inject mutation '{mutation.ReplacementNode}' in '{sourceNode}' because we cannot find the original code.");

    /// <summary>
    /// Get the gets accessor of a property, if any.
    /// </summary>
    /// <param name="propertyDeclaration">Property.</param>
    /// <returns>Get accessor for the property, or null if none.</returns>
    public static AccessorDeclarationSyntax GetAccessor(this PropertyDeclarationSyntax propertyDeclaration)
        => propertyDeclaration?.AccessorList?.Accessors.FirstOrDefault(IsGetter);

    /// <summary>
    /// Checks if the accessor is a getter.
    /// </summary>
    /// <param name="accessor">accessor to evaluate</param>
    /// <returns>true if <paramref name="accessor"/> is a getter.</returns>
    private static bool IsGetter(this AccessorDeclarationSyntax accessor) => accessor.Keyword.IsKind(SyntaxKind.GetKeyword);

    /// <summary>
    /// Return a default(type) expression.
    /// </summary>
    /// <param name="type">Type used in the resulting default expression.</param>
    /// <returns>An expression representing `default(<paramref name="type"/>'.</returns>
    public static ExpressionSyntax BuildDefaultExpression(this TypeSyntax type)
        => SyntaxFactory.DefaultExpression(type.WithoutTrivia()).WithLeadingTrivia(SyntaxFactory.Space);

    /// <summary>
    /// Check if a statements (or one of its child statements, if any) verifies some given predicate.
    /// </summary>
    /// <param name="syntax">initial statements</param>
    /// <param name="predicate">predicate to verify</param>
    /// <param name="skipBlocks">does not parse block statements if true</param>
    /// <returns>true if any of the child statements verify the predicate</returns>
    /// <remarks>scanning stops as soon as one child matches the predicate</remarks>
    public static bool ScanChildStatements(this StatementSyntax syntax, Func<StatementSyntax, bool> predicate, bool skipBlocks = false)
    {
        if (predicate(syntax))
        {
            return true;
        }
        // scan children with minor optimization for well known statement
        return syntax switch
        {
            BlockSyntax block => !skipBlocks && block.Statements.Any(s => s.ScanChildStatements(predicate)),
            LocalFunctionStatementSyntax => false,
            ForStatementSyntax forStatement => forStatement.Statement.ScanChildStatements(predicate, skipBlocks),
            WhileStatementSyntax whileStatement => whileStatement.Statement.ScanChildStatements(predicate, skipBlocks),
            DoStatementSyntax doStatement => doStatement.Statement.ScanChildStatements(predicate, skipBlocks),
            SwitchStatementSyntax switchStatement => switchStatement.Sections.SelectMany(s => s.Statements).Any(statement => statement.ScanChildStatements(predicate, skipBlocks)),
            IfStatementSyntax ifStatement => ifStatement.Statement.ScanChildStatements(predicate, skipBlocks)
                                || ifStatement.Else?.Statement.ScanChildStatements(predicate, skipBlocks) == true,
            _ => syntax.ChildNodes().OfType<StatementSyntax>()
                                .Any(s => s.ScanChildStatements(predicate, skipBlocks)),
        };
    }

    /// <summary>
    /// Scans recursively the node to find if a child node matches the predicated. Does not recurse into local functions or anonymous functions
    /// </summary>
    /// <param name="node">starting node</param>
    /// <param name="predicate">predicate to match</param>
    /// <param name="skipBlock">true to skip inner blocks</param>
    /// <returns>true if a child node matches the predicate</returns>
    public static bool ContainsNodeThatVerifies(this SyntaxNode node, Func<SyntaxNode, bool> predicate, bool skipBlock = true) =>
        // scan
        node.DescendantNodes((child) =>
            !(skipBlock && child is BlockSyntax)
            && (child.Parent is not AnonymousFunctionExpressionSyntax function || function.ExpressionBody != child)
            && (child.Parent is not LocalFunctionStatementSyntax localFunction || localFunction.ExpressionBody != child))
            .Any(predicate);

    /// <summary>
    /// Check if a node assigns a value to the provided identifier
    /// </summary>
    /// <param name="node">node to check</param>
    /// <param name="identifier">identifier to check</param>
    /// <returns>true if this is node is an assignment or an out variable assigning <paramref name="node"/> </returns>
    public static bool AssignsThis(this SyntaxNode node, string identifier) =>
        (node is AssignmentExpressionSyntax assignmentExpressionSyntax && assignmentExpressionSyntax.Left.ToString() == identifier)
        || (node is ArgumentSyntax arg && arg.ToString() == identifier && arg.RefOrOutKeyword.IsKind(SyntaxKind.OutKeyword));

    /// <summary>
    /// Clean trivia from a node
    /// </summary>
    /// <typeparam name="T">Syntax node exact type</typeparam>
    /// <param name="node">node on which to set the trivia</param>
    /// <returns>a <paramref name="node"/> copy with some of the original trivia .</returns>
    /// <remarks>uses <see cref="WithCleanTriviaFrom{T}(T, T)"/></remarks>
    public static T WithCleanTrivia<T>(this T node) where T : SyntaxNode
        => node.WithCleanTriviaFrom(node);

    /// <summary>
    /// Inject cleaned up trivia from another syntax node.
    /// </summary>
    /// <typeparam name="T">Syntax node exact type</typeparam>
    /// <param name="node">node on which to set the trivia</param>
    /// <param name="triviaSource">node from which extract the trivia</param>
    /// <returns>a <paramref name="node"/> copy with some trivia from <paramref name="triviaSource"/>.</returns>
    /// <remarks>Current implementation only applies whitespacetrivia (no comment, no attribute nor directives)</remarks>
    public static T WithCleanTriviaFrom<T>(this T node, T triviaSource) where T: SyntaxNode
        => node.WithLeadingTrivia(CleanupTrivia(triviaSource.GetLeadingTrivia()))
        .WithTrailingTrivia(CleanupTrivia(triviaSource.GetTrailingTrivia()));

    /// <summary>
    /// Inject cleaned up trivia from another syntax node.
    /// </summary>
    /// <param name="token">token on which to set the trivia</param>
    /// <param name="triviaSource">node from which extract the trivia</param>
    /// <returns>a <paramref name="token"/> copy with some trivia from <paramref name="triviaSource"/>.</returns>
    /// <remarks>Current implementation only applies whitespacetrivia (no comment, no attribute nor directives)</remarks>
    public static SyntaxToken WithCleanTriviaFrom(this SyntaxToken token, SyntaxToken triviaSource)
        => token.WithLeadingTrivia(CleanupTrivia(triviaSource.LeadingTrivia))
        .WithTrailingTrivia(CleanupTrivia(triviaSource.TrailingTrivia));

    private static IEnumerable<SyntaxTrivia> CleanupTrivia(SyntaxTriviaList list)
        => list.Where(t=>t.IsKind(SyntaxKind.WhitespaceTrivia) || t.IsKind(SyntaxKind.EndOfLineTrivia));

    /// <summary>
    /// Returns all siblings node before the provided one.
    /// </summary>
    /// <param name="node">reference node</param>
    /// <returns>a list of <see cref="SyntaxNode"/></returns>
    public static IEnumerable<SyntaxNode> GetPreviousSiblings(this SyntaxNode node) =>
        node.Parent != null
            ? [..node.Parent.ChildNodes().TakeWhile(syntaxNode => syntaxNode != node)] : [];

}
