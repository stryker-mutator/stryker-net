using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Stryker.Abstractions;

namespace Stryker.Core.MutantFilters;

/// <summary>
/// Finds mutants whose only possible effect is on whether, or with what, ignored methods are called:
/// the condition of an <c>if</c> whose branches only call ignored methods, and locals that are only
/// read by ignored calls (a counter that is logged and nothing else).
/// Anything that can change other state, throw, or change control flow is never reported.
/// Assumes property getters are pure and that a receiver the code does not null-test is not null.
/// </summary>
internal sealed class IgnoredCallOnlyEffect
{
    private readonly Func<SyntaxNode, bool> _isIgnoredCall;
    private readonly Dictionary<SyntaxNode, HashSet<string>> _deadLocalsByMember = new();

    public IgnoredCallOnlyEffect(Func<SyntaxNode, bool> isIgnoredCall) => _isIgnoredCall = isIgnoredCall;

    public bool IsOnlyEffect(Mutation mutation)
    {
        var node = mutation.OriginalNode;
        var member = node?.FirstAncestorOrSelf<MemberDeclarationSyntax>();
        if (member is null)
        {
            return false;
        }

        var dead = DeadLocals(member);
        switch (node)
        {
            case StatementSyntax statement when mutation.Type is Mutator.Block or Mutator.Statement:
                return IsInert(statement, dead, null);
            case PostfixUnaryExpressionSyntax or PrefixUnaryExpressionSyntax when mutation.Type == Mutator.Update:
                return IsDeadWrite(node, dead);
            case AssignmentExpressionSyntax assignment when mutation.Type == Mutator.Assignment:
                return IsDeadWrite(node, dead)
                    && mutation.ReplacementNode is not AssignmentExpressionSyntax
                    {
                        RawKind: (int)SyntaxKind.DivideAssignmentExpression or (int)SyntaxKind.ModuloAssignmentExpression
                    }
                    && (assignment.Kind() is SyntaxKind.AddAssignmentExpression or SyntaxKind.SubtractAssignmentExpression
                        || IsSafe(assignment.Right, NullTested(assignment)));
        }

        var nullTested = NullTested(node.FirstAncestorOrSelf<StatementSyntax>());
        return KeepsEvaluation(mutation, nullTested) && FlowsOnlyToIgnoredCalls(node, dead, nullTested);
    }

    // The mutated expression still evaluates exactly what the original did.
    private bool KeepsEvaluation(Mutation mutation, HashSet<string> nullTested) => mutation.Type switch
    {
        Mutator.Equality or Mutator.Boolean or Mutator.Unary => true,
        Mutator.Logical => mutation.OriginalNode is not BinaryExpressionSyntax
        {
            RawKind: (int)SyntaxKind.LogicalAndExpression or (int)SyntaxKind.LogicalOrExpression
        } binary || IsSafe(binary.Right, nullTested),
        Mutator.Conditional => mutation.OriginalNode is ConditionalExpressionSyntax conditional
            && IsSafe(conditional.Condition, nullTested)
            && IsSafe(conditional.WhenTrue, nullTested)
            && IsSafe(conditional.WhenFalse, nullTested),
        Mutator.Arithmetic => mutation.ReplacementNode is BinaryExpressionSyntax
        {
            RawKind: (int)SyntaxKind.AddExpression or (int)SyntaxKind.SubtractExpression or (int)SyntaxKind.MultiplyExpression
        },
        _ => false,
    };

    // Follows the value of node upwards until it lands somewhere only ignored calls observe.
    private bool FlowsOnlyToIgnoredCalls(SyntaxNode node, HashSet<string> dead, HashSet<string> nullTested)
    {
        var current = node;
        while (true)
        {
            var parent = current.Parent;
            switch (parent)
            {
                case ParenthesizedExpressionSyntax:
                case PrefixUnaryExpressionSyntax
                {
                    RawKind: (int)SyntaxKind.LogicalNotExpression or (int)SyntaxKind.UnaryMinusExpression or (int)SyntaxKind.UnaryPlusExpression
                }:
                case IsPatternExpressionSyntax:
                case PatternSyntax and not RecursivePatternSyntax { PositionalPatternClause: not null }:
                case SubpatternSyntax:
                case PropertyPatternClauseSyntax:
                    break;
                case BinaryExpressionSyntax binary:
                    switch (binary.Kind())
                    {
                        case SyntaxKind.LogicalAndExpression or SyntaxKind.LogicalOrExpression or SyntaxKind.CoalesceExpression:
                            if (current == binary.Left && !IsSafe(binary.Right, nullTested))
                            {
                                return false;
                            }
                            break;
                        case SyntaxKind.EqualsExpression or SyntaxKind.NotEqualsExpression
                            or SyntaxKind.LessThanExpression or SyntaxKind.LessThanOrEqualExpression
                            or SyntaxKind.GreaterThanExpression or SyntaxKind.GreaterThanOrEqualExpression
                            or SyntaxKind.BitwiseAndExpression or SyntaxKind.BitwiseOrExpression or SyntaxKind.ExclusiveOrExpression
                            or SyntaxKind.AddExpression or SyntaxKind.SubtractExpression or SyntaxKind.MultiplyExpression
                            or SyntaxKind.IsExpression or SyntaxKind.AsExpression:
                            break;
                        default:
                            return false;
                    }
                    break;
                case ConditionalExpressionSyntax conditional:
                    if (current == conditional.Condition
                        && !(IsSafe(conditional.WhenTrue, nullTested) && IsSafe(conditional.WhenFalse, nullTested)))
                    {
                        return false;
                    }
                    break;
                case IfStatementSyntax ifStatement when current == ifStatement.Condition:
                    var thenTested = new HashSet<string>(nullTested, StringComparer.Ordinal);
                    thenTested.ExceptWith(ProvenNotNull(ifStatement.Condition, node));
                    return IsInert(ifStatement.Statement, dead, thenTested)
                        && (ifStatement.Else is null || IsInert(ifStatement.Else.Statement, dead, nullTested));
                case EqualsValueClauseSyntax { Parent: VariableDeclaratorSyntax declarator }:
                    return declarator.Parent?.Parent is LocalDeclarationStatementSyntax
                        && dead.Contains(declarator.Identifier.ValueText);
                case AssignmentExpressionSyntax assignment when current == assignment.Right:
                    return IsDeadWrite(assignment, dead);
                default:
                    return false;
            }

            current = parent;
        }
    }

    // A statement that, when it runs (or no longer runs), can only change ignored calls.
    // nullTested is null for removal: an ignored call that no longer runs evaluates nothing.
    private bool IsInert(StatementSyntax statement, HashSet<string> dead, HashSet<string> nullTested) => statement switch
    {
        BlockSyntax block => block.Statements.All(s => IsInert(s, dead, nullTested)),
        EmptyStatementSyntax => true,
        ExpressionStatementSyntax { Expression: var expression } when IsIgnoredCall(expression) =>
            nullTested is null || IsSafeArgument(expression, nullTested),
        ExpressionStatementSyntax { Expression: PostfixUnaryExpressionSyntax or PrefixUnaryExpressionSyntax } expressionStatement =>
            IsDeadWrite(expressionStatement.Expression, dead),
        ExpressionStatementSyntax { Expression: AssignmentExpressionSyntax assignment } =>
            IsDeadWrite(assignment, dead) && IsSafe(assignment.Right, nullTested ?? NullTested(assignment)),
        IfStatementSyntax ifStatement => IsInertIf(ifStatement, dead, nullTested),
        _ => false,
    };

    private bool IsInertIf(IfStatementSyntax ifStatement, HashSet<string> dead, HashSet<string> nullTested)
    {
        var tested = NullTested(ifStatement);
        if (nullTested is not null)
        {
            tested.UnionWith(nullTested);
        }

        return IsSafe(ifStatement.Condition, tested)
            && IsInert(ifStatement.Statement, dead, nullTested is null ? null : tested)
            && (ifStatement.Else is null || IsInert(ifStatement.Else.Statement, dead, nullTested is null ? null : tested));
    }

    private bool IsIgnoredCall(ExpressionSyntax expression)
    {
        var call = expression is AwaitExpressionSyntax awaited ? awaited.Expression : expression;
        return call is InvocationExpressionSyntax or ConditionalAccessExpressionSyntax && _isIgnoredCall(call);
    }

    // `x++;`, `x += y;` or `x = y;` on a local that only ignored calls read.
    private static bool IsDeadWrite(SyntaxNode write, HashSet<string> dead) =>
        write.Parent is ExpressionStatementSyntax && write switch
        {
            PostfixUnaryExpressionSyntax { RawKind: (int)SyntaxKind.PostIncrementExpression or (int)SyntaxKind.PostDecrementExpression } postfix =>
                IsDeadName(postfix.Operand, dead),
            PrefixUnaryExpressionSyntax { RawKind: (int)SyntaxKind.PreIncrementExpression or (int)SyntaxKind.PreDecrementExpression } prefix =>
                IsDeadName(prefix.Operand, dead),
            AssignmentExpressionSyntax assignment => IsDeadName(assignment.Left, dead),
            _ => false,
        };

    private static bool IsDeadName(ExpressionSyntax expression, HashSet<string> dead) =>
        expression is IdentifierNameSyntax identifier && dead.Contains(identifier.Identifier.ValueText);

    // Evaluating this can neither change state nor throw.
    private bool IsSafe(ExpressionSyntax expression, HashSet<string> nullTested) =>
        expression is null || expression.DescendantNodesAndSelf().All(n => n switch
        {
            InvocationExpressionSyntax => _isIgnoredCall(n),
            BaseObjectCreationExpressionSyntax or ArrayCreationExpressionSyntax or ImplicitArrayCreationExpressionSyntax
                or AssignmentExpressionSyntax or AwaitExpressionSyntax or ThrowExpressionSyntax
                or ElementAccessExpressionSyntax or ElementBindingExpressionSyntax or CastExpressionSyntax
                or QueryExpressionSyntax or WithExpressionSyntax or PositionalPatternClauseSyntax => false,
            PrefixUnaryExpressionSyntax { RawKind: (int)SyntaxKind.PreIncrementExpression or (int)SyntaxKind.PreDecrementExpression } => false,
            PostfixUnaryExpressionSyntax
            {
                RawKind: (int)SyntaxKind.PostIncrementExpression or (int)SyntaxKind.PostDecrementExpression or (int)SyntaxKind.SuppressNullableWarningExpression
            } => false,
            BinaryExpressionSyntax { RawKind: (int)SyntaxKind.DivideExpression or (int)SyntaxKind.ModuloExpression } => false,
            MemberAccessExpressionSyntax access => !nullTested.Contains(Key(access.Expression)),
            _ => true,
        });

    // An ignored call that now runs when it did not: its own arguments must not dereference
    // what the condition null-tested, index, or cast.
    private static bool IsSafeArgument(ExpressionSyntax expression, HashSet<string> nullTested) =>
        expression.DescendantNodesAndSelf().All(n => n switch
        {
            ElementAccessExpressionSyntax or CastExpressionSyntax => false,
            PostfixUnaryExpressionSyntax { RawKind: (int)SyntaxKind.SuppressNullableWarningExpression } suppressed =>
                !nullTested.Contains(Key(suppressed.Operand)),
            MemberAccessExpressionSyntax access => !nullTested.Contains(Key(access.Expression)),
            _ => true,
        });

    // Expressions the statement's own expressions test for null; a flipped test dereferences null.
    private static HashSet<string> NullTested(SyntaxNode root)
    {
        var tested = new HashSet<string>(StringComparer.Ordinal);
        if (root is null)
        {
            return tested;
        }

        foreach (var node in root.DescendantNodesAndSelf(n => n == root || n is not StatementSyntax))
        {
            switch (node)
            {
                case BinaryExpressionSyntax { RawKind: (int)SyntaxKind.EqualsExpression or (int)SyntaxKind.NotEqualsExpression } binary:
                    if (binary.Right.IsKind(SyntaxKind.NullLiteralExpression))
                    {
                        tested.Add(Key(binary.Left));
                    }
                    else if (binary.Left.IsKind(SyntaxKind.NullLiteralExpression))
                    {
                        tested.Add(Key(binary.Right));
                    }
                    break;
                case IsPatternExpressionSyntax pattern:
                    tested.Add(Key(pattern.Expression));
                    break;
                case ConditionalAccessExpressionSyntax conditional:
                    tested.Add(Key(conditional.Expression));
                    break;
                case MemberAccessExpressionSyntax { Name.Identifier.ValueText: "HasValue" } access:
                    tested.Add(Key(access.Expression));
                    break;
                case InvocationExpressionSyntax { ArgumentList.Arguments.Count: > 0 } invocation
                    when invocation.Expression.ToString().Split('.').Last().StartsWith("IsNullOr", StringComparison.Ordinal):
                    tested.Add(Key(invocation.ArgumentList.Arguments[0].Expression));
                    break;
            }
        }

        return tested;
    }

    // The then-branch only runs when every top-level conjunct holds, so a conjunct the mutant leaves
    // untouched that proves a value non-null keeps proving it there.
    private static IEnumerable<string> ProvenNotNull(ExpressionSyntax condition, SyntaxNode mutated)
    {
        foreach (var conjunct in Conjuncts(condition))
        {
            if (conjunct.Span.OverlapsWith(mutated.Span))
            {
                continue;
            }

            switch (conjunct)
            {
                case BinaryExpressionSyntax { RawKind: (int)SyntaxKind.NotEqualsExpression } binary
                    when binary.Right.IsKind(SyntaxKind.NullLiteralExpression):
                    yield return Key(binary.Left);
                    break;
                case BinaryExpressionSyntax { RawKind: (int)SyntaxKind.NotEqualsExpression } binary
                    when binary.Left.IsKind(SyntaxKind.NullLiteralExpression):
                    yield return Key(binary.Right);
                    break;
                case IsPatternExpressionSyntax { Pattern: UnaryPatternSyntax { Pattern: ConstantPatternSyntax constant } } pattern
                    when constant.Expression.IsKind(SyntaxKind.NullLiteralExpression):
                    yield return Key(pattern.Expression);
                    break;
                case IsPatternExpressionSyntax { Pattern: RecursivePatternSyntax or DeclarationPatternSyntax or TypePatternSyntax } pattern:
                    yield return Key(pattern.Expression);
                    break;
                case MemberAccessExpressionSyntax { Name.Identifier.ValueText: "HasValue" } access:
                    yield return Key(access.Expression);
                    break;
            }
        }
    }

    private static IEnumerable<ExpressionSyntax> Conjuncts(ExpressionSyntax expression) => expression switch
    {
        ParenthesizedExpressionSyntax parenthesized => Conjuncts(parenthesized.Expression),
        BinaryExpressionSyntax { RawKind: (int)SyntaxKind.LogicalAndExpression } and =>
            Conjuncts(and.Left).Concat(Conjuncts(and.Right)),
        _ => [expression],
    };

    private static string Key(ExpressionSyntax expression)
    {
        while (true)
        {
            switch (expression)
            {
                case ParenthesizedExpressionSyntax parenthesized:
                    expression = parenthesized.Expression;
                    continue;
                case PostfixUnaryExpressionSyntax { RawKind: (int)SyntaxKind.SuppressNullableWarningExpression } suppressed:
                    expression = suppressed.Operand;
                    continue;
                case MemberAccessExpressionSyntax { Expression: ThisExpressionSyntax } access:
                    return access.Name.Identifier.ValueText;
                default:
                    return string.Concat(expression.DescendantTokens().Select(t => t.ValueText));
            }
        }
    }

    // Locals whose value only ever reaches ignored calls: the greatest set where every reference
    // is a statement-level write, an argument of an ignored call, or flows into such a local or
    // into the condition of an if whose branches are inert.
    private HashSet<string> DeadLocals(SyntaxNode member)
    {
        if (_deadLocalsByMember.TryGetValue(member, out var cached))
        {
            return cached;
        }

        var declared = member.DescendantNodes()
            .Select(DeclaredName)
            .Where(name => name is not null)
            .GroupBy(name => name)
            .Where(group => group.Count() == 1)
            .Select(group => group.Key)
            .ToHashSet(StringComparer.Ordinal);
        var candidates = member.DescendantNodes()
            .OfType<LocalDeclarationStatementSyntax>()
            .Where(local => local.UsingKeyword.IsKind(SyntaxKind.None) && !local.IsConst
                && local.Declaration.Type is not RefTypeSyntax && local.Parent is BlockSyntax or SwitchSectionSyntax)
            .SelectMany(local => local.Declaration.Variables.Select(v => (Name: v.Identifier.ValueText, Scope: local.Parent)))
            .Where(local => declared.Contains(local.Name))
            .ToDictionary(local => local.Name, local => References(local.Scope, local.Name).ToList(), StringComparer.Ordinal);

        var dead = candidates.Keys.ToHashSet(StringComparer.Ordinal);
        _deadLocalsByMember[member] = dead;
        bool changed;
        do
        {
            changed = dead.RemoveWhere(name => !candidates[name].All(reference => IsDeadReference(reference, dead))) > 0;
        }
        while (changed);

        return dead;
    }

    private static string DeclaredName(SyntaxNode node) => node switch
    {
        VariableDeclaratorSyntax declarator => declarator.Identifier.ValueText,
        ParameterSyntax parameter => parameter.Identifier.ValueText,
        SingleVariableDesignationSyntax designation => designation.Identifier.ValueText,
        ForEachStatementSyntax forEach => forEach.Identifier.ValueText,
        CatchDeclarationSyntax catchDeclaration => catchDeclaration.Identifier.ValueText,
        LocalFunctionStatementSyntax localFunction => localFunction.Identifier.ValueText,
        QueryClauseSyntax or FromClauseSyntax or QueryContinuationSyntax => node.ChildTokens()
            .FirstOrDefault(t => t.IsKind(SyntaxKind.IdentifierToken)).ValueText,
        _ => null,
    };

    private static IEnumerable<IdentifierNameSyntax> References(SyntaxNode scope, string name) =>
        scope.DescendantNodes().OfType<IdentifierNameSyntax>().Where(identifier =>
            identifier.Identifier.ValueText == name
            && identifier.Parent switch
            {
                MemberAccessExpressionSyntax access => access.Name != identifier,
                MemberBindingExpressionSyntax or NameColonSyntax or NameEqualsSyntax or QualifiedNameSyntax => false,
                AssignmentExpressionSyntax { Parent: InitializerExpressionSyntax } assignment => assignment.Left != identifier,
                _ => true,
            });

    private bool IsDeadReference(IdentifierNameSyntax reference, HashSet<string> dead)
    {
        if (IsDeadWrite(reference.Parent, dead) && reference.Parent switch
            {
                AssignmentExpressionSyntax assignment => assignment.Left == reference,
                _ => true,
            })
        {
            return true;
        }

        // an argument of an ignored call, passed by value
        var argument = reference.FirstAncestorOrSelf<ArgumentSyntax>();
        while (argument is not null)
        {
            if (argument.Parent?.Parent is InvocationExpressionSyntax invocation && _isIgnoredCall(invocation))
            {
                return reference.FirstAncestorOrSelf<ArgumentSyntax>().RefKindKeyword.IsKind(SyntaxKind.None);
            }

            argument = argument.Parent?.FirstAncestorOrSelf<ArgumentSyntax>();
        }

        var statement = reference.FirstAncestorOrSelf<StatementSyntax>();
        return FlowsOnlyToIgnoredCalls(reference, dead, NullTested(statement));
    }
}
