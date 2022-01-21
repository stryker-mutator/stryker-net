using System;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Stryker.Core.Mutants;

namespace Stryker.Core.Helpers
{
    internal static class RoslynHelper
    {
        /// <summary>
        /// Gets the return type of a method (incl. constructor, destructor...)
        /// </summary>
        /// <param name="baseMethod">method to analyze</param>
        /// <returns>method returns type. Null if this not relevant</returns>
        public static TypeSyntax ReturnType(this BaseMethodDeclarationSyntax baseMethod) =>
            baseMethod switch
            {
                ConstructorDeclarationSyntax _ => null,
                ConversionOperatorDeclarationSyntax conversion => conversion.Type,
                DestructorDeclarationSyntax _ => null,
                MethodDeclarationSyntax method => method.ReturnType,
                OperatorDeclarationSyntax operatorSyntax => operatorSyntax.ReturnType,
                _ => null
            };

        /// <summary>
        /// Gets the return type of a (get accessor)
        /// </summary>
        /// <param name="accessor">accessor</param>
        /// <returns>method returns type. Null if this not relevant</returns>
        public static TypeSyntax ReturnType(this AccessorDeclarationSyntax accessor)
        {
            if (accessor.IsGetter() && accessor.Parent is AccessorListSyntax accessorList && accessorList.Parent is PropertyDeclarationSyntax propertyDeclaration)
            {
                return propertyDeclaration.Type;
            }

            return null;
        }

        /// <summary>
        /// True if the block does not contain any statement.
        /// </summary>
        /// <param name="blockSyntax">block to test</param>
        /// <returns>true when the block contains no statement.</returns>
        public static bool IsEmpty(this BlockSyntax blockSyntax) => blockSyntax.Statements.Count == 0;

        /// <summary>
        /// Checks if a method ends with return.
        /// </summary>
        /// <param name="baseMethod">method to analyze.</param>
        /// <returns>true if <paramref name="baseMethod"/>is non void method, an operator or a conversion operator.</returns>
        public static bool NeedsReturn(this BaseMethodDeclarationSyntax baseMethod) =>
            baseMethod switch
            {
                MethodDeclarationSyntax method => !(method.ReturnType is PredefinedTypeSyntax predefinedType &&
                                                    predefinedType.Keyword.IsKind(SyntaxKind.VoidKeyword)),
                OperatorDeclarationSyntax => true,
                ConversionOperatorDeclarationSyntax => true,
                _ => false
            };

        /// <summary>
        /// Checks if a local function returns a value.
        /// </summary>
        /// <param name="localFunction">local function to evaluate</param>
        /// <returns>true is the method as non void return value</returns>
        public static bool NeedsReturn(this LocalFunctionStatementSyntax localFunction) =>
            !localFunction.ReturnType.IsVoid();

        /// <summary>
        /// Checks if a member is static
        /// </summary>
        /// <param name="node">Member to analyze</param>
        /// <returns>true if it is a static method, properties, fields...</returns>
        public static bool IsStatic(this MemberDeclarationSyntax node) => node.Modifiers.Any(x => x.Kind() == SyntaxKind.StaticKeyword);

        /// <summary>
        /// Checks if an accessor is a getter and needs to end with a return
        /// </summary>
        /// <param name="baseMethod"></param>
        /// <returns>true if this is a getter</returns>
        public static bool NeedsReturn(this AccessorDeclarationSyntax baseMethod) => baseMethod.IsGetter();

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
        /// <returns>A copy of <see cref="original"/> with the mutation applied.</returns>
        /// <exception cref="InvalidOperationException">when mutation does not belong to this node.</exception>
        /// <remarks><paramref name="sourceNode"/> can be any node that includes the original, non mutated node described in the mutation.</remarks>
        public static T InjectMutation<T>(this T sourceNode, Mutation mutation) where T : SyntaxNode
        {
            if (!sourceNode.Contains(mutation.OriginalNode))
            {
                // if this happens, there is a probably a bug in some orchestrator
                throw new InvalidOperationException($"Cannot inject mutation '{mutation.ReplacementNode}' in '{sourceNode}' because we cannot find the original code.");
            }
            return sourceNode.ReplaceNode(mutation.OriginalNode, mutation.ReplacementNode);
        }

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
        public static bool IsGetter(this AccessorDeclarationSyntax accessor) => accessor.Keyword.IsKind(SyntaxKind.GetKeyword);

        /// <summary>
        /// Return a default(type) expression.
        /// </summary>
        /// <param name="type">Type used in the resulting default expression.</param>
        /// <returns>An expression representing `default(<paramref name="type"/>'.</returns>
        public static ExpressionSyntax BuildDefaultExpression(this TypeSyntax type) => SyntaxFactory.DefaultExpression(type.WithoutTrailingTrivia()).WithLeadingTrivia(SyntaxFactory.Space);
    }
}
